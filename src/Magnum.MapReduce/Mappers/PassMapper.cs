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
namespace Magnum.MapReduce.Mappers
{
	/// <summary>
	/// A full-pass mapper that simply takes the imput and passes it to the collector
	/// </summary>
	/// <typeparam name="K">Key Type</typeparam>
	/// <typeparam name="V">Value Type</typeparam>
	public class PassMapper<K, V> :
		IMapper<K, V, K, V>
	{
		public long Count { get; protected set; }

		private IReporter _reporter;

		public PassMapper(IReporter reporter)
		{
			_reporter = reporter;
		}

		public void Map(K key, V value, ICollector<K, V> collector)
		{
			Count++;

			_reporter.RecordMapped();

			collector.Collect(key, value);
		}
	}
}