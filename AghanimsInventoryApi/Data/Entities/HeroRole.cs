namespace AghanimsInventoryApi.Data.Entities;

public class HeroRole
{
    public int Id { get; set; }

    public byte Intensity { get; set; }

    public int HeroId { get; set; }

    public int RoleId { get; set; }
}
