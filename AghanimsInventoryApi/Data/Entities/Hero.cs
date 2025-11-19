namespace AghanimsInventoryApi.Data.Entities;

public class Hero
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string DisplayName { get; set; }

    public int Complexity { get; set; }

    public string? IconUrl { get; set; }

    public string? ImageUrl { get; set; }

    public byte AttributeId { get; set; }

    public byte AttackTypeId { get; set; }
}
