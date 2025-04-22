using NextGameAPI.Constants;

namespace NextGameAPI.DTOs.Games
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Developer { get; set; }
        public bool Publisher {  get; set; }
        public bool Supporting { get; set; }
        public bool Porting { get; set; }
    }
}
