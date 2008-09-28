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
namespace Magnum.Common.Repository
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// A repository generalization
	/// </summary>
	public interface IRepository :
		IDisposable
	{
		/// <summary>
		/// Returns the entity for the Id specified.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		T Get<T>(object id) where T : class;

		/// <summary>
		/// Returns a list of objects
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IList<T> List<T>() where T : class;

		/// <summary>
		/// Saves an object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		void Save<T>(T item) where T : class;

        /// <summary>
        /// Updates an existing object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
	    void Update<T>(T item) where T : class;

	    /// <summary>
		/// Deletes an object from the repository
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		void Delete<T>(T item) where T : class;
	}

	/// <summary>
	/// A repository generalization
	/// </summary>
	public interface IRepository<T, K> :
		IQueryable<T>,
		IDisposable
		where T : class, IAggregateRoot<K>
	{
		/// <summary>
		/// Returns the entity for the Id specified.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns></returns>
		T Get(K id);

		/// <summary>
		/// Returns a list of objects
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IList<T> List();

		/// <summary>
		/// Saves an object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		void Save(T item);

        /// <summary>
        /// Updates an existing item
        /// </summary>
        /// <param name="item"></param>
	    void Update(T item);

		/// <summary>
		/// Deletes an object from the repository
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		void Delete(T item);
	}
}