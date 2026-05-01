using System.Text.Json.Serialization;

namespace DevEval.Common.Helpers.Pagination
{
    public class PaginatedResult<T>
    {
        [JsonPropertyName("data")]
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();

        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
