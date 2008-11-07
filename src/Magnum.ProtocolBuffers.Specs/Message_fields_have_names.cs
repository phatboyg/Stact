namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using NUnit.Framework;

    [TestFixture]
    public class Message_fields_have_names
    {
        [Test]
        public void Name_should_be_set_automatically()
        {
            Expression<Func<TestMessage, string>> expression = m => m.Name;
            var propInfo = ReflectionHelper.GetProperty(expression);
            var mapping = new FieldMap(propInfo, 1);
            Assert.AreEqual("Name", mapping.FieldName);
        }
    }

    public static class ReflectionHelper
    {
        public static PropertyInfo GetProperty<MODEL>(Expression<Func<MODEL, object>> expression)
        {
            MemberExpression memberExpression = getMemberExpression(expression);
            return (PropertyInfo)memberExpression.Member;
        }

        public static PropertyInfo GetProperty<MODEL, T>(Expression<Func<MODEL, T>> expression)
        {
            MemberExpression memberExpression = getMemberExpression(expression);
            return (PropertyInfo)memberExpression.Member;
        }

        private static MemberExpression getMemberExpression<MODEL, T>(Expression<Func<MODEL, T>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }



            if (memberExpression == null) throw new ArgumentException("Not a member access", "member");
            return memberExpression;
        }



    }
}