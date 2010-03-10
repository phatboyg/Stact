namespace Magnum.Tree
{
    using System;
    using System.Collections.Generic;

    public class Tree<T>
    {
        readonly Branch _branch;

        public Tree()
        {
            _branch = new Branch();
        }

        public Leaf<T> AddLeaf<T>(T leafValue)
        {
            var leaf = new Leaf<T>(leafValue);
            _branch.AddChild(leaf);
            return leaf;
        }

        public Branch AddBranch()
        {
            var branch = new Branch();
            _branch.AddChild(branch);
            return branch;
        }

        //enumerate depth first / breadth first

        //contains

        //root/leaf/node/branch

        //persistability
        // 1.1.2 ??
    }


    public interface Node
    {

    }

    public class Leaf<T> :
        Node
    {
        readonly T _value;

        public Leaf(T value)
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
        }
    }

    public class Branch :
        Node
    {
        readonly HashSet<Node> _children;

        public Branch()
        {
            _children = new HashSet<Node>();
        }

        public void AddChild(Node node)
        {
            _children.Add(node);
        }
    }

}