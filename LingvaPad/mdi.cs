using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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

            var doc = XDocument.Load(ofd.FileName);
            Db.Instance = new Db(doc.Root);
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
            t.Init(Db.Instance.Sentences);
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
    }
}
