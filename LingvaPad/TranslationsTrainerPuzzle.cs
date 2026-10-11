using AutoDialog.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Converters;
using System.Xml.Linq;
using WebBrowser = System.Windows.Forms.WebBrowser;


namespace LingvaPad
{
    public partial class TranslationsTrainerPuzzle : Form
    {
        public TranslationsTrainerPuzzle()
        {
            InitializeComponent();

            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox1.MouseDown += PictureBox1_MouseDown;
        }

        bool wasDown = false;

        Point downLocation;
        private void PictureBox1_MouseDown(object? sender, MouseEventArgs e)
        
        {
            downLocation = e.Location;
            wasDown = true;
        }

        public class PuzzlePiece
        {
            public string Text;
            public int Index;
        }
        List<PuzzlePiece> puzzlePieces = new List<PuzzlePiece>();
        List<PuzzlePiece> constructed = new List<PuzzlePiece>();
        private void PictureBox1_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            e.Graphics.Clear(Color.White);


            var curp = pictureBox1.PointToClient(Cursor.Position);

            var gr = e.Graphics;
            int xx = 0;
            int yy = 0;
            var font = new Font("Consoles", 18);
            int gap = 20;
            List<PuzzlePiece> toRemove = new List<PuzzlePiece>();
            foreach (var item in constructed)
            {

                var ms = gr.MeasureString(item.Text, font);
                gr.DrawRectangle(Pens.Black, xx, yy, ms.Width, ms.Height);
                if (new RectangleF(xx, yy, ms.Width, ms.Height).Contains(curp))
                {

                    gr.FillRectangle(Brushes.LightGreen, xx, yy, ms.Width, ms.Height);
                }
                if (wasDown && (new RectangleF(xx, yy, ms.Width, ms.Height).Contains(downLocation)))
                {
                    toRemove.Add(item);
                    wasDown = false;
                }
                gr.DrawString(item.Text, font, Brushes.Black, xx, yy);
                xx += (int)ms.Width + gap;
            }
            foreach (var item in toRemove)
            {
                constructed.Remove(item);
            }

            xx = 0;
            yy = 100;
            foreach (var item in puzzlePieces)
            {

                var ms = gr.MeasureString(item.Text, font);

              
                if (!constructed.Contains(item))
                {
                    if (wasDown &&  (new RectangleF(xx, yy, ms.Width, ms.Height).Contains(downLocation)))
                    {
                        constructed.Add(item);
                        wasDown = false;
                    }
                    gr.DrawRectangle(Pens.Black, xx, yy, ms.Width, ms.Height);
                    if (new RectangleF(xx, yy, ms.Width, ms.Height).Contains(curp))
                    {
                      
                        gr.FillRectangle(Brushes.LightGreen, xx, yy, ms.Width, ms.Height);
                    }
                    gr.DrawString(item.Text, new Font("Consoles", 18), Brushes.Black, xx, yy);

                }

                xx += (int)ms.Width + gap;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                Check();
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

        public void DrawPuzzle()
        {


        }
        public void Check()
        {
            textBox1.Text = Question.Text;
            showAnswer = true;

            var answer = string.Join(" ", constructed.Select(z => z.Text));
            var lcs = Db.Instance.TranslationsLookUp[Question].Select(z => TextDifferentiator.FindDifference(answer, z.Text)).OrderBy(z => z.Count(u => !u.Correct)).ToArray();
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

        void ShowNext()
        {
            questionIdx = r.Next(FilteredSentences.Length);
            puzzlePieces = Db.Instance.TranslationsLookUp[Question][0].Text.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(z => new PuzzlePiece() { Text = z }).ToList();
            puzzlePieces = puzzlePieces.OrderBy(z => r.Next(100)).ToList();

        }
        private void button2_Click(object sender, EventArgs e)
        {
            showAnswer = false;
            wb.DocumentText = "";
            ShowNext();            
            
            textBox1.Text = Question.Text;
            constructed.Clear();
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
            ShowNext();
            

            textBox1.Text = Question.Text;




            wb = new WebBrowser();

            tableLayoutPanel1.Controls.Add(wb, 0, 1);
            wb.Dock = DockStyle.Fill;

        }
        internal void InitByCulture(string culture, string targetCulture)
        {
            FilteredSentences = Db.Instance.Sentences.Where(z => z.Culture == culture).ToArray();
            FilteredSentences = FilteredSentences.Where(z => Db.Instance.Pairs.Any(u => (u.Target.Culture == targetCulture && u.Source == z) || (u.Target == z && u.Source.Culture == targetCulture))).ToArray();
            ShowNext();

            textBox1.Text = Question.Text;
            wb = new WebBrowser();

            tableLayoutPanel1.Controls.Add(wb, 0, 1);
            wb.Dock = DockStyle.Fill;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }
    }
}
