using System;

namespace Magnum.Specs.Integration
{
    public class Person
    {
        private string _firstName;
        private Guid _id;
        private string _lastName;

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
    }
}