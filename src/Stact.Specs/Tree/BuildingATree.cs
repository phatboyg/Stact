namespace Stact.Specs.Tree
{
    using Stact.Tree;
    using NUnit.Framework;

    [TestFixture]
    public class BuildingATree
    {

        [Test]
        public void NAME()
        {
            var tree = new Tree<object>();
            var node = tree.AddLeaf("value of T");
            var branch = tree.AddBranch();
        }
    }
}