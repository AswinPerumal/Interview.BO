using BidOne.Gateway.API;
using BidOne.Gateway.API.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;

namespace BidOne.Gateway.Tests.IdempotencyTests
{
    public class ProductControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;


        public ProductControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task SameRequest_WithSameIdempotencyKey_ReturnsSameResponse()
        {
            // Arrange
            var payload = new CreateProductRequestDto { Id = 3, Name = "Test Product", Price = 123 };
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            string key = Guid.NewGuid().ToString();


            _client.DefaultRequestHeaders.Add("Idempotency-Key", key);


            // Act - First Request
            var firstResponse = await _client.PostAsync("/v1/products", content);
            var firstContent = await firstResponse.Content.ReadAsStringAsync();


            // Act - Second Request
            var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Remove("Idempotency-Key");
            _client.DefaultRequestHeaders.Add("Idempotency-Key", key);
            var secondResponse = await _client.PostAsync("/v1/products", content2);
            var secondContent = await secondResponse.Content.ReadAsStringAsync();


            // Assert
            Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
            Assert.Equal(firstContent, secondContent);
        }


        [Fact]
        public async Task ConcurrentRequests_WithSameIdempotencyKey_ReturnsProcessingMessage()
        {
            // Arrange
            var payload = new CreateProductRequestDto { Id = 3, Name = "Test Product", Price = 123 };
            string json = JsonSerializer.Serialize(payload);
            string key = Guid.NewGuid().ToString();


            // Set header
            _client.DefaultRequestHeaders.Add("Idempotency-Key", key);


            // Fire first request but do not await immediately
            var task1 = _client.PostAsync("/v1/products", new StringContent(json, Encoding.UTF8, "application/json"));

            // give the first request a chance to set IsProcessing
            await Task.Delay(5000);

            // Second request triggers before first completes
            var content2 = new StringContent(json, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Remove("Idempotency-Key");
            _client.DefaultRequestHeaders.Add("Idempotency-Key", key);
            var task2 = _client.PostAsync("/v1/products", content2);
           


            var firstResponse = await task1;
            var secondResponse = await task2;
            var secondContent = await secondResponse.Content.ReadAsStringAsync();


            // Assert
            Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
            Assert.Contains("still being processed", secondContent);
        }
    }
}
