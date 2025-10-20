using System;

namespace Base.LayoutCalculator.Common;

public static class ArrayHelpers
{
    public static T[,,] Initialize3D<T>(int x, int y, int z, T value)
    {
        var arr = new T[z, y, x];
        arr.Parallel3D((x, y, z, _) => arr[z, y, x] = value);

        return arr;
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
