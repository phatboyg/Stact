namespace Magnum.Specs.Integration
{
    using System;
    using System.Collections.Generic;
    using Machine.Specifications;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [Concern("StorageContext")]
    public class When_an_object_is_stored
    {
        //TODO: static
        private static Person _person;
        private static StorageContext _context;

        Establish context = () =>
                                {
                                    _person = new Person();
                                    _person.Id = Guid.NewGuid();
                                    _person.FirstName = "Chris";
                                    _person.LastName = "Patterson";

                                    _context = new StorageContext();
                                    _context.RegisterClass<Person, Guid>(p => p.Id);
                                };

        Cleanup after_each = () =>
                                 {
                                     _person = null;
                                     _context = null;
                                 };

        Because of = () =>
                         {
                             _context.Save(_person);
                         };

        It should_return_the_stored_object = () =>
                                       {
                                           IList<Person> persons = _context.List<Person>();

                                           Assert.That(persons, Is.Not.Null);
                                           Assert.That(persons.Count, Is.EqualTo(1));
                                           Assert.That(persons[0].Id, Is.EqualTo(_person.Id));

                                       };

        It should_return_the_object_with_all_properties = () =>
                                                            {

                                                                var other = _context.Get<Person>(_person.Id);

                                                                Assert.That(other.Id, Is.EqualTo(_person.Id));
                                                                Assert.That(other.FirstName,
                                                                            Is.EqualTo(_person.FirstName));
                                                                Assert.That(other.LastName, Is.EqualTo(_person.LastName));

                                                            };
    }
}