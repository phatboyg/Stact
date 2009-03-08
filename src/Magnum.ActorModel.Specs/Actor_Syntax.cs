namespace Magnum.ActorModel.Specs
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using CommandQueues;
	using NUnit.Framework;
	using Threading;

	[TestFixture]
	public class Actor_Syntax
	{
		[Test]
		public void The_syntax_should_support_a_linear_flow_of_execution_without_inversion_of_control()
		{
			DefaultActorExecutor executor = new DefaultActorExecutor();
			TestActor actor = new TestActor(executor);

			int result = actor.Execute<int>(new Add(56, 23));

			Assert.AreEqual(79, result);
		}

		[Test]
		public void jflksjdf()
		{
			SessionStore sessionStore = new SessionStore();







			sessionStore.Send(new CreateNewSession(), x =>
				{
					x.When<Session>(session =>
						{
							session.Touch();

							// use the session for things that need to be done
							
						});
				});

		}
	}



		public class SessionStore
	{
		private readonly object _lock = new object();

		public IEnumerator<ActorStep> Act()
		{
			while(true)
			{
				yield return 
					Receive(x =>
					{
						x.When<GetExistingSession>(GetSession);

						x.When<CreateNewSession>(CreateSession);
					});
				
			}
		}

		private void CreateSession(CreateNewSession message)
		{
		}

		private ActorStep Receive(Action<ReceiveContext> action)
		{

			
		}

		private void GetSession(GetExistingSession message)
		{
			lock(_lock)
			{


			}
		}
	}

	public class CreateNewSession
	{
	}

	public class ReceiveContext
	{
		public void When<T>(Action<T> session)
		{
				
		}
	}

	public class GetExistingSession
	{
		public Guid SessionId { get; set; }
	}


		public class Session
	{
		private readonly Guid _sessionId;

		public SessionActor(Guid sessionId)
		{
			_sessionId = sessionId;
		}
	}










	public class TestActor
	{
		private ActorExecutor _executor;
		private CommandQueue _queue;

		public TestActor(ActorExecutor executor)
		{
			_executor = executor;
			_queue = new ThreadPoolCommandQueue(new SynchronousCommandExecutor());
		}

		public IEnumerator<ActorStep> Act(int a, int b, Action<int> respond)
		{
			AddingActor adder = new AddingActor();
			adder.Send(this, new Add(a, b));

			yield return
				Receive(x =>
					{
						x.When<int>(y => respond(y));
					});
		}

		public void Enqueue(Func<IEnumerator<ActorStep>> func)
		{
			_queue.Enqueue(() => _executor.Execute(func()));
		}

		public static ActorStep Suspend(int timeout)
		{
			//ThreadPool.RegisterWaitForSingleObject()

			return new SuspendActorStep(timeout);
		}

		public TResult Execute<TResult>(object input)
		{
		}

		public IEnumerator<ActorStep> Act(int a, int b, Action<int> respond)
		{
			AddingActor adder = new AddingActor();
			adder.Send(this, new Add(a, b));

			yield return
				Receive(x =>
					{
						x.When<int>(y => respond(y));
					});
		}
	}

	internal class AddingActor
	{
	}

	internal class Add
	{
		private readonly int _a;
		private readonly int _b;

		public Add(int a, int b)
		{
			_a = a;
			_b = b;
		}

		public int B
		{
			get { return _b; }
		}

		public int A
		{
			get { return _a; }
		}
	}

	public class SuspendActorStep : ActorStep
	{
		public SuspendActorStep(int timeout)
		{
			throw new NotImplementedException();
		}

		public bool IsCompleted
		{
			get { return false; }
		}
	}

	public interface ActorExecutor
	{
		/// <summary>
		/// Executes a method containing yields, but waits for said method to complete
		/// </summary>
		/// <param name="enumerator">The method to execute</param>
		void Execute(IEnumerator<ActorStep> enumerator);

		/// <summary>
		/// Asynchronously executes a method containing yields
		/// </summary>
		/// <param name="enumerator">The method to execute</param>
		/// <param name="callback">The asynchronous method to call back</param>
		/// <param name="state">A state object passed to the asynchronous callback</param>
		/// <returns></returns>
		IAsyncResult BeginExecute(IEnumerator<ActorStep> enumerator, AsyncCallback callback, object state);

		/// <summary>
		/// Completes the execution of an asynchronous methods
		/// </summary>
		/// <param name="asyncResult"></param>
		void EndExecute(IAsyncResult asyncResult);

		/// <summary>
		/// Cancel the execution of an asynchronous method
		/// </summary>
		void Cancel();

		/// <summary>
		/// Returns a callback for the BeginXXX method being yielded
		/// </summary>
		/// <returns></returns>
		AsyncCallback End();

		/// <summary>
		/// Returns the next async result in the queue of completed asynchronous methods
		/// </summary>
		/// <returns></returns>
		IAsyncResult Result();
	}

	public interface ActorStep
	{
		bool IsCompleted { get; }
	}

	public class DefaultActorExecutor :
		ActorExecutor
	{
		private readonly object _enumeratorLock = new object();
		private readonly ReaderWriterLockedObject<Queue<IAsyncResult>> _resultQueue;
		private readonly SynchronizationContext _syncContext;
		private readonly ReaderWriterLockedObject<int> _waitCount;
		private AsyncResult _asyncResult;
		private volatile bool _cancelled;
		private IEnumerator<ActorStep> _enumerator;
		private ActorStep _step;

		public DefaultActorExecutor()
		{
			_resultQueue = new ReaderWriterLockedObject<Queue<IAsyncResult>>(new Queue<IAsyncResult>());
			_waitCount = new ReaderWriterLockedObject<int>(0);

			_syncContext = SynchronizationContext.Current;
		}

		public void Cancel()
		{
			_cancelled = true;
		}

		public void Execute(IEnumerator<ActorStep> enumerator)
		{
			EndExecute(BeginExecute(enumerator, null, null));
		}

		public IAsyncResult BeginExecute(IEnumerator<ActorStep> enumerator, AsyncCallback callback, object state)
		{
			_enumerator = enumerator;
			_asyncResult = new AsyncResult(callback, state);

			ContinueEnumerator(true);

			return _asyncResult;
		}

		public void EndExecute(IAsyncResult asyncResult)
		{
			_asyncResult.EndInvoke();
			_asyncResult = null;
		}

		public AsyncCallback End()
		{
			return EnqueueResultToInbox;
		}

		public IAsyncResult Result()
		{
			return _resultQueue.WriteLock(x => x.Dequeue());
		}

		private void EnqueueResultToInbox(IAsyncResult asyncResult)
		{
			_resultQueue.WriteLock(x => x.Enqueue(asyncResult));

			if (_resultQueue.ReadLock(x => x.Count) == _waitCount.ReadLock(x => x))
			{
				ContinueEnumerator(false);
			}
		}

		private void ContinueEnumerator(bool outsideOfSyncContext)
		{
			if (_syncContext != null && outsideOfSyncContext)
			{
				_syncContext.Post(SyncContextContinueEnumerator, this);
				return;
			}

			Exception caughtException = null;

			lock (_enumeratorLock)
			{
				bool stillGoing = false;
				try
				{
					while ((stillGoing = _enumerator.MoveNext()))
					{
						if (HasBeenCancelled())
							continue;

						ActorStep step = _enumerator.Current;
						if (step.IsCompleted)
						{
							ThreadPool.QueueUserWorkItem(ThreadPoolContinueEnumerator, this);
							return;
						}

						var previousStep = Interlocked.Exchange(ref _step, step);
						return;
					}
				}
				catch (Exception ex)
				{
					caughtException = ex;
					throw;
				}
				finally
				{
					if (!stillGoing)
					{
						_enumerator.Dispose();

						if (caughtException != null)
						{
							_asyncResult.SetAsCompleted(caughtException);
						}
						else
						{
							_asyncResult.SetAsCompleted();
						}
					}
				}
			}
		}

		private bool HasBeenCancelled()
		{
			return _cancelled;
		}

		private static void SyncContextContinueEnumerator(object state)
		{
			((DefaultActorExecutor) state).ContinueEnumerator(false);
		}

		private static void ThreadPoolContinueEnumerator(object state)
		{
			((DefaultActorExecutor) state).ContinueEnumerator(true);
		}

		public static void Run(Func<IAsyncExecutor, IEnumerator<int>> action)
		{
			AsyncExecutor executor = new AsyncExecutor();

			executor.Execute(action(executor));
		}

		public static IAsyncResult RunAsync(Func<ActorExecutor, IEnumerator<ActorStep>> action, Action complete)
		{
			ActorExecutor executor = new DefaultActorExecutor();

			AsyncCallback callback = x =>
				{
					executor.EndExecute(x);
					complete();
				};

			return executor.BeginExecute(action(executor), callback, null);
		}

		public static IAsyncResult RunAsync<T>(Func<ActorExecutor, IEnumerator<ActorStep>> action, T state, Action<T> complete)
		{
			ActorExecutor executor = new DefaultActorExecutor();

			AsyncCallback callback = x =>
				{
					executor.EndExecute(x);
					complete(state);
				};

			return executor.BeginExecute(action(executor), callback, state);
		}
	}
}