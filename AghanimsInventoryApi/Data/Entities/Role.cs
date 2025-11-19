namespace AghanimsInventoryApi.Data.Entities;

public class Role
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public byte Order { get; set; }
}
