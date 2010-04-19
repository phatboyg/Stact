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
namespace Sample.WebActors.Controllers
{
	using System;
	using System.Web.Mvc;

	public class BenchmarkController :
		Controller
	{
		private static DateTime expected = new DateTime(1980, 12, 31);

		[HttpGet]
		public ActionResult Index()
		{
			return View(new BenchmarkFormViewModel());
		}

		[HttpPost]
		public ActionResult Index(BenchmarkFormViewModel model)
		{
			if (model.Name != "asdf")
				throw new InvalidOperationException("Invalid Name");
			if (model.Address != "jkl")
				throw new InvalidOperationException("Invalid Address");
			if (model.Age != 123)
				throw new InvalidOperationException("Invalid Age");
			if (!model.CreateDate.HasValue || model.CreateDate.Value != expected)
				throw new InvalidOperationException("Invalid Date");
			return Content("Thanks!");
		}
	}

	public class BenchmarkFormViewModel
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public int Age { get; set; }
		public DateTime? CreateDate { get; set; }
	}
}