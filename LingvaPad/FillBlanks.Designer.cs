namespace LingvaPad
{
    partial class FillBlanks
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tableLayoutPanel1 = new TableLayoutPanel();
            richTextBox1 = new RichTextBox();
            panel1 = new Panel();
            button1 = new Button();
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            contextMenuStrip1 = new ContextMenuStrip(components);
            editToolStripMenuItem = new ToolStripMenuItem();
            columnHeader4 = new ColumnHeader();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(richTextBox1, 0, 0);
            tableLayoutPanel1.Controls.Add(panel1, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(800, 450);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // richTextBox1
            // 
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
            richTextBox1.Location = new Point(3, 3);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(794, 219);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            // 
            // panel1
            // 
            panel1.Controls.Add(button1);
            panel1.Controls.Add(listView1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 228);
            panel1.Name = "panel1";
            panel1.Size = new Size(794, 219);
            panel1.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(565, 80);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 1;
            button1.Text = "check";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
            listView1.ContextMenuStrip = contextMenuStrip1;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Location = new Point(32, 20);
            listView1.Name = "listView1";
            listView1.Size = new Size(395, 190);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { editToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(95, 26);
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(94, 22);
            editToolStripMenuItem.Text = "edit";
            editToolStripMenuItem.Click += editToolStripMenuItem_Click;
            // 
            // FillBlanks
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "FillBlanks";
            Text = "FillBlanks";
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private RichTextBox richTextBox1;
        private Panel panel1;
        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem editToolStripMenuItem;
        private Button button1;
        private ColumnHeader columnHeader4;
    }
}