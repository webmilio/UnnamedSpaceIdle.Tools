using Base.LayoutCalculator.Common;

namespace Base.LayoutCalculator;

internal class Program
{
    private static void Main(string[] args)
    {
        var xx = Tile.Block;
        var oo = Tile.Slot;
        var bm = new Common.Bases.Base1.BuildingMats();
        var bs = new Common.Bases.Base1.Booster();

        var grid = new Tile[,]
        {
            { xx, xx, xx, xx, xx, xx },
            { xx, oo, xx, oo, oo, xx },
            { xx, oo, oo, oo, oo, xx },
            { xx, oo, oo, xx, oo, xx },
            { xx, xx, xx, xx, xx, xx },
        };


        var rows = grid.GetLength(0); // y
        var columns = grid.GetLength(1); // x
        var choices = new Tile[] { bm, bs };

        var replacableTiles = 0;
        grid.For2D(tile => replacableTiles = tile.Replacable ? (replacableTiles + 1) : replacableTiles);

        var possibilities = (int) Math.Pow(choices.Length, replacableTiles); // z
        var matrix = new Tile[possibilities, rows, columns];

        var multiplierMap = Helpers.InitializeMap(choices, matrix, 1d);
        var emptyMatrix = ArrayHelpers.Initialize3D(columns, rows, possibilities, 0d);

        // xx and oo share the same category.
        multiplierMap.Add(xx.Category, emptyMatrix);

        for (int z = 0; z < possibilities; z++)
        {
            var n = z;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (grid[y, x].Replacable)
                    {
                        var choice = n % choices.Length;
                        n /= choices.Length;

                        matrix[z, y, x] = choices[choice];
                    }
                    else
                    {
                        matrix[z, y, x] = grid[y, x];
                    }
                }
            }
        }

        matrix.Parallel3D(delegate (int x, int y, int z, Tile tile)
        {
            var multipliers = multiplierMap[tile.Category];
            tile.ApplyMultipliers(multipliers, x, y, z);
        });

        var productions = new double[possibilities];

        matrix.Parallel3D(delegate (int x, int y, int z, Tile tile)
        {
            var multipliers = multiplierMap[tile.Category];
            productions[z] += tile.GetProduction() * multipliers[z, y, x];
        });

        var maxProduction = 0d;
        var maxZs = new List<int>();

        for (int z = 0; z < possibilities; z++)
        {
            var production = productions[z];

            if (production > maxProduction)
            {
                maxZs.Clear();
                maxProduction = production;
            }

            if (production == maxProduction)
            {
                maxZs.Add(z);
            }
        }

        foreach (var z in maxZs)
        {
            Console.WriteLine("Max production on z-index {0} with total {1}", z, maxProduction);
            ArrayHelpers.PrintDimension(matrix, z);
        }
    }
}
