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
    public partial class Mods : Form
    {
        public Mods()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.mod_manager;

            foreach (Element.TyMod tymod in Program.Mods)
            {
                this.lbMods.Items.Add(tymod.ToPresentableString());
                if (Program.Config.EnabledMods.Contains(tymod.ToString()))
                    this.lbMods.SetItemChecked(this.lbMods.Items.Count - 1, true);
            }
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtOkay_Click(object sender, EventArgs e)
        {
            Program.Config.EnabledMods.Clear();

            // Save enabled mods
            for (int x = 0; x < this.lbMods.Items.Count; x++)
                if (this.lbMods.GetItemChecked(x))
                    Program.Config.EnabledMods.Add(Program.Mods[x].ToString());
            
            // Save Load Order

            Program.Config.Save(Program.ConfigPath);
            this.Close();
        }

        private void LbMods_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lbMods.SelectedIndex < 0 || this.lbMods.SelectedIndex >= Program.Mods.Count)
                return;

            if (Program.Mods[this.lbMods.SelectedIndex] == null || Program.Mods[this.lbMods.SelectedIndex].Description == null)
                return;

            this.lbDescription.Text = Program.Mods[this.lbMods.SelectedIndex].Description;
        }
    }
}
