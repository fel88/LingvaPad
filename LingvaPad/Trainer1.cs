
using NAudio.Wave;
using System.IO;
using System.Text;
using TranscribeCppSharp;
using TranscribeCppSharp.Cli;

namespace LingvaPad
{
    public partial class Trainer1 : Form
    {
        public Trainer1()
        {
            InitializeComponent();

            wb = new WebBrowser();

            tableLayoutPanel1.Controls.Add(wb, 0, 1);
            wb.Dock = DockStyle.Fill;
        }

        public void ShowNext()
        {
            var oldIdx = idx;
            if (FilteredSentences.Length > 1)
                while (oldIdx == idx)
                    idx = r.Next(FilteredSentences.Length);

            if (Db.Instance.TranslationsLookUp.ContainsKey(Sentence) && Db.Instance.TranslationsLookUp[Sentence].Any(z => z.Culture != Sentence.Culture))
            {
                textBox2.Text = Db.Instance.TranslationsLookUp[Sentence].First(z => z.Culture != Sentence.Culture).Text;
            }
            else
            {
                textBox2.Text = string.Empty;
            }

            textBox3.Text = Sentence.Text;
        }

        public void Init(IReadOnlyList<Sentence> sentences)
        {
            FilteredSentences = sentences.ToArray();
            toolStripStatusLabel1.Text = $"Total sentences: {FilteredSentences.Length}";
            ShowNext();
        }

        public Sentence[] FilteredSentences;

        Sentence Sentence => FilteredSentences[idx];
        int idx = 0;


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.MdiParent = MdiParent;
            f.Show();
        }

        Random r = new Random();


        private void button2_Click(object sender, EventArgs e)
        {
            ShowNext();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //filter
        }
        WebBrowser wb;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Visible = checkBox1.Checked;
        }
        StreamSTTMicrophoneRecorder rec = new StreamSTTMicrophoneRecorder();

        Session RuntimeSession;
        Model model;

        private void button6_Click(object sender, EventArgs e)
        {

            var data = rec.Stream.ToArray();

            IWaveProvider provider = new RawSourceWaveStream(
                                     new MemoryStream(data), rec.WaveFormat);

            WaveOut waveOut = new WaveOut();
            waveOut.Init(provider);
            waveOut.Play();
        }

        public void Check()
        {

        }

        private async void button5_Click(object sender, EventArgs e)
        {
            button5.Visible = false;
            button4.Visible = true;
            toolStripStatusLabel1.Text = "Stopped";
            rec.StopRecording();
            string res = string.Empty;

            var data = rec.Stream.ToArray();
            res = await TranslationsTrainer.DecodeRawWav(data, rec.WaveFormat);
            var diff = TextDifferentiator.FindDifference(textBox3.Text, res);
            wb.DocumentText = TranslationsTrainer.GetDiffHtml(diff); 
            Check();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button5.Visible = true;
            button4.Visible = false;

            //if (Configs.RuntimeTranscribing)
            //{
            //    if (model == null)
            //    {
            //        model = TranscribeCommand.LoadModel(Configs.GGUFPath);
            //    }
            //    RuntimeSession = model.CreateSession();
            //    rec.STTStream = RuntimeSession.CreateStream();
            //    rec.STTStream.Begin();
            //}

            toolStripStatusLabel1.Text = "Recording...";
            rec.TextChanged = () =>
            {

                toolStripStatusLabel1.Text = "Current transcription: " + rec.CurrentText;
            };

            rec.StartRecording();
        }
    }
}
