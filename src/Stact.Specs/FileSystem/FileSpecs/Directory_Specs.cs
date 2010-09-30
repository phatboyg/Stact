namespace Stact.Specs.FileSystem.FileSpecs
{
    using Stact.FileSystem;
    using TestFramework;

    public class Directory_Specs
    {
        DotNetDirectory _dir;
        DotNetFileSystem _fs;

        [Given]
        public void GivenADirectoryWithAFile()
        {
            _fs = new DotNetFileSystem();
            _dir = new DotNetDirectory(DirectoryName.GetDirectoryName("bob"));
            var file = _dir.GetChildFile("test.txt");

            _fs.CreateDirectory(_dir);
            _fs.CreateFile(file);
        }

        [Then]
        public void Copy()
        {
            _dir.CopyTo(DirectoryName.GetDirectoryName("copy"));
            _fs.DirectoryExists("copy").ShouldBeTrue();
            _fs.GetDirectory("copy").GetChildFile("test.txt").Exists().ShouldBeTrue();
        }
    }
}