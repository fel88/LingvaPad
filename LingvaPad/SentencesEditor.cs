using Catalyst;
using Catalyst.Models;
using Mosaik.Core;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LingvaPad
{
    public partial class SentencesEditor : Form
    {
        public SentencesEditor()
        {
            InitializeComponent();
            UpdateList();
        }

        private void UpdateList()
        {
            listView1.Items.Clear();

            var filterLow = filter.ToLower();
            foreach (var item in Db.Instance.Sentences)
            {
                var pairsQty = Db.Instance.TranslationsLookUp[item].Count;
                var tagsStr = item.Tags == null ? "" : string.Join(", ", item.Tags.Select(z => z.Name));
                string[] stringsToSearch = [item.Text.ToLower(), tagsStr, item.Culture];
                if (!string.IsNullOrEmpty(filter) && !stringsToSearch.Any(z => z.Contains(filterLow)))
                    continue;

                listView1.Items.Add(new ListViewItem(new string[] {
                    item.Id.ToString(), item.Text, item.Culture, tagsStr, pairsQty.ToString() })
                { Tag = item });
            }
        }

        public Sentence[] GetFilteredList()
        {
            var filterLow = filter.ToLower();
            List<Sentence> ret = new List<Sentence>();
            foreach (var item in Db.Instance.Sentences)
            {
                var tagsStr = item.Tags == null ? "" : string.Join(", ", item.Tags.Select(z => z.Name));
                string[] stringsToSearch = [item.Text.ToLower(), tagsStr];
                if (!string.IsNullOrEmpty(filter) && !stringsToSearch.Any(z => z.Contains(filterLow)))
                    continue;

                ret.Add(item);

            }
            return ret.ToArray();
        }


        string filter = "";
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            filter = textBox1.Text;
            UpdateList();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as Sentence;
            richTextBox1.Text = trans.Text;
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = AutoDialog.DialogHelpers.StartDialog();
            d.AddStringField("text", "Text");
            d.AddOptionsField("culture", "Culture", ["ru", "en"], 0);

            if (!d.ShowDialog())
                return;

            var text = d.GetStringField("text");
            var culture = d.GetOptionsField("culture");
            AddSentence(text, culture);
            UpdateList();
        }

        private static void AddSentence(string text, string culture)
        {
            int nextId = 1;

            if (Db.Instance.Sentences.Any())
                nextId = Db.Instance.Sentences.Max(z => z.Id) + 1;


            Db.Instance.Sentences.Add(new Sentence()
            {
                Text = text,
                Culture = culture,
                Id = nextId
            });
            Db.Instance.BuildTranslationsLookUp();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as Sentence;
            if (GuiHelpers.ShowQuestion($"Are you sure to delete  {trans.Text}?"))
            {
                Db.Instance.Sentences.Remove(trans);
                UpdateList();
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as Sentence;
            var d = AutoDialog.DialogHelpers.StartDialog();
            d.AddStringField("text", "Text", trans.Text);
            d.AddStringField("culture", "Culture", trans.Culture);
            d.AddStringField("tags", "Tags", trans.Tags == null ? "" : string.Join(", ", trans.Tags.Select(z => z.Name)));

            if (!d.ShowDialog())
                return;

            var text = d.GetStringField("text");
            var tagsStr = d.GetStringField("tags").Split(",", StringSplitOptions.RemoveEmptyEntries).Select(z => z.Trim()).ToArray();
            trans.Tags = tagsStr.Select(z => new Tag() { Name = z }).ToArray();
            trans.Text = text;
            trans.Culture = d.GetStringField("culture");

            UpdateList();
        }

        private void readAloudToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trainer1 t = new Trainer1();
            t.Init(GetFilteredList());
            t.MdiParent = MdiParent;
            t.Show();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as Sentence;
            Clipboard.SetText(trans.Text);

        }

        private void addPairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as Sentence;

            var d = AutoDialog.DialogHelpers.StartDialog();
            d.Text = "Add pair sentence";
            d.AddStringField("text", "Text");
            d.AddOptionsField("culture", "Culture", ["ru", "en"], 0);

            if (!d.ShowDialog())
                return;

            var text = d.GetStringField("text");
            var culture = d.GetOptionsField("culture");
            int nextId = 1;

            if (Db.Instance.Sentences.Any())
                nextId = Db.Instance.Sentences.Max(z => z.Id) + 1;

            var stoAdd = new Sentence()
            {
                Text = text,
                Culture = culture,
                Id = nextId
            };

            Db.Instance.Sentences.Add(stoAdd);



            Db.Instance.Pairs.Add(new SentencePair()
            {
                Source = trans,
                Target = stoAdd
            });

            Db.Instance.BuildTranslationsLookUp();
            UpdateList();

        }

        private async void fromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var text = Clipboard.GetText();
            var subs = text.Split(['\r', '\n',], StringSplitOptions.RemoveEmptyEntries).Select(z => z.Trim()).ToArray();
            for (int i = 0; i < subs.Length; i++)
            {
                var sub = subs[i];
                if (char.IsDigit(sub[0]))
                    sub = sub.Substring(1).Trim();
                if (char.IsPunctuation(sub[0]))
                    sub = sub.Substring(1).Trim();

                AddSentence(sub, await DetectCulture(sub) switch
                {
                    Language.Russian => "ru",
                    Language.English => "en",
                    _ => throw new Exception()
                });
            }
            UpdateList();
        }


        async Task<Language> DetectCulture(string text)
        {
            Catalyst.Models.English.Register();

            Storage.Current = new DiskStorage("catalyst-models");


            var nlp = await Pipeline.ForAsync(Language.English);

            Storage.Current = new DiskStorage("catalyst-models");
            var langDetector = await LanguageDetector.FromStoreAsync(Language.Any, Mosaik.Core.Version.Latest, "tag1");
            var doc = new Document(text, Language.Any);

            langDetector.Process(doc);

            return doc.Language;
        }

        private async void analyzeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as Sentence;
            await Analyze(trans.Text);
        }
                
        public static async Task<IToken[]> Analyze(string text)
        {
            Catalyst.Models.English.Register();

            Storage.Current = new DiskStorage("catalyst-models");


            var nlp = await Pipeline.ForAsync(Language.English);

            //AveragePerceptronTagger
            //AveragePerceptronDependencyParser
            var posTagger = await Catalyst.Models.AveragePerceptronTagger.FromStoreAsync(Language.English, Mosaik.Core.Version.Latest, "");
            nlp.Add(posTagger);


            var docs = new List<IDocument>();


            var doc = new Catalyst.Document(text, Language.English);
            docs.Add(doc);


            var processedDocs = nlp.Process(docs);

            List<IToken> ret = new List<IToken>();

            foreach (var d in processedDocs)
            {

                foreach (var sentence in d)
                {

                    foreach (var token in sentence)
                    {



                        Console.WriteLine($"Слово: {token.Value} -> Лемма: {token.Lemma}");
                    }
                }
                foreach (var sentence in d)
                {
                    
                    foreach (var token in sentence)
                    {
                        ret.Add(token);
                        
                        if (token.POS == PartOfSpeech.VERB)
                        {                            
                            //switch (token.Tag)
                            //{
                            //    case "VBN":
                            //        Console.WriteLine($"[V3 - Past Participle]: {token.Value}");
                            //        break;
                            //    case "VBD":
                            //        Console.WriteLine($"[V2 - Past Tense]: {token.Value}");
                            //        break;
                            //    case "VB":
                            //        Console.WriteLine($"[V1 - Base Form]: {token.Value}");
                            //        break;
                            //    default:
                            //        Console.WriteLine($"Other Verb Form ({token.Tag}): {token.Value}");
                            //        break;
                            //}
                        }
                    }
                  
                }
            }
            return ret.ToArray(); 
        }
    }
}