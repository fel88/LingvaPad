namespace LingvaPad
{
    public static class GuiHelpers
    {
        public static bool ShowQuestion(string text) => MessageBox.Show(text, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        public static void ShowInfo(string text) => MessageBox.Show(text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        public static void ShowWarning(string text) => MessageBox.Show(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        public static bool ShowWarningQuestion(string text) => MessageBox.Show(text, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
    }
}
