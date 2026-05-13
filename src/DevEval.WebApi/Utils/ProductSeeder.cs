using DevEval.Application.Products.Commands;
using DevEval.Application.Products.Dtos;
using DevEval.Common.Helpers.Pagination;
using DevEval.Domain.Repositories;

namespace DevEval.WebApi.Utils
{
    public static class ProductSeeder
    {
        public static async Task EnsureProductsExist(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
            var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();

            var existing = await repository.GetAllAsync(new PaginationParameters { Page = 1, PageSize = 1 });
            if (existing.TotalItems > 0)
            {
                Console.WriteLine("Products already exist, skipping seed.");
                return;
            }

            var products = new List<CreateProductCommand>
            {
                new()
                {
                    Title = "Fjallraven - Foldsack No. 1 Backpack",
                    Price = 109.95m,
                    Description = "Your perfect pack for everyday use and walks in the forest. Stash your laptop (up to 15 inches) in the padded sleeve, your everyday.",
                    Category = "men's clothing",
                    Image = "https://fakestoreapi.com/img/81fAn1sPlL._AC_UX679_.jpg",
                    Rating = new RatingDto { Rate = 3.9, Count = 120 }
                },
                new()
                {
                    Title = "Mens Casual Premium Slim Fit T-Shirts",
                    Price = 22.30m,
                    Description = "Slim-fitting style, contrast raglan long sleeve, three-button henley placket, light weight & soft fabric for breathable and comfortable wearing.",
                    Category = "men's clothing",
                    Image = "https://fakestoreapi.com/img/71-3HjGNDUL._AC_SY879._SX._UX._SY._UY_.jpg",
                    Rating = new RatingDto { Rate = 4.1, Count = 259 }
                },
                new()
                {
                    Title = "Womens Casual Chic Jacket",
                    Price = 56.99m,
                    Description = "95% POLYESTER, 5% SPANDEX, (partially recycleable), with pockets and adjustable waist for a perfect fit.",
                    Category = "women's clothing",
                    Image = "https://fakestoreapi.com/img/71HblAHs1xL._AC_UY879_-2.jpg",
                    Rating = new RatingDto { Rate = 2.6, Count = 235 }
                }
            };

            foreach (var command in products)
            {
                var result = await mediator.Send(command, CancellationToken.None);
                if (result.IsSuccess)
                    Console.WriteLine($"Product '{command.Title}' created.");
                else
                    Console.WriteLine($"Failed to create product '{command.Title}': {string.Join(", ", result.Errors.Select(e => e.Message))}");
            }
        }
    }
}
