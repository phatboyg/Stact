namespace Magnum.Specs.Integration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Machine.Specifications;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [Concern("Storage Nodes")]
    public class Given_a_storage_node_When_an_object_is_stored
    {
        //TODO: static
        private static Person _person;

        private Establish context = () =>
                                        {
                                            _person = new Person();
                                        };


        It should_return_objects = () =>
                                               {
                                                   _person.Id = Guid.NewGuid();
                                                   _person.FirstName = "Chris";
                                                   _person.LastName = "Patterson";

                                                   StorageContext scontext = new StorageContext();
                                                   scontext.RegisterClass<Person, Guid>(GetKey);

                                                   scontext.Save(_person);

                                                   IList<Person> persons = scontext.List<Person>();

                                                   Assert.That(persons, Is.Not.Null);
                                                   Assert.That(persons.Count, Is.EqualTo(1));
                                                   Assert.That(persons[0].Id, Is.EqualTo(_person.Id));

                                               };

        It should_return_the_object_in_a_usable_state = () =>
                           {
                               _person.Id = Guid.NewGuid();
                               _person.FirstName = "Chris";
                               _person.LastName = "Patterson";

                               StorageContext scontext = new StorageContext();
                               scontext.RegisterClass<Person, Guid>(GetKey);

                               scontext.Save(_person);

                               Person other = scontext.Get<Person>(_person.Id);

                               Assert.That(other.Id, Is.EqualTo(_person.Id));
                               Assert.That(other.FirstName, Is.EqualTo(_person.FirstName));
                               Assert.That(other.LastName, Is.EqualTo(_person.LastName));

                           };

        private static Guid GetKey(Person p)
        {
            return p.Id;
        }
    }

    public class StorageContext
    {
        private readonly Dictionary<Type, IClassStorageContext> _classStorage = new Dictionary<Type, IClassStorageContext>();

        public void Save<T>(T item) where T : class
        {
            if (_classStorage.ContainsKey(typeof (T)))
                _classStorage[typeof (T)].Save(item);
            else
                throw new ApplicationException("No class type supported");
        }

        public T Get<T>(object id) where T : class
        {
            if (_classStorage.ContainsKey(typeof (T)))
                return _classStorage[typeof (T)].Get<T>(id);

            throw new ApplicationException("No class type supported");
        }

        public void RegisterClass<T, K>(ObjectToKey<T, K> objectToKey) where T : class
        {
            IClassStorageContext context = new ClassStorageContext<T, K>(objectToKey);
            _classStorage.Add(typeof (T), context);
        }

        public IList<T> List<T>() where T : class
        {
            if (_classStorage.ContainsKey(typeof (T)))
                return _classStorage[typeof (T)].List<T>();

            return new List<T>();
        }
    }

    public interface IClassStorageContext
    {
        void Save<T>(T item);
        T Get<T>(object id) where T : class;
        IList<T> List<T>() where T : class;
    }

    public class ClassStorageContext<T, K> : IClassStorageContext where T : class
    {
        private readonly KeyMap<T, K> _keyMap;
        private readonly Dictionary<K, T> _storage = new Dictionary<K, T>();

        public ClassStorageContext(ObjectToKey<T, K> objectToKey)
        {
            _keyMap = new KeyMap<T, K>(objectToKey);
        }

        public void Save<T1>(T1 item)
        {
            if (typeof (T1) != typeof (T))
                throw new NotImplementedException();

            K key = _keyMap.GetKeyFromObject(item);

            if (_storage.ContainsKey(key))
                _storage[key] = item as T;
            else
                _storage.Add(key, item as T);
        }

        public T1 Get<T1>(object id) where T1 : class
        {
            if (typeof (T1) != typeof (T))
                throw new NotImplementedException();

            K key = _keyMap.GetKeyFromId(id);


            if (_storage.ContainsKey(key))
                return _storage[key] as T1;

            return default(T1);
        }

        public IList<T1> List<T1>() where T1 : class
        {
            List<T1> result = new List<T1>();

            foreach (T t in _storage.Values)
            {
                result.Add(t as T1);
            }

            return result;
        }
    }

    public delegate K ObjectToKey<T, K>(T item) where T : class;

    public class KeyMap<T, K> where T : class
    {
        private readonly ObjectToKey<T, K> _objectToKey;
        private readonly TypeConverter _tc = new TypeConverter();

        public KeyMap(ObjectToKey<T, K> objectToKey)
        {
            _objectToKey = objectToKey;
        }

        public K GetKeyFromObject(object obj)
        {
            return _objectToKey(obj as T);
        }

        public K GetKeyFromId(object id)
        {
            if (typeof (K).IsAssignableFrom(id.GetType()))
                return (K) id;

            throw new ApplicationException("Invalid key type specified");
        }
    }

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