using AuctionService.Consumers;
using AuctionService.Data;
using AuctionService.Data.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<AuctionDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(config =>
{ 
    config.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        o.UsePostgres();
        o.UseBusOutbox();
    }); 
    
    config.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
    
    config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));
    
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMQ:Username", "guest"));
            host.Username(builder.Configuration.GetValue("RabbitMQ:Password", "guest"));
        });
        
        cfg.ReceiveEndpoint("auction-created-fault", endpointConfig =>
        {
           endpointConfig.UseMessageRetry(retryConfig => retryConfig.Interval(5, 5));
           
           endpointConfig.ConfigureConsumer<AuctionCreatedFaultConsumer>(context);
        });
        
        cfg.ReceiveEndpoint("bid-placed", endpointConfig =>
        { 
            endpointConfig.UseMessageRetry(retryConfig => retryConfig.Interval(5, 5));
           
            endpointConfig.ConfigureConsumer<BidPlacedConsumer>(context);
        });
        
        cfg.ReceiveEndpoint("auction-finished", endpointConfig =>
        {
            endpointConfig.UseMessageRetry(retryConfig => retryConfig.Interval(5, 5));
            
            endpointConfig.ConfigureConsumer<AuctionFinishedConsumer>(context);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();
