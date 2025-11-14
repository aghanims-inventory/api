namespace AghanimsInventoryApi.Data.Entities;

public class Hero
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Complexity { get; set; }

    public string ImageUrl { get; set; }

    public int AttributeId { get; set; }

    public int AttackTypeId { get; set; }
}
