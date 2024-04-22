using System.Text.Json.Serialization;

namespace Story_Spoiler_Exam_Api_tests.Models
{
    public class ApiResponseDTO
    {
        [JsonPropertyName("msg")]
        public string? Msg { get; set; }

        [JsonPropertyName("id")]
        public string? StoryId  { get; set; }
    }
}
