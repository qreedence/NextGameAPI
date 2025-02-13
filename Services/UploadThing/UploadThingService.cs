using Microsoft.AspNetCore.Identity;
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
        private readonly HttpClient _httpClient;

        public UploadThingService(UploadThingHelpers uploadThingHelpers, UserManager<User> userManager, IUserSettings userSettingsRepo)
        {
            _uploadThingHelpers = uploadThingHelpers;
            _uploadThingToken = _uploadThingHelpers.DecodeToken(Environment.GetEnvironmentVariable("uploadthing-token"));
            _userManager = userManager;
            _userSettingsRepo = userSettingsRepo;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-uploadthing-api-key", _uploadThingToken.ApiKey);
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

        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string uri, StringContent content)
        {
            var request = new HttpRequestMessage(method, uri);
            request.Content = content;
            return await _httpClient.SendAsync(request);
        }

        public async Task<Dictionary<string, string>> GetPresignedUrlAsync(StringContent content)
        {
            var requestUri = "https://api.uploadthing.com/v7/prepareUpload";
            var response = await SendRequestAsync(HttpMethod.Post, requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Content.ReadAsStream());
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public async Task<bool> RegisterUploadAsync(string fileKey, string userName, string slug)
        {
            var requestUri = $"https://{_uploadThingToken.Regions.FirstOrDefault()}.ingest.uploadthing.com/route-metadata";
            var payload = new
            {
                fileKeys = new[] { fileKey },
                metadata = new
                {
                    user = userName,
                    slug = slug
                },
                callbackUrl = $"https://{Environment.GetEnvironmentVariable("api-server-url")}/api/uploadthing/callback",
                callbackSlug = slug,
                awaitServerData = false,
                isDev = false
            };
            var registerContent = ConvertAnonymousObjectToStringContent(payload);

            var response = await SendRequestAsync(HttpMethod.Post, requestUri, registerContent);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteImageAsync(string fileKey)
        {
            var requestUri = "https://api.uploadthing.com/v6/deleteFiles";
            var content = ConvertAnonymousObjectToStringContent(new { fileKeys = new[] { fileKey } });
            var response = await SendRequestAsync(HttpMethod.Post, requestUri, content);
            return response.IsSuccessStatusCode;
        }

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
            return ConvertAnonymousObjectToStringContent(uploadThingPrepareUploadRequest);
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
                    var settings = await _userSettingsRepo.GetUserSettingsByUserIdAsync(user.Id);
                    if (settings != null)
                    {
                        if (!string.IsNullOrEmpty(settings.AvatarFileKey))
                        {
                            var deleteImage = await DeleteImageAsync(settings.AvatarFileKey);
                            if (!deleteImage)
                            {
                                return false;
                            }
                        }
                        settings.Avatar = fileUrl;
                        settings.AvatarFileKey = fileKey;
                        await _userSettingsRepo.UpdateUserSettings(settings);
                        return true;
                    }
                }
            }
            return false;
        }

        private StringContent ConvertAnonymousObjectToStringContent(object payload)
        {
            return new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        }

        #endregion
    }
}