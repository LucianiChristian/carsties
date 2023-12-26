using Contracts;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers;

public class AuctionDeletedConsumer() : IConsumer<AuctionDeleted> 
{
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        Console.WriteLine("--> Consuming auction deleted: " + context.Message.ToJson());
        
        var item = context.Message;

        var result = await DB.DeleteAsync<Item>(item.Id);

        if (!result.IsAcknowledged)
        {
            throw new MessageException(typeof(AuctionDeleted), "Problem deleting auction");
        }
    }
}