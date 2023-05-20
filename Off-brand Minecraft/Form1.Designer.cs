namespace Off_brand_Minecraft
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tickSpeed = new System.Windows.Forms.Timer(this.components);
            this.lblPlayerAngle = new System.Windows.Forms.Label();
            this.lblPickedBlockAmountIndicator = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.pbxLogo = new System.Windows.Forms.PictureBox();
            this.btnChange = new System.Windows.Forms.Button();
            this.lbxControls = new System.Windows.Forms.ListBox();
            this.btnControls = new System.Windows.Forms.Button();
            this.cbxInvertY = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // tickSpeed
            // 
            this.tickSpeed.Interval = 1;
            this.tickSpeed.Tick += new System.EventHandler(this.tickSpeed_Tick);
            // 
            // lblPlayerAngle
            // 
            this.lblPlayerAngle.AutoSize = true;
            this.lblPlayerAngle.Location = new System.Drawing.Point(0, 0);
            this.lblPlayerAngle.Name = "lblPlayerAngle";
            this.lblPlayerAngle.Size = new System.Drawing.Size(0, 25);
            this.lblPlayerAngle.TabIndex = 0;
            // 
            // lblPickedBlockAmountIndicator
            // 
            this.lblPickedBlockAmountIndicator.AutoSize = true;
            this.lblPickedBlockAmountIndicator.Location = new System.Drawing.Point(585, 95);
            this.lblPickedBlockAmountIndicator.Name = "lblPickedBlockAmountIndicator";
            this.lblPickedBlockAmountIndicator.Size = new System.Drawing.Size(0, 25);
            this.lblPickedBlockAmountIndicator.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Swis721 Blk BT", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button1.Location = new System.Drawing.Point(52, 95);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(324, 61);
            this.button1.TabIndex = 2;
            this.button1.Text = "Save and exit";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pbxLogo
            // 
            this.pbxLogo.Image = global::Off_brand_Minecraft.Properties.Resources.Mine;
            this.pbxLogo.Location = new System.Drawing.Point(382, 12);
            this.pbxLogo.Name = "pbxLogo";
            this.pbxLogo.Size = new System.Drawing.Size(391, 144);
            this.pbxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxLogo.TabIndex = 3;
            this.pbxLogo.TabStop = false;
            // 
            // btnChange
            // 
            this.btnChange.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnChange.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnChange.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChange.Font = new System.Drawing.Font("Swis721 Blk BT", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnChange.Location = new System.Drawing.Point(52, 199);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(324, 61);
            this.btnChange.TabIndex = 4;
            this.btnChange.Text = "Change background";
            this.btnChange.UseVisualStyleBackColor = false;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // lbxControls
            // 
            this.lbxControls.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lbxControls.Font = new System.Drawing.Font("Swis721 Blk BT", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lbxControls.FormattingEnabled = true;
            this.lbxControls.ItemHeight = 21;
            this.lbxControls.Items.AddRange(new object[] {
            "Controls:",
            "W = Walk forwards",
            "S = Walk backwards",
            "A = Strafe left",
            "D = Strafe right",
            "Shift = Run",
            "Space = Jump",
            "G = toggle graphics quality",
            "Right-Arrow = Decrease render distance",
            "Left-Arrow = Increase render distance",
            "E = Inventory",
            "ESC = Pause/Exit menu/Close game from main menu",
            "Right mouse button = destroy block / pick up half of the content in an inventory " +
                "slot",
            "Left mouse button = place block / pick up the content in an inventory slot"});
            this.lbxControls.Location = new System.Drawing.Point(382, 187);
            this.lbxControls.Name = "lbxControls";
            this.lbxControls.Size = new System.Drawing.Size(857, 340);
            this.lbxControls.TabIndex = 5;
            // 
            // btnControls
            // 
            this.btnControls.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnControls.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnControls.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnControls.Font = new System.Drawing.Font("Swis721 Blk BT", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnControls.Location = new System.Drawing.Point(52, 266);
            this.btnControls.Name = "btnControls";
            this.btnControls.Size = new System.Drawing.Size(324, 61);
            this.btnControls.TabIndex = 6;
            this.btnControls.Text = "Controls";
            this.btnControls.UseVisualStyleBackColor = false;
            this.btnControls.Click += new System.EventHandler(this.btnControls_Click);
            // 
            // cbxInvertY
            // 
            this.cbxInvertY.AutoSize = true;
            this.cbxInvertY.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.cbxInvertY.Location = new System.Drawing.Point(622, 231);
            this.cbxInvertY.Name = "cbxInvertY";
            this.cbxInvertY.Size = new System.Drawing.Size(134, 29);
            this.cbxInvertY.TabIndex = 7;
            this.cbxInvertY.Text = "Invert Y-axis";
            this.cbxInvertY.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cbxInvertY);
            this.Controls.Add(this.btnControls);
            this.Controls.Add(this.lbxControls);
            this.Controls.Add(this.btnChange);
            this.Controls.Add(this.pbxLogo);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblPickedBlockAmountIndicator);
            this.Controls.Add(this.lblPlayerAngle);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Shown += new System.EventHandler(this.LoadMenu);
            this.Click += new System.EventHandler(this.tickSpeed_Tick);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDownEvent);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer tickSpeed;
        private Label lblPlayerAngle;
        private Label lblPickedBlockAmountIndicator;
        private Button button1;
        private PictureBox pbxLogo;
        private Button btnChange;
        private ListBox lbxControls;
        private Button btnControls;
        private CheckBox cbxInvertY;
    }
}