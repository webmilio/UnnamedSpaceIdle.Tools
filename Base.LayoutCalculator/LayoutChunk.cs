using Base.LayoutCalculator.Common;

namespace Base.LayoutCalculator;

public class LayoutChunk
{
    public record ProductionResult(double Max, IList<Tile[]> Grids);

    public readonly double offset;
    public readonly int depth, rows, columns, gridSize;

    private readonly Tile[] _matrix;
    private IDictionary<string, double[]> _multipliers;

    public LayoutChunk(Tile[] choices, double offset, int depth, int rows, int columns)
    {
        this.offset = offset;

        this.depth = depth;
        this.rows = rows;
        this.columns = columns;

        this.gridSize = rows * columns;

        _matrix = new Tile[depth * gridSize];
    }

    public void InitializeMatrix(Tile[,] template, Tile[] choices)
    {
        Parallel.For(0, depth, delegate (int z)
        {
            var n = z + offset;
            var zOffset = z * gridSize;

            for (var y = 0; y < rows; y++)
            {
                var yOffset = zOffset + y * columns;

                for (var x = 0; x < columns; x++)
                {
                    var i = yOffset + x;

                    if (template[y, x].Replacable)
                    {
                        var choice = (int)(n % choices.Length);
                        n /= choices.Length;

                        _matrix[i] = choices[choice];
                    }
                    else
                    {
                        _matrix[i] = template[y, x];
                    }
                }
            }
        });
    }

    public void InitializeMultipliers(ICollection<string> categories, Tile fill, double[] template)
    {
        _multipliers = new Dictionary<string, double[]>(categories.Count + 1);

        foreach (var category in categories)
        {
            var arr = new double[template.Length];
            Buffer.BlockCopy(template, 0, arr, 0, template.Length * sizeof(double));

            _multipliers[category] = arr;
        }

        _multipliers[fill.Category] = template;
    }

    public void ApplyMultipliers(CancellationToken cancellation)
    {
        Parallel.For(0, depth, delegate (int z)
        //for (int z = 0; z < depth; z++)
        {
            var zOffset = z * gridSize;

            for (var y = 0; y < rows; y++)
            {
                var yOffset = zOffset + y * columns;

                for (var x = 0; x < columns; x++)
                {
                    var tile = _matrix[yOffset + x];
                    var multipliers = _multipliers[tile.Category];

                    tile.ApplyMultipliers(multipliers, zOffset, columns, x, y);
                }
            }
        }
        );
    }
    
    public ProductionResult GetMaxProduction(CancellationToken cancellation)
    {
        var max = 0d;
        var grids = new List<Tile[]>();
        var gridLock = new object();

        //for (int z = 0; z < depth; z++)
        Parallel.For(0, depth, delegate (int z)
        {
            var production = 0d;
            var zOffset = z * gridSize;

            for (int i = 0; i < gridSize; i++)
            {
                var idx = zOffset + i;
                var tile = _matrix[idx];

                production += tile.GetProduction() * _multipliers[tile.Category][idx];
            }

            if (production > max)
            {
                max = production;
                
                lock (gridLock)
                {
                    grids.Clear();
                }
            }

            if (production == max)
            {
                var grid = new Tile[gridSize];
                Array.Copy(_matrix, zOffset, grid, 0, grid.Length);

                lock (gridLock)
                {
                    grids.Add(grid);
                }
            }
        }
        );

        return new(max, grids);
    }
}
