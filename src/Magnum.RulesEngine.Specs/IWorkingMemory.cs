namespace Magnum.RulesEngine.Specs
{
	using System.Collections.Generic;

	public interface IWorkingMemory
	{
		void Add(params object[] objs);

		IEnumerator<T> List<T>() where T : class;
	}
}