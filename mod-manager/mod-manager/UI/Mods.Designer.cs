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
            this.btCancel = new System.Windows.Forms.Button();
            this.btOkay = new System.Windows.Forms.Button();
            this.gbMods = new System.Windows.Forms.GroupBox();
            this.gbLoadOrder = new System.Windows.Forms.GroupBox();
            this.lbDescription = new System.Windows.Forms.Label();
            this.lbMods = new System.Windows.Forms.CheckedListBox();
            this.gbMods.SuspendLayout();
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
            this.gbMods.Controls.Add(this.lbMods);
            this.gbMods.Location = new System.Drawing.Point(12, 12);
            this.gbMods.Name = "gbMods";
            this.gbMods.Size = new System.Drawing.Size(327, 193);
            this.gbMods.TabIndex = 2;
            this.gbMods.TabStop = false;
            this.gbMods.Text = "Mods";
            // 
            // gbLoadOrder
            // 
            this.gbLoadOrder.Location = new System.Drawing.Point(12, 211);
            this.gbLoadOrder.Name = "gbLoadOrder";
            this.gbLoadOrder.Size = new System.Drawing.Size(327, 165);
            this.gbLoadOrder.TabIndex = 3;
            this.gbLoadOrder.TabStop = false;
            this.gbLoadOrder.Text = "Custom Map Import Order";
            // 
            // lbDescription
            // 
            this.lbDescription.Location = new System.Drawing.Point(13, 383);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(326, 56);
            this.lbDescription.TabIndex = 4;
            // 
            // lbMods
            // 
            this.lbMods.FormattingEnabled = true;
            this.lbMods.Location = new System.Drawing.Point(6, 19);
            this.lbMods.Name = "lbMods";
            this.lbMods.Size = new System.Drawing.Size(315, 169);
            this.lbMods.TabIndex = 0;
            this.lbMods.SelectedIndexChanged += new System.EventHandler(this.LbMods_SelectedIndexChanged);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOkay;
        private System.Windows.Forms.GroupBox gbMods;
        private System.Windows.Forms.GroupBox gbLoadOrder;
        private System.Windows.Forms.Label lbDescription;
        private System.Windows.Forms.CheckedListBox lbMods;
    }
}