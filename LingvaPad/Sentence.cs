
using System.Xml.Linq;

namespace LingvaPad
{
    public class Sentence
    {
        public string Text;
        public Tag[] Tags;

        public Sentence() { }
        public Sentence(XElement? xElement)
        {
            Text = xElement.Element("text").Value;
        }

        internal XElement ToXml()
        {
            XElement ret = new XElement("sentence");
            ret.Add(new XElement("text", new XCData(Text)));
            //todo tags
            return ret;
        }
    }
}
