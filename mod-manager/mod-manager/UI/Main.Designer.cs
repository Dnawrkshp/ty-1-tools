namespace TyModManager.UI
{
    partial class Main
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
            this.lbPlay = new TyModManager.UI.ButtonLabel();
            this.lbMods = new TyModManager.UI.ButtonLabel();
            this.lbExit = new TyModManager.UI.ButtonLabel();
            this.lbTest = new TyModManager.UI.ButtonLabel();
            this.buttonLink1 = new TyModManager.UI.ButtonLink();
            this.lbOptions = new TyModManager.UI.ButtonLabel();
            this.lbLog = new TyModManager.UI.LogLabel();
            this.SuspendLayout();
            // 
            // lbPlay
            // 
            this.lbPlay.BackColor = System.Drawing.Color.Transparent;
            this.lbPlay.ClickColor = System.Drawing.Color.White;
            this.lbPlay.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPlay.ForeColor = System.Drawing.SystemColors.Control;
            this.lbPlay.HoverColor = System.Drawing.Color.Gray;
            this.lbPlay.Location = new System.Drawing.Point(510, 10);
            this.lbPlay.Name = "lbPlay";
            this.lbPlay.Size = new System.Drawing.Size(78, 31);
            this.lbPlay.TabIndex = 1;
            this.lbPlay.Text = "PLAY";
            this.lbPlay.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbMods
            // 
            this.lbMods.BackColor = System.Drawing.Color.Transparent;
            this.lbMods.ClickColor = System.Drawing.Color.White;
            this.lbMods.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMods.ForeColor = System.Drawing.SystemColors.Control;
            this.lbMods.HoverColor = System.Drawing.Color.Gray;
            this.lbMods.Location = new System.Drawing.Point(497, 90);
            this.lbMods.Name = "lbMods";
            this.lbMods.Size = new System.Drawing.Size(91, 31);
            this.lbMods.TabIndex = 2;
            this.lbMods.Text = "MODS";
            this.lbMods.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbExit
            // 
            this.lbExit.BackColor = System.Drawing.Color.Transparent;
            this.lbExit.ClickColor = System.Drawing.Color.White;
            this.lbExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbExit.ForeColor = System.Drawing.SystemColors.Control;
            this.lbExit.HoverColor = System.Drawing.Color.Gray;
            this.lbExit.Location = new System.Drawing.Point(517, 170);
            this.lbExit.Name = "lbExit";
            this.lbExit.Size = new System.Drawing.Size(71, 31);
            this.lbExit.TabIndex = 3;
            this.lbExit.Text = "EXIT";
            this.lbExit.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbTest
            // 
            this.lbTest.BackColor = System.Drawing.Color.Transparent;
            this.lbTest.ClickColor = System.Drawing.Color.White;
            this.lbTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTest.ForeColor = System.Drawing.SystemColors.Control;
            this.lbTest.HoverColor = System.Drawing.Color.Gray;
            this.lbTest.Location = new System.Drawing.Point(506, 50);
            this.lbTest.Name = "lbTest";
            this.lbTest.Size = new System.Drawing.Size(82, 31);
            this.lbTest.TabIndex = 4;
            this.lbTest.Text = "TEST";
            this.lbTest.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // buttonLink1
            // 
            this.buttonLink1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonLink1.ForeColor = System.Drawing.Color.LightGray;
            this.buttonLink1.Image = global::TyModManager.Properties.Resources.GitHub_Mark_Light_32px;
            this.buttonLink1.Location = new System.Drawing.Point(556, 339);
            this.buttonLink1.Name = "buttonLink1";
            this.buttonLink1.Size = new System.Drawing.Size(32, 32);
            this.buttonLink1.TabIndex = 5;
            this.buttonLink1.Text = "https://github.com/Dnawrkshp/ty-1-tools";
            // 
            // lbOptions
            // 
            this.lbOptions.BackColor = System.Drawing.Color.Transparent;
            this.lbOptions.ClickColor = System.Drawing.Color.White;
            this.lbOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbOptions.ForeColor = System.Drawing.SystemColors.Control;
            this.lbOptions.HoverColor = System.Drawing.Color.Gray;
            this.lbOptions.Location = new System.Drawing.Point(454, 130);
            this.lbOptions.Name = "lbOptions";
            this.lbOptions.Size = new System.Drawing.Size(134, 31);
            this.lbOptions.TabIndex = 6;
            this.lbOptions.Text = "OPTIONS";
            this.lbOptions.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lbLog
            // 
            this.lbLog.BackColor = System.Drawing.Color.Transparent;
            this.lbLog.Location = new System.Drawing.Point(12, 225);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(502, 146);
            this.lbLog.TabIndex = 7;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(600, 380);
            this.ControlBox = false;
            this.Controls.Add(this.lbLog);
            this.Controls.Add(this.lbOptions);
            this.Controls.Add(this.buttonLink1);
            this.Controls.Add(this.lbTest);
            this.Controls.Add(this.lbExit);
            this.Controls.Add(this.lbMods);
            this.Controls.Add(this.lbPlay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TY Mod Manager";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Main_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Main_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Main_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
        private ButtonLabel lbPlay;
        private ButtonLabel lbMods;
        private ButtonLabel lbExit;
        private ButtonLabel lbTest;
        private ButtonLink buttonLink1;
        private ButtonLabel lbOptions;
        private LogLabel lbLog;
    }
}

