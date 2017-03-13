using System;
using System.Collections;
using System.Collections.Generic;

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

/*
 * Two resources for Red Black Tree deletion:
 *    http://software.ucv.ro/~mburicea/lab8ASD.pdf
 *    http://staff.ustc.edu.cn/~csli/graduate/algorithms/book6/chap14.htm
 */

/*
 * C# 6.0 in a Nutshell: claims that SortedDictionary is implemented as a Red Black Tree.
 */

namespace RedBlackTree
{
    public class RedBlackTree<T> : ICollection<T> where T : IComparable
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

        protected Node NullBlack { get; set; }

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

        protected void ResetNullBlack()
        {
            NullBlack = new Node { value = default(T), parent = null, leftChild = null, rightChild = null, color = NodeColor.Black };
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
            bool parentHasOneChild = ( !parentHasNoChildren && !parentHasTwoChildren );

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
            bool nodeHasTargetValue = (node.value.CompareTo(value) == 0);
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
                bool isNodeParentLeftChild = ( node.parent.leftChild == node );
                bool isNodeParentRightChild = ( node.parent.rightChild == node );

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

            bool gotGrandParent = ( newNode.parent != null && newNode.parent.parent != null );
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


        protected void UnbalancedInsert(Node node, Node newNode)
        {
            if ( newNode.value.CompareTo(node.value) < 0 )
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
                if ( node.rightChild == null )
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
                throw new System.InvalidOperationException();
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

        protected void UnbalancedDelete(Node node, out Node nodeDeleted, out Node nodeReplacedDeleted)
        {
            bool nodeHasNoChildren  = ( node.leftChild == null && node.rightChild == null );
            bool nodeHasTwoChildren = ( node.leftChild != null && node.rightChild != null );
            bool nodeHasOneChild    = ( !nodeHasNoChildren && !nodeHasTwoChildren );

            if ( nodeHasNoChildren )
            {
                nodeDeleted = node;
                if ( nodeDeleted.color == NodeColor.Black )
                {
                    bool nodeIsRoot = ( node == root );
                    bool nodeIsLeftChild  = ( !nodeIsRoot && node == node.parent.leftChild );
                    bool nodeIsRightChild = ( !nodeIsRoot && node == node.parent.rightChild );
                    Node parent = ( node.parent ?? null );
                    nodeIsLeftChild = ( node == node.parent?.leftChild );
                    nodeIsRightChild = ( node == node.parent?.rightChild );

                    RemoveChildFromParent(node);

                    if ( nodeIsLeftChild || nodeIsRightChild )
                    {
                        ResetNullBlack();
                        nodeReplacedDeleted = NullBlack;

                        if ( nodeIsLeftChild )
                        {
                            MakeLeftChild(parent, NullBlack);
                        }
                        else if ( nodeIsRightChild )
                        {
                            MakeRightChild(parent, NullBlack);
                        }
                    }
                    else
                    {
                        // node is root
                        nodeReplacedDeleted = null;
                    }
                }
                else
                {
                    RemoveChildFromParent(node);
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
                // binary tree is now broken since successor's new value breaks the rules
                // but not to worry, next step is to delete successor

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
                        ResetNullBlack();
                        // bit of a hack
                        MakeLeftChild(successor, NullBlack);
                        ReplaceParentWithChild(successor, NullBlack);
                        nodeReplacedDeleted = NullBlack;
                    }
                    else
                    {
                        RemoveChildFromParent(successor);
                        nodeReplacedDeleted = null;
                    }
                }
                else
                {
                    // successor should not have two children or a single left child
                    throw new System.InvalidOperationException();
                }
            }
        }

        protected void BalanceTreeAfterDelete(Node node)
        {
            if ( root == null )
            {
                // the black node deleted was the root
                return;
            }

            bool leftChildOfRightSiblingIsBlack;
            bool rightChildOfRightSiblingIsBlack;
            bool leftChildOfLeftSiblingIsBlack;
            bool rightChildOfLeftSiblingIsBlack;

            while ( node != root && node.color == NodeColor.Black )
            {
                //  node is the nonroot black node that has an implicit extra black

                bool isLeftChild = ( node == node.parent.leftChild );
                if ( isLeftChild )
                {
                    // node is the left child of its parent

                    Node rightSibling = node.parent.rightChild;

                    if ( rightSibling.color == NodeColor.Red )
                    {
                        // Left Child: Case 1
                        //Console.WriteLine("Left Child: Case 1");

                        // both of rightSibling's children are non-null and black.
                        // node is double black => rightSibling subtree contains an extra black
                        // rightSibling is red => each child subtree contains an extra black => each child is non-null
                        // rightSibling is red and has non-null children => children can not be red => children are non-null and black
                        // rightSibling is red => parent node and rightSibling can not both be red => parent is black

                        rightSibling.color = NodeColor.Black;
                        node.parent.color = NodeColor.Red;
                        LeftRotate(node.parent);
                        rightSibling = node.parent.rightChild;
                    }

                    // both node and rightSibling are now black

                    leftChildOfRightSiblingIsBlack =
                        ( rightSibling.leftChild == null || rightSibling.leftChild.color == NodeColor.Black );
                    rightChildOfRightSiblingIsBlack =
                        ( rightSibling.rightChild == null || rightSibling.rightChild.color == NodeColor.Black );

                    if ( leftChildOfRightSiblingIsBlack && rightChildOfRightSiblingIsBlack )
                    {
                        // Left Child: Case 2
                        //Console.WriteLine("Left Child: Case 2");

                        // node is double black => rightSibling subtree contains an extra black
                        // rightSibling is black => children may be null and black
                        // when node is null => both rightSibling's children are null
                        // parent of node is of unknown color

                        rightSibling.color = NodeColor.Red;

                        // moving node up the tree to its parent
                        // made rightSibling red => took a black out of the red subtree
                        // node and rightSibling both need an extra black => push the extra black to the parent
                        // when parent and rightSibling are now both red => fall out of the main loop => parent is made black => done
                        // when parent is black => restart the loop with the parent being the extra black node

                        Node parent = node.parent;
                        if ( node == NullBlack )
                        {
                            // turn back into a regular null
                            RemoveChildFromParent(node);
                        }
                        node = parent;
                    }
                    else
                    {
                        // rightSibling has at least one red child

                        rightChildOfRightSiblingIsBlack =
                            ( rightSibling.rightChild == null || rightSibling.rightChild.color == NodeColor.Black );

                        if ( rightChildOfRightSiblingIsBlack )
                        {
                            // Left Child: Case 3
                            //Console.WriteLine("Left Child: Case 3");

                            // rightSibling's right child is black, possibly null
                            // rightSibling's left child is red
                            // parent of node and rightSibling is of unknown color

                            rightSibling.leftChild.color = NodeColor.Black;
                            rightSibling.color = NodeColor.Red;
                            RightRotate(rightSibling);
                            rightSibling = node.parent.rightChild;
                        }

                        // rightSibling's right child is red

                        // Left Child: Case 4
                        //Console.WriteLine("Left Child: Case 4");

                        // node and rightSibling are both black
                        // rightSibling's left child is black and right child is red
                        // below transform balances the tree 
                        // from parent's right subtree, we remove a red node, made it black and add it to the left subtree
                        // parent of node and rightSibling is of unknown color

                        rightSibling.color = node.parent.color;
                        node.parent.color = NodeColor.Black;
                        rightSibling.rightChild.color = NodeColor.Black;
                        LeftRotate(node.parent);

                        if ( node == NullBlack )
                        {
                            // can turn this back into a regular null
                            RemoveChildFromParent(node);
                        }
                        node = root;
                    }
                }
                else
                {
                    // node is the right child of its parent

                    Node leftSibling = node.parent.leftChild;

                    if ( leftSibling.color == NodeColor.Red )
                    {
                        // Right Child: Case 1
                        //Console.WriteLine("Right Child: Case 1");

                        // both of leftSibling's children are non-null and black.
                        // node is double black => leftSibling subtree contains an extra black
                        // leftSibling is red => each child subtree contains an extra black => each child is non-null
                        // leftSibling is red and has non-null children => children can not be red => children are non-null and black
                        // leftSibling is red => parent node and leftSibling can not both be red => parent is black

                        leftSibling.color = NodeColor.Black;
                        node.parent.color = NodeColor.Red;
                        RightRotate(node.parent);
                        leftSibling = node.parent.leftChild;
                    }

                    // both node and leftSibling are now black

                    leftChildOfLeftSiblingIsBlack =
                        ( leftSibling.leftChild == null || leftSibling.leftChild.color == NodeColor.Black );
                    rightChildOfLeftSiblingIsBlack =
                        ( leftSibling.rightChild == null || leftSibling.rightChild.color == NodeColor.Black );

                    if ( leftChildOfLeftSiblingIsBlack && rightChildOfLeftSiblingIsBlack )
                    {
                        // Right Child: Case 2
                        //Console.WriteLine("Right Child: Case 2");

                        // node is double black => leftSibling subtree contains an extra black
                        // leftSibling is black => children may be null and black
                        // when node is null => both leftSibling's children are null
                        // parent of node is of unknown color

                        leftSibling.color = NodeColor.Red;

                        // moving node up the tree to its parent
                        // made leftSibling red => took a black out of the left subtree
                        // node and leftSibling both need an extra black => push the extra black to the parent
                        // when parent and leftSibling are now both red => fall out of the main loop => parent is made black => done
                        // when parent is black => restart the loop with the parent being the extra black node

                        Node parent = node.parent;
                        if ( node == NullBlack )
                        {
                            // turn back into a regular null
                            RemoveChildFromParent(node);
                        }
                        node = parent;
                    }
                    else
                    {
                        // leftSibling has at least one red child

                        leftChildOfLeftSiblingIsBlack =
                            ( leftSibling.leftChild == null || leftSibling.leftChild.color == NodeColor.Black );

                        if ( leftChildOfLeftSiblingIsBlack )
                        {
                            // Right Child: Case 3
                            //Console.WriteLine("Right Child: Case 3");

                            // leftSibling's left child is black, possibly null
                            // leftSibling's right child is red
                            // parent of node and rightSibling is of unknown color

                            leftSibling.rightChild.color = NodeColor.Black;
                            leftSibling.color = NodeColor.Red;
                            LeftRotate(leftSibling);
                            leftSibling = node.parent.leftChild;
                        }

                        // leftSibling's left child is red

                        // Right Child: Case 4
                        //Console.WriteLine("Right Child: Case 4");

                        // node and leftSibling are both black
                        // leftSibling's right child is black and left child is red
                        // below transform balances the tree 
                        // from parent's left subtree, we remove a red node, made it black and add it to the right subtree
                        // parent of node and rightSibling is of unknown color

                        leftSibling.color = node.parent.color;
                        node.parent.color = NodeColor.Black;
                        leftSibling.leftChild.color = NodeColor.Black;
                        RightRotate(node.parent);

                        if ( node == NullBlack )
                        {
                            // can turn this back into a regular null
                            RemoveChildFromParent(node);
                        }
                        node = root;
                    }
                }
            }

            node.color = NodeColor.Black;
        }

        //
        // Iterator
        //

        // compiler parses "yield return" and 
        // builds a hidden nested IEnumerable class 
        // (who has the all important method GetEnumerator()) and 
        // refactors below method to return it.

        // wrt foreach (T value in <object>), the compiler implicitly casts <object> 
        // to IEnumerable<T> and calls its GetEnumerator(), which returns an iterator.

        protected IEnumerable<T> RecursivelyIterate(Node node)
        {
            if ( node.leftChild != null )
            {
                foreach ( var child in RecursivelyIterate(node.leftChild) )
                {
                    yield return child;
                }
            }

            yield return node.value;

            if ( node.rightChild != null )
            {
                foreach ( var child in RecursivelyIterate(node.rightChild) )
                {
                    yield return child;
                }
            }
        }

        public IEnumerable<T> RecursivelyIterate()
        {
            return RecursivelyIterate(root);
        }

        //
        // Validation
        //

        protected void ValidateInOrderTraverse(Node node, SortedDictionary<T, int> blackNodeCount)
        {
            if ( node == root && root == null )
            {
                return;
            }

            if ( root != null && node == root && node.color != NodeColor.Black )
            {
                Console.WriteLine("**** Violation: root is not black.");
                throw new System.InvalidOperationException();
            }

            if ( node.parent != null && node.color == NodeColor.Red && node.parent.color == NodeColor.Red )
            {
                Console.WriteLine("**** Violation: two adjacent nodes are red.");
                throw new System.InvalidOperationException();
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
                if ( node.value.CompareTo(node.leftChild.value) < 0 )
                {
                    Console.WriteLine($"**** Violation: value of the node({node.value}) should be greater than the value of its left child({node.leftChild.value}).");
                    throw new System.InvalidOperationException();
                }
                ValidateInOrderTraverse(node.leftChild, blackNodeCount);
            }

            if ( node.rightChild != null )
            {
                if ( node.value.CompareTo(node.rightChild.value) > 0 )
                {
                    Console.WriteLine($"**** Violation: value of the node({node.value}) should be greater than the value of its right child({node.rightChild.value}).");
                    throw new System.InvalidOperationException();
                }
                ValidateInOrderTraverse(node.rightChild, blackNodeCount);
            }
        }

        public void ValidateInOrderTraverse(bool includeHeader = true)
        {
            if ( includeHeader )
            {
                Console.WriteLine("Validating all of the Red Back Tree requirements:\n");
            }

            SortedDictionary<T, int> blackNodeCount = new SortedDictionary<T, int>();
            ValidateInOrderTraverse(root, blackNodeCount);

            int theBlackNodeCountForTree = 0;
            bool failedToValidate = false;
            foreach ( KeyValuePair<T, int> pair in blackNodeCount )
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
                        Console.WriteLine($"**** Violation: found at least two different block node counts for leafs: {theBlackNodeCountForTree} and {pair.Value}.");
                        break;
                    }
                }
            }

            if ( failedToValidate )
            {
                Console.Write("**** ( ");
                foreach ( KeyValuePair<T, int> pair in blackNodeCount )
                {
                    Console.Write($"({pair.Key}, {pair.Value}) ");
                }
                Console.WriteLine(")\n");
                throw new System.InvalidOperationException();
            }
        }

        //
        // Logging
        //

        public void LogInOrderTraverse()
        {
            Console.WriteLine("In order traversal of the Red Black Binary Tree. Values should be in sorted order:\n");
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

            string nodeColor = (node.color == NodeColor.Red ? "r" :
                                 node.color == NodeColor.Black ? "b" :
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
            while ( thisNode != null )
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
            if ( node == root && node == null )
            {
                // empty RBT
                return;
            }

            if ( node.leftChild == null || node.rightChild == null )
            {
                // we are at a leaf child
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

        //
        // implement ICollection<T>
        //
        // hint: at coding startup, add the interface ICollection<T>
        // and let the Visual Studio generate the required, empty methods.
        //

        protected int count { get; set; }
        public int Count { get { return count; } }

        protected bool isReadOnly { get; set; } // TODO: make Add() and Remove() abide
        public bool IsReadOnly { get { return isReadOnly; } }

        public void Add(T item)
        {
            Node newNode = UnbalancedInsert(item);
            newNode.color = NodeColor.Red;
            //LogNode(newNode, "insert new value");

            BalanceTreeAfterInsert(newNode);
            //LogTree();
            //ValidateInOrderTraverse();
            //LogInOrderTraverse();

            count++;
        }

        public void Clear()
        {
            root = null;
            count = 0;
            isReadOnly = false;
        }

        public bool Contains(T item)
        {
            return ( Find(item) != null );
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int idx = arrayIndex;
            foreach ( T value in (IEnumerable<T>) this ) // make the impicit cast explicit, as a remider how iterators actually works
            {
                array[idx++] = value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            IEnumerator<T> iterator = RecursivelyIterate().GetEnumerator();
            return iterator;
        }

        public bool Remove(T item)
        {
            Node node = Find(item);
            if ( node == null )
            {
                return false;
            }

            Node nodeDeleted, nodeReplacedDeleted;
            UnbalancedDelete(node, out nodeDeleted, out nodeReplacedDeleted);

            // balance tree after delete, if a black node has been deleted
            // since not all paths from root to leaf still have the black count.

            bool blackNodeDeleted = (nodeDeleted.color == NodeColor.Black);
            if ( blackNodeDeleted )
            {
                BalanceTreeAfterDelete(nodeReplacedDeleted);
                //LogTree();
                ValidateInOrderTraverse(false);  // silently validate the black node deletion rebalance
                //LogInOrderTraverse();
            }

            count--;

            return true;
        }
    }
}
