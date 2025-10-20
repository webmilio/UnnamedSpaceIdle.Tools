using System;
using System.Collections.Generic;

namespace Base.LayoutCreator.Common;

public abstract class Tile
{
    public abstract Dictionary<string, double[,]> GetMultiplier(Tile[,] grid, int x, int y);
}
