namespace Magnum.ActorModel.WebSpecs
{
	using System;
	using System.IO;
	using System.Net;
	using System.Text;
	using Monads;

	public static class ExtensionsForWebStuff
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
					StringBuilder sb = new StringBuilder();
					byte[] buffer = new byte[1024];

					Func<IAsyncResult> readChunk = null;

					readChunk = () => stream.BeginRead(buffer, 0, 1024, result =>
						{
							int read = stream.EndRead(result);
							if (read > 0)
							{
								sb.Append(Encoding.UTF8.GetString(buffer, 0, read));

								readChunk();
							}
							else
							{
								stream.Dispose();

								respond(sb.ToString());
							}
						}, null);
					readChunk();
				};
		}
	}
}