using System.Collections.Generic;

namespace Base.LayoutCalculator.Common.Bases.Base1;

public class Booster : Tile
{
    public const string TileName = nameof(Booster);

    public override string Name => TileName;
    public override string Category => BuildingMats.TileName;

    public override char Display => '+';

    public override void ApplyMultipliers(double[,,] multipliers, int x, int y, int z)
    {
        multipliers[z, y, x - 1]        *= 2;
        multipliers[z, y, x + 1]        *= 2;
        multipliers[z, y + 1, x - 1]    *= 2;
        multipliers[z, y + 1, x + 1]    *= 2;
    }

    public override double GetProduction()
    {
        return 0;
    }
}
