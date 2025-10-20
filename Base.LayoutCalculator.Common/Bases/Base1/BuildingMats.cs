namespace Base.LayoutCalculator.Common.Bases.Base1;

public class BuildingMats : Tile
{
    public const string TileName = nameof(BuildingMats);

    public override string Name => TileName;
    public override string Category => TileName;

    public override char Display => 'M';

    public override void ApplyMultipliers(double[,,] multiplierMap, int x, int y, int z)
    {
        
    }

    public override double GetProduction()
    {
        return 1;
    }
}
