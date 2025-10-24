namespace Base.LayoutCalculator.Common.Bases.Base6;

public class BuildingMaterials : Tile
{
    public const string TileName = nameof(BuildingMaterials);

    public override string Name => TileName;
    public override string Category => TileName;

    public override char Display => 'A';

    public override void ApplyMultipliers(double[] multipliers, int zOffset, int columns, int x, int y)
    {

    }

    public override double GetProduction()
    {
        return 1;
    }
}
