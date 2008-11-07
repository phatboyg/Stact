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
namespace Magnum.Common.Specs.Threading
{
	using System.Threading;
	using Common.Threading;
	using DateTimeExtensions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class WrappedObjectLocking_Specs
	{
		#region Setup/Teardown

		[SetUp]
		public void Setup()
		{
			_initialValue = "Start";
			_finalValue = "Stop";
			_value = new ReaderWriterLockedObject<string>(_initialValue);
		}

		[TearDown]
		public void Teardown()
		{
			using (_value)
			{
			}
		}

		#endregion

		private string _initialValue;
		private string _finalValue;

		private ReaderWriterLockedObject<string> _value;

		[Test]
		public void A_locked_read_should_work()
		{
			string value = string.Empty;
			_value.ReadLock(x => {value = x;});

			Assert.That(value, Is.EqualTo(_initialValue));
		}

		[Test]
		public void A_locked_read_with_a_timeout_should_work()
		{
			string value = string.Empty;

			bool locked = _value.ReadLock(1.Seconds(), x => {value = x;});

			Assert.That(locked, Is.True, "Unable to obtain lock");

			Assert.That(value, Is.EqualTo(_initialValue));
		}

		[Test]
		public void An_upgradable_lock_with_a_timeout_should_work()
		{
			string value = string.Empty;

			bool locked = _value.UpgradeableReadLock(1.Seconds(), x => {value = x;});

			Assert.That(locked, Is.True, "Unable to obtain lock");

			Assert.That(value, Is.EqualTo(_initialValue));
		}

		[Test]
		public void A_separate_thread_trying_read_a_read_locked_object_should_succeed()
		{
			string threadValue = string.Empty;

			Thread lockThread = new Thread(() =>
				{
					_value.ReadLock(x =>
						{
							threadValue = x;
							Thread.Sleep(2.Seconds());
						});
				});

			lockThread.Start();

			Thread.Sleep(500.Milliseconds());

			string value = string.Empty;

			bool locked = _value.ReadLock(1.Seconds(), x => {value = x;});

			lockThread.Join();

			Assert.That(locked, Is.True);

			Assert.That(value, Is.EqualTo(_initialValue));
			Assert.That(threadValue, Is.EqualTo(_initialValue));
		}

		[Test]
		public void A_separate_thread_trying_read_a_write_locked_object_should_timeout()
		{
			Thread lockThread = new Thread(() =>
				{
					_value.WriteLock(x =>
						{
							Thread.Sleep(3.Seconds());
							return x;
						});
				});
			lockThread.Start();

			Thread.Sleep(500.Milliseconds());

			string value = string.Empty;

			bool locked = _value.ReadLock(1.Seconds(), x => {value = x;});

			lockThread.Join();

			Assert.That(locked, Is.False);

			Assert.That(value, Is.EqualTo(string.Empty));
		}

		[Test]
		public void A_timer_based_write_lock_should_be_allowed()
		{
			bool locked = _value.WriteLock(1.Seconds(), x => _finalValue);

			Assert.That(locked, Is.True);
			_value.ReadUnlocked(y => Assert.That(y, Is.EqualTo(_finalValue)));
		}

		[Test]
		public void A_write_lock_should_be_allowed()
		{
			_value.WriteLock(x => _finalValue);

			_value.ReadUnlocked(y => Assert.That(y, Is.EqualTo(_finalValue)));
		}

		[Test]
		public void Unlocked_reading_should_be_permitted()
		{
			string value = string.Empty;

			_value.ReadUnlocked(x => value = x);

			Assert.That(value, Is.EqualTo(_initialValue));
		}

		[Test]
		public void An_upgradeable_read_lock_should_be_possible()
		{
			string value = string.Empty;
			_value.UpgradeableReadLock(x =>
			{
				value = x;

				_value.WriteLock(y => _finalValue);
			});

			Assert.That(value, Is.EqualTo(_initialValue));
			_value.ReadUnlocked(y => Assert.That(y, Is.EqualTo(_finalValue)));
		}
	}
}