namespace Magnum.Specs.CommandLine
{
    using System;
    using MbUnit.Framework;

    public class ArgumentOrderPolicy_Specs
    {
        private string[] _good_just_positional = new[] {"magnum"};
        private string[] _good_positional_then_named = new[] {"magnum", "-l:local"};
        private string[] _bad_named_before_positional = new[] {"-l:local","magnum"};
        
        [Test]
        public void You_can_have_just_one()
        {
            new ArgumentOrderPolicy<TwoArguments>().Verify(_good_just_positional);
        }

        [Test]
        public void Positional_then_named()
        {
            new ArgumentOrderPolicy<TwoArguments>().Verify(_good_positional_then_named);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void You_cant_have_named_before_positional()
        {
            new ArgumentOrderPolicy<TwoArguments>().Verify(_bad_named_before_positional);
        }
    }
}