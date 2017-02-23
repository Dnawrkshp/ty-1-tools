using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TyModManager.Localization;

namespace TyModManager.UI
{
    public partial class Main : Form, ILocale
    {
        private readonly Color LabelFore = Color.FromArgb(0xA0, 0xA8, 0xB0);
        private readonly Color LabelHover = Color.FromArgb(0xC0, 0xC8, 0xD0);
        private readonly Color LabelClick = Color.FromArgb(0xFF, 0xFF, 0xFF);
        private readonly Color LogFore = Color.FromArgb(0xA0, 0xC0, 0xA8);

        public Main()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.mod_manager;
            this.Text = "Ty The Tasmanian Tiger " + "r" + Program.RVersion.ToString("G") + "_v" + Program.VVersion.ToString("F");
            this.BackgroundImage = Properties.Resources.mod_manager_bg;
            
            lbPlay.ForeColor = LabelFore;
            lbPlay.HoverColor = LabelHover;
            lbPlay.ClickColor = LabelClick;
            lbPlay.Click += LbPlay_Click;
            
            lbTest.ForeColor = LabelFore;
            lbTest.HoverColor = LabelHover;
            lbTest.ClickColor = LabelClick;
            lbTest.Click += LbTest_Click;
            
            lbMods.ForeColor = LabelFore;
            lbMods.HoverColor = LabelHover;
            lbMods.ClickColor = LabelClick;
            lbMods.Click += LbMods_Click;
            
            lbOptions.ForeColor = LabelFore;
            lbOptions.HoverColor = LabelHover;
            lbOptions.ClickColor = LabelClick;
            lbOptions.Click += LbOptions_Click;
            
            lbExit.ForeColor = LabelFore;
            lbExit.HoverColor = LabelHover;
            lbExit.ClickColor = LabelClick;
            lbExit.Click += LbExit_Click;
            
            lbLog.ForeColor = LogFore;
            lbLog.MouseDown += Main_MouseDown;
            lbLog.MouseUp += Main_MouseUp;
            lbLog.MouseMove += Main_MouseMove;

            // Setup locale
            if (Program.Config.Language == null || Program.Config.Language == String.Empty)
                Locale.ChangeLocale(CultureInfo.InstalledUICulture.Name);
            else
                Locale.ChangeLocale(Program.Config.Language);

            Localize();
        }

        public void Localize()
        {
            // Apply font
            lbPlay.Font = new System.Drawing.Font(Locale.GetFontRegular(), 20f, FontStyle.Bold);
            lbTest.Font = lbPlay.Font;
            lbMods.Font = lbPlay.Font;
            lbOptions.Font = lbPlay.Font;
            lbExit.Font = lbPlay.Font;
            lbLog.Font = new System.Drawing.Font(Locale.GetFontMono(), 8.75f, FontStyle.Regular);

            // Apply text
            lbPlay.Text = Locale.Language.Main.Play.Text;
            lbTest.Text = Locale.Language.Main.Test.Text;
            lbMods.Text = Locale.Language.Main.Mods.Text.Replace("%%", Program.Mods.Count(x => x.Enabled).ToString());
            lbOptions.Text = Locale.Language.Main.Options.Text;
            lbExit.Text = Locale.Language.Main.Exit;

            // Apply tooltip
            toolTip.SetToolTip(lbPlay, Locale.Language.Main.Play.Tooltip);
            toolTip.SetToolTip(lbTest, Locale.Language.Main.Test.Tooltip);
            toolTip.SetToolTip(lbMods, Locale.Language.Main.Mods.Tooltip);
            toolTip.SetToolTip(lbOptions, Locale.Language.Main.Options.Tooltip);
            toolTip.SetToolTip(lbExit, null);
            toolTip.SetToolTip(lbWiki, Locale.Language.Main.Wiki.Tooltip);
            toolTip.SetToolTip(lbFolder, Locale.Language.Main.Folder.Tooltip);
            toolTip.SetToolTip(lbGithub, Locale.Language.Main.Github.Tooltip);
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
            // Update cursor
            Cursor = Cursors.WaitCursor;

            // Apply and start
            if (ApplyEnabledMods())
            {
                Program.Start(String.Empty);
                this.Close();
            }

            Cursor = DefaultCursor;
        }

        private void LbTest_Click(object sender, EventArgs e)
        {
            // Update cursor
            Cursor = Cursors.WaitCursor;

            if (!Program.Config.TestStartOnly && !ApplyEnabledMods())
            {
                Cursor = DefaultCursor;
                return;
            }

            // Start
            Program.Start(Program.Config.TestCommand);
            this.Close();
        }

        private void LbMods_Click(object sender, EventArgs e)
        {
            Mods mods = new Mods()
            {
                Location = new Point(this.Location.X, this.Location.Y - this.Height / 4)
            };

            mods.Shown += SubForm_Shown;
            mods.FormClosed += SubForm_FormClosed;

            mods.Show();
        }

        private void LbOptions_Click(object sender, EventArgs e)
        {
            Options options = new Options()
            {
                Location = new Point(this.Location.X, this.Location.Y + this.Height / 4)
            };

            options.Shown += SubForm_Shown;
            options.FormClosed += SubForm_FormClosed;

            options.Show();
        }

        private void LbExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Sub Form

        private void SubForm_Shown(object sender, EventArgs e)
        {
            // Make invisible out label buttons
            lbExit.Visible = false;
            lbPlay.Visible = false;
            lbMods.Visible = false;
            lbOptions.Visible = false;
            lbTest.Visible = false;
        }

        private void SubForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Make visible out label buttons
            lbExit.Visible = true;
            lbPlay.Visible = true;
            lbMods.Visible = true;
            lbOptions.Visible = true;
            lbTest.Visible = true;

            lbMods.Text = Locale.Language.Main.Mods.Text.Replace("%%", Program.Mods.Count(x => x.Enabled).ToString());
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
