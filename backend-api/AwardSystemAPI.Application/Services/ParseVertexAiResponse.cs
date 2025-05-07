namespace AwardSystemAPI.Application.Services;
using Newtonsoft.Json.Linq;
using System;

public class VertexAiResponseParser
{
    public static string ParseVertexAiResponse(string responseJson)
    {
        try
        {
            var jsonResponse = JArray.Parse(responseJson);

            // extracting the candidates and concatenating the text parts
            var allText =
                (from parts in (from candidates in jsonResponse.Select(item => item["candidates"]).OfType<JToken>()
                        from candidate in candidates
                        select candidate["content"]?["parts"]).OfType<JToken>()
                    from part in parts
                    select part["text"].ToString()).ToList();

            // Joining all parts together to a single string (summary)
            var finalText = string.Join(" ", allText);

            return finalText;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing response: {ex.Message}");
            return string.Empty;
        }
    }
}
