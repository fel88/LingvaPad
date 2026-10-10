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
            questionIdx = r.Next(Db.Instance.Translations.Count);

            textBox1.Text = Question.Sentence.Text;


            richTextBox3.Font = textBox1.Font;

            wb = new WebBrowser();

            tableLayoutPanel1.Controls.Add(wb, 0, 1);
            wb.Dock = DockStyle.Fill;
        }

        WebBrowser wb;
        SentenceTranslation Question => Db.Instance.Translations[questionIdx];
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

            textBox1.Text = Question.Sentence.Text;
            showAnswer = true;

            var lcs = Question.Translations.Select(z => TextDifferentiator.FindDifference(richTextBox3.Text, z.Text)).OrderBy(z => z.Count(u => !u.Correct)).ToArray();
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

            foreach (var item in ret)
            {
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
            showAnswer = false;
            wb.DocumentText = "";
            questionIdx = r.Next(Db.Instance.Translations.Count);
            textBox1.Text = Question.Sentence.Text;
            richTextBox3.Text = "";
            label1.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Question.Translations.Add(new Sentence() { Text = richTextBox3.Text });
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }
    }
}
