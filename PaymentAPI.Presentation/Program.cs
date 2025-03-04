using PaymentAPI.Infrastructure.Services.Logging;
using PaymentAPI.Infrastructure.Services.Logging.Configuration;
using Sentry.AspNetCore;
using PaymentAPI.Infrastructure.Services.Logging.Middleware;
using PaymentAPI.Infrastructure.Databases.Configurations.MongoDb;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.WebHost.UseSentry();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

//builder.Services.AddMongoDb(builder.Configuration);

//builder.Services.AddApiLoggingService(builder.Configuration, options => 
//    builder.Configuration.GetSection("ApiLoggingSettings").Bind(options));

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

app.MapOpenApi();
app.MapScalarApiReference();

//app.UseMiddleware<ApiLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/");

app.Run();
