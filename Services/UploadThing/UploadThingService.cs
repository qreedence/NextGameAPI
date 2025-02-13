using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using NextGameAPI.DTOs;
using System.Text;
using System.Text.Json;

namespace NextGameAPI.Services.UploadThing
{
    public class UploadThingService
    {
        private readonly UploadThingToken _uploadThingToken;
        private readonly UploadThingHelpers _uploadThingHelpers;
        private readonly UserManager<User> _userManager;
        private readonly IUserSettings _userSettingsRepo;

        public UploadThingService(UploadThingHelpers uploadThingHelpers, UserManager<User> userManager, IUserSettings userSettingsRepo)
        {
            _uploadThingHelpers = uploadThingHelpers;
            _uploadThingToken = _uploadThingHelpers.DecodeToken(Environment.GetEnvironmentVariable("uploadthing-token"));
            _userManager = userManager;
            _userSettingsRepo = userSettingsRepo;
        }

        #region Validators
        public bool checkActionType(string actionType, string requestActionType) => requestActionType == actionType;

        public bool checkAllowedFileType(string requestType, string allowedTypesString)
        {
            if (!string.IsNullOrEmpty(requestType) && !string.IsNullOrEmpty(allowedTypesString))
            {
                var allowedTypes = allowedTypesString.Split(',');
                if (!allowedTypes.Contains(requestType))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        #endregion

        #region Requests
        public StringContent ConvertRequestToStringContent(FileUploadRequest request, string slug)
        {
            var uploadThingPrepareUploadRequest = new UploadThingPrepareUploadRequest
            {
                Filename = request.Name,
                Filesize = request.Size,
                Slug = slug,
                Filetype = request.Type,
                ExpiresIn = 3600,
            };
            var jsonPayload = JsonSerializer.Serialize(uploadThingPrepareUploadRequest);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            return content;
        }

        public async Task<Dictionary<string, string>> GetPresignedUrlAsync(StringContent content)
        {
            var uploadThingResponse = new Dictionary<string, string>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-uploadthing-api-key", _uploadThingToken.ApiKey);
                var uploadThingUrl = "https://api.uploadthing.com/v7/prepareUpload";
                var response = await client.PostAsync(uploadThingUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error calling UploadThing API: {response.StatusCode} - {errorContent}");
                    return null;
                }
                uploadThingResponse = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(await response.Content.ReadAsStreamAsync());
                if (uploadThingResponse == null)
                {
                    return null;
                }
            }
            return uploadThingResponse;
        }

        public async Task<bool> RegisterUploadAsync(string fileKey, string userName, string slug)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-uploadthing-api-key", _uploadThingToken.ApiKey);

                var registerUploadUrl = $"https://{_uploadThingToken.Regions.FirstOrDefault()}.ingest.uploadthing.com/route-metadata";
                var registerUploadPayload = new
                {
                    fileKeys = new[] { fileKey },
                    metadata = new
                    {
                        user = userName,
                        slug = slug,
                    },
                    callbackUrl = $"https://{Environment.GetEnvironmentVariable("api-server-url")}/api/uploadthing/callback",
                    callbackSlug = slug,
                    awaitServerData = false,
                    isDev = false
                };

                var registerContent = new StringContent(JsonSerializer.Serialize(registerUploadPayload), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(registerUploadUrl, registerContent);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error registering upload: {response.StatusCode} - {errorContent}");
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region Callback Actions

        public JsonElement GetJsonElementFromRequestBody(string requestBody)
        {
            JsonDocument jsonDocument;
            try
            {
                jsonDocument = JsonDocument.Parse(requestBody);
            }
            catch (JsonException ex)
            {
                Console.WriteLine(ex.Message );
                return default;
            }
            return jsonDocument.RootElement;
        }

        public bool IsSignatureVerified(string hook, string signature, string requestBody)
        {
            if (string.IsNullOrEmpty(hook) | string.IsNullOrEmpty(signature) | string.IsNullOrEmpty(requestBody))
            {
                return false;
            }
            string expectedSignature = _uploadThingHelpers.DecodeUploadThingSignature(requestBody, _uploadThingToken.ApiKey);
            if (signature != $"hmac-sha256={expectedSignature}")
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UploadThingEventHandlerAsync(JsonElement root)
        {
            var status = root.GetProperty("status").GetString();

            if (status == "uploaded")
            {
                var handleUpload = await HandleUploadAsync(root);
                if (handleUpload)
                {
                    return true;
                }
                return false;
            }

            return false;
        }

        #endregion

        #region Private Methods
        private async Task<bool> HandleUploadAsync(JsonElement root)
        {
            var file = root.GetProperty("file");
            var fileKey = file.GetProperty("key").GetString();
            var fileName = file.GetProperty("name").GetString();
            var fileSize = file.GetProperty("size").GetInt64();
            var fileUrl = file.GetProperty("url").GetString();

            var metadata = root.GetProperty("metadata");
            var userName = metadata.GetProperty("user").GetString();
            var slug = metadata.GetProperty("slug").GetString();

            if (slug == "profilePicture" && !string.IsNullOrEmpty(userName))
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    var settings = await _userSettingsRepo.GetUserSettingsDTOByUserIdAsync(user.Id);
                    if (settings != null)
                    {
                        settings.Avatar = fileUrl;
                        await _userSettingsRepo.UpdateUserSettings(settings);
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion
    }
}
    

