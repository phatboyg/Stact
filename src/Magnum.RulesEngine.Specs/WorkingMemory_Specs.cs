namespace Magnum.RulesEngine.Specs
{
	using System;
	using System.Diagnostics;
	using System.Linq.Expressions;
	using System.Text.RegularExpressions;
	using MbUnit.Framework;
	using Model;

	[TestFixture]
	public class WorkingMemory_Specs
	{
		[Test]
		public void Objects_should_be_stored_in_working_memory()
		{
			Person p = new Person {FirstName = "Chris", LastName = "Patterson"};
			Address a = new Address {Street = "123 American Way", City = "Anytown", State = "OK", Country = "USA"};
			Member m = new Member {FirstName = "Joe", LastName = "Schmoe"};

			IWorkingMemory memory = new WorkingMemory();
			memory.Add(p);
			memory.Add(a);
			memory.Add(m);


			var list = memory.List<Person>();
			while (list.MoveNext())
			{
				Trace.WriteLine("LastName: " + list.Current.LastName);
			}

			Expression<Func<Person, bool>> exp = x => Regex.Match(x.FirstName, "Chr.*").Success;

			Func<Person, bool> fun = exp.Compile();
		}
	}
}