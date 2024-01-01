using AuctionService.Data;
using AuctionService.Entities;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class BidPlacedConsumer(AuctionDbContext dbContext) : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        var placedBid = context.Message;

        Console.WriteLine("--> Consuming BidPlaced: " + placedBid.Id); 

        var auction = await dbContext.Auctions.FindAsync(placedBid.AuctionId);

        if (auction is null)
        {
            throw new MessageException(typeof(BidPlaced), "--> No auction found");
        }

        if (!IsBidSuccessful(placedBid, auction)) return;
            
        auction.CurrentHighBid = placedBid.Amount;

        var saveOccurred = await dbContext.SaveChangesAsync() > 0;

        if (!saveOccurred)
        {
            throw new MessageException(typeof(BidPlaced), "--> This message could not be saved");
        } 
    }

    private static bool IsBidSuccessful(BidPlaced placedBid, Auction auction)
    {
        if (auction.CurrentHighBid is null) return true;

        return placedBid.Amount > auction.CurrentHighBid && 
               placedBid.BidStatus.Contains("Accepted");
    } 
}