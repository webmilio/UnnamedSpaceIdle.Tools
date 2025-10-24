namespace Base.LayoutCalculator.Common.Bases.Base6;

public class PartBooster : Tile
{
    public override string Name => nameof(PartBooster);
    public override string Category => Parts.TileName;

    public override char Display => 'B';
    public override int MaxRange => 3;

    public override void ApplyMultipliers(double[] multipliers, int zOffset, int columns, int x, int y)
    {
        var yOffset = zOffset + y * columns;

        multipliers[zOffset + (y - 3) * columns + x] *= 2;
        multipliers[zOffset + (y - 2) * columns + x] *= 2;
        multipliers[zOffset + (y - 1) * columns + x] *= 2;
        multipliers[yOffset + x - 3] *= 2;
        multipliers[yOffset + x - 2] *= 2;
        multipliers[yOffset + x - 1] *= 2;
        multipliers[yOffset + x + 1] *= 2;
        multipliers[yOffset + x + 2] *= 2;
        multipliers[yOffset + x + 3] *= 2;
        multipliers[zOffset + (y + 1) * columns + x] *= 2;
        multipliers[zOffset + (y + 2) * columns + x] *= 2;
        multipliers[zOffset + (y + 3) * columns + x] *= 2;
    }

    public override double GetProduction()
    {
        return 0;
    }
}
