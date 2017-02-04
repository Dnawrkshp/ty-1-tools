using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TyModManager.UI
{
    public partial class Main : Form
    {
        private readonly Color LabelFore = Color.FromArgb(0xA0, 0xA8, 0xB0);
        private readonly Color LabelHover = Color.FromArgb(0xC0, 0xC8, 0xD0);
        private readonly Color LabelClick = Color.FromArgb(0xFF, 0xFF, 0xFF);
        private readonly Color LogFore = Color.FromArgb(0xA0, 0xC0, 0xA8);

        public Main()
        {
            InitializeComponent();

            this.Text = "Ty The Tasmanian Tiger " + "r" + Program.RVersion.ToString("G") + "_v" + Program.VVersion.ToString("F");
            this.BackgroundImage = Properties.Resources.mod_manager_bg;

            lbPlay.Font = new System.Drawing.Font(UI.Font.Raleway.RalewayRegular, 20f, FontStyle.Bold);
            lbPlay.ForeColor = LabelFore;
            lbPlay.HoverColor = LabelHover;
            lbPlay.ClickColor = LabelClick;
            lbPlay.Click += LbPlay_Click;

            lbTest.Font = lbPlay.Font;
            lbTest.ForeColor = LabelFore;
            lbTest.HoverColor = LabelHover;
            lbTest.ClickColor = LabelClick;
            lbTest.Click += LbTest_Click;

            lbMods.Font = lbPlay.Font;
            lbMods.ForeColor = LabelFore;
            lbMods.HoverColor = LabelHover;
            lbMods.ClickColor = LabelClick;
            lbMods.Click += LbMods_Click;

            lbOptions.Font = lbPlay.Font;
            lbOptions.ForeColor = LabelFore;
            lbOptions.HoverColor = LabelHover;
            lbOptions.ClickColor = LabelClick;
            lbOptions.Click += LbOptions_Click; ;

            lbExit.Font = lbPlay.Font;
            lbExit.ForeColor = LabelFore;
            lbExit.HoverColor = LabelHover;
            lbExit.ClickColor = LabelClick;
            lbExit.Click += LbExit_Click;

            lbLog.Font = new System.Drawing.Font(UI.Font.Ubuntu.UbuntuMono, 8.75f, FontStyle.Regular);
            lbLog.ForeColor = LogFore;
            lbLog.MouseDown += Main_MouseDown;
            lbLog.MouseUp += Main_MouseUp;
            lbLog.MouseMove += Main_MouseMove;
        }

        #region Mods

        private bool ApplyEnabledMods()
        {
            List<Element.TyMod> tymods = new List<Element.TyMod>();

            // Add all enabled mods to the list
            foreach (Element.TyMod tymod in Program.Mods)
                tymods.Add(tymod);

            // Apply
            return Program.ApplyMods(tymods);
        }

        #endregion

        #region Label Click

        private void LbPlay_Click(object sender, EventArgs e)
        {
            // Apply and start
            if (ApplyEnabledMods())
            {
                Program.Start(String.Empty);
                this.Close();
            }
        }

        private void LbTest_Click(object sender, EventArgs e)
        {
            if (!Program.Config.TestStartOnly && !ApplyEnabledMods())
                return;

            // Start
            Program.Start(Program.Config.TestCommand);
            this.Close();
        }

        private void LbMods_Click(object sender, EventArgs e)
        {

        }

        private void LbOptions_Click(object sender, EventArgs e)
        {
            
        }

        private void LbExit_Click(object sender, EventArgs e)
        {
            //Program.Config.Save(Program.ConfigPath);
            this.Close();
        }

        #endregion

        #region Form Drag

        private bool mainMouseDown = false;
        private Point mainMouseDownPos = Point.Empty;
        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            mainMouseDown = true;
            mainMouseDownPos = e.Location;
        }

        private void Main_MouseUp(object sender, MouseEventArgs e)
        {
            mainMouseDown = false;
        }

        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            Point scrPos;

            if (!mainMouseDown)
                return;

            scrPos = PointToScreen(e.Location);
            this.Location = new Point(scrPos.X - mainMouseDownPos.X, scrPos.Y - mainMouseDownPos.Y);
        }

        #endregion

    }
}
