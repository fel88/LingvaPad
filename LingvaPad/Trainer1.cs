
namespace LingvaPad
{
    public partial class Trainer1 : Form
    {
        public Trainer1()
        {
            InitializeComponent();


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

                textBox1.Text = Sentence.Text;
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Visible = checkBox1.Checked;
        }
    }
}
