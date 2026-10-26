using AutoDialog.Extensions;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;
using TranscribeCppSharp;
using TranscribeCppSharp.Audio;
using TranscribeCppSharp.Cli;
using static System.Net.Mime.MediaTypeNames;
using Font = System.Drawing.Font;


namespace LingvaPad
{
    public partial class TranslationsTrainer : Form
    {
        public TranslationsTrainer()
        {
            InitializeComponent();
            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox1.MouseDown += PictureBox1_MouseDown;
        }
        bool wasDown = false;

        System.Drawing.Point downLocation;
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
                    if (wasDown && (new RectangleF(xx, yy, ms.Width, ms.Height).Contains(downLocation)))
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

            var answer = richTextBox3.Text;
            if (puzzleMode)
                answer = string.Join(" ", constructed.Select(z => z.Text));

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

            var html = GetDiffHtml(ret);
            wb.DocumentText = html;


        }

        public static string GetDiffHtml(TextDifferentiator.TextDiffBlock[] ret)
        {
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
            return html.ToString();
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

        void ShowNext()
        {
            var oldIdx = questionIdx;
            if (FilteredSentences.Length > 1)
                do
                {
                    questionIdx = r.Next(FilteredSentences.Length);
                } while (questionIdx == oldIdx);
            puzzlePieces = Db.Instance.TranslationsLookUp[Question][0].Text.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(z => new PuzzlePiece() { Text = z }).ToList();
            puzzlePieces = puzzlePieces.OrderBy(z => r.Next(100)).ToList();

        }
        public void Next()
        {
            showAnswer = false;
            wb.DocumentText = "";

            ShowNext();
            constructed.Clear();

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
            ShowNext();


            textBox1.Text = Question.Text;


            richTextBox3.Font = textBox1.Font;

            wb = new WebBrowser();

            tableLayoutPanel1.Controls.Add(wb, 0, 1);
            wb.Dock = DockStyle.Fill;
        }

        internal void InitByCulture(string culture, string targetCulture)
        {
            FilteredSentences = Db.Instance.Sentences.Where(z => z.Culture == culture).ToArray();
            FilteredSentences = FilteredSentences.Where(z => Db.Instance.Pairs.Any(u => (u.Target.Culture == targetCulture && u.Source == z) || (u.Target == z && u.Source.Culture == targetCulture))).ToArray();


            ShowNext();

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

        bool puzzleMode = false;
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            puzzleMode = radioButton1.Checked;
            pictureBox1.Visible = puzzleMode;
            richTextBox3.Visible = !puzzleMode;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            puzzleMode = radioButton1.Checked;
            pictureBox1.Visible = puzzleMode;
            richTextBox3.Visible = !puzzleMode;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();

        }
        StreamSTTMicrophoneRecorder rec = new StreamSTTMicrophoneRecorder();

        Session RuntimeSession;
        Model model;

        private void button4_Click(object sender, EventArgs e)
        {
            button5.Visible = true;
            button4.Visible = false;

            if (Configs.RuntimeTranscribing)
            {
                if (model == null)
                {
                    model = TranscribeCommand.LoadModel(Configs.GGUFPath);
                }
                RuntimeSession = model.CreateSession();
                rec.STTStream = RuntimeSession.CreateStream();
                rec.STTStream.Begin();
            }

            toolStripStatusLabel1.Text = "Recording...";
            rec.TextChanged = () =>
            {

                toolStripStatusLabel1.Text = "Current transcription: " + rec.CurrentText;
            };

            rec.StartRecording();
        }
        public static async Task<string> DecodeRawWav(byte[] data, WaveFormat format)
        {
            MemoryStream ms = new MemoryStream();
            using (WaveFileWriter writer = new WaveFileWriter(ms, format))
            {
                writer.Write(data, 0, data.Length);
            }

            return await Task.Run(() =>
            {
                return TranscribeCommand.DecodeWavFile(Configs.GGUFPath, WavPcmSource.TryOpen(ms.ToArray()));
            });

        }

        private async void button5_Click(object sender, EventArgs e)
        {
            button5.Visible = false;
            button4.Visible = true;
            toolStripStatusLabel1.Text = "Stopped";
            rec.StopRecording();
            string res = string.Empty;

            var data = rec.Stream.ToArray();
            res = await DecodeRawWav(data, rec.WaveFormat);

            richTextBox3.Text = res;
            Check();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
            var data = rec.Stream.ToArray();

            IWaveProvider provider = new RawSourceWaveStream(
                                     new MemoryStream(data), rec.WaveFormat);

            WaveOut waveOut = new WaveOut();
            waveOut.Init(provider);
            waveOut.Play();
        }
    }
}
