// Copyright 2010 Chris Patterson
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
namespace Stact.Specs.AsyncIO
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using Magnum.Extensions;
	using Magnum.TestFramework;


	[Scenario]
	public class When_reading_a_file
	{
		[Then]
		public void Should_property_manage_the_async_bits()
		{
			string path = Assembly.GetExecutingAssembly().Location;

			var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);

			Continuation<string> continuation = fileStream.AsyncReadToEnd(x => { });

			var result = new Future<string>();

			continuation(result.Complete);

			result.WaitUntilCompleted(10.Seconds()).ShouldBeTrue();

			Trace.WriteLine(result.Value.Length);
		}
	}


	public class Identity<T>
	{
		public Identity(T value)
		{
			Value = value;
		}

		public T Value { get; private set; }

		public static Func<T, T> Function
		{
			get { return x => x; }
		}
	}


	public static class ExtensionsToIdentity
	{
		public static Identity<T> ToIdentity<T>(this T value)
		{
			return new Identity<T>(value);
		}

		public static Identity<U> SelectMany<T, U>(this Identity<T> id, Func<T, Identity<U>> k)
		{
			return k(id.Value);
		}

		public static Identity<V> SelectMany<T, U, V>(this Identity<T> id, Func<T, Identity<U>> k, Func<T, U, V> s)
		{
			return s(id.Value, k(id.Value).Value).ToIdentity();
		}
	}


	public delegate void Continuation<T>(Action<T> callback);


	public static class ExtensionsToContinuation
	{
		public static Continuation<T> ToContinuation<T>(this T value)
		{
			return c => c(value);
		}

		public static Continuation<U> SelectMany<T, U>(this Continuation<T> m, Func<T, Continuation<U>> k)
		{
			return c => m(x => k(x)(c));
		}

		public static Continuation<V> SelectMany<T, U, V>(this Continuation<T> m, Func<T, Continuation<U>> k, Func<T, U, V> s)
		{
			return m.SelectMany(x => k(x).SelectMany(y => s(x, y).ToContinuation()));
		}

		public static Continuation<U> Select<U, T>(this Continuation<T> m, Func<T, U> k)
		{
			return c => m(x => c(k(x)));
		}
	}


	public static class superEx
	{
		public static Continuation<string> AsyncReadToEnd(this Stream stream, Continuation<Action<Exception>> onException)
		{
			return stream.AsyncReadToEnd(onException, Encoding.UTF8);
		}

		public static void CloseAndDispose(this Stream stream)
		{
			if (stream == null)
				return;

			stream.Close();
			stream.Dispose();
		}


		/// <summary>
		/// Since the method consumes the stream, it is disposed when complete
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="onException"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static Continuation<string> AsyncReadToEnd(this Stream stream,
		                                                  Continuation<Action<Exception>> onException,
		                                                  Encoding encoding)
		{
			return respond =>
				{
					var content = new MemoryStream();
					var buffer = new byte[4096];

					Action<Exception> handleException = ex =>
						{
							stream.CloseAndDispose();
							content.CloseAndDispose();

							onException(x => x(ex));
						};

					Action reader = null;
					reader = () =>
						{
							try
							{
								stream.BeginRead(buffer, 0, buffer.Length, result =>
									{
										try
										{
											int bytesRead = stream.EndRead(result);
											if (bytesRead > 0)
											{
												content.Write(buffer, 0, bytesRead);

												reader();
											}
											else
											{
												stream.Dispose();

												byte[] bytes = content.ToArray();
												content.Dispose();

												respond(encoding.GetString(bytes));
											}
										}
										catch (Exception ex)
										{
											handleException(ex);
										}
									}, null);
							}
							catch (Exception ex)
							{
								handleException(ex);
							}
						};
					reader();
				};
		}
	}
}