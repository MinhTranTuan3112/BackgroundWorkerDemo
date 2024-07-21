using Microsoft.EntityFrameworkCore;
using Repos;
using Repos.Interfaces;
using Repos.Repositories;
using Services;
using Services.Interfaces;
using Services.Services;
using StackExchange.Redis;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped(typeof(IRedisCacheService<>), typeof(RedisCacheService<>));
builder.Services.AddHostedService<CacheRefresherBackgroundWorker>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:LocalConnection"]!;
    options.InstanceName = configuration["Redis:InstanceName"]!;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
