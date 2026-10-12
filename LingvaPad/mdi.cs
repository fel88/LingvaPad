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
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media.Animation;
using System.Xml.Linq;

namespace LingvaPad
{
    public partial class mdi : Form
    {
        public mdi()
        {
            InitializeComponent();


        }

        private void translationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslationsTrainer f = new TranslationsTrainer();
            var d = AutoDialog.DialogHelpers.StartDialog();
            d.AddOptionsField("mode", "Mode", ["ru->eng", "eng->ru"], 0);
            if (!d.ShowDialog())
                return;

            var modeIdx = d.GetOptionsFieldIdx("mode");
            if (modeIdx == 0)
                f.InitByCulture("ru", "en");
            else
                f.InitByCulture("en", "ru");

            f.MdiParent = this;
            f.Show();
        }

        private void tmplateToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Xml files (*.xml)|*.xml";

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            var xml = Db.Instance.ToXml();
            File.WriteAllText(sfd.FileName, xml.ToString());
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Xml files (*.xml)|*.xml";

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            lastOpenedPath = ofd.FileName;
            SaveSettings();
            Open(ofd.FileName);
        }

        private void openRecentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists("settings.xml"))
                return;

            var doc = XDocument.Load("settings.xml");
            var path = doc.Descendants("path").First().Value;

            if (!GuiHelpers.ShowQuestion($"Are you sure to open: {path}?"))
                return;

            lastOpenedPath = path;
            Open(path);            
        }

        private void Open(string path)
        {
            var doc = XDocument.Load(path);
            Db.Instance = new Db(doc.Root);
            toolStripStatusLabel3.Text = $"Sentences: {Db.Instance.Sentences.Count}   Pairs: {Db.Instance.Pairs.Count}";
        }

        string lastOpenedPath;
        void SaveSettings()
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("root", new XElement("setting", new XElement("path", new XCData(lastOpenedPath)))));
            doc.Save("settings.xml");
        }

        private void translationsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.MdiParent = this;
            form.Show();
        }

        private void quizToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //simple 4 answer options test
        }

        private void simpleRepeatAloudToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trainer1 t = new Trainer1();
            t.Init(Db.Instance.Sentences.Where(z => z.Culture == "en").ToArray());
            t.MdiParent = this;
            t.Show();
        }

        private void sentencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SentencesEditor form = new SentencesEditor();
            form.MdiParent = this;
            form.Show();
        }

        private void fillBlankToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void translateAloudToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trainer1 t = new Trainer1();
            t.Init(Db.Instance.Sentences.Where(z => z.Culture == "ru").ToArray());
            t.MdiParent = this;
            t.Show();
        }

        private void flashcardsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FlashcardsEditor f = new FlashcardsEditor();
            f.MdiParent = this;
            f.Show();
        }

        private void flashcardsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //flashcards trainer
        }

        private void customPairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TranslationsTrainer f = new TranslationsTrainer();
            var d = AutoDialog.DialogHelpers.StartDialog();
            var grouped = Db.Instance.Pairs.SelectMany(z => z.Tags).GroupBy(z => z.Name).ToArray();
            var allTags = grouped.Select(z => $"{z.Key} ({z.Count()})").ToArray();
            d.AddOptionsField("filter", "Filter", allTags.ToArray(), 0);
            if (!d.ShowDialog())
                return;

            var filterIdx = d.GetOptionsFieldIdx("filter");
            f.InitByTag(grouped[filterIdx].Key);

            f.MdiParent = this;
            f.Show();
        }

        

        private void simpleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

      
    }
}
