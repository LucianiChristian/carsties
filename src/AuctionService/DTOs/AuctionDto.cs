namespace AuctionService.DTOs;

public class AuctionDto
{
    public Guid Id { get; init; }
    public int ReservePrice { get; init; }
    public string Seller { get; init; }
    public string Winner { get; init; }
    public int SoldAmount { get; init; }
    public int CurrentHighBid { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime AuctionEnd { get; init; }
    public string Status { get; init; }
    
    public string Make { get; init; }
    public string Model { get; init; }
    public int Year { get; init; }
    public string Color { get; init; }
    public int Mileage { get; init; }
    public string ImageUrl { get; init; }
}
