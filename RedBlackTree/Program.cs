using System;
using RNG;

namespace RedBlackTree
{
    class Program
    {
        static RandomNumberGenerator rng = new RandomNumberGenerator();

        static void RandomizeValues(int[] values)
        {
            int valueCount = values.GetLength(0);

            int collisions = 0;
            for (int i = 0; i < 10 * valueCount; i++)
            {
                int idx1 = rng.Next(valueCount);
                int idx2 = rng.Next(valueCount);
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
        }

        static void Main(string[] args)
        {
            const int treeNodeCount = 10; // readonly instead?

            int[] values = new int[treeNodeCount];
            for (int i = 0; i < treeNodeCount; i++)
            {
                values[i] = i + 1;
            }

            //
            // randomize the order of values[] before insert.
            //

            RandomizeValues(values);

            // test only
            values = new int[treeNodeCount] { 9, 4, 2, 7, 5, 10, 1, 8, 3, 6 };
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

            //
            // randomize the order of values[] before delete.
            //

            RandomizeValues(values);

            // test only
            // values = new int[treeNodeCount] { 9, 4, 2, 7, 5, 10, 1, 8, 3, 6 };
            // values = new int[treeNodeCount] { 8, 1, 3, 6, 2, 10, 4, 7, 5, 9 };
            // values = new int[treeNodeCount] { 6, 1, 9, 10, 5, 8, 4, 7, 3, 2 }; // no DoubleBlack

            for (int i = 0; i < treeNodeCount; i++)
            {
                redBlackTree.Delete(values[i]);
                Console.WriteLine($"Deleting {values[i]} from RBT.");
                redBlackTree.LogInOrderTraverse();
            }

            RandomizeValues(values);
        }
    }
}
