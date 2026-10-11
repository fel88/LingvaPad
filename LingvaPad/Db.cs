
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Xml.Linq;

namespace LingvaPad
{
    public class Db
    {
        public static Db Instance = new Db();
        public List<SentencePair> Pairs = new List<SentencePair>();
        public List<Sentence> Sentences = new List<Sentence>();
        public List<Flashcard> Flashcards = new List<Flashcard>();

        public Dictionary<Sentence, List<Sentence>> TranslationsLookUp = new Dictionary<Sentence, List<Sentence>>();
        public Db() { }
        public Db(XElement? root)
        {
            XmlSerializeDataContext dataContext = new XmlSerializeDataContext();
            var pairs = root.Element("pairs");
            var sentences = root.Element("sentences");
            var flashcards = root.Element("flashcards");
            dataContext.Current = this;
            foreach (var item in sentences.Elements())
            {
                Sentences.Add(new Sentence(item, dataContext));
            }

            foreach (var item in pairs.Elements())
            {
                Pairs.Add(new SentencePair(item, dataContext));
            }

            if (flashcards != null)
                foreach (var item in flashcards.Elements())
                {
                    Flashcards.Add(new Flashcard(item, dataContext));
                }
            BuildTranslationsLookUp();
        }

        private void BuildTranslationsLookUp()
        {
            foreach (var item in Sentences)
            {
                var tt = Pairs.Where(z => z.Source == item).ToArray();
                TranslationsLookUp.Add(item, tt.Select(z => z.Target).ToList());
            }
            foreach (var item in Sentences)
            {
                var tt = Pairs.Where(z => z.Target == item).ToArray();
                TranslationsLookUp[item].AddRange(tt.Select(z => z.Source).ToList());
            }
        }

        internal XElement ToXml()
        {
            XElement ret = new XElement("root");
            XElement sentences = new XElement("sentences");
            XElement pairs = new XElement("pairs");
            XElement flashcards = new XElement("flashcards");
            ret.Add(sentences);
            ret.Add(pairs);
            ret.Add(flashcards);
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
            foreach (var tr in Pairs)
            {
                pairs.Add(tr.ToXml(dataContext));
            }
            foreach (var item in Flashcards)
            {
                flashcards.Add(item.ToXml(dataContext));
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
