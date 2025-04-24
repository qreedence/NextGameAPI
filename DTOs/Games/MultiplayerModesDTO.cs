using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class MultiplayerModesDTO
    {
        [JsonPropertyName("campaigncoop")]
        public bool CampaignCoop { get; set; }
        [JsonPropertyName("dropin")]
        public bool DropIn { get; set; }
        [JsonPropertyName("lancoop")]
        public bool LanCoop { get; set; }
        [JsonPropertyName("offlinecoop")]
        public bool OfflineCoop { get; set; }
        [JsonPropertyName("offlinecoopmax")]
        public int OfflineCoopMax { get; set; }
        [JsonPropertyName("offlinemax")]
        public int OfflineMax { get; set; }
        [JsonPropertyName("onlinecoop")]
        public bool OnlineCoop { get; set; }
        [JsonPropertyName("onlinecoopmax")]
        public int OnlineCoopMax { get; set; }
        [JsonPropertyName("onlinemax")]
        public int OnlineMax { get; set; }
        [JsonPropertyName("splitscreen")]
        public bool SplitScreen { get; set; }
    }
}