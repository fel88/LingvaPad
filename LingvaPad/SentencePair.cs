
using System.Transactions;
using System.Xml.Linq;

namespace LingvaPad
{
    public class SentencePair
    {
        public Sentence Source;
        public Sentence Target;
        public Tag[] Tags; //it can be translation, inversion, active to passive transforming, etc

        public SentencePair() { }
        public SentencePair(XElement item, XmlSerializeDataContext dataContext)
        {
            var sourceId = int.Parse(item.Attribute("sourceId").Value);
            var targetId = int.Parse(item.Attribute("targetId").Value);

            Source = dataContext.Current.Sentences.First(z => z.Id == sourceId);
            Target = dataContext.Current.Sentences.First(z => z.Id == targetId);

            List<Tag> tags = new List<Tag>();
            if (item.Element("tags") != null)
                foreach (var titem in item.Element("tags").Elements())
                    tags.Add(new Tag(titem.Value.Trim()));

            Tags = tags.ToArray();
        }       

        internal XElement ToXml(XmlSerializeDataContext dataContext)
        {
            var ret = new XElement("pair");
            ret.Add(new XAttribute("sourceId", Source.Id));
            ret.Add(new XAttribute("targetId", Target.Id));
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
