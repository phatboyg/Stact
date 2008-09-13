namespace Magnum.Specs.Integration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class Given_a_storage_node_When_an_object_is_stored
    {
        [Test]
        public void A_list_of_objects_should_be_returned()
        {
            Person p = new Person();

            p.Id = Guid.NewGuid();
            p.FirstName = "Chris";
            p.LastName = "Patterson";

            StorageContext context = new StorageContext();
            context.RegisterClass<Person, Guid>(GetKey);

            context.Save(p);

            IList<Person> persons = context.List<Person>();

            Assert.That(persons, Is.Not.Null);
            Assert.That(persons.Count, Is.EqualTo(1));
            Assert.That(persons[0].Id, Is.EqualTo(p.Id));
        }

        [Test]
        public void The_object_should_return_in_a_usable_state()
        {
            Person p = new Person();

            p.Id = Guid.NewGuid();
            p.FirstName = "Chris";
            p.LastName = "Patterson";

            StorageContext context = new StorageContext();
            context.RegisterClass<Person, Guid>(GetKey);

            context.Save(p);

            Person other = context.Get<Person>(p.Id);

            Assert.That(other.Id, Is.EqualTo(p.Id));
            Assert.That(other.FirstName, Is.EqualTo(p.FirstName));
            Assert.That(other.LastName, Is.EqualTo(p.LastName));
        }

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