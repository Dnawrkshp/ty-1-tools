using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace TyModManager.UI
{
    public class ButtonLink : Label
    {
        public ButtonLink()
        {
            this.Click += ButtonLink_Click;

            this.Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image == null)
            {
                base.OnPaint(e);
                return;
            }

            GraphicsUnit unit = GraphicsUnit.Pixel;
            e.Graphics.DrawImage(Image, ClientRectangle, Image.GetBounds(ref unit), unit);
        }
        
        private void ButtonLink_Click(object sender, EventArgs e)
        {
            if (Text != null)
            {
                if (!Text.StartsWith("http") && !System.IO.File.Exists(Text) && !System.IO.Directory.Exists(Text))
                    System.IO.Directory.CreateDirectory(Text);

                Process.Start(Text);
            }
        }
    }
}
