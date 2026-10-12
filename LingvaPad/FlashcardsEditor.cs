using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LingvaPad
{
    public partial class FlashcardsEditor : Form
    {
        public FlashcardsEditor()
        {
            InitializeComponent();
            UpdateList();
        }

        public void UpdateList()
        {
            listView1.Items.Clear();
            foreach (var item in Db.Instance.Flashcards)
            {
                var tagsStr = item.Tags == null ? "" : string.Join(", ", item.Tags.Select(z => z.Name));

                listView1.Items.Add(new ListViewItem(new string[] { item.Id.ToString(), item.Description, tagsStr }) { Tag = item });
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var nextId = 1;
            if (Db.Instance.Flashcards.Any())
            {
                nextId = Db.Instance.Flashcards.Max(z => z.Id) + 1;
            }


            Db.Instance.Flashcards.Add(new Flashcard(nextId, Clipboard.GetImage() as Bitmap));
            UpdateList();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var fc = listView1.SelectedItems[0].Tag as Flashcard;
            pictureBox1.Image = fc.Image;
            toolStripStatusLabel1.Text = $"{Math.Round((fc.Data.Length / 1024.0 / 1024.0), 2)}Mb";
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;


            var fc = listView1.SelectedItems[0].Tag as Flashcard;

            if (!GuiHelpers.ShowQuestion("Are you sure to delete?"))
                return;

            Db.Instance.Flashcards.Remove(fc);
            UpdateList();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var fc = listView1.SelectedItems[0].Tag as Flashcard;

            var d = AutoDialog.DialogHelpers.StartDialog();
            d.AddStringField("desc", "Desc", fc.Description);
            d.AddStringField("tags", "Tags", fc.Tags == null ? "" : string.Join(", ", fc.Tags.Select(z => z.Name)));

            if (!d.ShowDialog())
                return;

            fc.Description = d.GetStringField("desc");

            var tagsStr = d.GetStringField("tags").Split(",", StringSplitOptions.RemoveEmptyEntries).Select(z => z.Trim()).ToArray();
            fc.Tags = tagsStr.Select(z => new Tag() { Name = z }).ToArray();
            UpdateList();
        }

        private void copyImageToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var fc = listView1.SelectedItems[0].Tag as Flashcard;
            Clipboard.SetImage(fc.Image);
        }

        private void resaveToJpegToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var fc = listView1.SelectedItems[0].Tag as Flashcard;
            var ms = new MemoryStream();
            fc.Image.Save(ms, ImageFormat.Jpeg);
            var data = ms.ToArray();
            var ratio = fc.Data.Length / (double)data.Length;
            if (ratio > 1)
            {
                fc.Data = data;
                fc.Image = Bitmap.FromStream(new MemoryStream(data)) as Bitmap;
                pictureBox1.Image = fc.Image;
            }

        }
    }
}
