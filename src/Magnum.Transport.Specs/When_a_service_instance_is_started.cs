namespace Magnum.Transport.Specs
{
    using System.Text;
    using Machine.Specifications;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [Concern("")]
    public class When_a_service_instance_is_started
    {
        private static Participant p;
        private static Participant q;
        static MeshAuthorizationKey authorizationKey;
        static MeshAuthorizationKey authorizationKey2;


        Establish context = () =>
                                        {
                                            IParticipantConfiguration configuration = new ParticipantConfiguration();
                                            IParticipantConfiguration configuration2 = new ParticipantConfiguration();
                                            authorizationKey = new MeshAuthorizationKey("Some Random String of Characters Used To Build An Authorization Key");

                                            authorizationKey2 = new MeshAuthorizationKey("Another random string of characters");

                                            configuration.AuthorizationKey = authorizationKey;
                                            configuration2.AuthorizationKey = authorizationKey2;
                                            p = new Participant(configuration);
                                            q = new Participant(configuration2);

                                        };

        Because of = () =>
                                 {

                                 };

        private It should_have_a_unique_id = () =>
                                                 {
                                                     Assert.That(p.Id, Is.Not.EqualTo(q.Id));

                                                 };

        private It should_have_a_passed_auth_key = () =>
                                                       {
                                                           byte[] phrase = Encoding.UTF8.GetBytes("Hello");

                                                           Assert.That(p.AuthorizationKey.Challenge(phrase), Is.EqualTo(authorizationKey.Challenge(phrase)));
                                                       };

        private It should_fail_if_keys_dont_match = () =>
                                                        {
                                                            byte[] phrase = Encoding.UTF8.GetBytes("Hello");

                                                            Assert.That(p.AuthorizationKey.Challenge(phrase), Is.Not.EqualTo(q.AuthorizationKey.Challenge(phrase)));
                                                        };
    }
}