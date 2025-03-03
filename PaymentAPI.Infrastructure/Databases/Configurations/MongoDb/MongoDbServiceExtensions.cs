using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace PaymentAPI.Infrastructure.Databases.Configurations.MongoDb;

public static class MongoDbServiceExtensions
{
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<MongoDatabaseSettings>? configureOptions = null)
    {
        // Configure MongoDB conventions
        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true)
        };
        ConventionRegistry.Register("CamelCase", conventionPack, type => true);

        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            // Register MongoDB settings
            services.Configure<MongoDbSettings>(options =>
                configuration.GetSection(nameof(MongoDbSettings)).Bind(options));
        }

        // Register MongoDB client as singleton
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = configuration
                .GetSection(nameof(MongoDbSettings))
                .Get<MongoDbSettings>();

            if (settings?.ConnectionString == null)
            {
                throw new InvalidOperationException(
                    "MongoDB connection string is not configured.");
            }

            return new MongoClient(settings.ConnectionString);
        });

        // Register MongoDB database
        services.AddSingleton(sp =>
        {
            var settings = configuration
                .GetSection(nameof(MongoDbSettings))
                .Get<MongoDbSettings>();

            if (settings?.DatabaseName == null)
            {
                throw new InvalidOperationException(
                    "MongoDB database name is not configured.");
            }

            var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(settings.DatabaseName);
        });
        

        return services;
    }
} 