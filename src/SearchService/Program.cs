using System.Net;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<AuctionSvcHttpClient>(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration.GetValue<string>("AuctionServiceUrl"));
}).AddPolicyHandler(GetPolicy());

builder.Services.AddMassTransit(config =>
{
    config.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    
    config.SetEndpointNameFormatter((new KebabCaseEndpointNameFormatter("search", false)));
    
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("search-auction-created", endpointConfig =>
        {
           // Will be triggered if an exception is thrown from the consumer's `Consume` method 
           endpointConfig.UseMessageRetry(retryConfig => retryConfig.Interval(5, 5));
           
           endpointConfig.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });
        
        cfg.ReceiveEndpoint("search-auction-deleted", endpointConfig =>
        {
           // Will be triggered if an exception is thrown from the consumer's `Consume` method 
           endpointConfig.UseMessageRetry(retryConfig => retryConfig.Interval(5, 5));
           
           endpointConfig.ConfigureConsumer<AuctionDeletedConsumer>(context);
        });
        
        cfg.ReceiveEndpoint("search-auction-updated", endpointConfig =>
        {
           // Will be triggered if an exception is thrown from the consumer's `Consume` method 
           endpointConfig.UseMessageRetry(retryConfig => retryConfig.Interval(5, 5));
           
           endpointConfig.ConfigureConsumer<AuctionUpdatedConsumer>(context);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitDb(app);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
});


app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy() =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));