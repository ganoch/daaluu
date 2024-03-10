using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daaluu
{
    static class RandomExtensions
    {
        public static void Shuffle<T>(this Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
        public static void Shuffle<T>(this Random rng, List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }
        public static Random RandomInstance = new Random();
        public static int getSkip(int size, int prime)
        {
            int rnd0 = RandomInstance.Next(prime) + 1;
            int rnd1 = RandomInstance.Next(prime) + 1;
            int rnd2 = RandomInstance.Next(prime) + 1;
            return rnd0 * size + rnd1 * size * size + rnd2;
        }
    }
}
