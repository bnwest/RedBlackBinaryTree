using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * http://www.geeksforgeeks.org/red-black-tree-set-1-introduction-2/
 * 
 * Red-Black Tree is a self-balancing Binary Search Tree (BST) where every node follows following rules:
 * 
 * 1) Every node has a color either red or black.
 * 
 * 2) Root of tree is always black.
 * 
 * 3) There are no two adjacent red nodes (A red node cannot have a red parent or red child).
 * 
 * 4) Every path from root to a NULL node has same number of black nodes.
 * 
 * If we make sure that height of the tree remains O(Logn) after every insertion and deletion, 
 * then we can guarantee an upper bound of O(Logn) for all these operations. The height of a Red Black tree 
 * is always O(Logn) where n is the number of nodes in the tree.
 * 
 * Insert: http://www.geeksforgeeks.org/red-black-tree-set-2-insert/
 * Delete: http://www.geeksforgeeks.org/red-black-tree-set-3-delete-2/
 */

/*
 * http://quiz.geeksforgeeks.org/binary-search-tree-set-1-search-and-insertion/
 * 
 * Binary Search Tree, is a node-based binary tree data structure which has the following properties:
 *   The left subtree of a node contains only nodes with keys less than the node’s key.
 *   The right subtree of a node contains only nodes with keys greater than the node’s key.
 *   The left and right subtree each must also be a binary search tree.
 *   There must be no duplicate nodes.
 *
 */

namespace RedBlackTree
{
    public class RedBlackTree<T> where T : IComparable
    {
        protected enum NodeColor { None, Red, Black };
        protected class Node
        {
            public T value;
            public Node parent;
            public Node leftChild;
            public Node rightChild;
            public NodeColor color;
        }

        protected Node root;

        public RedBlackTree()
        {
            root = null;
        }

        protected Node createNode(T newValue)
        {
            // example of an object initializer:
            Node newNode = new Node { value = newValue, parent = null, leftChild = null, rightChild = null, color = NodeColor.None };
            return newNode;
        }

        protected void MakeRoot(Node node)
        {
            root = node;
            node.parent = null;
        }

        protected void MakeLeftChild(Node parent, Node child)
        {
            parent.leftChild = child;
            if ( child != null )
            {
                child.parent = parent;
            }
        }

        protected void MakeRightChild(Node parent, Node child)
        {
            parent.rightChild = child;
            if ( child != null )
            {
                child.parent = parent;
            }
        }

        protected void RightRotate(Node node)
        {
            // move up node.leftChild, node's parent now points at it
            // node.leftChild makes old parent it's right child
            // node.leftChild old right child is now parent's new left child
            Node leftChild = node.leftChild;
            Node leftChildRightChild = leftChild?.rightChild; // example of a null-conditioned operator

            bool gotParent = ( node.parent != null );
            if ( gotParent )
            {
                bool isNodeParentLeftChild  = ( node.parent.leftChild  == node );
                bool isNodeParentRightChild = ( node.parent.rightChild == node );

                if ( isNodeParentLeftChild )
                {
                    MakeLeftChild(node.parent, leftChild);
                }
                else if ( isNodeParentRightChild )
                {
                    MakeRightChild(node.parent, leftChild);
                }
            }
            else
            {
                MakeRoot(leftChild);
            }

            // orphan input node
            node.parent = null;

            MakeRightChild(leftChild, node);

            MakeLeftChild(node, leftChildRightChild);
        }

        protected void LeftRotate(Node node)
        {
            // move up node.rightChild, node's parent now points at it
            // node rightChild make old parent it's left child
            // node rightChild old left child is now parent's new right child
            Node rightChild = node.rightChild;
            Node rightChildLeftChild = rightChild?.leftChild;

            bool gotParent = ( node.parent != null );
            if ( gotParent )
            {
                bool isNodeParentLeftChild = (node.parent.leftChild == node);
                bool isNodeParentRightChild = (node.parent.rightChild == node);

                if ( isNodeParentLeftChild )
                {
                    MakeLeftChild(node.parent, rightChild);
                }
                else if ( isNodeParentRightChild )
                {
                    MakeRightChild(node.parent, rightChild);
                }
            }
            else
            {
                MakeRoot(rightChild);
            }

            // orphan input node
            node.parent = null;

            MakeLeftChild(rightChild, node);

            MakeRightChild(node, rightChildLeftChild);
        }

        protected void LeftLeftTransform(Node node)
        {
            // 1. Right rotate grandParent
            // 2. swap colors for parent and grandparent
            Node parent = node.parent;
            Node grandParent = node.parent.parent;

            RightRotate(grandParent);
            // grandParent and node are now siblings

            NodeColor swap = parent.color;
            parent.color = grandParent.color;
            grandParent.color = swap;
        }

        protected void LeftRightTransform(Node node)
        {
            // 1. Left rotate parent
            // 2. Apply Left Left transform on parent
            Node parent = node.parent;

            LeftRotate(parent);
            // node is now parent to parent

            LeftLeftTransform(parent);
        }

        protected void RightRightTransform(Node node)
        {
            // 1. Left rotate grandParent
            // 2. swap colors for parent and grandparent
            Node parent = node.parent;
            Node grandParent = node.parent.parent;

            LeftRotate(grandParent);
            // grandParent and node are now siblings

            NodeColor swap = parent.color;
            parent.color = grandParent.color;
            grandParent.color = swap;
        }

        protected void RightLeftTransform(Node node)
        {
            // 1. Right rotate parent
            // 2. Apply Right Right transform on parent
            Node parent = node.parent;

            RightRotate(parent);
            // node is parent to parent

            RightRightTransform(parent);
        }

        protected void BalanceTree(Node newNode)
        {
            if ( newNode.parent == null )
            {
                // root is always black
                newNode.color = NodeColor.Black;
                return;
            }

            if ( newNode.parent != null && newNode.parent.color == NodeColor.Black )
            {
                // added a red child to a black parent node.  tree remains valid.
                // all paths from NULL nodes to root have the same black node count.
                return;
            }

            // Color of a NULL node is considered as BLACK

            bool gotGrandParent = (newNode.parent != null && newNode.parent.parent != null);
            if ( gotGrandParent )
            {
                Node parent = newNode.parent;
                Node grandParent = newNode.parent.parent;

                // may have an uncle which will trigger balancing
                bool isNodeLeftChild  = ( parent.leftChild == newNode );
                bool isNodeRightChild = ( parent.rightChild == newNode );

                bool isParentLeftChild  = ( grandParent != null && grandParent.leftChild  == parent );
                bool isParentRightChild = ( grandParent != null && grandParent.rightChild == parent );

                Node rightUncle = ( isParentLeftChild  && grandParent.rightChild != null ? grandParent.rightChild : null );
                Node leftUncle  = ( isParentRightChild && grandParent.leftChild  != null ? grandParent.leftChild  : null );

                // uncle can null
                Node uncle = ( isParentLeftChild ? rightUncle : leftUncle );

                bool gotRedUncle   = ( uncle != null && uncle.color == NodeColor.Red );
                bool gotBlackUncle = ( uncle == null || uncle.color == NodeColor.Black );

                if ( gotRedUncle )
                {
                    // two adjacent nodes can not be red => red uncle has a black parent (newNode's grandparent)

                    // (i) Change color of parent and uncle as BLACK
                    newNode.parent.color = NodeColor.Black;
                    uncle.color = NodeColor.Black;

                    // (ii) color of grand parent as RED
                    newNode.parent.parent.color = NodeColor.Red;

                    // (iii) balance tree for x’s grandparent
                    BalanceTree(newNode.parent.parent);
                }
                else if ( gotBlackUncle )
                {
                    //
                    // Four cases:
                    //

                    // (i)   isParentLeftChild and isNodeLeftChild   (aka Left  Left  transform)
                    // 1. Right rotate grandParent
                    // 2. swap colors for parent and grandparent
                    if ( isParentLeftChild && isNodeLeftChild )
                    {
                        LeftLeftTransform(newNode);
                    }

                    // (ii)  isParentLeftChild and isNodeRightChild  (aka Left  Right transform)
                    // 1. Left rotate parent
                    // 2. Apply Left Left transform on parent
                    if ( isParentLeftChild && isNodeRightChild )
                    {
                        LeftRightTransform(newNode);
                    }

                    // (iii) isParentRightChild and isNodeRightChild (aka Right Right transform)
                    // 1. Left rotate grandParent
                    // 2. swap colors for parent and grandparent
                    if ( isParentRightChild && isNodeRightChild )
                    {
                        RightRightTransform(newNode);
                    }

                    // (iv)  isParentRightChild and isNodeLeftChild  (aka Right Left  transform)
                    // 1. Right rotate parent
                    // 2. Apply Left Left transform on parent
                    if ( isParentRightChild && isNodeLeftChild )
                    {
                        RightLeftTransform(newNode);
                    }
                }
            }
        }

        public void Insert(T newValue)
        {
            Node newNode = UnbalancedInsert(newValue);
            newNode.color = NodeColor.Red;
            //LogNode(newNode, "insert new value");

            BalanceTree(newNode);
            //LogTree();
            //ValidateInOrderTraverse();
            //LogInOrderTraverse();
        }

        protected void UnbalancedInsert(Node node, Node newNode)
        {
            if (newNode.value.CompareTo(node.value) < 0)
            {
                // newValue < value
                if ( node.leftChild == null )
                {
                    MakeLeftChild(node, newNode);
                }
                else
                {
                    UnbalancedInsert(node.leftChild, newNode);
                }
            }
            else if (newNode.value.CompareTo(node.value) > 0)
            {
                // newValue > value
                if (node.rightChild == null)
                {
                    MakeRightChild(node, newNode);
                }
                else
                {
                    UnbalancedInsert(node.rightChild, newNode);
                }
            }
            //else if ( EqualityComparer<T>.Default.Equals(newValue, root.value) )
            else if (newNode.value.CompareTo(node.value) == 0)
            {
                // newValue == value
                // should never get here.  we require uniqueness, so do not insert duplicate value.
            }
        }

        protected Node UnbalancedInsert(T newValue)
        {
            Node newNode = createNode(newValue);

            if ( root == null )
            {
                MakeRoot(newNode);
            }
            else
            {
                UnbalancedInsert(root, newNode);
            }

            return newNode;
        }

        public void ValidateInOrderTraverse()
        {
            ValidateInOrderTraverse(root);
            Console.WriteLine();
        }

        protected void ValidateInOrderTraverse(Node node)
        {
            if ( node == root && node.color != NodeColor.Black )
            {
                Console.WriteLine("Violation: root is not black.");
            }

            if ( node.parent != null && node.color == NodeColor.Red && node.parent.color == NodeColor.Red )
            {
                Console.WriteLine("Violation: two adjacent nodes are red.");
            }

            if ( node.leftChild == null || node.rightChild == null )
            {
                int blackNodeCount = 0;
                Node thisNode = node;
                while ( thisNode != null )
                {
                    if ( thisNode.color == NodeColor.Black )
                    {
                        blackNodeCount++;
                    }
                    thisNode = thisNode.parent;
                }
                Console.Write($"(node={node.value},blacks={blackNodeCount}) ");
            }

            if ( node.leftChild != null )
            {
                ValidateInOrderTraverse(node.leftChild);
            }

            if ( node.rightChild != null )
            {
                ValidateInOrderTraverse(node.rightChild);
            }
        }

        public void LogInOrderTraverse()
        {
            LogInOrderTraverse(root);
            Console.WriteLine();
        }

        protected void LogInOrderTraverse(Node node)
        {
            if ( node.leftChild != null )
            {
                LogInOrderTraverse(node.leftChild);
            }

            char nodeColor = (node.color == NodeColor.Red ? 'r' : node.color == NodeColor.Black ? 'b' : 'n');
            Console.Write($"{node.value}{nodeColor} ");

            if ( node.rightChild != null )
            {
                LogInOrderTraverse(node.rightChild);
            }
        }

        protected void LogNode(Node node, string description = "")
        {
            if ( description.Length > 0 )
            {
                Console.Write($"{description} : ");
            }

            Node thisNode = node;
            while (thisNode != null)
            {
                char nodeColor = (thisNode.color == NodeColor.Red ? 'r' : thisNode.color == NodeColor.Black ? 'b' : 'n');
                if (thisNode.parent == null)
                {
                    Console.Write($"{thisNode.value}{nodeColor}");
                }
                else
                {
                    if (thisNode.parent.leftChild == thisNode)
                    {
                        Console.Write($"{thisNode.value}{nodeColor} < ");
                    }
                    if (thisNode.parent.rightChild == thisNode)
                    {
                        Console.Write($"{thisNode.value}{nodeColor} > ");
                    }
                }
                thisNode = thisNode.parent;
            }
            Console.WriteLine();
        }

        public void LogTree()
        {
            LogTree(root);
        }

        protected void LogTree(Node node)
        {
            if ( node.leftChild == null && node.rightChild == null )
            {
                // we are at a leaf node
                LogNode(node);
            }
            else
            {
                if ( node.leftChild != null )
                {
                    LogTree(node.leftChild);
                }
                if ( node.rightChild != null )
                {
                    LogTree(node.rightChild);
                }
            }
        }
    }
}
