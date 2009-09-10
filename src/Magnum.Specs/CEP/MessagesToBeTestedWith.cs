namespace Magnum.Specs.CEP
{
    using System;

    public class LoginSucceeded : LoginAttempt
    {
        public LoginSucceeded(string username)
        {
            Username = username;
        }

        public string Username
        {
            get; private set;
        }
    }

    public class LoginFailed : LoginAttempt
    {
        public LoginFailed(string username)
        {
            Username = username;
        }

        public string Username { get; private set; }
    }

    public interface LoginAttempt
    {
        string Username { get; }
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