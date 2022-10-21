using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Front_inz_meil
{
    public static class Improvements
    {
        public static void AppendText(this RichTextBox box, string text, Color color, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,bool scroll = true, bool AddNewLine = true)
        {
            if (AddNewLine)
            {
                text += Environment.NewLine;
            }

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.SelectionAlignment = horizontalAlignment;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            if(scroll) box.ScrollToCaret();
        }
    }
}
