namespace AghanimsInventoryApi.Data.Entities;

public class HeroAttribute
{
    public int Id { get; set; }

    public int Health { get; set; }

    public int Mana { get; set; }

    public decimal BaseHealthRegen { get; set; }

    public decimal BaseManaRegen { get; set; }

    public int Strength { get; set; }

    public int Agility { get; set; }

    public int Intelligence { get; set; }

    public decimal StrengthPerLevel { get; set; }

    public decimal AgilityPerLevel { get; set; }

    public decimal IntelligencePerLevel { get; set; }

    public int HeroId { get; set; }
}
