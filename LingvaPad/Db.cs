
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
        public List<Flashcard> Flashcards = new List<Flashcard>();

        public Dictionary<Sentence, List<Sentence>> TranslationsLookUp = new Dictionary<Sentence, List<Sentence>>();
        public Db() { }
        public Db(XElement? root)
        {
            XmlSerializeDataContext dataContext = new XmlSerializeDataContext();
            var translations = root.Element("translations");
            var sentences = root.Element("sentences");
            var flashcards = root.Element("flashcards");
            dataContext.Current = this;
            foreach (var item in sentences.Elements())
            {
                Sentences.Add(new Sentence(item, dataContext));
            }

            foreach (var item in translations?.Elements("translation"))
            {
                Translations.Add(new SentenceTranslation(item, dataContext));
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
                var tt = Translations.Where(z => z.Sentence == item).ToArray();
                TranslationsLookUp.Add(item, tt.Select(z => z.Translation).ToList());
            }
            foreach (var item in Sentences)
            {
                var tt = Translations.Where(z => z.Translation == item).ToArray();
                TranslationsLookUp[item].AddRange(tt.Select(z => z.Sentence).ToList());
            }
        }

        internal XElement ToXml()
        {
            XElement ret = new XElement("root");
            XElement sentences = new XElement("sentences");
            XElement trans = new XElement("translations");
            XElement flashcards = new XElement("flashcards");
            ret.Add(sentences);
            ret.Add(trans);
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
            foreach (var tr in Translations)
            {
                trans.Add(tr.ToXml(dataContext));
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
