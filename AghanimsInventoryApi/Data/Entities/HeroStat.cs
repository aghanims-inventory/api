namespace AghanimsInventoryApi.Data.Entities;

public class HeroStat
{
    public int Id { get; set; }

    public decimal? Value { get; set; }

    public int StatId { get; set; }

    public int HeroId { get; set; }
}
