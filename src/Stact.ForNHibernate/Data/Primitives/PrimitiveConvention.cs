namespace Stact.ForNHibernate.Data.Primitives
{
    using FluentNHibernate.Conventions;

    //how to make this better / like a scanner or something
    public class PrimitiveConvention<PRIM, VALUE> :
        UserTypeConvention<PrimitiveUserType<PRIM, VALUE>>
        where VALUE : struct 
        where PRIM : Primitive<VALUE>
    {
        
    }
}