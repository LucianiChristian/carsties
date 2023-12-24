using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Services;

public class AuctionSvcHttpClient(HttpClient httpClient)
{
    public async Task<List<Item>> GetItemsForSearchDb()
    {
        var dateOfLastUpdatedItem = await DB.Find<Item, string>()
            .Sort(options => options.Descending(x => x.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString())
            .ExecuteFirstAsync();

        return await httpClient.GetFromJsonAsync<List<Item>>("/api/auctions?date=" + dateOfLastUpdatedItem);
    }
}