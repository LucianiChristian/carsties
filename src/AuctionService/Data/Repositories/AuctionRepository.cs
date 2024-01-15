using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data.Repositories;

public class AuctionRepository(AuctionDbContext dbContext, IMapper mapper) : IAuctionRepository
{
    public Task<List<AuctionDto>> GetAuctionsAsync(string date)
    {
        var query = dbContext.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if (!string.IsNullOrEmpty(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }

        return query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync();
    }

    public Task<AuctionDto> GetAuctionByIdAsync(Guid id)
    {
        return dbContext.Auctions
                .ProjectTo<AuctionDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<Auction> GetAuctionEntityByIdAsync(Guid id)
    {
        return dbContext.Auctions.FirstOrDefaultAsync(x => x.Id == id);
    }

    public void AddAuction(Auction auction)
    { 
        dbContext.Auctions.Add(auction);
    }

    public void RemoveAuction(Auction auction)
    {
        dbContext.Auctions.Remove(auction);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}