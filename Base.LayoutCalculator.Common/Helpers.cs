using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Base.LayoutCalculator.Common;

public static class Helpers
{
    public delegate void Action2D<T>(int x, int y, T item);
    public delegate void Action3D<T>(int x, int y, long z, T item);

    public static IDictionary<string, T[,]> InitializeMap<T>(Tile[] possibilities, int x, int y, T value)
    {
        var map = new ConcurrentDictionary<string, T[,]>();
        foreach (var possibility in possibilities)
        {
            map.TryAdd(possibility.Category, ArrayHelpers.Initialize(x, y, value));
        }

        return map;
    }

    public static IDictionary<string, T[,,]> InitializeMap<T>(Tile[] possibilities, int columns, int rows, int depth, T value)
    {
        var map = new ConcurrentDictionary<string, T[,,]>();

        Parallel.ForEach(possibilities, delegate (Tile possibility)
        {
            map.TryAdd(possibility.Category, ArrayHelpers.Initialize(
                depth,
                rows,
                columns,
                value));
        });

        return map;
    }

    public static IDictionary<string, T[,,]> InitializeFlatMap<T>(Tile[] possibilities, int columns, int rows, int depth, T value)
    {
        var map = new ConcurrentDictionary<string, T[,,]>();

        Parallel.ForEach(possibilities, delegate (Tile possibility)
        {
            map.TryAdd(possibility.Category, ArrayHelpers.Initialize(
                depth,
                rows,
                columns,
                value));
        });

        return map;
    }

    public static IDictionary<string, T[,,]> InitializeMap<T>(Tile[] possibilities, Tile[,,] grid, T value)
    {
        var map = new ConcurrentDictionary<string, T[,,]>();

        Parallel.ForEach(possibilities, delegate (Tile possibility)
        {
            map.TryAdd(possibility.Category, ArrayHelpers.Initialize(
                grid.GetLength(2),
                grid.GetLength(1),
                grid.GetLength(0),
                value));
        });

        return map;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void For2D<T>(this T[,] array, Action<T> action)
    {
        for (int y = 0; y < array.GetLength(0); y++)
        {
            for (int x = 0; x < array.GetLength(1); x++)
            {
                action(array[y, x]);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void For2D<T>(this T[,] array, Action2D<T> action)
    {
        for (int y = 0; y < array.GetLength(0); y++)
        {
            for (int x = 0; x < array.GetLength(1); x++)
            {
                action(x, y, array[y, x]);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Parallel3D<T>(this T[,,] array, Action3D<T> action)
    {
        var depth = array.GetLength(0);
        var rows = array.GetLength(1);
        var columns = array.GetLength(2);

        Parallel.For(0, array.GetLength(0), delegate (int z)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    action(x, y, z, array[z, y, x]);
                }
            }
        });
    }
}
