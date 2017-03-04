using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TyModManager.Localization;

namespace TyModManager.UI
{
    public partial class Mods : Form, ILocale
    {
        private List<Element.TyLevel> _tylevels = new List<Element.TyLevel>();

        public Mods()
        {
            

            InitializeComponent();

            this.Icon = Properties.Resources.mod_manager;

            // Loop through mods
            foreach (Element.TyMod tymod in Program.Mods)
            {
                // Add all ordered levels elements
                foreach (Element.TyLevel tylevel in tymod.Levels)
                    _tylevels.Add(tylevel);

                // Add mods and enable if enabled
                bool compat = tymod.TyVersion == null || tymod.TyVersion.ContainsVersion(Program.RVersion);
                this.dgvMods.Rows.Add(compat && tymod.Enabled, tymod);
                if (!compat)
                {
                    // Make sure the user understand the mod is incompatible
                    this.dgvMods.Rows[this.dgvMods.RowCount - 1].Cells[0].ReadOnly = true;
                    this.dgvMods.Rows[this.dgvMods.RowCount - 1].Cells[1].Style = new DataGridViewCellStyle(this.dgvMods.Rows[this.dgvMods.RowCount - 1].Cells[1].Style) { ForeColor = Color.Red };
                    this.dgvMods.Rows[this.dgvMods.RowCount - 1].Cells[0].ToolTipText = Locale.Language.Mods.IncompatibleMod.Replace("%%", string.Format(Locale.Language.Culture, "r{0:0}", Program.RVersion));
                    this.dgvMods.Rows[this.dgvMods.RowCount - 1].Cells[1].ToolTipText = this.dgvMods.Rows[this.dgvMods.RowCount - 1].Cells[0].ToolTipText;
                }
            }

            // Sort greatest to least
            _tylevels.Sort((a, b) => b.IndexOffset.CompareTo(a.IndexOffset));

            Localize();
        }

        public void Localize()
        {
            // Apply text
            this.Text = Locale.Language.Mods.Title;
            this.gbMods.Text = Locale.Language.Mods.ModsContainer.Text;
            this.gbLoadOrder.Text = Locale.Language.Mods.LevelContainer.Text;
            this.btOkay.Text = Locale.Language.Okay;
            this.btCancel.Text = Locale.Language.Cancel;

            // Apply tooltips
            this.toolTip.SetToolTip(this.gbMods, Locale.Language.Mods.ModsContainer.Tooltip);
            this.toolTip.SetToolTip(this.gbLoadOrder, Locale.Language.Mods.LevelContainer.Tooltip);
            this.toolTip.SetToolTip(this.lbDescription, Locale.Language.Mods.DescriptionContainer.Tooltip);
            this.dgvMods.Columns[0].ToolTipText = Locale.Language.Mods.ModsContainer.Columns.Enabled.Tooltip;
            this.dgvMods.Columns[1].ToolTipText = Locale.Language.Mods.ModsContainer.Columns.Name.Tooltip;
            this.dgvLoadOrder.Columns[0].ToolTipText = Locale.Language.Mods.LevelContainer.Columns.ID.Tooltip;
            this.dgvLoadOrder.Columns[1].ToolTipText = Locale.Language.Mods.LevelContainer.Columns.Level.Tooltip;
            this.dgvLoadOrder.Columns[2].ToolTipText = Locale.Language.Mods.LevelContainer.Columns.U.Tooltip;
            this.dgvLoadOrder.Columns[3].ToolTipText = Locale.Language.Mods.LevelContainer.Columns.D.Tooltip;

            // Apply font
            this.Font = new Font(Locale.GetFontRegular(), 8.75f, FontStyle.Regular, GraphicsUnit.Point);
            this.gbMods.Font = this.Font;
            this.gbLoadOrder.Font = this.Font;
            this.dgvMods.Font = this.Font;
            this.dgvLoadOrder.Font = this.Font;
            this.btOkay.Font = this.Font;
            this.btCancel.Font = this.Font;

            // Add levels
            this.dgvLoadOrder.Rows.Clear();
            for (int x = 0; x < _tylevels.Count; x++)
            {
                // Find empty level to replace
                if (_tylevels[x].IndexOffset < 0)
                    for (_tylevels[x].IndexOffset = 0; _tylevels[x].IndexOffset < this.dgvLoadOrder.Rows.Count; _tylevels[x].IndexOffset++)
                        if (this.dgvLoadOrder.Rows[_tylevels[x].IndexOffset].Cells[1].Value is string)
                            break;

                if (this.dgvLoadOrder.Rows.Count <= _tylevels[x].IndexOffset)
                {
                    while (this.dgvLoadOrder.Rows.Count < _tylevels[x].IndexOffset)
                        this.dgvLoadOrder.Rows.Add(this.dgvLoadOrder.Rows.Count.ToString(), Locale.Language.Mods.EmptyLevel);

                    this.dgvLoadOrder.Rows.Add(this.dgvLoadOrder.Rows.Count.ToString(), _tylevels[x]);
                }
                else
                {
                    this.dgvLoadOrder.Rows.RemoveAt(_tylevels[x].IndexOffset);
                    this.dgvLoadOrder.Rows.Insert(_tylevels[x].IndexOffset, _tylevels[x].IndexOffset.ToString(), _tylevels[x]);
                }
            }
        }

        // Update all index references to index in list
        // Truncate any extra empty levels
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

            // Truncate unnecessary empty levels at end
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

        private void DgvMods_SelectionChanged(object sender, EventArgs e)
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
                        dgvLoadOrder.Rows.Add("", Locale.Language.Mods.EmptyLevel);

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
