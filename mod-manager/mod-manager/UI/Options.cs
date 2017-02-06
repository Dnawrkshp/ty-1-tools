using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TyModManager.UI
{
    public partial class Options : Form
    {
        const string tooltipTestArgs = "The command line arguments to pass to the application on Test.";
        const string tooltipStartOnly = "Launch Ty.exe without installing mods on Test. This does not affect any previously installed mods.";

        public Options()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.mod_manager;

            this.toolTip.SetToolTip(this.gbTestArgs, tooltipTestArgs);
            this.toolTip.SetToolTip(this.tbTestArgs, tooltipTestArgs);
            this.toolTip.SetToolTip(this.gbStartOnly, tooltipStartOnly);
            this.toolTip.SetToolTip(this.cbStartOnly, tooltipStartOnly);

            this.tbTestArgs.Text = Program.Config.TestCommand;
            this.cbStartOnly.Checked = Program.Config.TestStartOnly;
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtOkay_Click(object sender, EventArgs e)
        {
            Program.Config.TestCommand = tbTestArgs.Text;
            Program.Config.TestStartOnly = cbStartOnly.Checked;
            Program.Config.Save(Program.ConfigPath);
            this.Close();
        }
    }
}
