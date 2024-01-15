using AuctionService.Controllers;
using AuctionService.Data.Repositories;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoFixture;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTests;

public class AuctionControllerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepo;
    private readonly Mock<IPublishEndpoint> _publishEndpoint;
    private readonly Fixture _fixture;
    private readonly AuctionsController _controller;
    private readonly IMapper _mapper;
    
    // each test gets what we put in the constructor (the constructor is re-run for each)
    public AuctionControllerTests()
    {
        _fixture = new Fixture();
        _auctionRepo = new Mock<IAuctionRepository>();
        _publishEndpoint = new Mock<IPublishEndpoint>();

        var mockMapper = new MapperConfiguration(mc =>
        {
            mc.AddMaps(typeof(MappingProfiles));
        }).CreateMapper().ConfigurationProvider;

        _mapper = new Mapper(mockMapper);

        _controller = new AuctionsController(_auctionRepo.Object, _mapper, _publishEndpoint.Object)
        {
            ControllerContext = 
            {
                HttpContext = new DefaultHttpContext { User = Helpers.GetClaimsPrincipal() }
            }
        };
    }

    [Fact]
    public async Task GetAllAuctions_WithNoParams_Returns10Auctions()
    {
        // arrange
        var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
        
        _auctionRepo.Setup(repo => repo.GetAuctionsAsync(null))
            .ReturnsAsync(auctions);

        // act
        var result = await _controller.GetAllAuctions(null);
        
        // assert
        Assert.Equal(10, result.Value.Count);
        Assert.IsType<ActionResult<List<AuctionDto>>>(result);
    }

    [Fact]
    public async Task GetAuctionById_WithValidGuid_ReturnsAuction()
    {
        // arrange
        var auction = _fixture.Create<AuctionDto>();
        
        _auctionRepo.Setup(repo => repo.GetAuctionByIdAsync(auction.Id))
            .ReturnsAsync(auction);
        
        // act
        var result = await _controller.GetAuctionById(auction.Id);
        
        // assert
        Assert.Equal(auction.Id, result.Value.Id);
        Assert.IsType<ActionResult<AuctionDto>>(result);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidGuid_ReturnsNotFound()
    {
        // arrange
        _auctionRepo.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);
        
        // act
        var result = await _controller.GetAuctionById(Guid.NewGuid());
        
        // assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAuction_WithValidCreateAuctionDto_ReturnsCreatedAtAction()
    {
        // arrange
        var auctionDto = _fixture.Create<CreateAuctionDto>();
        
        _auctionRepo.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

        // act
        var result = await _controller.CreateAuction(auctionDto);
        // have to cast here to access ActionName
        var narrowedResult = result.Result as CreatedAtActionResult;

        // assert 
        Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("GetAuctionById", narrowedResult?.ActionName);
        Assert.IsType<AuctionDto>(narrowedResult?.Value);
    }
    
    [Fact]
    public async Task CreateAuction_FailedSave_Returns400BadRequest()
    {
        // arrange
        var auctionDto = _fixture.Create<CreateAuctionDto>();
    
        _auctionRepo.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);
    
        // act
        var result = await _controller.CreateAuction(auctionDto);
    
        // assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidGuid_ReturnsNotFound()
    {
        // arrange
        var id = new Guid();
        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();
        
        _auctionRepo
            .Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        // act
        var result = await _controller.UpdateAuction(id, updateAuctionDto);

        // assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidIdentity_ReturnsForbid()
    {
        // arrange
        var id = new Guid();
        var updateAuctionDto = new UpdateAuctionDto();
        
        var entity = _fixture.Build<Auction>().Without(x => x.Item).Create();
        entity.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();

        _auctionRepo
            .Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(entity);
        
        // act 
        var result = await _controller.UpdateAuction(id, updateAuctionDto);
        
        // assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task UpdateAuction_WithValidIdentityAndId_ReturnsOk()
    {
        // arrange
        var id = new Guid();
        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();

        var entity = _fixture.Build<Auction>().Without(x => x.Item).Create();
        entity.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        entity.Seller = Helpers.GetClaimsPrincipal().Identity?.Name;
        
        _auctionRepo
            .Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(entity);
        _auctionRepo
            .Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(true);
        
        // act
        var result = await _controller.UpdateAuction(id, updateAuctionDto);
        
        // assert
        Assert.IsType<OkResult>(result);
    }
    
    [Fact]
    public async Task DeleteAuction_WithInvalidId_ReturnsNotFound()
    {
        // arrange
        var id = new Guid();
        
        _auctionRepo
            .Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        // act
        var result = await _controller.DeleteAuction(id);

        // assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAuction_WithInvalidIdentity_ReturnsNotFound()
    {
        // arrange
        var id = new Guid();
        
        var entity = _fixture.Build<Auction>().Without(x => x.Item).Create();
        entity.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
        
        _auctionRepo
            .Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(entity);

        // act
        var result = await _controller.DeleteAuction(id);

        // assert
        Assert.IsType<ForbidResult>(result.Result);
    }
    
    [Fact]
    public async Task DeleteAuction_WithUnsuccessfulSave_ReturnsBadRequest()
    {
        // arrange
        var id = new Guid();
        
        var entity = _fixture.Build<Auction>().Without(x => x.Item).Create();
        entity.Seller = Helpers.GetClaimsPrincipal().Identity?.Name;

        _auctionRepo
            .Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(entity);
        _auctionRepo
            .Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(false);

        // act
        var result = await _controller.DeleteAuction(id);

        // assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
    
    [Fact]
    public async Task DeleteAuction_WithValidIdentityAndId_ReturnsOk()
    {
        // arrange
        var id = new Guid();
        
        var entity = _fixture.Build<Auction>().Without(x => x.Item).Create();
        entity.Seller = Helpers.GetClaimsPrincipal().Identity?.Name;

        _auctionRepo
            .Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(entity);
        _auctionRepo
            .Setup(repo => repo.SaveChangesAsync())
            .ReturnsAsync(true);

        // act
        var result = await _controller.DeleteAuction(id);

        // assert
        Assert.IsType<OkResult>(result.Result);
    }
}