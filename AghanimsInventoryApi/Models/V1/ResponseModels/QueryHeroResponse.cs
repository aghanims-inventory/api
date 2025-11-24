namespace AghanimsInventoryApi.Models.V1.ResponseModels;

public class QueryHeroResponse
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string DisplayName { get; set; }

    public string? IconUrl { get; set; }

    public string? ImageUrl { get; set; }

    public int AttributeId { get; set; }

    public int AttackTypeId { get; set; }

    public required string FormattedAttribute { get; set; }

    public required string FormattedAttackType { get; set; }
}
