namespace LingvaPad
{
    public static class GuiHelpers
    {
        public static bool ShowQuestion(string text) => MessageBox.Show(text, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}
