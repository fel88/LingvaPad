using System.Net.Http.Headers;

namespace LingvaPad
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            UpdateList();

        }

        private void UpdateList()
        {
            listView1.Items.Clear();
            foreach (var item in Db.Instance.Translations)
            {
                listView1.Items.Add(new ListViewItem(new string[] { item.Sentence.Text, item.Translation.Text, item.Tags == null ? "" : string.Join(',', item.Tags.Select(z => z.Name)) }) { Tag = item });
            }
        }

        List<SentenceTranslation> translations = new List<SentenceTranslation>();

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as SentenceTranslation;
            richTextBox1.Text = trans.Sentence.Text;
            richTextBox2.Text = trans.Translation.Text;

        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = AutoDialog.DialogHelpers.StartDialog();
            d.AddStringField("text", "Text");
            d.AddStringField("translation", "Translation");

            if (!d.ShowDialog())
                return;

            var text = d.GetStringField("text");
            var translation = d.GetStringField("text");

            Db.Instance.Translations.Add(new SentenceTranslation()
            {
                Sentence = new Sentence() { Text = text },
                Translation = new Sentence() { Text = translation }
            });

            UpdateList();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as SentenceTranslation;
            if (GuiHelpers.ShowQuestion($"Are you sure to delete  {trans.Sentence.Text}?"))
            {
                Db.Instance.Translations.Remove(trans);
                UpdateList();
            }
        }
    }
}
