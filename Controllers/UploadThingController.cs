using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;
using NextGameAPI.Services.UploadThing;
using System.Buffers.Text;
using System.Text;
using System.Text.Json;
using System.Web;

namespace NextGameAPI.Controllers
{
    [Route("api/uploadthing")]
    [ApiController]
    public class UploadThingController : ControllerBase
    {
        private static readonly List<FileRoute> FileRoutes = new List<FileRoute>
        {
            new() { Slug = "profilePicture", AllowedFileTypes = "image/png,image/jpeg", MaxFileSize = 1024 * 1024 * 1 }
        };
        private readonly UploadThingService _uploadThingService;
        private readonly UploadThingHelpers _uploadThingHelpers;
        private readonly UploadThingToken _uploadThingToken;
        private readonly UserManager<User> _userManager;
        private readonly IUserSettings _userSettingsRepo;

        public UploadThingController(UploadThingHelpers uploadThingHelpers, UserManager<User> userManager, IUserSettings userSettingsRepo, UploadThingService uploadThingService)
        {
            _uploadThingHelpers = uploadThingHelpers;
            _uploadThingToken = _uploadThingHelpers.DecodeToken(Environment.GetEnvironmentVariable("uploadthing-token"));
            _userManager = userManager;
            _userSettingsRepo = userSettingsRepo;
            _uploadThingService = uploadThingService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetMetadata()
        {
            var metadata = FileRoutes.Select(r => new
            {
                slug = r.Slug,
                config = new
                {
                    allowedFileTypes = r.AllowedFileTypes,
                    maxFileSize = r.MaxFileSize
                }
            }).ToList();
            return Ok(metadata);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PrepareUpload([FromQuery] string slug, [FromQuery] string actionType, [FromBody] FileUploadRequestPayload requestPayload)
        {
            if (User == null | User?.Identity == null)
            {
                return Unauthorized();
            }
            var user = await _userManager.GetUserAsync(User);
            var fileRoute = FileRoutes.FirstOrDefault(r => r.Slug == slug);
            var request = requestPayload.Files.FirstOrDefault();

            if (actionType != "upload" | fileRoute == null | request == null | request?.Size > fileRoute.MaxFileSize | !_uploadThingService.checkAllowedFileType(request?.Type, fileRoute.AllowedFileTypes))
            {
                return BadRequest();
            }

            var content = _uploadThingService.ConvertRequestToStringContent(request, slug);

            var presignedUrlResponse = await _uploadThingService.GetPresignedUrlAsync(content);
            if (presignedUrlResponse == null)
            {
                return BadRequest();
            }
            string fileKey = presignedUrlResponse["key"];

            var registerResponse = await _uploadThingService.RegisterUploadAsync(fileKey, user.UserName, slug);
            if (!registerResponse)
            {
                return BadRequest();
            }
            return Ok(presignedUrlResponse);
        }


        [HttpPost("callback")]
        [AllowAnonymous]
        public async Task<IActionResult> Callback()
        {
            string requestBody;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            if (!_uploadThingService.IsSignatureVerified(Request.Headers["uploadthing-hook"].FirstOrDefault(), Request.Headers["x-uploadthing-signature"].FirstOrDefault(), requestBody))
            {
                return Unauthorized();
            }

            var root = _uploadThingService.GetJsonElementFromRequestBody(requestBody);
            if (root.ValueKind == JsonValueKind.Undefined) 
            {
                return BadRequest();
            }

            var eventHandled = await _uploadThingService.UploadThingEventHandlerAsync(root);
            if (!eventHandled)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
