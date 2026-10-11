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
                listView1.Items.Add(new ListViewItem(new string[] {
                    item.Sentence.Text, item.Translation.Text })
                { Tag = item });
            }
        }


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

            UpdateList2();

        }

        private void UpdateList2()
        {
            listView2.Items.Clear();
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as SentenceTranslation;

            //foreach (var item in trans.Translations)
            {
                // listView2.Items.Add(new ListViewItem(new string[] { item.Text }) { Tag = item });
                listView2.Items.Add(new ListViewItem(new string[] { trans.Translation.Text }) { Tag = trans.Translation });
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = AutoDialog.DialogHelpers.StartDialog();

            Sentence sentence1 = null;
            d.AddCustomDialogField("s1", "Sentence", () =>
            {
                SentencesSelector selector = new SentencesSelector();

                selector.ShowDialog();
                sentence1 = selector.SelectedSentence;
            });

            Sentence sentence2 = null;
            d.AddCustomDialogField("s2", "Translation", () =>
            {
                SentencesSelector selector = new SentencesSelector();

                selector.ShowDialog();
                sentence2 = selector.SelectedSentence;
            });

            if (!d.ShowDialog())
                return;

            var fr = Db.Instance.Translations.FirstOrDefault(z => z.Sentence == sentence1 && z.Translation == sentence2);
            var fr2 = Db.Instance.Translations.FirstOrDefault(z => z.Sentence == sentence2 && z.Translation == sentence1);
            if (fr == null && fr2 == null)
            {
                Db.Instance.Translations.Add(new SentenceTranslation()
                {
                    Sentence = sentence1,
                    Translation = sentence2
                });
            }
            else            
                GuiHelpers.ShowWarning("Already exists");                       

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

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as SentenceTranslation;
            var d = AutoDialog.DialogHelpers.StartDialog();
            d.AddStringField("text", "Text", trans.Sentence.Text);
            //d.AddStringField("translation", "Translation", trans.Translations[0].Text);

            if (!d.ShowDialog())
                return;

            var text = d.GetStringField("text");
            var translation = d.GetStringField("translation");
            trans.Sentence.Text = text;
            //trans.Translations[0].Text = translation;

            UpdateList();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as SentenceTranslation;

            var d = AutoDialog.DialogHelpers.StartDialog();

            d.AddStringField("translation", "Translation");

            if (!d.ShowDialog())
                return;


            var translation = d.GetStringField("translation");


            /*trans.Translations.Add(new Sentence()
            {
                Text = translation
            });*/

            UpdateList2();
        }

        private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            if (listView2.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as SentenceTranslation;
            var trans1 = listView2.SelectedItems[0].Tag as Sentence;
            if (!GuiHelpers.ShowQuestion($"Are you sure to delete: {trans1.Text}?"))
                return;

            //trans.Translations.Remove(trans1);

            UpdateList2();
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            if (listView2.SelectedItems.Count == 0)
                return;


            var trans1 = listView2.SelectedItems[0].Tag as Sentence;


            var d = AutoDialog.DialogHelpers.StartDialog();

            d.AddStringField("translation", "Translation", trans1.Text);

            if (!d.ShowDialog())
                return;


            var translation = d.GetStringField("translation");
            trans1.Text = translation;

            UpdateList2();
        }
    }
}
