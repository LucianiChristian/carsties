using System.IO.MemoryMappedFiles;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController(
   AuctionDbContext dbContext, IMapper mapper, IPublishEndpoint publishEndpoint)
   : ControllerBase
{
   [HttpGet]
   public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
   {
      var query = dbContext.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

      if (!string.IsNullOrEmpty(date))
      {
         query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
      }
       
      return await query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync();
   }

   [HttpGet("{id:Guid}")]
   public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
   {
      var auction = await dbContext.Auctions
         .Include(s => s.Item)
         .FirstOrDefaultAsync(x => x.Id == id);

      if (auction is null)
      {
         return NotFound();
      }

      return mapper.Map<AuctionDto>(auction);
   }
   
   [HttpPost]
   public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto dto)
   {
      var auction = mapper.Map<Auction>(dto);
      
      // TODO: add current user as seller on auction

      // Transaction on EFCore -> Auctions
      dbContext.Auctions.Add(auction);
      // Auto generated ID on auction is now present
      var newAuction = mapper.Map<AuctionDto>(auction);
      
      // Performs Transaction on EFCore -> Outbox, etc. IF no connection to MsgQueue 
      await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));
      
      // Then any changes from both are saved 
      var saveSuccessful = await dbContext.SaveChangesAsync() > 0;

      if (!saveSuccessful)
      {
         return BadRequest("Could not save changes to the DB");
      }

      return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, newAuction);
   }

   [HttpPut("{id:Guid}")]
   public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto dto)
   {
      var auction = await dbContext.Auctions
         .Include(x => x.Item)
         .FirstOrDefaultAsync(x => x.Id == id);

      if (auction is null)
      {
         return NotFound();
      }
      
      // TODO: Check seller on auction == username

      auction.Item.Make = dto.Make ?? auction.Item.Make;
      auction.Item.Model = dto.Model ?? auction.Item.Model;
      auction.Item.Color = dto.Color ?? auction.Item.Color;
      auction.Item.Mileage = dto.Mileage ?? auction.Item.Mileage;
      auction.Item.Year = dto.Year ?? auction.Item.Year;

      var auctionUpdatedMessage = mapper.Map<AuctionUpdated>(auction);

      await publishEndpoint.Publish(auctionUpdatedMessage);

      var saveSuccessful = await dbContext.SaveChangesAsync() > 0;

      return saveSuccessful ? 
         Ok() : BadRequest("Could not save changes to the DB");
   }

   [HttpDelete("{id:Guid}")]
   public async Task<ActionResult<Guid>> DeleteAuction(Guid id)
   {
      var auction = await dbContext.Auctions
         .FirstOrDefaultAsync(x => x.Id == id);

      if (auction is null)
      {
         return NotFound();
      }

      dbContext.Auctions.Remove(auction);
      
      await publishEndpoint.Publish(new AuctionDeleted { Id = id.ToString() });

      var saveSuccessful = await dbContext.SaveChangesAsync() > 0;
      
      return saveSuccessful ? 
         Ok() : BadRequest("Could not save changes to the DB");
   }
}