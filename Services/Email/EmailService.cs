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
            await SendEmail(user.Email!, 
                $"Hello {user.UserName}", 
                "This is a test message");
        }

        public async Task SendForgotPasswordEmail(User user, string token)
        {
            await SendEmail(user.Email!,
                "Your password reset link", 
                $"Here's your link: {Environment.GetEnvironmentVariable("cors-client-https-url")}/reset-password/{token}"
                );
        }

        private async Task SendEmail(string email, string subject, string message)
        {
            var request = new RestRequest();
            request.AddParameter("domain", "mail.nxtgm.com", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "NextGame <noreply@nxtgm.com>");
            request.AddParameter("to", email);
            request.AddParameter("subject", subject);
            request.AddParameter("text", message);
            request.Method = Method.Post;
            var response = _restClient.Execute(request);
        }
    }
}
