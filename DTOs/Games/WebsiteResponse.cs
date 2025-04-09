using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class WebsiteResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("trusted")]
        public bool Trusted { get; set; }
        [JsonPropertyName("type")]
        public WebsiteType WebsiteType { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public enum WebsiteType
    {
        Official = 1,
        Wikia = 2,
        Wikipedia = 3,
        Facebook = 4,
        Twitter = 5,
        Twitch = 6,
        Instagram = 8,
        Youtube = 9,
        Iphone = 10,
        Ipad = 11,
        Android = 12,
        Steam = 13,
        Reddit = 14,
        Itch = 15,
        Epicgames = 16,
        Gog = 17,
        Discord = 18,
        Bluesky = 19
    }
}
