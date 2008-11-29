namespace Magnum.Common.Reflection
{
    using System;
    using System.Collections;
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

    public class FastCollection<TClass, TCollection, TElement> where TCollection : ICollection<TElement>
    {
        public FastProperty<TClass, TCollection> FastProperty { get; set; }
        public PropertyInfo Property { get; set; }
        public Type CollectionType { get; set; }
        public Action<TCollection, TElement> AddDelegate { get; set;}
        public Action<TCollection, TElement> RemoveDelegate { get; set; }

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
            var addMethod = typeof(ICollection<TElement>).GetMethod("Add");

            var instance = Expression.Parameter(typeof(ICollection<TElement>), "instance");
            var value = Expression.Parameter(typeof(TElement), "item");
            AddDelegate = Expression.Lambda<Action<TCollection, TElement>>(Expression.Call(instance, addMethod, value), new[] { instance, value }).Compile();

        }
        private void InitializeRemove()
        {
            var removeMethod = typeof(ICollection<TElement>).GetMethod("Remove");

            var instance = Expression.Parameter(typeof(ICollection<TElement>), "instance");
            var value = Expression.Parameter(typeof(TElement), "item");
            RemoveDelegate = Expression.Lambda<Action<TCollection, TElement>>(Expression.Call(instance, removeMethod, value), new[] { instance, value }).Compile();
        }
    }
}