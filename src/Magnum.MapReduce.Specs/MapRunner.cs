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
namespace Magnum.MapReduce.Specs
{
	using System.Collections.Generic;
	using Common;

	internal class MapRunner<KInput, VInput, KOutput, VOutput>
	{
		private readonly ICollector<KOutput, VOutput> _collector;
		private readonly IMapper<KInput, VInput, KOutput, VOutput> _mapper;
		private readonly IRecordReader<KInput, VInput> _reader;

		public MapRunner(IRecordReader<KInput, VInput> reader, IMapper<KInput, VInput, KOutput, VOutput> mapper, ICollector<KOutput, VOutput> collector)
		{
			_reader = reader;
			_collector = collector;
			_mapper = mapper;
		}

		public void Run()
		{
			KeyValuePair<KInput, VInput> tuple;
			while ((tuple = _reader.Next()).Equals(default(KeyValuePair<KInput, VInput>)) == false)
			{
				_mapper.Map(tuple.Key, tuple.Value, _collector);
			}
		}
	}
}