namespace Magnum.Transport.Tests
{
	using System;
	using System.Security.Cryptography;
	using System.Text;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	/*
	 * 
	 * 
	 * d
	 * 7:channel
	 * 15:/meta/handshake
	 * 7:version
	 * 3:1.0
	 * 24:supportedConnectionTypes
	 * l9:long-poll4:poll13:bidirectionale
	 * 3:key
	 * 16:Un1qu3Id3nt1f13r
	 * 4:hash
	 * 32:12345678901234567890123456789012
	 * e
	 * 
	 * 
	 * d
	 * 7:channel
	 * 15:/meta/handshake
	 * 7:version
	 * 3:1.0
	 * 24:supportedConnectionTypes
	 * l9:long-poll4:poll13:bidirectionale
	 * 8:clientId
	 * 16:1234567890123456
	 * 6:advice
	 * l9:reconnect5:retrye
	 * e
	 * 
	 * */

	[TestFixture]
	public class When_a_service_instance_is_started
	{
		[Test]
		public void Each_instance_should_have_a_unique_instance_id()
		{
			Participant p = new Participant();

			Participant q = new Participant();

			Assert.That(p.Id, Is.Not.EqualTo(q.Id));
		}

		[Test]
		public void The_authentication_key_for_a_participant_should_be_passed()
		{
			MeshAuthorizationKey authorizationKey = new MeshAuthorizationKey("Some Random String of Characters Used To Build An Authorization Key");

			IParticipantConfiguration configuration = new ParticipantConfiguration();
			configuration.AuthorizationKey = authorizationKey;

			Participant p = new Participant(configuration);

			byte[] phrase = Encoding.UTF8.GetBytes("Hello");

			Assert.That(p.AuthorizationKey.Challenge(phrase), Is.EqualTo(authorizationKey.Challenge(phrase)));
		}		
		
		[Test]
		public void The_authentication_key_for_a_participant_should_fail_if_they_dont_match()
		{
			MeshAuthorizationKey authorizationKey = new MeshAuthorizationKey("Some Random String of Characters Used To Build An Authorization Key");

			IParticipantConfiguration configuration = new ParticipantConfiguration();
			configuration.AuthorizationKey = authorizationKey;

			Participant p = new Participant(configuration);

			authorizationKey = new MeshAuthorizationKey("Another random string of characters");

			configuration = new ParticipantConfiguration();
			configuration.AuthorizationKey = authorizationKey;

			Participant q = new Participant(configuration);

			byte[] phrase = Encoding.UTF8.GetBytes("Hello");

			Assert.That(p.AuthorizationKey.Challenge(phrase), Is.Not.EqualTo(q.AuthorizationKey.Challenge(phrase)));
		}
	}
}