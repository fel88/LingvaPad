using Catalyst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LingvaPad
{
    public partial class FillBlanks : Form
    {
        public FillBlanks()
        {
            InitializeComponent();
        }

        internal async void Init(string v)
        {
            var ret = await SentencesEditor.Analyze(v);
            int index = 1;
            List<string> accum = new List<string>();
            for (int i = 0; i < ret.Length; i++)
            {
                Catalyst.IToken? item = ret[i];

                if (item.POS == Catalyst.PartOfSpeech.VERB)
                {
                    IToken next = null;
                    if (i < ret.Length - 1)
                    {
                        next = ret[i + 1];
                    }
                    if (next != null && (item.Lemma.ToLower() == "be" || item.Lemma.ToLower() == "have" || item.Lemma.ToLower() == "will") && next.POS == PartOfSpeech.VERB)
                    {
                        //do nothing
                        accum.Add(item.Value);
                    }
                    else
                    {
                        accum.Add(item.Value);

                        richTextBox1.Text += $"___(#{index}) ({item.Lemma})";
                        listView1.Items.Add(new ListViewItem(new string[] { index.ToString(), item.Lemma, "","" }) { Tag = accum.ToArray() });
                        index++;
                        accum.Clear();
                    }
                }
                else
                    richTextBox1.Text += item.Value;

                richTextBox1.Text += " ";
            }

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var d = AutoDialog.DialogHelpers.StartDialog();

            d.AddStringField("text", "Text");
            if (!d.ShowDialog())
                return;

            var text = d.GetStringField("text");


            listView1.SelectedItems[0].SubItems[2].Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                var lvi = listView1.Items[i];
                var ans = lvi.Tag as string[];
                lvi.SubItems[3].Text = string.Join(' ',ans);
                if(lvi.SubItems[2].Text == lvi.SubItems[3].Text)
                {
                    lvi.BackColor = Color.LightGreen;
                }
                else
                {
                    lvi.BackColor = Color.Red;
                }
            }
        }
    }
}
