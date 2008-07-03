namespace Magnum.Transport.Tests
{
	using System.Security.Cryptography;
	using System.Text;

	public class MeshAuthorizationKey
	{
		private readonly HMACSHA512 _hmac;

		public MeshAuthorizationKey(string salt)
		{
			_hmac = new HMACSHA512(Encoding.UTF8.GetBytes(salt));
		}

		public byte[] Challenge(byte[] challenge)
		{
			return _hmac.ComputeHash(challenge);
		}
	}
}