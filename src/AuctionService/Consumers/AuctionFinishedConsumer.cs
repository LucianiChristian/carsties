using AuctionService.Data;
using AuctionService.Entities;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Consumers;

public class AuctionFinishedConsumer(AuctionDbContext dbContext) : IConsumer<AuctionFinished>
{
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        var finishedAuction = context.Message;

        Console.WriteLine("--> Consuming AuctionFinished: " + finishedAuction.AuctionId);

        var auction = await dbContext.Auctions.FindAsync(finishedAuction.AuctionId);

        if (auction is null) throw new MessageException(typeof(AuctionFinished), "--> No auction found");

        if (finishedAuction.ItemSold)
        {
            auction.Winner = finishedAuction.Winner;
            auction.SoldAmount = finishedAuction.Amount;
        }

        auction.Status = auction.SoldAmount > auction.ReservePrice ? 
            Status.Finished : Status.ReserveNotMet;

        var saveOccurred = await dbContext.SaveChangesAsync() > 0;

        if (!saveOccurred)
        {
            throw new MessageException(typeof(AuctionFinished), "--> This message could not be saved");
        } 
    }
}