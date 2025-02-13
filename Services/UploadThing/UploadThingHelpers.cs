using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sprache;
using Sqids;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private int Djb2(string str)
        {
            int hash = 5381;
            foreach (char c in str)
            {
                hash = ((hash << 5) + hash) ^ c;
            }
            return hash & 0x7FFFFFFF;
        }

        public string Shuffle(string str, string seed)
        {
            char[] chars = str.ToCharArray();
            int seedNum = Djb2(seed);

            char temp;
            int j;
            for (int i = 0; i < chars.Length; i++)
            {
                j = ((seedNum % (i + 1)) + i) % chars.Length;
                temp = chars[i];
                chars[i] = chars[j];
                chars[j] = temp;
            }

            return new string(chars);
        }

        public string GenerateKey(string fileSeed)
        {
            var sqids = new SqidsEncoder<int>(new SqidsOptions
            {
                Alphabet = ShuffleAlphabet("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", fileSeed),  // Default URL-safe alphabet
                MinLength = 12
            });

            string encodedFileSeed = EncodeBase64(fileSeed);
            return $"{sqids.Encode(Math.Abs(Djb2(fileSeed)))}{encodedFileSeed}";
        }

        public string HmacSha256(string url, string apiKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(url)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(apiKey));
                return Convert.ToBase64String(hash)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .TrimEnd('=');
            }
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

        private string EncodeBase64(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText))
           .TrimEnd('=')
           .Replace('+', '-')
           .Replace('/', '_');
        }

        private string ShuffleAlphabet(string alphabet, string seed)
        {
            char[] chars = alphabet.ToCharArray();
            int seedValue = Djb2(seed);
            for (int i = chars.Length - 1; i > 0; i--)
            {
                int j = Math.Abs(seedValue % (i + 1));
                (chars[i], chars[j]) = (chars[j], chars[i]); 
                seedValue = ((seedValue * 33) ^ chars[i]);
            }

            return new string(chars);
        }
    }
}
