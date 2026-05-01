using Microsoft.AspNetCore.Mvc;

namespace DevEval.WebApi.Requests
{
    public class ProductQueryRequest : PaginationRequest
    {
        [FromQuery(Name = "title")]
        public string? Title { get; set; }

        [FromQuery(Name = "category")]
        public string? Category { get; set; }

        [FromQuery(Name = "price")]
        public decimal? Price { get; set; }

        [FromQuery(Name = "_minPrice")]
        public decimal? MinPrice { get; set; }

        [FromQuery(Name = "_maxPrice")]
        public decimal? MaxPrice { get; set; }
    }
}
