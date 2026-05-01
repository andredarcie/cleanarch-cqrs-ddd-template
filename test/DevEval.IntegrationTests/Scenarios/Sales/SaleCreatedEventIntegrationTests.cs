using System.Net;
using System.Net.Http.Json;
using DevEval.IntegrationTests.Fixtures;
using DevEval.IntegrationTests.Infrastructure;

namespace DevEval.IntegrationTests.Scenarios.Sales;

[Collection(IntegrationTestCollection.Name)]
public sealed class SaleCreatedEventIntegrationTests : IntegrationTestBase
{
    public SaleCreatedEventIntegrationTests(IntegrationTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task CreateSale_WhenPayloadIsValid_ShouldPublishSaleCreatedEvent()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var saleNumber = $"SALE-{Guid.NewGuid():N}";
        var payload = new
        {
            saleNumber,
            saleDate = DateTime.UtcNow.AddMinutes(-1),
            customerId = Guid.NewGuid(),
            customerName = "Integration Test Customer",
            branchId = Guid.NewGuid(),
            branchName = "Integration Test Branch",
            items = new[]
            {
                new
                {
                    productId = 101,
                    quantity = 4,
                    unitPrice = 25.5m,
                    discount = 0.1m
                }
            }
        };

        var response = await client.PostAsJsonAsync("/api/sales", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdSale = await response.Content.ReadFromJsonAsync<CreateSaleResponse>();
        Assert.NotNull(createdSale);

        var publishedEvent = Fixture.Factory.FakeSaleCreatedEventProducer.Published
            .SingleOrDefault(e => e.SaleNumber == saleNumber);

        Assert.NotNull(publishedEvent);
        Assert.Equal(createdSale.Id, publishedEvent.SaleId);
        Assert.Equal(saleNumber, publishedEvent.SaleNumber);
        Assert.Equal(payload.customerId, publishedEvent.CustomerId);
        Assert.Equal(payload.customerName, publishedEvent.CustomerName);
        Assert.Equal(payload.branchId, publishedEvent.BranchId);
        Assert.Equal(payload.branchName, publishedEvent.BranchName);
        Assert.Equal(createdSale.TotalAmount, publishedEvent.TotalAmount);
        Assert.False(publishedEvent.IsCancelled);
        Assert.Equal(1, publishedEvent.ItemsCount);
    }

    private sealed record CreateSaleResponse(Guid Id, decimal TotalAmount);
}
