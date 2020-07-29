using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Shared.QnAMaker
{
    public class QnAMaker
    {
        private static HttpClient client = new HttpClient();
        private readonly IConfiguration configuration;

        public bool IsConfigured { get; private set; }

        public QnAMaker(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.IsConfigured = !string.IsNullOrEmpty(configuration.QnAMakerEndpoint()) && !string.IsNullOrEmpty(configuration.QnAMakerEndpointKey());
        }

        public async Task<string> GetAnswerFromQnAMaker(string question, CancellationToken cancellationToken)
        {
            var result = await QnAMakerToDataAsync(question, cancellationToken);
            return result?.Answers?.FirstOrDefault()?.Answer;
        }

        public async Task<QnAMakerApiResponse> QnAMakerToDataAsync(string text, CancellationToken cancellationToken)
        {
            var body = new Question(text) ;
            var requestBody = JsonConvert.SerializeObject(body);

            using (var request = new HttpRequestMessage())
            {
                var uri = this.configuration.QnAMakerEndpoint();
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Authorization", configuration.QnAMakerEndpointKey());

                var response = await client.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"The call to the QnAMaker cognitive bot returned HTTP status code {response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<QnAMakerApiResponse>(responseBody);
            }
        }

        public class Question
        {
            public string question { get; set; }
            public Question(string text) { this.question = text; }
        }
    }
}
