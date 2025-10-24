using Base.LayoutCalculator.Common;
using System.Diagnostics;

namespace Base.LayoutCalculator;

internal static class Program
{
    private static void Main(string[] args)
    {
        var xx = Tile.Block;
        var oo = Tile.Slot;
        var aa = new Common.Bases.Base6.Parts();
        var bb = new Common.Bases.Base6.PartBooster();

        var choices = new Tile[] { aa, bb };
        var categories = choices
            .DistinctBy(c => c.Category)
            .Select(c => c.Category)
            .ToArray();

        /*var template = PadPattern(choices, xx, new Tile[,]
        {
            { oo, oo, xx, oo, oo, oo, oo, xx, oo, oo },
            { xx, oo, oo, xx, oo, oo, xx, oo, oo, xx },
            { oo, xx, oo, oo, xx, xx, oo, oo, xx, oo },
            { oo, xx, oo, xx, oo, oo, xx, oo, xx, oo },
            { oo, xx, oo, xx, oo, oo, xx, oo, xx, oo },
            { oo, xx, oo, oo, xx, xx, oo, oo, xx, oo },
            { xx, oo, oo, xx, oo, oo, xx, oo, oo, xx },
            { oo, oo, xx, oo, oo, oo, oo, xx, oo, oo },
        });*/
        var template = PadPattern(choices, xx, new Tile[,]
        {
            { oo, oo, xx, oo, oo },
            { xx, oo, oo, xx, oo },
            { oo, xx, oo, oo, xx },
            { oo, xx, oo, xx, oo },
            { oo, xx, oo, xx, oo },
            { oo, xx, oo, oo, xx },
            { xx, oo, oo, xx, oo },
            { oo, oo, xx, oo, oo },
        });

        var slots = 0;
        template.For2D(tile => slots = tile.Replacable ? (slots + 1) : slots);

        var maxDepth = GetLegalDepth(template, sizeof(double));
        var chunkSize = GetGroupSize(maxDepth);

        var possibilities = GetPossibleLayoutsCount(choices.Length, slots);
        
        var chunks = Math.Ceiling(possibilities / chunkSize);
        var chunkedChunks = chunks > int.MaxValue ? (int)(chunks / int.MaxValue) : 1;

        Console.WriteLine("Computing {0} chunks", chunks);

        var rows = template.GetLength(0);
        var columns = template.GetLength(1);
        var gridSize = rows * columns;

        var multiplierTemplate = new double[gridSize * chunkSize];
        Array.Fill(multiplierTemplate, 1);

        var cancellation = new CancellationTokenSource();

        var completed = 0d;

        Console.CancelKeyPress += (_, eArgs) =>
        {
            eArgs.Cancel = true; // We handle cancellation ourselves.

            if (!cancellation.IsCancellationRequested)
            {
                cancellation.Cancel();
                Console.WriteLine("Cancellation requested, this can take a moment...");
            }
        };

        void UpdateUi()
        {
            while (!cancellation.IsCancellationRequested && completed < chunks)
            {
                Console.Title = $"{completed}/{chunks}";
                Thread.Sleep(1000);
            }
        }

        var uiThread = new Thread(UpdateUi);
        uiThread.Start();

        var bestProduction = new LayoutChunk.ProductionResult(0, []);


        for (double a = 0; a < chunkedChunks; a++)
        {
            double aOffset = a * int.MaxValue;
            var localChunks = (int) Math.Min(chunks - aOffset, int.MaxValue);

            try
            {
                Parallel.For(0, localChunks, new ParallelOptions()
                {
                    CancellationToken = cancellation.Token
                }, 
                delegate (int b)
                {
                    var offset = aOffset + b;

                    Console.WriteLine("Starting layout {0}", offset);
                    var timer = Stopwatch.StartNew();

                    var layout = new LayoutChunk(choices, offset, chunkSize, rows, columns);
                    layout.InitializeMatrix(template, choices);
                    layout.InitializeMultipliers(categories, xx, multiplierTemplate);
                    layout.ApplyMultipliers(cancellation.Token);

                    var result = layout.GetMaxProduction(cancellation.Token);

                    if (result.Max > bestProduction.Max)
                    {
                        bestProduction = result;
                    }

                    timer.Stop();
                    Console.WriteLine("Layout {0} completed in {1}ms", offset, timer.ElapsedMilliseconds);

                    completed++;
                });
            }
            catch (OperationCanceledException)
            {
            }
        }

        uiThread.Join();

        /*var layout = new LayoutChunk(choices, 0, chunkSize, rows, columns);

        Console.Write("Initializing layout {2}, offset {0}, depth {1}... ", layout.offset, chunkSize, layout.GetHashCode());
        var timer = Stopwatch.StartNew();
        layout.InitializeMatrix(template, choices);
        Console.WriteLine("{0}ms", timer.ElapsedMilliseconds);

        Console.Write("Initializing multipliers for layout {0}... ", layout.GetHashCode());
        timer.Restart();
        layout.InitializeMultipliers(categories, xx);
        Console.WriteLine("{0}ms", timer.ElapsedMilliseconds);

        Console.Write("Applying multipliers for layout {0}... ", layout.GetHashCode());
        timer.Restart();
        layout.ApplyMultipliers(cancellation.Token);
        Console.WriteLine("{0}ms", timer.ElapsedMilliseconds);

        Console.Write("Calculating production for layout {0}... ", layout.GetHashCode());
        timer.Restart();
        var maxs = layout.GetMaxProduction(cancellation.Token);
        Console.WriteLine("{0}ms", timer.ElapsedMilliseconds);*/

        Console.WriteLine("Found max production of {0}:", bestProduction.Max);
        foreach (var grid in bestProduction.Grids)
        {
            ArrayHelpers.PrintTable(grid, columns);
        }
    }

    private static int GetLegalDepth(Tile[,] template, double biggestMemoryAllocation)
    {
        const int maxMemory = (int) (8 * 1000 * 1000);

        var clrMax = (int) Array.MaxLength / template.Length;
        var memoryMax = maxMemory / biggestMemoryAllocation;

        return (int) Math.Min(clrMax, memoryMax);
    }

    private static int GetGroupSize(int maxDepth)
    {
        return maxDepth;
    }

    private static Tile[,] PadPattern(Tile[] choices, Tile fill, Tile[,] pattern)
    {
        var padding = choices.Max(c => c.MaxRange);
        var template = new Tile[
            pattern.GetLength(0) + (padding << 1),
            pattern.GetLength(1) + (padding << 1)];

        var rows = template.GetLength(0); // y
        var columns = template.GetLength(1); // x

        var yEnd = pattern.GetLength(0) + padding - 1;

        Console.WriteLine("Padding pattern with max range {0}", padding);
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                template[y, x] = fill;
            }
        }

        // Copy 2D
        for (int y = 0; y < pattern.GetLength(0); y++)
        {
            for (int x = 0; x < pattern.GetLength(1); x++)
            {
                template[y + padding, x + padding] = pattern[y, x];
            }
        }

        return template;
    }

    private static double GetPossibleLayoutsCount(int choices, int slots) => Math.Pow(choices, slots);
}
