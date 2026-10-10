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


namespace LingvaPad
{
    public partial class TranslationsTrainer : Form
    {
        public TranslationsTrainer()
        {
            InitializeComponent();
            questionIdx = r.Next(Db.Instance.Translations.Count);

            textBox1.Text = Question.Sentence.Text;
            pictureBox1.Paint += PictureBox1_Paint;

            richTextBox3.Font = textBox1.Font;
        }

        SentenceTranslation Question => Db.Instance.Translations[questionIdx];
        int questionIdx = 0;
        bool showAnswer = false;
        private void PictureBox1_Paint(object? sender, PaintEventArgs e)
        {

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.White);
            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            var font1 = new Font(textBox1.Font.FontFamily.Name, textBox1.Font.Size);
            if (showAnswer)
                e.Graphics.DrawString(Question.Translation.Text, textBox1.Font, Brushes.Green, new RectangleF(0, 0, pictureBox1.Width, pictureBox1.Height), stringFormat);
        }

        void DrawAnswer()
        {
            pictureBox1.Invalidate();
        }

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
            pictureBox1.Invalidate();

            if (richTextBox3.Text.ToLower() == Question.Translation.Text)
            {

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            showAnswer = false;
            pictureBox1.Invalidate();

            questionIdx = r.Next(Db.Instance.Translations.Count);
            textBox1.Text = Question.Sentence.Text;



        }
    }
}
