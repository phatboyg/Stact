// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Actions
{
	using System;
	using Logging;

	public class SynchronousActionQueue :
		ActionQueue
	{
		private readonly ILogger _log = Logger.GetLogger<SynchronousActionQueue>();

		private bool _disabled;

		public void Enqueue(Action action)
		{
			if (_disabled)
				return;

			try
			{
				action();
			}
			catch (Exception ex)
			{
				_log.Error(ex);
			}
		}

		public void EnqueueMany(params Action[] actions)
		{
			actions.Each(action =>
				{
					if (_disabled)
						return;

					try
					{
						action();
					}
					catch (Exception ex)
					{
						_log.Error(ex);
					}
				});
		}

		public bool RunAll(TimeSpan timeout)
		{
			return true;
		}

		public void Disable()
		{
			_disabled = true;
		}
	}
}