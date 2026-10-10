
using System.Xml.Linq;

namespace LingvaPad
{
    public class SentenceTranslation
    {
        public Sentence Sentence;
        public Sentence Translation;
        public Tag[] Tags;

        public SentenceTranslation() { }
        public SentenceTranslation(XElement item)
        {
            Translation = new Sentence(item.Element("translation").Element("sentence"));
            Sentence = new Sentence(item.Element("sentence").Element("sentence"));
        }

        internal XElement ToXml()
        {
            var ret = new XElement("translation");
            var s1 = new XElement("sentence");
            var t1 = new XElement("translation");
            s1.Add(Sentence.ToXml());
            t1.Add(Translation.ToXml());
            ret.Add(s1);
            ret.Add(t1);

            return ret;
        }
    }
}
