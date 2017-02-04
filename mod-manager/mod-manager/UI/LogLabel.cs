using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace TyModManager.UI
{
    public class LogLabel : Label
    {
        public LogLabel()
        {
            if (Program.Logstream != null)
                Program.Logstream.OnLog += Logstream_OnLog;
        }

        private void Logstream_OnLog(string log)
        {
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle region = new Rectangle(1, ClientRectangle.Height, ClientRectangle.Width, ClientRectangle.Height);

            if (Program.Logstream == null)
                return;

            // Go to end
            Program.Logstream.Seek(0, SeekOrigin.End);

            // Draw line by line
            while (true) {

                // Get previous line
                if (!GetPreviousLine(Program.Logstream, out string text))
                    break;

                // Indent string
                text = "  " + text;

                // Adjust Y
                region.Y -= (int)e.Graphics.MeasureString(text, Font, ClientRectangle.Size).Height + 5;

                // Ensure we are drawing within bounds
                if (region.Y < 0)
                    break;

                TextRenderer.DrawText(e.Graphics, text, Font,
                      region,
                      ForeColor, Color.Empty,
                      TextFormatFlags.Left |
                      TextFormatFlags.TextBoxControl |
                      TextFormatFlags.WordBreak);
            }
        }

        // Get previous line from current position in stream
        private bool GetPreviousLine(LogStream ls, out string result)
        {
            result = null;
            long start;

            if (ls == null)
                return false;

            if (!ls.CanRead || ls.Position <= 0)
                return false;

            start = ls.Position;
            try
            {
                while (ls.Position >= 0)
                {
                    if (!ls.CanRead)
                        return false;

                    if ((char)ls.ReadByte() == '\n' && (start - ls.Position) > 1)
                        break;

                    ls.Position -= 2;
                }
            }
            catch { ls.Position = 0; }

            if (ls.CanRead)
            {
                start = ls.Position;
                result = ls.Reader.ReadLine();
                ls.Position = start;
                return result != null;
            }

            return false;
        }

    }
}
