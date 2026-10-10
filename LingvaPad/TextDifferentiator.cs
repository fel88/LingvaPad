namespace LingvaPad
{
    public static class TextDifferentiator
    {
        public class TextDiffBlock
        {
            public char Char;
            public bool Correct;
        }

        public static TextDiffBlock[] FindDifference(string leftText, string rightText)
        {
            //find LCS/Common Text
            var commonText = LCS(leftText, rightText);

            return highlightText(commonText, rightText);
        }

        static string LCS(string text1, string text2)
        {
            int rows = text1.Length;
            int cols = text2.Length;

            //create a matrix of size (rows+1, cols+1) and fill it with 0s
            var matrix = new int[rows + 1][];
            int i = 0;
            int j = 0;
            for (i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new int[cols + 1];
            }

            for (i = 1; i <= rows; i++)
            {
                for (j = 1; j <= cols; j++)
                {
                    //if the characters are equal, value of LCS is 1 + LCS[i-1][j-1]
                    if (text1[i - 1] == text2[j - 1])
                    {
                        matrix[i][j] = 1 + matrix[i - 1][j - 1];
                    }
                    //else if the characters are un-equal, value of LCS is max of prev row-same col OR prev col-same row
                    else
                    {
                        matrix[i][j] = Math.Max(matrix[i - 1][j], matrix[i][j - 1]);
                    }
                }
            }
            i = rows;
            j = cols;
            var reverseCommonText = "";
            while (i > 0 && j > 0)
            {
                // if characters are equal, append to output and backtrack to diagonal element (i-1, j-1)
                if (text1[i - 1] == text2[j - 1])
                {
                    reverseCommonText = reverseCommonText + text1[i - 1];
                    i = i - 1;
                    j = j - 1;
                }
                // if characters are un-equal, backtrack to max element between (i-1, j) OR (i, j-1)
                else
                {
                    if (matrix[i - 1][j] > matrix[i][j - 1]) i = i - 1;
                    else j = j - 1;
                }
            }

            //return the reversed string since original order of characters is the opposite
            return Reverse(reverseCommonText);
        }

        static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        static TextDiffBlock[] highlightText(string commonText, string originalText)
        {
            List<TextDiffBlock> ret = new List<TextDiffBlock>();
            var i = 0;
            var j = 0;
            var text = "";
            text += "<span style='text-align: center; font-size:24px; color:white;'>";
            while (i < commonText.Length && j < originalText.Length)
            {
                //for matching letters, add a span with the background color green for that letter
                if (commonText[i] == originalText[j])
                {
                    ret.Add(new TextDiffBlock() { Char = originalText[j], Correct = true });
                    text =
                      $"{text}<span style='background-color:green;'>{originalText[j]}</span>";
                    i++;
                    j++;
                }
                //for non-matching letters, add a span with the background color red for that letter
                else
                {
                    ret.Add(new TextDiffBlock() { Char = originalText[j], Correct = false });

                    text =
                     $"{text}<span style='background-color:red;'>{originalText[j]}</span>";
                    j++;
                }
            }

            //additional unmatched text in the original text
            while (j < originalText.Length)
            {
                ret.Add(new TextDiffBlock() { Char = originalText[j], Correct = false });
                text = $"{text}<span style='background-color:red;'>{originalText[j]}</span>";
                j++;
            }
            text += "</span>";
            return ret.ToArray();
        }


    }
}
