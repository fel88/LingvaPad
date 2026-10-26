using Mosaik.Core;
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
            var filterLow = filter.ToLower();
            foreach (var item in Db.Instance.Pairs)
            {
                var tagsStr = item.Tags == null ? "" : string.Join(", ", item.Tags.Select(z => z.Name));
                string[] stringsToSearch = [item.Source.Text, item.Target.Text, tagsStr];
                if (!string.IsNullOrEmpty(filter) && !stringsToSearch.Any(z => z.ToLower().Contains(filterLow)))
                    continue;

                listView1.Items.Add(new ListViewItem(new string[] {
                    item.Source.Text, item.Target.Text,tagsStr })
                { Tag = item });
            }
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

            var trans = listView1.SelectedItems[0].Tag as SentencePair;
            richTextBox1.Text = trans.Source.Text;
            richTextBox2.Text = trans.Target.Text;

            UpdateList2();

        }

        private void UpdateList2()
        {
            listView2.Items.Clear();
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as SentencePair;

            //foreach (var item in trans.Translations)
            {
                // listView2.Items.Add(new ListViewItem(new string[] { item.Text }) { Tag = item });
                listView2.Items.Add(new ListViewItem(new string[] { trans.Target.Text }) { Tag = trans.Target });
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

            d.AddStringField("tags", "Tags");

            if (!d.ShowDialog())
                return;

            var fr = Db.Instance.Pairs.FirstOrDefault(z => z.Source == sentence1 && z.Target == sentence2);
            var fr2 = Db.Instance.Pairs.FirstOrDefault(z => z.Source == sentence2 && z.Target == sentence1);
            if (fr != null || fr2 != null)
                if (!GuiHelpers.ShowWarningQuestion("Already exists. Add anyway?"))
                    return;

            var tagsStr = d.GetStringField("tags").Split(",", StringSplitOptions.RemoveEmptyEntries).Select(z => z.Trim()).ToArray();
            Db.Instance.Pairs.Add(new SentencePair()
            {
                Source = sentence1,
                Target = sentence2,
                Tags = tagsStr.Select(z => new Tag() { Name = z }).ToArray()
            });
            Db.Instance.BuildTranslationsLookUp();

            UpdateList();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as SentencePair;
            if (GuiHelpers.ShowQuestion($"Are you sure to delete  {trans.Source.Text}?"))
            {
                Db.Instance.Pairs.Remove(trans);
                UpdateList();
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            var trans = listView1.SelectedItems[0].Tag as SentencePair;

            var d = AutoDialog.DialogHelpers.StartDialog();

            Sentence sentence1 = trans.Source;
            d.AddCustomDialogField("s1", "Sentence", () =>
            {
                SentencesSelector selector = new SentencesSelector();

                selector.ShowDialog();
                sentence1 = selector.SelectedSentence;
            });

            Sentence sentence2 = trans.Target;
            d.AddCustomDialogField("s2", "Translation", () =>
            {
                SentencesSelector selector = new SentencesSelector();

                selector.ShowDialog();
                sentence2 = selector.SelectedSentence;
            });

            d.AddStringField("tags", "Tags", trans.Tags == null ? "" : string.Join(", ", trans.Tags.Select(z => z.Name)));

            if (!d.ShowDialog())
                return;

            var fr = Db.Instance.Pairs.FirstOrDefault(z => z.Source == sentence1 && z.Target == sentence2);
            var fr2 = Db.Instance.Pairs.FirstOrDefault(z => z.Source == sentence2 && z.Target == sentence1);

            trans.Source = sentence1;
            trans.Target = sentence2;

            var tagsStr = d.GetStringField("tags").Split(",", StringSplitOptions.RemoveEmptyEntries).Select(z => z.Trim()).ToArray();
            trans.Tags = tagsStr.Select(z => new Tag() { Name = z }).ToArray();
            UpdateList();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var trans = listView1.SelectedItems[0].Tag as SentencePair;

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

            var trans = listView1.SelectedItems[0].Tag as SentencePair;
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

        private async void fromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = AutoDialog.DialogHelpers.StartDialog();

            if (!d.ShowDialog())
                return;

            var text = Clipboard.GetText();
            var lines = text.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (var item in lines)
            {
                var spl = item.Split('|').ToArray();
                var s1 = await Db.Instance.AddOrGetSentence(spl[0]);
                var s2 = await Db.Instance.AddOrGetSentence(spl[1]);
                Db.Instance.Pairs.Add(new SentencePair() { Source = s1, Target = s2 });
            }
        }
    }
}
