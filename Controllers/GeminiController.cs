using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YourNamespace.Models; // Use your actual namespace
using Microsoft.Extensions.Configuration;
using charac.ViewModels;


namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeminiController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;

        public GeminiController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = config["Gemini:ApiKey"];
        }

        
        [HttpPost("GenerateText")]
        public async Task<IActionResult> GenerateText([FromBody] CompleteInfoViewModel model)
        {
            // Create the prompt based on the passed model
            var prompt = $"Subject: {model.SubName}\n" +
                         $"Genre: {model.SubGenre}\n" +
                         $"Acts: Act 1: {model.ActOne}, Act 2: {model.ActTwo}, Act 3: {model.ActThree}\n" +
                         $"Characters: ";

            foreach (var character in model.Characters)
            {
                // Check if briefDescription is not null and use it accordingly
                string briefDesc = character.briefDescription ?? "No biography available";
                prompt += $"\n- {character.chName} ({(character.isNegative ? "Antagonist" : "Protagonist")})\n" +
                          $"  Description: {character.chDescription}\n" +
                          $"  Biography: {briefDesc}";
            }

            // Add instruction to generate story at the end
            prompt += "\nGenerate a story based on the details above.";

            // Send the prompt to Gemini API
            var client = _httpClientFactory.CreateClient();
            var request = new GeminiRequest
            {
                contents = new List<Content>
        {
            new Content
            {
                parts = new List<Part>
                {
                    new Part { text = prompt }
                }
            }
        }
            };

            var jsonRequest = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var url = $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-pro:generateContent?key={_apiKey}";

            var response = await client.PostAsync(url, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse);
                var generatedText = result?.candidates?[0]?.content?.parts?[0]?.text ?? "No response";
                return Json(new { success = true, response = generatedText });
            }

            return Json(new { success = false, message = "Failed to get response from Gemini API." });
        }


    }

}
