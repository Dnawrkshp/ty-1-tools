using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace TyModManager.UI
{
    public class ButtonLabel : Label
    {
        private bool _isHovered = false;
        private bool _isClicked = false;

        // Color when mouse hovers
        public Color HoverColor { get; set; } = Color.Gray;

        // Color when mouse clicks
        public Color ClickColor { get; set; } = Color.White;

        public ButtonLabel()
        {
            this.MouseEnter += ButtonLabel_MouseEnter;
            this.MouseLeave += ButtonLabel_MouseLeave;
            this.MouseDown += ButtonLabel_MouseDown;
            this.MouseUp += ButtonLabel_MouseUp;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            TextFormatFlags flag = TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
            switch (this.TextAlign)
            {
                case ContentAlignment.BottomCenter:
                    flag = TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.BottomLeft:
                    flag = TextFormatFlags.Bottom | TextFormatFlags.Left;
                    break;
                case ContentAlignment.BottomRight:
                    flag = TextFormatFlags.Bottom | TextFormatFlags.Right;
                    break;
                case ContentAlignment.MiddleCenter:
                    flag = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.MiddleLeft:
                    flag = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    break;
                case ContentAlignment.MiddleRight:
                    flag = TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                    break;
                case ContentAlignment.TopCenter:
                    flag = TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.TopLeft:
                    flag = TextFormatFlags.Top | TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopRight:
                    flag = TextFormatFlags.Top | TextFormatFlags.Right;
                    break;
            }

            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, _isHovered ? (_isClicked ? ClickColor : HoverColor) : ForeColor, flag);
        }

        private void ButtonLabel_MouseLeave(object sender, EventArgs e)
        {
            _isHovered = false;
            this.Refresh();
        }

        private void ButtonLabel_MouseEnter(object sender, EventArgs e)
        {
            _isHovered = true;
            this.Refresh();
        }

        private void ButtonLabel_MouseUp(object sender, MouseEventArgs e)
        {
            _isClicked = false;
            this.Refresh();
        }

        private void ButtonLabel_MouseDown(object sender, MouseEventArgs e)
        {
            _isClicked = true;
            this.Refresh();
        }
    }
}
