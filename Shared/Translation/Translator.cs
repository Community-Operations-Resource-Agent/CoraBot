using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Translation
{
    public class Translator
    {
        public static string DefaultLanguage = "en";

        private static HttpClient client = new HttpClient();
        private readonly IConfiguration configuration;

        public bool IsConfigured { get; private set; }


        public Translator(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.IsConfigured = !string.IsNullOrEmpty(configuration.TranslationSubscriptionKey()) && !string.IsNullOrEmpty(configuration.TranslationUrlFormat());
        }

        public async Task<string> TranslateAsync(string text, string targetLocale, CancellationToken cancellationToken = default)
        {
            var result = await TranslateToDataAsync(text, targetLocale, cancellationToken);
            return result?.Translations?.FirstOrDefault()?.Text;
        }

        public async Task<TranslatorApiResponse> TranslateToDataAsync(string text, string targetLocale, CancellationToken cancellationToken)
        {
            // From Cognitive Services translation documentation:
            // https://docs.microsoft.com/en-us/azure/cognitive-services/translator/quickstart-csharp-translate
            var body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var request = new HttpRequestMessage())
            {
                var uri = string.Format(this.configuration.TranslationUrlFormat(), targetLocale);
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", this.configuration.TranslationSubscriptionKey());

                var response = await client.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"The call to the translation service returned HTTP status code {response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TranslatorApiResponse[]>(responseBody)?.FirstOrDefault();
            }
        }
    }
}