
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Linq;

namespace LingvaPad
{
    public class Flashcard
    {
        public Flashcard() { }
        public Flashcard(int id, Image image)
        {
            Id = id;
            using MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);
            Data = ms.ToArray();
            using var ms2 = new MemoryStream(Data);
            Image = Bitmap.FromStream(ms2) as Bitmap;
        }

        public Flashcard(XElement elem, XmlSerializeDataContext dataContext)
        {
            Id = int.Parse(elem.Attribute("id").Value);
            Description = elem.Element("desc").Value;
            var b64 = Convert.FromBase64String(elem.Element("data").Value);
            Data = b64;
            var ms = new MemoryStream(b64);
            Image = Bitmap.FromStream(ms) as Bitmap;
        }

        public int Id;
        public string Description;//like movie
        public byte[] Data;
        public Bitmap Image;
        public Tag[] Tags;

        internal XElement ToXml(XmlSerializeDataContext dataContext)
        {
            XElement ret = new XElement("flashcard");
            ret.Add(new XAttribute("id", Id));

            var b64 = Convert.ToBase64String(Data);
            ret.Add(new XElement("data", new XCData(b64)));
            ret.Add(new XElement("desc", new XCData(Description ?? string.Empty)));
            XElement tags = new XElement("tags");
            ret.Add(tags);
            if (Tags != null)
                foreach (var item in Tags)
                {
                    tags.Add(new XElement("tag", new XCData(item.Name)));
                }
            return ret;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
