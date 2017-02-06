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
        const string tooltipMods = "The mods to install.";
        const string tooltipLoadOrder = "The level index of each custom map.\nThis will be managed by the mod manager. Only mess with this if you know what you're doing.";
        const string tooltipDescription = "Description of the selected mod.";

        public Mods()
        {
            List<Element.TyLevel> tylevels = new List<Element.TyLevel>();

            InitializeComponent();

            this.Icon = Properties.Resources.mod_manager;
            this.toolTip.SetToolTip(this.gbMods, tooltipMods);
            this.toolTip.SetToolTip(this.gbLoadOrder, tooltipLoadOrder);
            this.toolTip.SetToolTip(this.lbDescription, tooltipDescription);

            // Loop through mods
            foreach (Element.TyMod tymod in Program.Mods)
            {
                // Add all ordered levels elements
                foreach (Element.TyLevel tylevel in tymod.Levels)
                    tylevels.Add(tylevel);

                // Add mods and enable if enabled
                bool compat = tymod.TyVersion == null || tymod.TyVersion.ContainsVersion(Program.RVersion);
                this.dgvMods.Rows.Add(compat && tymod.Enabled, tymod);
                if (!compat)
                {
                    // Make sure the user understand the mod is incompatible
                    this.dgvMods.Rows[this.dgvMods.RowCount - 1].Cells[0].ReadOnly = true;
                    this.dgvMods.Rows[this.dgvMods.RowCount - 1].Cells[1].Style = new DataGridViewCellStyle(this.dgvMods.Rows[this.dgvMods.RowCount - 1].Cells[1].Style) { ForeColor = Color.Red };
                    this.dgvMods.Rows[this.dgvMods.RowCount - 1].Cells[0].ToolTipText = "This mod is not compatible with Ty r" + Program.RVersion.ToString() + ".";
                    this.dgvMods.Rows[this.dgvMods.RowCount - 1].Cells[1].ToolTipText = this.dgvMods.Rows[this.dgvMods.RowCount - 1].Cells[0].ToolTipText;
                }
            }

            // Sort greatest to least
            tylevels.Sort((a, b) => b.IndexOffset.CompareTo(a.IndexOffset));

            // Add all with offsets
            for (int x = 0; x < tylevels.Count; x++)
            {
                // Find empty level to replace
                if (tylevels[x].IndexOffset < 0)
                    for (tylevels[x].IndexOffset = 0; tylevels[x].IndexOffset < this.dgvLoadOrder.Rows.Count; tylevels[x].IndexOffset++)
                        if (this.dgvLoadOrder.Rows[tylevels[x].IndexOffset].Cells[1].Value is string && this.dgvLoadOrder.Rows[tylevels[x].IndexOffset].Cells[1].Value.ToString() == "Empty Level")
                            break;

                if (this.dgvLoadOrder.Rows.Count <= tylevels[x].IndexOffset) {
                    while (this.dgvLoadOrder.Rows.Count < tylevels[x].IndexOffset)
                        this.dgvLoadOrder.Rows.Add(this.dgvLoadOrder.Rows.Count.ToString(), "Empty Level");

                    this.dgvLoadOrder.Rows.Add(this.dgvLoadOrder.Rows.Count.ToString(), tylevels[x]);
                }
                else
                {
                    this.dgvLoadOrder.Rows.RemoveAt(tylevels[x].IndexOffset);
                    this.dgvLoadOrder.Rows.Insert(tylevels[x].IndexOffset, tylevels[x].IndexOffset.ToString(), tylevels[x]);
                }
            }
        }

        // Update all index references to index in list
        // Truncate any extra "Empty Level"
        private void CleanLoadOrder()
        {
            int lastValid = -1;

            // Loop through and update names and IndexOffsets based on row index
            for (int x = 0; x < dgvLoadOrder.RowCount; x++)
            {
                dgvLoadOrder.Rows[x].Cells[0].Value = x.ToString();
                if (dgvLoadOrder.Rows[x].Cells[1].Value is Element.TyLevel)
                {
                    (dgvLoadOrder.Rows[x].Cells[1].Value as Element.TyLevel).IndexOffset = x;
                    lastValid = x;
                }
            }

            // Truncate unnecessary "Empty Level" at end
            while (dgvLoadOrder.RowCount - 1 > lastValid)
                dgvLoadOrder.Rows.RemoveAt(lastValid + 1);
        }

        #region Events

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtOkay_Click(object sender, EventArgs e)
        {
            Element.TyMod tymod;

            Program.Config.EnabledMods.Clear();
            Program.Config.LevelOrder.Clear();

            // Save enabled mods
            for (int x = 0; x < this.dgvMods.RowCount; x++)
            {
                tymod = this.dgvMods.Rows[x].Cells[1].Value as Element.TyMod;
                tymod.Enabled = Convert.ToBoolean(this.dgvMods.Rows[x].Cells[0].Value);
                if (tymod.Enabled)
                    Program.Config.EnabledMods.Add(tymod.ToString());
            }
            
            // Save Load Order
            for (int x = 0; x < this.dgvLoadOrder.RowCount; x++)
            {
                // Add only if not "Empty Level"
                if (this.dgvLoadOrder.Rows[x].Cells[1].Value is string)
                    Program.Config.LevelOrder.Add(String.Empty);
                else
                    Program.Config.LevelOrder.Add(this.dgvLoadOrder.Rows[x].Cells[1].Value.ToString());
            }

            Program.Config.Save(Program.ConfigPath);
            this.Close();
        }

        private void dgvMods_SelectionChanged(object sender, EventArgs e)
        {
            Element.TyMod tymod;

            this.lbDescription.Text = "";
            if (this.dgvMods.SelectedRows.Count == 0)
                return;

            // Set the description if one exists
            tymod = this.dgvMods.SelectedRows[0].Cells[1].Value as Element.TyMod;
            if (tymod.Description != null)
                this.lbDescription.Text = tymod.Description;
        }

        private void DgvLoadOrder_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row;

            switch (e.ColumnIndex)
            {
                case 2: // Move up
                    if (e.RowIndex == 0)
                        break;

                    row = dgvLoadOrder.Rows[e.RowIndex];
                    dgvLoadOrder.Rows.Remove(row);
                    dgvLoadOrder.Rows.Insert(e.RowIndex - 1, row);
                    dgvLoadOrder.ClearSelection();
                    dgvLoadOrder.Rows[e.RowIndex - 1].Selected = true;

                    CleanLoadOrder();
                    break;
                case 3: // Move down


                    row = dgvLoadOrder.Rows[e.RowIndex];
                    dgvLoadOrder.Rows.Remove(row);

                    // If adding to end of list, go ahead add a new Empty Level
                    while (e.RowIndex >= dgvLoadOrder.RowCount)
                        dgvLoadOrder.Rows.Add("", "Empty Level");
                    dgvLoadOrder.Rows.Insert(e.RowIndex + 1, row);

                    dgvLoadOrder.ClearSelection();
                    dgvLoadOrder.Rows[e.RowIndex + 1].Selected = true;

                    CleanLoadOrder();
                    break;
                default:
                    break;
            }
        }

        // Select the parent TyMod in dgvMods
        private void DgvLoadOrder_SelectionChanged(object sender, EventArgs e)
        {
            Element.TyLevel tylevel;
            int x = -1;

            if (dgvLoadOrder.SelectedRows.Count == 0)
                return;

            dgvMods.ClearSelection();
            if (!(dgvLoadOrder.SelectedRows[0].Cells[1].Value is Element.TyLevel))
                return;

            // Grab tylevel
            tylevel = dgvLoadOrder.SelectedRows[0].Cells[1].Value as Element.TyLevel;

            // Find parent tymod
            for (x = 0; x < dgvMods.RowCount; x++)
                foreach (Element.TyLevel level in (this.dgvMods.Rows[x].Cells[1].Value as Element.TyMod).Levels)
                    if (level == tylevel)
                        goto exit;

            // Select parent
            exit: if (x >= 0 && x < dgvMods.RowCount)
                dgvMods.Rows[x].Selected = true;
        }

        #endregion

    }
}
