using AuctionService.Consumers;
using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
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
        cfg.ReceiveEndpoint("auction-created-fault", x =>
        {
           x.ConfigureConsumer<AuctionCreatedFaultConsumer>(context);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

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
