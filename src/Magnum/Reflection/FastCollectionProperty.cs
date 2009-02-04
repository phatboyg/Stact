namespace Magnum.Reflection
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a property that has a collection type that needs to be manipulated
    /// </summary>
    public class FastCollectionProperty
    {
        private FastCollection fc;
        private FastProperty fp;
    }

    /// <summary>
    /// Represents a property that has a collection type that needs to be manipulated
    /// </summary>
    public class FastCollectionProperty<TClass>
    {
        private FastCollection fc;
        private FastProperty<TClass> fp;
    }

    /// <summary>
    /// Represents a property that has a collection type that needs to be manipulated
    /// </summary>
    public class FastCollectionProperty<TClass, TCollection> where TCollection : IList
    {
        private FastCollection<TCollection> fc;
        private FastProperty<TClass, TCollection> fp;
    }

    /// <summary>
    /// Represents a property that has a collection type that needs to be manipulated
    /// </summary>
    public class FastCollectionProperty<TClass, TCollection, TElement> where TCollection : ICollection<TElement>
    {
        private FastCollection<TCollection, TElement> fc;
        private FastProperty<TClass, TCollection> fp;
    }
}