namespace Magnum.Common.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public class FastCollection
    {
        public Action<object, object> AddDelegate;
    }

    public class FastCollection<TClass>
    {
        public Action<TClass, object> AddDelegate;
    }

    public class FastCollection<TClass, TCollection, TElement>
    {
        public FastProperty<TClass, TCollection> FastProperty { get; set; }
        public PropertyInfo Property { get; set; }
        public Type CollectionType { get; set; }
        public Action<TCollection, TElement> AddDelegate { get; set;}

        public FastCollection(PropertyInfo property)
        {
            Property = property;
            CollectionType = typeof (TCollection);

            if(Property.PropertyType != typeof(TCollection))
                throw new Exception("bob");
            if(!typeof(ICollection<TElement>).IsAssignableFrom(CollectionType))
                throw new Exception("alice");

            InitializeAdd();
            InitializeRemove();
        }

        private void InitializeAdd()
        {
            var addMethod = CollectionType.GetMethod("Add", BindingFlags.Public);

            var instance = Expression.Parameter(typeof(TCollection), "instance");
            var value = Expression.Parameter(typeof(TElement), "item");
            AddDelegate = Expression.Lambda<Action<TCollection, TElement>>(Expression.Call(instance, addMethod, value), new[] { instance, value }).Compile();

        }
        private void InitializeRemove()
        {
            var removeMethod = CollectionType.GetMethod("Remove");
        }
    }
}