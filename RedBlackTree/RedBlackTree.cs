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
    public class RedBlackTree<T> where T : IComparable, new()
    {
        protected enum NodeColor { None, Red, Black, DoubleBlack };
        protected class Node
        {
            public T value;
            public Node parent;
            public Node leftChild;
            public Node rightChild;
            public NodeColor color;
        }

        protected Node root;

        protected Node DoubleBlack { get; set; }

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

        protected void ResetDoubleBlack()
        {
            DoubleBlack = new Node { value = new T(), parent = null, leftChild = null, rightChild = null, color = NodeColor.DoubleBlack };
        }

        protected void MakeRoot(Node node)
        {
            if ( node == null )
            {
                root = null;
            }
            else
            {
                root = node;
                node.parent = null;
            }
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

        protected void RemoveChildFromParent(Node child)
        {
            Node parent = child.parent;

            if ( parent == null )
            {
                // child does not have parent and thus is root
                MakeRoot(null);
            }
            else
            {
                if ( parent.leftChild == child )
                {
                    RemoveLeftChild(parent);
                }
                else if ( parent.rightChild == child )
                {
                    RemoveRightChild(parent);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        protected void RemoveLeftChild(Node parent)
        {
            if ( parent.leftChild != null )
            {
                Node leftChild = parent.leftChild;
                parent.leftChild = null;
                leftChild.parent = null; 
            }
        }

        protected void RemoveRightChild(Node parent)
        {
            if ( parent.rightChild != null )
            {
                Node rightChild = parent.rightChild;
                parent.rightChild = null;
                rightChild.parent = null;
            }
        }

        protected void ReplaceParentWithChild(Node parent, Node child)
        {
            // hard assumption: parent only has one child.

            bool parentHasNoChildren = ( parent.leftChild == null && parent.rightChild == null );
            bool parentHasTwoChildren = ( parent.leftChild != null && parent.rightChild != null );
            bool parentHasOneChild = ( !parentHasNoChildren && !parentHasTwoChildren);

            if ( parentHasOneChild )
            {
                Node grandparent = parent.parent;
                if ( grandparent == null )
                {
                    // parent is root
                    RemoveChildFromParent(child);
                    MakeRoot(child);
                }
                else
                {
                    bool parentIsLeftChildOfGrandParent = ( grandparent.leftChild == parent );
                    bool parentIsRightChildOfGrandParent = ( grandparent.rightChild == parent );
                    RemoveChildFromParent(parent);
                    RemoveChildFromParent(child);
                    if ( parentIsLeftChildOfGrandParent )
                    {
                        MakeLeftChild(grandparent, child);
                    }
                    else if ( parentIsRightChildOfGrandParent )
                    {
                        MakeRightChild(grandparent, child);
                    }
                    else
                    {
                        // grandparent does not have parent as one of its children
                        throw new System.ArgumentException();
                    }
                }
            }
            else
            {
                throw new System.ArgumentException();
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

        protected void BalanceTreeAfterInsert(Node newNode)
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
                    BalanceTreeAfterInsert(newNode.parent.parent);
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

            BalanceTreeAfterInsert(newNode);
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
            else if ( newNode.value.CompareTo(node.value) > 0 )
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
            else if ( newNode.value.CompareTo(node.value) == 0 )
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
            Console.WriteLine("Validating all of the Red Back Tree requirements:\n");

            SortedDictionary<T, int> blackNodeCount = new SortedDictionary<T, int>();
            ValidateInOrderTraverse(root, blackNodeCount);

            int theBlackNodeCountForTree = 0;
            bool failedToValidate = false;
            foreach (KeyValuePair<T, int> pair in blackNodeCount)
            {
                if ( pair.Value > 0 )
                {
                    if ( theBlackNodeCountForTree == 0 )
                    {
                        theBlackNodeCountForTree = pair.Value;
                    }
                    else if ( pair.Value != theBlackNodeCountForTree )
                    {
                        failedToValidate = true;
                        Console.WriteLine($"Violation: found at least two different block node counts for leafs: {theBlackNodeCountForTree} and {pair.Value}.");
                        break;
                    }
                }
            }

            if ( failedToValidate )
            {
                Console.Write("( ");
                foreach (KeyValuePair<T, int> pair in blackNodeCount)
                {
                    Console.Write($"({pair.Key}, {pair.Value}) ");
                }
                Console.WriteLine(")\n");
            }
        }

        protected Node FindSuccessor(Node node)
        {
            Node successor;

            if ( node == null )
            {
                successor = null;
            }
            else if ( node.leftChild != null )
            {
                successor = FindSuccessor(node.leftChild);
            }
            else
            {
                successor = node;
            }

            return successor;
        }

        protected Node Find(Node node, T value)
        {
            bool nodeHasTargetValue = ( node.value.CompareTo(value) == 0 );
            if ( nodeHasTargetValue )
            {
                return node;
            }

            Node matchingnode;

            if ( node.leftChild != null )
            {
                matchingnode = Find(node.leftChild, value);
                if ( matchingnode != null )
                {
                    return matchingnode;
                }
            }

            if ( node.rightChild != null )
            {
                matchingnode = Find(node.rightChild, value);
                if ( matchingnode != null )
                {
                    return matchingnode;
                }
            }

            return null;
        }

        protected Node Find(T value)
        {
            Node node = Find(root, value);
            return node;
        }

        public void Delete(T value)
        {
            Node node = Find(value);
            if ( node == null )
            {
                return;
            }

            Node nodeDeleted, nodeReplacedDeleted;
            UnbalancedDelete(node, out nodeDeleted, out nodeReplacedDeleted);

            // balance tree after delete
        }

        protected void UnbalancedDelete(Node node, out Node nodeDeleted, out Node nodeReplacedDeleted)
        {
            bool nodeHasNoChildren = ( node.leftChild == null && node.rightChild == null );
            bool nodeHasTwoChildren = ( node.leftChild != null && node.rightChild != null );
            bool nodeHasOneChild = ( !nodeHasNoChildren && !nodeHasTwoChildren );

            if ( nodeHasNoChildren )
            {
                RemoveChildFromParent(node);
                nodeDeleted = node;
                if ( nodeDeleted.color == NodeColor.Black )
                {
                    ResetDoubleBlack();
                    nodeReplacedDeleted = DoubleBlack;
                }
                else
                {
                    nodeReplacedDeleted = null;  // implicit SingleBlack node
                }
            }
            else if ( nodeHasOneChild )
            {
                Node child = ( node.leftChild ?? node.rightChild );  // example of the null-coalescing (binary) operator
                ReplaceParentWithChild(node, child);
                nodeDeleted = node;
                nodeReplacedDeleted = child;
            }
            else // if ( nodeHasTwoChildren )
            {
                Node successor = FindSuccessor(node.rightChild); // returns rightChild or its left most descendent
                // successor has no children or single right child

                // swap values for node and successor (in preparation for deleting)
                T swap = node.value;
                node.value = successor.value;
                successor.value = swap;
                // binary tree is now broke since successor's value breaks the rules
                // but not to worry, next step is delete successor

                nodeDeleted = successor;

                bool successorHasNoChildren = ( successor.leftChild == null && successor.rightChild == null );
                bool successorHasRightChild = ( successor.rightChild != null );
                if ( successorHasRightChild )
                {
                    Node rightChild = successor.rightChild;
                    ReplaceParentWithChild(successor, rightChild);

                    if ( nodeDeleted.color == NodeColor.Black )
                    {
                        nodeReplacedDeleted = rightChild;
                    }
                    else
                    {
                        nodeReplacedDeleted = null;
                    }
                }
                else if ( successorHasNoChildren )
                {
                    if ( nodeDeleted.color == NodeColor.Black )
                    {
                        ResetDoubleBlack();
                        // bit of a hack
                        MakeLeftChild(successor, DoubleBlack);
                        ReplaceParentWithChild(successor, DoubleBlack);
                        nodeReplacedDeleted = DoubleBlack;
                    }
                    else
                    {
                        RemoveChildFromParent(successor);
                        nodeReplacedDeleted = null;
                    }
                }
                else
                {
                    // successor should not have two children
                    throw new System.InvalidOperationException();
                }
            }
        }

        protected void ValidateInOrderTraverse(Node node, SortedDictionary<T, int> blackNodeCount)
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
                blackNodeCount[node.value] = 0;
                Node thisNode = node;
                while ( thisNode != null )
                {
                    if ( thisNode.color == NodeColor.Black )
                    {
                        blackNodeCount[node.value]++;
                    }
                    thisNode = thisNode.parent;
                }
            }

            if ( node.leftChild != null )
            {
                ValidateInOrderTraverse(node.leftChild, blackNodeCount);
            }

            if ( node.rightChild != null )
            {
                ValidateInOrderTraverse(node.rightChild, blackNodeCount);
            }
        }

        public void LogInOrderTraverse()
        {
            Console.WriteLine("In order traversal of the Red Black BinaryTree. Inserted values should be in sorted order:\n");
            if ( root != null )
            {
                LogInOrderTraverse(root);
                Console.WriteLine("\n");
            }
        }

        protected void LogInOrderTraverse(Node node)
        {
            if ( node.leftChild != null )
            {
                LogInOrderTraverse(node.leftChild);
            }

            string nodeColor = ( node.color == NodeColor.Red ? "r" : 
                                 node.color == NodeColor.Black ? "b" : 
                                 node.color == NodeColor.DoubleBlack ? "bb" : 
                                 "n");
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
                if ( thisNode.parent == null )
                {
                    Console.Write($"{thisNode.value}{nodeColor}");
                }
                else
                {
                    if ( thisNode.parent.leftChild == thisNode )
                    {
                        Console.Write($"{thisNode.value}{nodeColor} < ");
                    }
                    if ( thisNode.parent.rightChild == thisNode )
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
            Console.WriteLine("Logging all of the NULL children (exclusive) path to root:\n");
            LogTree(root);
            Console.WriteLine();
        }

        protected void LogTree(Node node)
        {
            if ( node.leftChild == null || node.rightChild == null )
            {
                // we are at a NULL child
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
