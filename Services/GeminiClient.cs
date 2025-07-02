using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;


namespace eCommerceLite.Services
{
    public class GeminiClient
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public GeminiClient(string apiKey)
        {
            _apiKey = apiKey;
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent")
            };
        }

        public async Task<string> AskAsync(string userMessage)
        {
            var requestBody = new
            {
                contents = new[]
                {
                new {
                    parts = new[] {
                        new { text = userMessage }
                    }
                }
            }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _http.PostAsync($"?key={_apiKey}", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            dynamic parsed = JsonConvert.DeserializeObject(result);

            return parsed.candidates[0].content.parts[0].text;
        }
    }
}


