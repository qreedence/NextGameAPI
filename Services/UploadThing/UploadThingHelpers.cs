using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NextGameAPI.Services.UploadThing
{
    public class UploadThingHelpers
    {
        public UploadThingToken DecodeToken(string encodedToken)
        {
            var base64EncodedBytes = Convert.FromBase64String(encodedToken);
            var jsonString = Encoding.UTF8.GetString(base64EncodedBytes);
            var token = JsonSerializer.Deserialize<UploadThingToken>(jsonString);

            if (token == null || string.IsNullOrEmpty(token.AppId) || string.IsNullOrEmpty(token.ApiKey) || token.Regions.Count <= 0)
            {
                throw new Exception("Invalid UploadThing token format");
            }
            return token;
        }

        public string DecodeUploadThingSignature(string data, string apiKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return hashString;
            }
        }
    }
}
