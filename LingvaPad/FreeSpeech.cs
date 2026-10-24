using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using TranscribeCppSharp;
using TranscribeCppSharp.Audio;
using TranscribeCppSharp.Cli;

namespace LingvaPad
{
    public partial class FreeSpeech : Form
    {
        public FreeSpeech()
        {
            InitializeComponent();
        }

        Session RuntimeSession;
        Model model;
        StreamSTTMicrophoneRecorder rec = new StreamSTTMicrophoneRecorder();

        private void button1_Click(object sender, EventArgs e)
        {

        }
        string history = "";
        static async Task<string> DecodeRawWav(byte[] data, WaveFormat format)
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
        private async void button2_Click(object sender, EventArgs e)
        {


        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

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

                richTextBox1.Text = history + rec.CurrentText;
            };

            rec.StartRecording();
        }

        private async void toolStripButton2_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Stopped";
            rec.StopRecording();
            string res = string.Empty;
            if (true)
            {
                var data = rec.Stream.ToArray();
                res = await DecodeRawWav(data, rec.WaveFormat);
                history += res;
            }
            richTextBox1.Text = history;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            history = string.Empty;
            richTextBox1.Text = history;
        }
    }
}
