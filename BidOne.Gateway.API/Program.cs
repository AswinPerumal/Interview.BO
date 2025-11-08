using Asp.Versioning;
using BidOne.Gateway.API.Filters;
using BidOne.Gateway.API.Helpers;
using BidOne.Gateway.API.Mappings;
using BidOne.Gateway.API.Middlewares;
using BidOne.Gateway.Application.Implementation;
using BidOne.Gateway.Application.InfraInterfaces;
using BidOne.Gateway.Application.Interfaces;
using BidOne.Gateway.Cache;
using BidOne.Gateway.HttpClients;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
})
     .ConfigureApiBehaviorOptions(options =>
     {
         options.InvalidModelStateResponseFactory = context =>
         {
             var errors = context.ModelState.Values
                 .SelectMany(v => v.Errors)
                 .Select(e => e.ErrorMessage);
             var response = APIResponse.BadRequest(string.Join(",", errors));
             return new BadRequestObjectResult(response);
         };
     });


// Register AutoMapper
builder.Services.AddAutoMapper(x => x.AddMaps(typeof(ProductProfile).Assembly));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddHttpClient<IErpClient, ErpClient>();
builder.Services.AddHttpClient<IWarehouseClient, WarehouseClient>();
builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();
builder.Services.AddScoped<IdempotencyFilter>();

builder.Services.AddMemoryCache();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // adds API-Supported headers
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // v1, v2
    options.SubstituteApiVersionInUrl = true;
});

var jsonSerializerOptions = new JsonSerializerOptions()
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
};

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
