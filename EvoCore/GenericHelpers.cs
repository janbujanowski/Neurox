﻿using System;
using System.Collections.Generic;

namespace EvoCore
{
    public static class GenericHelpers
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = GeneticEnvironment.CUBE.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = GeneticEnvironment.CUBE.Next(n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }
    }
}
