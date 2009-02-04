namespace Magnum.Specs.CommandLine
{
    using MbUnit.Framework;

    public class Argument_Spec
    {
        [Test]
        public void Positional()
        {
            Argument arg = new Argument("magnum");
            arg.IsPostional.ShouldBeTrue();
            arg.Value.ShouldEqual("magnum");
        }

        [Test]
        public void Named()
        {
            Argument arg = new Argument("-m:\"billy\"");
            arg.IsPostional.ShouldBeFalse();
            arg.Name.ShouldEqual("m");
            arg.Value.ShouldEqual("billy");
            arg.IsShortForm.ShouldBeTrue();
        }

        [Test]
        public void Named_With_Value_Space()
        {
            Argument arg = new Argument("-m:\"billy bob\"");
            arg.IsPostional.ShouldBeFalse();
            arg.IsShortForm.ShouldBeTrue();
            arg.Name.ShouldEqual("m");
            arg.Value.ShouldEqual("billy bob");
        }

        [Test]
        public void Short_Form()
        {
            Argument arg = new Argument("-l:local");
            arg.IsPostional.ShouldBeFalse();
            arg.IsShortForm.ShouldBeTrue();
            arg.Name.ShouldEqual("l");
            arg.Value.ShouldEqual("local");
        }

        [Test]
        public void Long_Form()
        {
            Argument arg = new Argument("--Location:local");
            arg.IsPostional.ShouldBeFalse();
            arg.IsShortForm.ShouldBeFalse();
            arg.Name.ShouldEqual("Location");
            arg.Value.ShouldEqual("local");
        }
    }
}