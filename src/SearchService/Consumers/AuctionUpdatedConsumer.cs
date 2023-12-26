using System.Dynamic;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers;

public class AuctionUpdatedConsumer(IMapper mapper) : IConsumer<AuctionUpdated> 
{
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("--> Consuming auction updated using: " + context.Message.ToJson());

        var updatedItem = mapper.Map<Item>(context.Message); 

        var result = await DB.Update<Item>()
            .Match(x => x.ID == context.Message.Id)
            .ModifyOnly(x => new
            {
                x.ID,
                x.Make,
                x.Model,
                x.Year,
                x.Color,
                x.Mileage
            }, updatedItem)
            .ExecuteAsync();

        if (!result.IsAcknowledged)
        {
            throw new MessageException(typeof(AuctionUpdated), "--> Problem updating mongodb");
        }
    }
}