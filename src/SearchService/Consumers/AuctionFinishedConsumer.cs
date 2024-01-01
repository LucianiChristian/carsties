using Contracts;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        Console.WriteLine("--> Consuming auction finished using: " + context.Message.ToJson());
        
        var finishedAuction = context.Message;

        var item = await DB.Find<Item>().OneAsync(finishedAuction.AuctionId);
        
        if (item is null)
        {
            throw new MessageException(typeof(AuctionFinished), "--> No item found");
        }

        if (finishedAuction.ItemSold)
        {
            item.Winner = finishedAuction.Winner;
            item.SoldAmount = finishedAuction.Amount;
        }

        item.Status = "Finished";
        
        await item.SaveAsync();
    }
}