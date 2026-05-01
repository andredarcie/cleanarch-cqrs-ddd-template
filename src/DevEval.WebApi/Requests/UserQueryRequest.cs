using DevEval.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DevEval.WebApi.Requests
{
    public class UserQueryRequest : PaginationRequest
    {
        [FromQuery(Name = "username")]
        public string? Username { get; set; }

        [FromQuery(Name = "email")]
        public string? Email { get; set; }

        [FromQuery(Name = "status")]
        public UserStatus? Status { get; set; }

        [FromQuery(Name = "role")]
        public UserRole? Role { get; set; }
    }
}
