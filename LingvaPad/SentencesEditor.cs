using System.Net.Http.Headers;

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
            UpdateList();
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
    }
}