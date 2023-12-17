using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController(AuctionDbContext dbContext, IMapper mapper) : ControllerBase
{
   [HttpGet]
   public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
   {
      var auctions = await dbContext.Auctions
         .Include(s => s.Item)
         .OrderBy(x => x.Item.Make)
         .ToListAsync();

      return mapper.Map<List<AuctionDto>>(auctions);
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

      dbContext.Auctions.Add(auction);

      var saveSuccessful = await dbContext.SaveChangesAsync() > 0;

      if (!saveSuccessful)
      {
         return BadRequest("Could not save changes to the DB");
      }

      return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, mapper.Map<AuctionDto>(auction));
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

      var saveSuccessful = await dbContext.SaveChangesAsync() > 0;
      
      return saveSuccessful ? 
         Ok() : BadRequest("Could not save changes to the DB");
   }
}