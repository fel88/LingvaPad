
using System.Diagnostics;
using System.Xml.Linq;

namespace LingvaPad
{
    public class Db
    {
        public static Db Instance = new Db();
        public List<SentenceTranslation> Translations = new List<SentenceTranslation>();

        public Db() { }
        public Db(XElement? root)
        {
            var translations = root.Elements("translations");
            foreach (var item in translations.Elements("translation"))
            {
                Translations.Add(new SentenceTranslation(item));
            }
        }

        internal XElement ToXml()
        {
            XElement ret = new XElement("root");
            XElement trans = new XElement("translations");
            ret.Add(trans);
            foreach (var tr in Translations)
            {
                trans.Add(tr.ToXml());
            }
            return ret;
        }
    }
}
