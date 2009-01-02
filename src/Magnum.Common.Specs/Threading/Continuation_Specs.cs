namespace Magnum.Common.Specs.Threading
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Threading;
	using DateTimeExtensions;
	using MbUnit.Framework;
	using Monads;

	[TestFixture]
	public class Continuation_Specs
	{
		[Test]
		public void I_want_a_nice_way_to_express_functions_without_all_the_noise()
		{
			var r = 5.ToIdentity().SelectMany(x => 6.ToIdentity(), (x, y) => x + y);

			Assert.AreEqual(11, r.Value);

			var t = from x in 5.ToIdentity()
			from y in 6.ToIdentity()
			select x + y;

			Assert.AreEqual(11, t.Value);
		}

		[Test]
		public void Should_propogate_nullable_values()
		{
			var r = from x in 5.ToMaybe() from y in Maybe<int>.Nothing select x + y;

			Assert.IsFalse(r.HasValue);
		}

		[Test]
		public void Lets_pull_some_magic_juice()
		{
			var requests = new[]
			               	{
			               		WebRequest.Create("http://www.google.com/"),
			               		WebRequest.Create("http://www.yahoo.com/"),
			               		WebRequest.Create("http://channel9.msdn.com/")
			               	};
			var pages = from request in requests
			select
				from response in request.GetResponseAsync()
				let stream = response.GetResponseStream()
				from html in stream.ReadToEndAsync()
				select new {html, response};

			foreach (var page in pages)
			{
				page(d =>
					{
						Trace.WriteLine(d.response.ResponseUri.ToString());
						Trace.WriteLine(d.html.Substring(0, 40));
					});
			}

			Thread.Sleep(5000);

			Trace.WriteLine("Done");
		}



		[Test]
		public void Something_special_this_way_comes()
		{

			var r = from request in new[]{new MyRequestMessage()}
			select
				from response in request.MakeRequest()
				let responseMessage = response.GetResponse<MyResponseMessage>()
				let otherResponse = response.GetResponse<OtherResponseMessage>()
				select new { Response = response, Message = responseMessage, OtherMessage = otherResponse};



			foreach (var reply in r)
			{
				reply(x =>
					{
// so this gets called when the response is received I supppose
					});
			}



		}






	}

	internal class OtherResponseMessage
	{
	}

	internal class MyResponseMessage
	{
	}

	public class MyRequestMessage
	{
		public void BeginGetResponse(AsyncCallback callback, object o)
		{
			Action makeCallback = () => callback(null);

			makeCallback.DeferFor(1.Seconds());
		}


		public RRScope EndGetResponse(IAsyncResult result)
		{
			return new RRScope();
		}
	}

	public static class sstuu
	{
		public static K<RRScope> MakeRequest(this MyRequestMessage request)
		{
			return respond => request.BeginGetResponse(result =>
			{
				var response = request.EndGetResponse(result);
				respond(response);
			}, null);
		}

		
	}

	public class RRScope
	{
		public T GetResponse<T>()
		{
			return default(T);
			
		}
	}


	// TODO
	// it may be possible to use this deferred async model to return messages
	// to a batch consumer on the same thread as the dispatcher from the bus
	// this would make it slick and retain our thread management rules

	// me likey monads~




	public static class AsyncExtensionsForStuff
	{
		public static K<WebResponse> GetResponseAsync(this WebRequest request)
		{
			return respond => request.BeginGetResponse(result =>
				{
					var response = request.EndGetResponse(result);
					respond(response);
				}, null);
		}

		public static K<string> ReadToEndAsync(this Stream stream)
		{
			return respond =>
				{
					using (MemoryStream content = new MemoryStream())
					{
						byte[] buffer = new byte[1024];

						Func<IAsyncResult> readChunk = null;

						readChunk = () => stream.BeginRead(buffer, 0, 1024, result =>
							{
								int read = stream.EndRead(result);
								if (read > 0)
								{
									content.Write(buffer, 0, read);

									readChunk();
								}
								else
								{
									stream.Dispose();

									respond(Encoding.UTF8.GetString(content.ToArray()));
								}
							}, null);
						readChunk();
					}
				};
		}
	}
}