namespace Magnum.Transport.Specs
{
    public class ParticipantConfiguration : IParticipantConfiguration
    {
        private MeshAuthorizationKey _authorizationKey;

        public MeshAuthorizationKey AuthorizationKey
        {
            get { return _authorizationKey; }
            set { _authorizationKey = value; }
        }
    }
}