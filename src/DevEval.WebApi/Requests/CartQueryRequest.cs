using Microsoft.AspNetCore.Mvc;

namespace DevEval.WebApi.Requests
{
    public class CartQueryRequest : PaginationRequest
    {
        [FromQuery(Name = "userId")]
        public int? UserId { get; set; }

        [FromQuery(Name = "_minDate")]
        public DateTime? MinDate { get; set; }

        [FromQuery(Name = "_maxDate")]
        public DateTime? MaxDate { get; set; }
    }
}
