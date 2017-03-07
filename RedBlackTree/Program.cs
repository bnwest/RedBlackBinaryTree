using System;
using RNG;

namespace RedBlackTree
{
    class Program
    {
        static void Main(string[] args)
        {
            const int treeNodeCount = 100; // readonly instead?

            int[] values = new int[treeNodeCount];
            for (int i = 0; i < treeNodeCount; i++)
            {
                values[i] = i + 1;
            }

            //
            // randomize the order of values[].
            //

            RandomNumberGenerator rng = new RandomNumberGenerator();
            int collisions = 0;
            for (int i = 0; i < 100; i++)
            {
                int idx1 = rng.Next(treeNodeCount);
                int idx2 = rng.Next(treeNodeCount);
                if (idx1 != idx2)
                {
                    // swap values[idx1] with values[idx2]
                    int swap = values[idx1];
                    values[idx1] = values[idx2];
                    values[idx2] = swap;
                }
                else
                {
                    collisions++;
                }
            }

            // test only
            // values = new int[treeNodeCount] { 9, 4, 2, 7, 5, 10, 1, 8, 3, 6 };
            // values = new int[treeNodeCount] { 8, 1, 3, 6, 2, 10, 4, 7, 5, 9 };

            //
            // insert a new node in the Red Black Tree.
            //

            var redBlackTree = new RedBlackTree<int>();
            for (int i = 0; i < treeNodeCount; i++)
            {
                redBlackTree.Insert(values[i]);
            }
            redBlackTree.LogTree();
            redBlackTree.ValidateInOrderTraverse();
            redBlackTree.LogInOrderTraverse();
        }
    }
}
