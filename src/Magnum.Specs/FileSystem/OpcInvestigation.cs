namespace Magnum.Specs.FileSystem
{
    using System;
    using System.IO;
    using System.IO.Packaging;
    using System.Text;

    public class OpcInvestigation
    {
        public void Bob()
        {
            using(Package p = Package.Open("bob.zip", FileMode.Create))
            {
                var u = new Uri("/bob/send.xml", UriKind.Relative);
                var rel = p.CreateRelationship(u, TargetMode.Internal, ".txt");
                
                var part = p.CreatePart(u, "text/xml");
                var data = Encoding.UTF8.GetBytes("<test />");
                part.GetStream().Write(data, 0, data.Length);


                p.Close();
            }

        }

        public void Bill()
        {
            using (var p = Package.Open(@"C:\Users\Administrator\Desktop\CUEFlow.pptx"))
            {
                
                p.Close();
            }
        }
    }
}