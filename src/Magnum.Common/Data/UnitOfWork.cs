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
namespace Magnum.Common.Data
{
	using System;

	/// <summary>
	/// This is really just a facade around the <see cref="LocalContext"/> storage
	/// </summary>
	public static class UnitOfWork
	{
		public const string CurrentUnitOfWorkKey = "UnitOfWork.Current.Key";

		private static Func<IUnitOfWork> _createUnitOfWork = () => { throw new InvalidOperationException("No provider was setup"); };

		public static void SetUnitOfWorkProvider(Func<IUnitOfWork> provider)
		{
			_createUnitOfWork = provider;
		}

		public static bool IsActive
		{
			get { return LocalContext.Current.Contains(CurrentUnitOfWorkKey); }
		}

		public static IUnitOfWork Current
		{
			get { return IsActive ? LocalContext.Current.Retrieve<IUnitOfWork>(CurrentUnitOfWorkKey) : null; }
			set
			{
				if (value == null)
					LocalContext.Current.Remove(CurrentUnitOfWorkKey);
				else
					LocalContext.Current.Store(CurrentUnitOfWorkKey, value);
			}
		}

		public static IUnitOfWork Start()
		{
			if (IsActive)
				return Current;

			Current = _createUnitOfWork();

			return Current;
		}

		public static void Finish()
		{
			if (!IsActive)
				throw new InvalidOperationException("NO unit of work is active");

			Current.Dispose();
			Current = null;
		}
	}
}