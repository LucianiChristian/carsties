export type PagedResult<T> = {
    results: T[];
    pageCount: number;
    totalCount: number;
}

export enum PageSize {
    Four = "4",
    Eight = "8",
    Twelve = "12"
};

export enum AuctionOrdering {
    Make = "make",
    CreatedAt = "new",
    AuctionEnd = "auctionEnd"
} 

export enum AuctionFilter {
    Finished = "finished",
    EndingSoon = "endingSoon",
    Live = "live"
}

export type Auction = {
    reservePrice: number;
    seller: string;
    winner?: string;
    soldAmount: number;
    currentHighBid: number;
    createdAt: string;
    updatedAt: string;
    auctionEnd: string;
    status: string;
    make: string;
    model: string;
    year: number;
    color: string;
    mileage: number;
    imageUrl: string;
    id: string;
};