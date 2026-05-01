using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DevEval.IntegrationTests.Fixtures;
using DevEval.IntegrationTests.Infrastructure;

namespace DevEval.IntegrationTests.Scenarios.Products;

[Collection(IntegrationTestCollection.Name)]
public sealed class ProductsContractTests : IntegrationTestBase
{
    public ProductsContractTests(IntegrationTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetProducts_WhenRequestIsUnauthenticated_ShouldReturn401()
    {
        using var client = CreateApiClient();

        var response = await client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WhenPayloadIsValid_ShouldReturn201LocationAndBody()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var payload = new
        {
            title = "Integration Test Product",
            price = 149.90m,
            description = "Product created by integration tests",
            category = "integration-tests",
            image = "https://example.com/product.png",
            rating = new
            {
                rate = 4.5,
                count = 12
            }
        };

        var response = await client.PostAsJsonAsync("/api/products", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var createdProduct = await response.Content.ReadFromJsonAsync<ProductResponse>();

        Assert.NotNull(createdProduct);
        Assert.True(createdProduct.Id > 0);
        Assert.Equal(payload.title, createdProduct.Title);
        Assert.Equal(payload.price, createdProduct.Price);
        Assert.Equal(payload.description, createdProduct.Description);
        Assert.Equal(payload.category, createdProduct.Category);
        Assert.Equal(payload.image, createdProduct.Image);
        Assert.NotNull(createdProduct.Rating);
        Assert.Equal(payload.rating.rate, createdProduct.Rating.Rate);
        Assert.Equal(payload.rating.count, createdProduct.Rating.Count);
        Assert.EndsWith($"/api/products/{createdProduct.Id}", response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetProducts_WhenDatabaseIsResetForTest_ShouldReturnEmptyList()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedResponse<ProductResponse>>();

        Assert.NotNull(paginatedResult);
        Assert.Empty(paginatedResult.Data);
        Assert.Equal(0, paginatedResult.TotalItems);
        Assert.Equal(1, paginatedResult.CurrentPage);
        Assert.Equal(0, paginatedResult.TotalPages);
    }

    private sealed record ProductResponse(
        int Id,
        string Title,
        decimal Price,
        string Description,
        string Category,
        string Image,
        RatingResponse Rating);

    private sealed record RatingResponse(double Rate, int Count);

    private sealed record PaginatedResponse<T>(
        [property: JsonPropertyName("data")] IReadOnlyList<T> Data,
        int TotalItems,
        int CurrentPage,
        int TotalPages);
}
