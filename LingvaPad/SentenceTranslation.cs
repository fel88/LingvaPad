
using System.Transactions;
using System.Xml.Linq;

namespace LingvaPad
{
    public class SentenceTranslation
    {
        public Sentence Sentence;
        public Sentence Translation;        

        public SentenceTranslation() { }
        public SentenceTranslation(XElement item, XmlSerializeDataContext dataContext)
        {
            var sourceId = int.Parse(item.Attribute("sourceId").Value);
            var translationId = int.Parse(item.Attribute("translationId").Value);            

            Sentence = dataContext.Current.Sentences.First(z => z.Id == sourceId);
            Translation = dataContext.Current.Sentences.First(z => z.Id == translationId);
        }

        internal XElement ToXml(XmlSerializeDataContext dataContext)
        {
            var ret = new XElement("translation");
            ret.Add(new XAttribute("sourceId", Sentence.Id));
            ret.Add(new XAttribute("translationId", Translation.Id));
            
            return ret;
        }
    }
}
