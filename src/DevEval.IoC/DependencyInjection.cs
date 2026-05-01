using DevEval.Application.Sales.Services;
using DevEval.Common.Services;
using DevEval.Domain.Repositories;
using DevEval.IoC.Kafka;
using DevEval.ORM.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DevEval.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISaleRepository, SaleRepository>();

            services.AddSingleton<IPasswordService, PasswordService>();
            services.AddSingleton<ISaleCreatedEventProducer, KafkaSaleCreatedEventProducer>();
            services.AddScoped<ISaleEventPublisher, SaleEventPublisher>();

            return services;
        }
    }
}
