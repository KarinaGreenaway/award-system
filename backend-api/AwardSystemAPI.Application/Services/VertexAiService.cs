using System.Text;
using AwardSystemAPI.Application.Options;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AwardSystemAPI.Application.Services;

public interface IVertexAiService
{
    Task<string> GenerateNominationSummaryAsync(string nominationJson);
    Task<string> GenerateFeedbackSummaryAsync(string feedbackJson);
}

public class VertexAiService : IVertexAiService
{
    private readonly VertexAiOptions _opts;
    
    public VertexAiService(IOptions<VertexAiOptions> opts)
    {
        _opts = opts.Value 
                ?? throw new ArgumentNullException(nameof(opts), "VertexAiOptions not configured");
    }
    
    public async Task<string> GenerateNominationSummaryAsync(string nominationJson)
    {
        try
        {
            // Get the OAuth 2.0 access token using Application Default Credentials (ADC)
            var credential = await GoogleCredential.GetApplicationDefaultAsync();
            var accessToken = await ((ITokenAccess)credential).GetAccessTokenForRequestAsync();

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new
                            {
                                text =
                                    $"You are Stanley's Assistant. Write a summary of this nomination (which I will provide to you). Introduce yourself very quickly first then give the summary, I want the summary to be a short paragraph that can be split up into multiple sections but don't do any special formatting (so NO bold, italics etc, but do have new lines and can use - or numbers for lists). You have 200 words max. Here is the nomination:\n\n{nominationJson}"
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    responseModalities = new[] { "TEXT" },
                    temperature = 1,
                    maxOutputTokens = 8192,
                    topP = 0.95
                },
                safetySettings = new[]
                {
                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "OFF" },
                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "OFF" },
                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "OFF" },
                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "OFF" }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);

            var requestUri =
                $"https://{_opts.ApiEndpoint}/v1/projects/{_opts.ProjectId}/locations/{_opts.LocationId}/publishers/google/models/{_opts.ModelId}:streamGenerateContent";

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri),
                Headers =
                {
                    { "Authorization", $"Bearer {accessToken}" }
                },
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            var summary = VertexAiResponseParser.ParseVertexAiResponse(responseContent);

            return summary;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return "Sorry! No summary has been generated for you.";
        }
    }

    public async Task<string> GenerateFeedbackSummaryAsync(string feedbackJson)
    {
        try
        {
            // Get the OAuth 2.0 access token using Application Default Credentials (ADC)
            var credential = await GoogleCredential.GetApplicationDefaultAsync();
            var accessToken = await ((ITokenAccess)credential).GetAccessTokenForRequestAsync();

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new
                            {
                                text =
                                    $"You are Stanley's Assistant. Write a summary of this feedback (which I will provide to you). Introduce yourself very quickly first then give the summary with the overall feedback on each question, I want the summary to be a short paragraph, it can be split into different sections but don't do any special formatting (so NO bold, italics etc, but do have new lines and can use - or numbers for lists). You have 200 words max. Here is the feedback:\n\n{feedbackJson}"
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    responseModalities = new[] { "TEXT" },
                    temperature = 1,
                    maxOutputTokens = 8192,
                    topP = 0.95
                },
                safetySettings = new[]
                {
                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "OFF" },
                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "OFF" },
                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "OFF" },
                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "OFF" }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);

            var requestUri =
                $"https://{_opts.ApiEndpoint}/v1/projects/{_opts.ProjectId}/locations/{_opts.LocationId}/publishers/google/models/{_opts.ModelId}:streamGenerateContent";

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri),
                Headers =
                {
                    { "Authorization", $"Bearer {accessToken}" }
                },
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var summary = VertexAiResponseParser.ParseVertexAiResponse(responseContent);
            return summary;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return "Sorry! No summary has been generated for you.";
        }
    }
}