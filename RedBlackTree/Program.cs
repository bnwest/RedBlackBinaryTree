using System;
using RNG;
using System.Collections.Generic;

namespace RedBlackTree
{
    class Program
    {
        static RandomNumberGenerator rng = new RandomNumberGenerator();

        static void RandomizeValues(int[] values)
        {
            int valueCount = values.GetLength(0);

            int collisions = 0;
            for ( int i = 0; i < 10 * valueCount; i++ )
            {
                int idx1 = rng.Next(valueCount);
                int idx2 = rng.Next(valueCount);
                if ( idx1 != idx2 )
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
            const int treeNodeCount = 100; // readonly instead?

            int[] values = new int[treeNodeCount];
            for ( int i = 0; i < treeNodeCount; i++ )
            {
                values[i] = i + 1;
            }

            //
            // randomize the order of values[] before insert.
            //

            RandomizeValues(values);

            // test only
            // values = new int[treeNodeCount] { 9, 4, 2, 7, 5, 10, 1, 8, 3, 6 };
            // values = new int[treeNodeCount] { 8, 1, 3, 6, 2, 10, 4, 7, 5, 9 };

            Console.Write("Insert order: { ");
            foreach ( var value in values ) Console.Write($"{value}, ");
            Console.WriteLine("}\n");

            //
            // insert new nodes in the Red Black Tree.
            //

            var redBlackTree = new RedBlackTree<int>();

            for ( int i = 0; i < treeNodeCount; i++ )
            {
                redBlackTree.Add(values[i]);
            }
            redBlackTree.LogTree();
            redBlackTree.ValidateInOrderTraverse();
            redBlackTree.LogInOrderTraverse();

            //
            // iterate over the Red Black Tree.
            //

            Console.Write("Iterate over Red Black Tree:\n{ ");
            foreach ( int value in redBlackTree )
            {
                Console.Write($"{value}, ");
            }
            Console.WriteLine("}\n");

            //
            // randomize the order of values[] before delete.
            //

            RandomizeValues(values);

            // test only
            // insert order: values = new int[treeNodeCount] { 9, 4, 2, 7, 5, 10, 1, 8, 3, 6, };
            // delete order: values = new int[treeNodeCount] { 8, 1, 3, 6, 2, 10, 4, 7, 5, 9, }; // LCase {1,2} RCase {2}
            // delete order: values = new int[treeNodeCount] { 10, 9, 8, 2, 3, 4, 6, 7, 5, 1, }; // LCase {2} RCase {2,3,4}
            // delete order: values = new int[treeNodeCount] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, }; // LCase {1,2,4}  RCase {}

            // insert order: values = new int[treeNodeCount] { 5, 3, 8, 6, 9, 4, 10, 7, 1, 2, };
            // delete order: values = new int[treeNodeCount] { 7, 5, 9, 8, 6, 4, 1, 10, 2, 3, }; // LCase {4} RCase {1,2}

            // insert order: values = new int[treeNodeCount] { 5, 4, 3, 7, 6, 10, 1, 8, 9, 2, };
            // delete order: values = new int[treeNodeCount] { 7, 3, 6, 4, 8, 2, 9, 10, 5, 1, }; // LCase {2,3,4} RCase {2,4}

            Console.Write("Delete order: { ");
            foreach ( var value in values ) Console.Write($"{value}, ");
            Console.WriteLine("}\n");

            //
            // delete nodes in the Red Black Tree.
            //

            for ( int i = 0; i < treeNodeCount; i++ )
            {
                redBlackTree.Remove(values[i]);
            }

            redBlackTree.LogInOrderTraverse();
        }
    }
}
