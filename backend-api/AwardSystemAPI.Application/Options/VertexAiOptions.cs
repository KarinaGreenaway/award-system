namespace AwardSystemAPI.Application.Options
{
    public class VertexAiOptions
    {
        public string ProjectId     { get; set; } = "";
        public string LocationId    { get; set; } = "";
        public string ModelId       { get; set; } = "";
        public string ApiEndpoint   { get; set; } = "";
        public string CredentialsPath { get; set; } = "";
    }
}