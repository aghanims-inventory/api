namespace AghanimsInventoryApi.Models.V1.ResponseModels;

public class GetHeroPageFilterResponse
{
    public List<GetHeroPageFilterAttributeResponse> AttributeTypes { get; set; } = new();

    public List<GetHeroPageFilterAttackTypeResponse> AttackTypes { get; set; } = new();

    public List<GetHeroPageFilterStatTypeResponse> StatTypes { get; set; } = new();
}

public class GetHeroPageFilterAttributeResponse
{
    public byte Id { get; set; }

    public required string Name { get; set; }
}

public class GetHeroPageFilterAttackTypeResponse
{
    public byte Id { get; set; }

    public required string Name { get; set; }
}

public class GetHeroPageFilterStatTypeResponse
{
    public byte Id { get; set; }

    public required string Name { get; set; }
}
