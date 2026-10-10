
using System.Transactions;
using System.Xml.Linq;

namespace LingvaPad
{
    public class SentenceTranslation
    {
        public Sentence Sentence;
        public List<Sentence> Translations = new List<Sentence>();//correct translations
        public Tag[] Tags;

        public SentenceTranslation() { }
        public SentenceTranslation(XElement item)
        {
            foreach (var titem in item.Element("translations").Elements())
            {
                Translations.Add(new Sentence(titem));
            }
            
            Sentence = new Sentence(item.Element("sentence").Element("sentence"));
        }

        internal XElement ToXml()
        {
            var ret = new XElement("translation");
            var s1 = new XElement("sentence");
            var t1 = new XElement("translations");
            
            s1.Add(Sentence.ToXml());
            foreach (var item in Translations)
            {
                t1.Add(item.ToXml());
            }
            
            ret.Add(s1);
            ret.Add(t1);

            return ret;
        }
    }
}
