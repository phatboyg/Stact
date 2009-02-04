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
	/// Supports the creation of repository instances that should be disposed once they are used
	/// </summary>
	public interface IRepositoryFactory :
		IDisposable
	{
		/// <summary>
		/// Returns an untyped repository that can be used for any class stored in the database
		/// </summary>
		/// <returns></returns>
		IRepository GetRepository();

		/// <summary>
		/// Returns a typed repository that can be used and queried (using LINQ)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IRepository<T, Guid> GetRepository<T>() where T : class, IAggregateRoot;

		/// <summary>
		/// Returns a typed repository that can be used and queried using LINQ
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="K"></typeparam>
		/// <returns></returns>
		IRepository<T, K> GetRepository<T, K>() where T : class, IAggregateRoot<K>;
	}
}