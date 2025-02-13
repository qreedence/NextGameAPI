namespace NextGameAPI.Services.UploadThing
{
    public class FileRoute
    {
        public string Slug { get; set; }
        public string AllowedFileTypes { get; set; }
        public long MaxFileSize { get; set; }
    }
}
