using System.Collections.Generic;

namespace Base.LayoutCalculator.Common.Bases.Base6;

public class MaterialBooster : Tile
{
    public override string Name => nameof(MaterialBooster);
    public override char Display => '+';
    public override string Category => BuildingMaterials.TileName;
    public override int MaxRange => 2;

    public override void ApplyMultipliers(double[] multipliers, int zOffset, int columns, int x, int y)
    {
        multipliers[zOffset + (y - 2) * columns + x - 2]      *= 2;
        multipliers[zOffset + (y - 1) * columns + x - 1]      *= 2;
        multipliers[zOffset + (y - 2) * columns + x    ]      *= 2;
        multipliers[zOffset + (y - 1) * columns + x    ]      *= 2;
        multipliers[zOffset + (y - 1) * columns + x + 1]      *= 2;
        multipliers[zOffset + (y - 2) * columns + x + 2]      *= 2;

        multipliers[zOffset + (y    ) * columns + x - 2]      *= 2;
        multipliers[zOffset + (y    ) * columns + x - 1]      *= 2;
        multipliers[zOffset + (y    ) * columns + x + 1]      *= 2;
        multipliers[zOffset + (y    ) * columns + x + 2]      *= 2;

        multipliers[zOffset + (y + 1) * columns + x - 1]      *= 2;
        multipliers[zOffset + (y + 2) * columns + x - 2]      *= 2;
        multipliers[zOffset + (y + 1) * columns + x    ]      *= 2;
        multipliers[zOffset + (y + 2) * columns + x    ]      *= 2;
        multipliers[zOffset + (y + 1) * columns + x + 1]      *= 2;
        multipliers[zOffset + (y + 2) * columns + x + 2]      *= 2;
    }

    public override double GetProduction()
    {
        return 0;
    }
}
