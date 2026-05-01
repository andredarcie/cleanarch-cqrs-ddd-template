using System.Text.Json.Serialization;
using DevEval.Application.Carts.Dtos;
using FluentResults;
using MediatR;

namespace DevEval.Application.Carts.Commands
{
    public class CreateCartCommand : IRequest<Result<CartDto>>
    {
        /// <summary>
        /// The ID of the user who owns the cart.
        /// </summary>
        [JsonIgnore]
        public int UserId { get; set; }

        /// <summary>
        /// The date when the cart was created.
        /// </summary>
        [JsonIgnore]
        public DateTime Date { get; set; }

        /// <summary>
        /// The list of products in the cart.
        /// </summary>
        public List<CartProductDto> Products { get; set; } = new();
    }
}
