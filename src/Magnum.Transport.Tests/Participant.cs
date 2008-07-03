namespace Magnum.Transport.Tests
{
	using System;

	public class Participant
	{
		private readonly MeshAuthorizationKey _authorizationKey;
		private readonly Guid _id;

		public Participant()
		{
			_id = Guid.NewGuid();
		}

		public Participant(IParticipantConfiguration configuration)
		{
			_id = Guid.NewGuid();
			_authorizationKey = configuration.AuthorizationKey;
		}

		public Guid Id
		{
			get { return _id; }
		}

		public MeshAuthorizationKey AuthorizationKey
		{
			get { return _authorizationKey; }
		}
	}
}