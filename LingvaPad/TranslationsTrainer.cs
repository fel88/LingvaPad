using AutoDialog.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;


namespace LingvaPad
{
    public partial class TranslationsTrainer : Form
    {
        public TranslationsTrainer()
        {
            InitializeComponent();

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                Check();
                return true;
            }
            if (keyData == Keys.Right)
            {
                Next();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        WebBrowser wb;
        Sentence[] FilteredSentences;
        SentencePair[] FilteredPairs;
        Sentence Question => FilteredSentences[questionIdx];
        int questionIdx = 0;
        bool showAnswer = false;


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.MdiParent = MdiParent;
            f.Show();
        }
        Random r = new Random();


        private void button1_Click(object sender, EventArgs e)
        {
            Check();
        }
        public void Check()
        {
            textBox1.Text = Question.Text;
            showAnswer = true;

            var lcs = Db.Instance.TranslationsLookUp[Question].Select(z => TextDifferentiator.FindDifference(richTextBox3.Text, z.Text)).OrderBy(z => z.Count(u => !u.Correct)).ToArray();
            //find closest answer and make a decision about corectness according current checking policy
            var ret = lcs.First();
            label1.Font = new System.Drawing.Font("Consolas", 18);
            label1.Visible = true;
            if (IsAnswerCorrect(ret))
            {
                label1.Text = "Correct";
                label1.ForeColor = Color.White;
                label1.BackColor = Color.Green;
            }
            else
            {
                label1.Text = "Wrong";
                label1.ForeColor = Color.White;
                label1.BackColor = Color.Red;
            }
            StringBuilder html = new StringBuilder();
            html.Append("<span style='text-align: center; font-size:24px; color:white;'>");

            for (int i = 0; i < ret.Length; i++)
            {
                TextDifferentiator.TextDiffBlock? item = ret[i];

                if (item.Correct)
                {
                    html.Append($"<span style='background-color:green;'>{item.Char}</span>");
                }
                else
                {
                    html.Append($"<span style='background-color:red;'>{item.Char}</span>");
                }
            }

            html.Append("</span>");
            wb.DocumentText = html.ToString();


        }

        private bool IsAnswerCorrect(TextDifferentiator.TextDiffBlock[] ret)
        {
            var perc = ret.Count(z => z.Correct) / (double)ret.Length;
            return perc > .95;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Next();
        }

        public void Next()
        {
            showAnswer = false;
            wb.DocumentText = "";
            var oldIdx = questionIdx;
            if (FilteredSentences.Length > 1)
                do
                {
                    questionIdx = r.Next(FilteredSentences.Length);
                } while (questionIdx == oldIdx);
            var tags = Db.Instance.TranslationsLookUp[Question].SelectMany(z => z.Tags).ToList();
            tags.AddRange(Question.Tags);
            label2.Text = string.Join(", ", tags.Distinct().Select(z => z.Name));
            textBox1.Text = Question.Text;
            richTextBox3.Text = "";
            label1.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Question.Translations.Add(new Sentence() { Text = richTextBox3.Text });
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }
        internal void InitByTag(string tag)
        {
            FilteredPairs = Db.Instance.Pairs.Where(z => z.Tags.Any(u => u.Name == tag)).ToArray();
            FilteredSentences = FilteredPairs.Select(z => z.Source).Distinct().ToArray();
            toolStripStatusLabel1.Text = $"Sentences: {FilteredSentences.Length}";
            questionIdx = r.Next(FilteredSentences.Length);

            textBox1.Text = Question.Text;


            richTextBox3.Font = textBox1.Font;

            wb = new WebBrowser();

            tableLayoutPanel1.Controls.Add(wb, 0, 1);
            wb.Dock = DockStyle.Fill;

        }
        internal void InitByCulture(string culture, string targetCulture)
        {
            FilteredSentences = Db.Instance.Sentences.Where(z => z.Culture == culture).ToArray();
            questionIdx = r.Next(FilteredSentences.Length);
            toolStripStatusLabel1.Text = $"Sentences: {FilteredSentences.Length}";

            textBox1.Text = Question.Text;


            richTextBox3.Font = textBox1.Font;

            wb = new WebBrowser();

            tableLayoutPanel1.Controls.Add(wb, 0, 1);
            wb.Dock = DockStyle.Fill;

        }
        bool showTags = false;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            showTags = checkBox1.Checked;
            label2.Visible = showTags;
        }
    }
}
