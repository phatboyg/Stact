namespace Magnum.Serialization.Tests
{
	using System.Diagnostics;
	using NUnit.Framework;

	[TestFixture]
	public class ReflectiveVisitor_Exp
	{
		[Test]
		public void We_should_go_through_the_list_quickly()
		{
			DerivedObject obj = new DerivedObject("Chris", "Patterson", "Pimp");

			//IObjectFormatter fomatter = new XmlObjectFormatter();

			IObjectFormatter formatter = new BEncodeObjectFormatter();

			IObjectSerializer serializer = new BasicObjectSerializer(formatter);

			serializer.Serialize(obj);

			Debug.WriteLine("DATA: " + formatter.GetString());
		}
	}
}