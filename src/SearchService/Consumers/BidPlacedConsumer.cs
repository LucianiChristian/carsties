using Contracts;
using MassTransit;
using MongoDB.Bson;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("--> Consuming bid placed using: " + context.Message.ToJson());

        var placedBid = context.Message;

        var item = await DB.Find<Item>().OneAsync(placedBid.AuctionId);

        if (item is null)
        {
            throw new MessageException(typeof(BidPlaced), "--> No item found");
        }
        
        if(!IsBidSuccessful(placedBid, item)) return;

        item.CurrentHighBid = placedBid.Amount;

        await item.SaveAsync();
    }
    
    private static bool IsBidSuccessful(BidPlaced placedBid, Item auction)
    {
        if (auction.CurrentHighBid is null) return true;

        return placedBid.Amount > auction.CurrentHighBid && 
               placedBid.BidStatus.Contains("Accepted");
    } 
}