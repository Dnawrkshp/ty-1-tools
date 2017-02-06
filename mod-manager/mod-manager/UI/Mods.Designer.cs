namespace TyModManager.UI
{
    partial class Mods
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btCancel = new System.Windows.Forms.Button();
            this.btOkay = new System.Windows.Forms.Button();
            this.gbMods = new System.Windows.Forms.GroupBox();
            this.gbLoadOrder = new System.Windows.Forms.GroupBox();
            this.dgvLoadOrder = new System.Windows.Forms.DataGridView();
            this.ColumnID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnUp = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnDown = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lbDescription = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.dgvMods = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbMods.SuspendLayout();
            this.gbLoadOrder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMods)).BeginInit();
            this.SuspendLayout();
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(264, 442);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 0;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.BtCancel_Click);
            // 
            // btOkay
            // 
            this.btOkay.Location = new System.Drawing.Point(170, 442);
            this.btOkay.Name = "btOkay";
            this.btOkay.Size = new System.Drawing.Size(75, 23);
            this.btOkay.TabIndex = 1;
            this.btOkay.Text = "OK";
            this.btOkay.UseVisualStyleBackColor = true;
            this.btOkay.Click += new System.EventHandler(this.BtOkay_Click);
            // 
            // gbMods
            // 
            this.gbMods.Controls.Add(this.dgvMods);
            this.gbMods.Location = new System.Drawing.Point(12, 12);
            this.gbMods.Name = "gbMods";
            this.gbMods.Size = new System.Drawing.Size(327, 193);
            this.gbMods.TabIndex = 2;
            this.gbMods.TabStop = false;
            this.gbMods.Text = "Mods";
            // 
            // gbLoadOrder
            // 
            this.gbLoadOrder.Controls.Add(this.dgvLoadOrder);
            this.gbLoadOrder.Location = new System.Drawing.Point(12, 211);
            this.gbLoadOrder.Name = "gbLoadOrder";
            this.gbLoadOrder.Size = new System.Drawing.Size(327, 165);
            this.gbLoadOrder.TabIndex = 3;
            this.gbLoadOrder.TabStop = false;
            this.gbLoadOrder.Text = "Custom Map Import Order";
            // 
            // dgvLoadOrder
            // 
            this.dgvLoadOrder.AllowUserToAddRows = false;
            this.dgvLoadOrder.AllowUserToDeleteRows = false;
            this.dgvLoadOrder.AllowUserToResizeColumns = false;
            this.dgvLoadOrder.AllowUserToResizeRows = false;
            this.dgvLoadOrder.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvLoadOrder.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvLoadOrder.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvLoadOrder.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvLoadOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLoadOrder.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnID,
            this.ColumnName,
            this.ColumnUp,
            this.ColumnDown});
            this.dgvLoadOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLoadOrder.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvLoadOrder.Location = new System.Drawing.Point(3, 16);
            this.dgvLoadOrder.MultiSelect = false;
            this.dgvLoadOrder.Name = "dgvLoadOrder";
            this.dgvLoadOrder.ReadOnly = true;
            this.dgvLoadOrder.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvLoadOrder.RowHeadersVisible = false;
            this.dgvLoadOrder.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLoadOrder.Size = new System.Drawing.Size(321, 146);
            this.dgvLoadOrder.TabIndex = 0;
            this.dgvLoadOrder.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvLoadOrder_CellContentClick);
            this.dgvLoadOrder.SelectionChanged += new System.EventHandler(this.DgvLoadOrder_SelectionChanged);
            // 
            // ColumnID
            // 
            this.ColumnID.FillWeight = 25F;
            this.ColumnID.HeaderText = "ID";
            this.ColumnID.MinimumWidth = 25;
            this.ColumnID.Name = "ColumnID";
            this.ColumnID.ReadOnly = true;
            this.ColumnID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnID.ToolTipText = "The index of the custom map.";
            this.ColumnID.Width = 25;
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnName.HeaderText = "Map Name";
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            this.ColumnName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnName.ToolTipText = "The name of the custom map.";
            // 
            // ColumnUp
            // 
            this.ColumnUp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ColumnUp.FillWeight = 20F;
            this.ColumnUp.HeaderText = "U";
            this.ColumnUp.MinimumWidth = 20;
            this.ColumnUp.Name = "ColumnUp";
            this.ColumnUp.ReadOnly = true;
            this.ColumnUp.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnUp.Text = "";
            this.ColumnUp.ToolTipText = "Shift the level up one index.";
            this.ColumnUp.Width = 20;
            // 
            // ColumnDown
            // 
            this.ColumnDown.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ColumnDown.FillWeight = 20F;
            this.ColumnDown.HeaderText = "D";
            this.ColumnDown.MinimumWidth = 20;
            this.ColumnDown.Name = "ColumnDown";
            this.ColumnDown.ReadOnly = true;
            this.ColumnDown.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnDown.Text = "";
            this.ColumnDown.ToolTipText = "Shift the level down one index.";
            this.ColumnDown.Width = 20;
            // 
            // lbDescription
            // 
            this.lbDescription.Location = new System.Drawing.Point(13, 383);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(326, 56);
            this.lbDescription.TabIndex = 4;
            // 
            // dgvMods
            // 
            this.dgvMods.AllowUserToAddRows = false;
            this.dgvMods.AllowUserToDeleteRows = false;
            this.dgvMods.AllowUserToResizeColumns = false;
            this.dgvMods.AllowUserToResizeRows = false;
            this.dgvMods.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvMods.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvMods.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvMods.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvMods.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMods.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dgvMods.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMods.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dgvMods.Location = new System.Drawing.Point(3, 16);
            this.dgvMods.MultiSelect = false;
            this.dgvMods.Name = "dgvMods";
            this.dgvMods.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvMods.RowHeadersVisible = false;
            this.dgvMods.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvMods.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMods.Size = new System.Drawing.Size(321, 174);
            this.dgvMods.TabIndex = 1;
            this.dgvMods.SelectionChanged += new System.EventHandler(this.dgvMods_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "(none";
            this.dataGridViewTextBoxColumn1.FalseValue = "False";
            this.dataGridViewTextBoxColumn1.FillWeight = 20F;
            this.dataGridViewTextBoxColumn1.HeaderText = "E";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 20;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn1.ToolTipText = "Whether or not to install the mod.";
            this.dataGridViewTextBoxColumn1.TrueValue = "True";
            this.dataGridViewTextBoxColumn1.Width = 20;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "Mod Name";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.ToolTipText = "The name of the mod.";
            // 
            // Mods
            // 
            this.AcceptButton = this.btOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(351, 477);
            this.Controls.Add(this.lbDescription);
            this.Controls.Add(this.gbLoadOrder);
            this.Controls.Add(this.gbMods);
            this.Controls.Add(this.btOkay);
            this.Controls.Add(this.btCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Mods";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TY Mods";
            this.TopMost = true;
            this.gbMods.ResumeLayout(false);
            this.gbLoadOrder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLoadOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMods)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOkay;
        private System.Windows.Forms.GroupBox gbMods;
        private System.Windows.Forms.GroupBox gbLoadOrder;
        private System.Windows.Forms.Label lbDescription;
        private System.Windows.Forms.DataGridView dgvLoadOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnUp;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnDown;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.DataGridView dgvMods;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}