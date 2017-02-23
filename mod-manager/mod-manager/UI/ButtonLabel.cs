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

        private TextFormatFlags _textFormat = TextFormatFlags.Left | TextFormatFlags.HorizontalCenter;

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
            this.TextChanged += ButtonLabel_TextChanged;
            this.TextAlignChanged += ButtonLabel_TextAlignChanged;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, _isHovered ? (_isClicked ? ClickColor : HoverColor) : ForeColor, _textFormat);
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

        private void ButtonLabel_TextChanged(object sender, EventArgs e)
        {
            int textWidth = TextRenderer.MeasureText(Text, Font, ClientRectangle.Size, _textFormat).Width;
            switch (this.TextAlign)
            {
                case ContentAlignment.BottomCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                    this.Left = this.Left + (this.Width / 2) - (textWidth / 2);
                    this.Width = textWidth;
                    break;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                    this.Width = textWidth;
                    break;
                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                    this.Left = (this.Left + this.Width) - textWidth;
                    this.Width = textWidth;
                    break;
            }
        }

        private void ButtonLabel_TextAlignChanged(object sender, EventArgs e)
        {
            switch (this.TextAlign)
            {
                case ContentAlignment.BottomCenter:
                    _textFormat = TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.BottomLeft:
                    _textFormat = TextFormatFlags.Bottom | TextFormatFlags.Left;
                    break;
                case ContentAlignment.BottomRight:
                    _textFormat = TextFormatFlags.Bottom | TextFormatFlags.Right;
                    break;
                case ContentAlignment.MiddleCenter:
                    _textFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.MiddleLeft:
                    _textFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    break;
                case ContentAlignment.MiddleRight:
                    _textFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                    break;
                case ContentAlignment.TopCenter:
                    _textFormat = TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.TopLeft:
                    _textFormat = TextFormatFlags.Top | TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopRight:
                    _textFormat = TextFormatFlags.Top | TextFormatFlags.Right;
                    break;
            }

            ButtonLabel_TextChanged(sender, e);
        }
    }
}
