namespace Magnum.RulesEngine.Specs
{
	using System.Collections.Generic;

	public class WorkingMemory :
		IWorkingMemory
	{
		private readonly HashSet<object> _objects = new HashSet<object>();

		public void Add(params object[] objs)
		{
			for (int i = 0; i < objs.Length; i++)
			{
				_objects.Add(objs[i]);
			}
		}

		public IEnumerator<T> List<T>() where T : class
		{
			foreach (object obj in _objects)
			{
				if (typeof(T).IsAssignableFrom(obj.GetType()))
					yield return obj as T;
			}
		}
	}
}