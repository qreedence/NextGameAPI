using NextGameAPI.Data.Models;
using RestSharp;
using RestSharp.Authenticators;
using System.Net.Http.Headers;
using System.Text;

namespace NextGameAPI.Services.Email
{
    public class EmailService
    {
        private readonly RestClient _restClient;

        public EmailService()
        {
            var options = new RestClientOptions($"https://api.eu.mailgun.net/v3")
            {
                Authenticator = new HttpBasicAuthenticator("api", Environment.GetEnvironmentVariable("mailgun-api-key"))
            };
            _restClient = new RestClient(options);
        }

        public async Task TestEmail(User user)
        {
            await SendEmail(user, "This is a test message");
        }

        private async Task SendEmail(User user, string message)
        {
            var request = new RestRequest();
            request.AddParameter("domain", "mail.nxtgm.com", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "NextGame <noreply@nxtgm.com>");
            request.AddParameter("to", user.Email);
            request.AddParameter("subject", $"Hello {user.UserName}");
            request.AddParameter("text", message);
            request.Method = Method.Post;
            var response = _restClient.Execute(request);
        }
    }
}
