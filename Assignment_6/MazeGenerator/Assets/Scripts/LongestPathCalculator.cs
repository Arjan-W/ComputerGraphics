using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
 
namespace MazeGenerator.Assest.Scripts.LongestPathCalculator
{
    /* Code inspired by https://www.csharpstar.com/dijkstra-algorithm-csharp/. Referenced on May 8, 2020.
    
    Example graph:
    int[,] graph =  {
                         { 0, 6, 0, 0, 0, 0, 0, 9, 0 },
                         { 6, 0, 9, 0, 0, 0, 0, 11, 0 },
                         { 0, 9, 0, 5, 0, 6, 0, 0, 2 },
                         { 0, 0, 5, 0, 9, 16, 0, 0, 0 },
                         { 0, 0, 0, 9, 0, 10, 0, 0, 0 },
                         { 0, 0, 6, 0, 10, 0, 2, 0, 0 },
                         { 0, 0, 0, 16, 0, 2, 0, 1, 6 },
                         { 9, 11, 0, 0, 0, 0, 1, 0, 5 },
                         { 0, 0, 2, 0, 0, 0, 6, 5, 0 }
                            };
    */
    class LongestPathCalculator
    {
 
        private static int MaximumDistance(int[] distance, bool[] longestPathTreeSet, int verticesCount)
        {
            int max = int.MinValue;
            int maxIndex = 0;
 
            for (int v = 0; v < verticesCount; ++v)
            {
                if (longestPathTreeSet[v] == false && distance[v] >= max)
                {
                    max = distance[v];
                    maxIndex = v;
                }
            }
 
            return maxIndex;
        }
 
        private static void Print(int[] distance, int verticesCount)
        {
            Console.WriteLine("Vertex    Distance from source");
 
            for (int i = 0; i < verticesCount; ++i)
                Console.WriteLine("{0}\t  {1}", i, distance[i]);
        }
 
        // How to use: e.g. CalculateLongestPath(graph, 0, 9)
        public static void CalculateLongestPath(int[,] graph, int source, int verticesCount)
        {
            int[] distance = new int[verticesCount];
            bool[] longestPathTreeSet = new bool[verticesCount];
 
            for (int i = 0; i < verticesCount; ++i)
            {
                distance[i] = int.MinValue;
                longestPathTreeSet[i] = false;
            }
 
            distance[source] = 0;
 
            for (int count = 0; count < verticesCount - 1; ++count)
            {
                int u = MaximumDistance(distance, longestPathTreeSet, verticesCount);
                longestPathTreeSet[u] = true;
 
                for (int v = 0; v < verticesCount; ++v)
                    if (!longestPathTreeSet[v] && Convert.ToBoolean(graph[u, v]) && distance[u] != int.MinValue && distance[u] + graph[u, v] < distance[v])
                        distance[v] = distance[u] + graph[u, v];
            }
 
            Print(distance, verticesCount);
        }
    }
}