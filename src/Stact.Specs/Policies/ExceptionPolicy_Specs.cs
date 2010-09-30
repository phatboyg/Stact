namespace Stact.Specs.Policies
{
	using System;
	using Stact.Extensions;
	using Stact.Policies;
	using NUnit.Framework;

	[TestFixture]
	public class Specifying_a_retry_policy_on_an_action_that_throws_an_exception :
		BehaviorTest
	{
		private ExceptionPolicy _policy;
		private int _attemptCount;
		private int _failureCount;

		[Test]
		public void Should_call_the_action_the_number_of_retries_plus_one()
		{
			Assert.AreEqual(6, _attemptCount);
		}

		[Test]
		public void Should_call_the_retry_action_once_for_each_failure()
		{
			Assert.AreEqual(5, _failureCount);
		}

		protected override void Given()
		{
			_attemptCount = 0;
			_failureCount = 0;
			_policy = ExceptionPolicy.InCaseOf<InvalidOperationException>().Retry(5, (ex, c) => _failureCount++);
		}

		protected override void When()
		{
			try
			{
				_policy.Do(() =>
					{
						_attemptCount++;

						throw new InvalidOperationException();
					});
			}
			catch (InvalidOperationException)
			{
			}
		}

	}


	[TestFixture]
	public class Specifying_a_policy_that_returns_a_value :
		BehaviorTest
	{
		private ExceptionPolicy _policy;
		private int _expectedValue;
		private int _returnedValue;

		[Test]
		public void Should_return_the_value_to_the_caller()
		{
			Assert.AreEqual(_expectedValue, _returnedValue);
		}

		protected override void Given()
		{
			_policy = ExceptionPolicy.InCaseOf<InvalidOperationException>().Retry(5);
			_expectedValue = 27;
		}

		protected override void When()
		{
			_returnedValue = _policy.Do(() => GetValue());
		}

		private int GetValue()
		{
			return _expectedValue;
		}

	}

	[TestFixture]
	public class Specifying_a_retry_policy_with_a_series_of_time_intervals :
		BehaviorTest
	{
		private ExceptionPolicy _policy;
		private int _attemptCount;
		private int _failureCount;

		[Test]
		public void Should_use_all_of_the_times_before_throwing_out()
		{
			Assert.AreEqual(4, _attemptCount);
		}

		[Test]
		public void Should_call_the_retry_action_once_for_each_failure()
		{
			Assert.AreEqual(3, _failureCount);
		}

		protected override void Given()
		{
			_attemptCount = 0;
			_failureCount = 0;
			var intervals = new[]{10.Milliseconds(), 20.Milliseconds(), 30.Milliseconds()};
			_policy = ExceptionPolicy.InCaseOf<InvalidOperationException>().Retry(intervals, (ex, c) => _failureCount++);
		}

		protected override void When()
		{
			try
			{
				_policy.Do(() =>
					{
						_attemptCount++;

						throw new InvalidOperationException();
					});
			}
			catch (InvalidOperationException)
			{
			}
		}

	}

	[TestFixture]
	public class Specifying_a_retry_policy_with_a_repeating_time_interval :
		BehaviorTest
	{
		private ExceptionPolicy _policy;
		private int _attemptCount;
		private int _failureCount;

		[Test]
		public void Should_use_all_of_the_times_before_throwing_out()
		{
			Assert.AreEqual(6, _attemptCount);
		}

		[Test]
		public void Should_call_the_retry_action_once_for_each_failure()
		{
			Assert.AreEqual(5, _failureCount);
		}

		protected override void Given()
		{
			_attemptCount = 0;
			_failureCount = 0;
			var intervals = 10.Milliseconds().Repeat(5);
			_policy = ExceptionPolicy.InCaseOf<InvalidOperationException>().Retry(intervals, (ex, c) => _failureCount++);
		}

		protected override void When()
		{
			try
			{
				_policy.Do(() =>
					{
						_attemptCount++;

						throw new InvalidOperationException();
					});
			}
			catch (InvalidOperationException)
			{
			}
		}

	}

	[TestFixture]
	public class Specifying_a_circuit_breaker_policy :
		BehaviorTest
	{
		private ExceptionPolicy _policy;
		private int _attemptCount;
		private int _failureCount;

		private readonly int _attemptLimit = 10;

		[Test]
		public void Should_stop_calling_the_resource_after_the_specified_number_of_breaks()
		{
			Assert.AreEqual(1, _attemptCount);
		}

		[Test]
		public void Should_ensure_all_attempts_were_tried()
		{
			Assert.AreEqual(_attemptLimit, _failureCount);
		}

		protected override void Given()
		{
			_attemptCount = 0;
			_failureCount = 0;
			_policy = ExceptionPolicy.InCaseOf<InvalidOperationException>().CircuitBreak(_attemptLimit.Seconds(), 1);
		}

		protected override void When()
		{
			for (int i = 0; i < _attemptLimit; i++)
			{
				try
				{
					_policy.Do(() =>
						{
							_attemptCount++;

							throw new InvalidOperationException();
						});
				}
				catch (InvalidOperationException)
				{
					_failureCount++;
				}
			}
		}

	}

	[TestFixture]
	public class Specifying_a_circuit_breaker_policy_with_a_break_duration :
		BehaviorTest
	{
		private ExceptionPolicy _policy;
		private int _attemptCount;

		[Test]
		public void Should_resume_calling_the_action_after_the_break_duration_expires()
		{
			Assert.AreEqual(2, _attemptCount);
		}

		protected override void Given()
		{
			_attemptCount = 0;
			_policy = ExceptionPolicy.InCaseOf<InvalidOperationException>().CircuitBreak(1.Seconds(), 1);
		}

		protected override void When()
		{
			Action action = () =>
				{
					_attemptCount++;

					throw new InvalidOperationException();
				};

			SystemUtil.Reset();

			DateTime now = SystemUtil.Now;

			DateTime start = now;
			SystemUtil.SetNow(() => start);

			for (int i = 0; i < 10; i++)
			{
				try
				{
					_policy.Do(action);
				}
				catch (InvalidOperationException)
				{
				}
			}

			DateTime next =now + 1.Seconds();
			SystemUtil.SetNow(() => next);

			try
			{
				_policy.Do(action);
			}
			catch (InvalidOperationException)
			{
			}

			SystemUtil.Reset();
		}

	}
}
