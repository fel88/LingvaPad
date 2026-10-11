
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Xml.Linq;

namespace LingvaPad
{
    public class Db
    {
        public static Db Instance = new Db();
        public List<SentenceTranslation> Translations = new List<SentenceTranslation>();
        public List<Sentence> Sentences = new List<Sentence>();

        public Db() { }
        public Db(XElement? root)
        {
            XmlSerializeDataContext dataContext = new XmlSerializeDataContext();
            var translations = root.Elements("translations");
            var sentences = root.Elements("sentences");
            dataContext.Current = this;
            foreach (var item in sentences.Elements())
            {
                Sentences.Add(new Sentence(item, dataContext));
            }

            foreach (var item in translations.Elements("translation"))
            {
                Translations.Add(new SentenceTranslation(item, dataContext));
            }
        }

        internal XElement ToXml()
        {
            XElement ret = new XElement("root");
            XElement sentences = new XElement("sentences");
            XElement trans = new XElement("translations");
            ret.Add(sentences);
            ret.Add(trans);
            XmlSerializeDataContext dataContext = new XmlSerializeDataContext();
            for (int i = 0; i < Sentences.Count; i++)
            {
                Sentence? item = Sentences[i];
                dataContext.SentencesIds.Add(item, i);
            }

            foreach (var tr in Sentences)
            {
                sentences.Add(tr.ToXml(dataContext));
            }
            foreach (var tr in Translations)
            {
                trans.Add(tr.ToXml(dataContext));
            }
            return ret;
        }
    }

    public class XmlSerializeDataContext
    {
        public Db Current;
        public Dictionary<Sentence, int> SentencesIds = new Dictionary<Sentence, int>();
    }
}
