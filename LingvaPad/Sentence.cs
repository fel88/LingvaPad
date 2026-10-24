
using Microsoft.VisualBasic;
using System.Xml.Linq;

namespace LingvaPad
{
    public class Sentence
    {
        public int Id;
        public string Text;
        public Tag[] Tags;
        public string Culture;

        public Sentence() { }
        public Sentence(XElement? xElement, XmlSerializeDataContext dataContext)
        {
            Text = xElement.Element("text").Value;
            Culture = xElement.Attribute("culture").Value;
            Id = int.Parse(xElement.Attribute("id").Value);
            List<Tag> tags = new List<Tag>();
            if (xElement.Element("tags") != null)
                foreach (var item in xElement.Element("tags").Elements())
                    tags.Add(new Tag(item.Value.Trim()));

            Tags = tags.ToArray();
        }

        internal XElement ToXml(XmlSerializeDataContext dataContext)
        {
            XElement ret = new XElement("sentence");
            ret.Add(new XAttribute("id", Id));
            ret.Add(new XElement("text", new XCData(Text)));
            ret.Add(new XAttribute("culture", Culture));
            XElement tags = new XElement("tags");
            ret.Add(tags);
            if (Tags != null)
                foreach (var item in Tags)
                {
                    tags.Add(new XElement("tag", new XCData(item.Name)));
                }
            return ret;
        }


    }

}