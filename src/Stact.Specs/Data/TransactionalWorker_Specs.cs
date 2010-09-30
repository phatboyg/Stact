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
namespace Stact.Specs.Data
{
	using System;
	using System.Diagnostics;
	using System.Security.AccessControl;
	using System.Threading;
	using System.Transactions;
	using Fibers;
	using Stact.Extensions;
	using NUnit.Framework;

	[TestFixture]
	public class Creating_a_transaction_scope_as_part_of_an_action
	{
		[Test]
		public void Should_be_passable_to_another_action_queue()
		{
			Fiber fiber = new ThreadPoolFiber();


			var timer = Stopwatch.StartNew();

			Stopwatch inner = new Stopwatch();
			Stopwatch dep = new Stopwatch();
			try
			{
				using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
				{
					inner.Start();

					DependentTransaction dependentClone = Transaction.Current.DependentClone(DependentCloneOption.BlockCommitUntilComplete);

					dependentClone.TransactionCompleted += (sender,args) =>
						{
							Trace.WriteLine("Completed");		
						};

					fiber.Add(() =>
						{
							dep.Start();
							try
							{
								//ThreadUtil.Sleep(20.Milliseconds());

								Trace.WriteLine("complieing");
								dependentClone.Complete();
								dep.Stop();
								Trace.WriteLine("done");
							}
							catch (Exception ex)
							{
								dependentClone.Rollback(ex);
							}
							finally
							{
								dependentClone.Dispose();
								Trace.WriteLine("clone disposed");
							}
						});

					scope.Complete();
					Trace.WriteLine("scope complete");
				}

				Trace.WriteLine("all done");
			}
			catch (Exception)
			{
			}

			inner.Stop();
			timer.Stop();

			Trace.WriteLine("Timer: " + (int)timer.ElapsedMilliseconds);
			Trace.WriteLine("Inner: " + (int)inner.ElapsedMilliseconds);
			Trace.WriteLine("Dep: " + (int)dep.ElapsedMilliseconds);
		}

		public void DoSomething(object state)
		{
			var transaction = state as DependentTransaction;

			Guard.AgainstNull(transaction, "transaction");
			using (transaction)
			{
				ThreadUtil.Sleep(100.Milliseconds());

				transaction.Rollback(new InvalidOperationException("System screwed up"));
			//	transaction.Complete();
			}
		}
	}
}