using AuctionService.Data.Repositories;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController(
   IAuctionRepository repo, IMapper mapper, IPublishEndpoint publishEndpoint)
   : ControllerBase
{
   [HttpGet]
   public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
   {
      return await repo.GetAuctionsAsync(date);
   }

   [HttpGet("{id:Guid}")]
   public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
   {
      var auction = await repo.GetAuctionByIdAsync(id);

      if (auction is null)
      {
         return NotFound();
      }

      return auction;
   }
   
   [Authorize]
   [HttpPost]
   public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto dto)
   {
      var auction = mapper.Map<Auction>(dto);
      
      auction.Seller = User.Identity?.Name; 

      // Transaction on EFCore -> Auctions
      repo.AddAuction(auction);
      
      // Auto generated ID on auction is now present
      var newAuction = mapper.Map<AuctionDto>(auction);
      
      // Performs Transaction on EFCore -> Outbox, etc. IF no connection to MsgQueue 
      await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));
      
      // Then any changes from both are saved 
      var saveSuccessful = await repo.SaveChangesAsync();

      if (!saveSuccessful)
      {
         return BadRequest("Could not save changes to the DB");
      }

      return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, newAuction);
   }

   [Authorize]
   [HttpPut("{id:Guid}")]
   public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto dto)
   {
      var auction = await repo.GetAuctionEntityByIdAsync(id);

      if (auction is null)
      {
         return NotFound();
      }
      
      if (auction.Seller != User.Identity?.Name) return Forbid(); 

      auction.Item.Make = dto.Make ?? auction.Item.Make;
      auction.Item.Model = dto.Model ?? auction.Item.Model;
      auction.Item.Color = dto.Color ?? auction.Item.Color;
      auction.Item.Mileage = dto.Mileage ?? auction.Item.Mileage;
      auction.Item.Year = dto.Year ?? auction.Item.Year;

      var auctionUpdatedMessage = mapper.Map<AuctionUpdated>(auction);

      await publishEndpoint.Publish(auctionUpdatedMessage);

      var saveSuccessful = await repo.SaveChangesAsync();

      return saveSuccessful ? 
         Ok() : BadRequest("Could not save changes to the DB");
   }

   [Authorize]
   [HttpDelete("{id:Guid}")]
   public async Task<ActionResult<Guid>> DeleteAuction(Guid id)
   {
      var auction = await repo.GetAuctionEntityByIdAsync(id);

      if (auction is null)
      {
         return NotFound();
      }

      if (auction.Seller != User.Identity?.Name) return Forbid();

      repo.RemoveAuction(auction);
      
      await publishEndpoint.Publish(new AuctionDeleted { Id = id.ToString() });

      var saveSuccessful = await repo.SaveChangesAsync();
      
      return saveSuccessful ? 
         Ok() : BadRequest("Could not save changes to the DB");
   }
}