using System;
using System.Threading.Tasks;

namespace Base.LayoutCalculator.Common;

public static class ArrayHelpers
{
    public static void Copy<T>(T[,] sourceArray, 
        int sourceRow, int sourceColumn, 
        T[,] destinationArray, 
        int destinationRow, int destinationColumn, 
        int rowLength, int columnLength)
    {
        for (int y = sourceRow; y < rowLength; y++)
        {
            for (int x = sourceColumn; x < columnLength; x++)
            {
                destinationArray[y + destinationRow, x + destinationColumn] = sourceArray[y + sourceRow, x + sourceColumn];
            }
        }
    }

    public static long SizeOfObjectArray(Array array)
    {
        // Header: 16, Length: 4, Padding: 4
        // 8 bytes per pointer
        return 24 + array.Length * 8;
    }

    public static long SizeOfStructArray(Array array, int structSize)
    {
        // Header: 16, Length: 4, Padding: 4
        // 8 bytes per pointer
        return 24 + array.Length * structSize;
    }

    public static T[,] Initialize<T>(int x, int y, T value)
    {
        var arr = new T[y, x];
        arr.For2D((x, y, _) => arr[y, x] = value);

        return arr;
    }

    public static T[,,] Initialize<T>(int columns, int rows, int depth, T value)
    {
        var arr = new T[depth, rows, columns];
        //arr.Parallel3D((x, y, z, _) => arr[z, y, x] = value);

        Parallel.For(0, depth, delegate (int z)
        {
            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < columns; x++)
                {
                    arr[z, y, x] = value;
                }
            }
        });

        return arr;
    }

    public static void PrintTable<T>(T[,] stack)
    {
        for (int y = 0; y < stack.GetLength(0); y++)
        {
            for (int x = 0; x < stack.GetLength(1); x++)
            {
                var item = stack[y, x];
                Console.Write(item);
            }

            Console.WriteLine();
        }
    }

    public static void PrintTable<T>(T[] table, int columns)
    {
        var rows = table.Length / columns;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var item = table[y * columns + x];
                Console.Write(item);
            }

            Console.WriteLine();
        }
    }

    public static void PrintDimension<T>(T[,,] stack, int layer)
    {
        for (int y = 0; y < stack.GetLength(1); y++)
        {
            for (int x = 0; x < stack.GetLength(2); x++)
            {
                var item = stack[layer, y, x];
                Console.Write(item);
            }

            Console.WriteLine();
        }
    }
}
