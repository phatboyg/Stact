namespace Magnum.Specs.CEP
{
    public class LoginSucceeded
    {
        
    }

    public class LoginFailed
    {
        public LoginFailed(string password)
        {
            Password = password;
        }

        public string Password { get; set; }
    }

    public class PossibleBruteForceAttack
    {
        public PossibleBruteForceAttack(params object[] messages)
        {
            Messages = messages;
        }

        object[] Messages { get; set; }
    }
}