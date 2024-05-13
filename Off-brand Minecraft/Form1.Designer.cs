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
            components = new System.ComponentModel.Container();
            tickSpeed = new System.Windows.Forms.Timer(components);
            lblPlayerAngle = new Label();
            lblPickedBlockAmountIndicator = new Label();
            button1 = new Button();
            pbxLogo = new PictureBox();
            btnChange = new Button();
            lbxControls = new ListBox();
            btnControls = new Button();
            cbxInvertY = new CheckBox();
            lightupdates = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pbxLogo).BeginInit();
            SuspendLayout();
            // 
            // tickSpeed
            // 
            tickSpeed.Interval = 10;
            tickSpeed.Tick += tickSpeed_Tick;
            // 
            // lblPlayerAngle
            // 
            lblPlayerAngle.AutoSize = true;
            lblPlayerAngle.Location = new Point(0, 0);
            lblPlayerAngle.Name = "lblPlayerAngle";
            lblPlayerAngle.Size = new Size(0, 25);
            lblPlayerAngle.TabIndex = 0;
            // 
            // lblPickedBlockAmountIndicator
            // 
            lblPickedBlockAmountIndicator.AutoSize = true;
            lblPickedBlockAmountIndicator.Location = new Point(585, 95);
            lblPickedBlockAmountIndicator.Name = "lblPickedBlockAmountIndicator";
            lblPickedBlockAmountIndicator.Size = new Size(0, 25);
            lblPickedBlockAmountIndicator.TabIndex = 1;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ActiveCaption;
            button1.BackgroundImageLayout = ImageLayout.Zoom;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Swis721 Blk BT", 12F, FontStyle.Regular, GraphicsUnit.Point);
            button1.Location = new Point(52, 95);
            button1.Name = "button1";
            button1.Size = new Size(324, 61);
            button1.TabIndex = 2;
            button1.Text = "Save and exit";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // pbxLogo
            // 
            pbxLogo.Image = Properties.Resources.Mine;
            pbxLogo.Location = new Point(382, 12);
            pbxLogo.Name = "pbxLogo";
            pbxLogo.Size = new Size(391, 144);
            pbxLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            pbxLogo.TabIndex = 3;
            pbxLogo.TabStop = false;
            // 
            // btnChange
            // 
            btnChange.BackColor = SystemColors.ActiveCaption;
            btnChange.BackgroundImageLayout = ImageLayout.Zoom;
            btnChange.FlatStyle = FlatStyle.Flat;
            btnChange.Font = new Font("Swis721 Blk BT", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnChange.Location = new Point(52, 199);
            btnChange.Name = "btnChange";
            btnChange.Size = new Size(324, 61);
            btnChange.TabIndex = 4;
            btnChange.Text = "Change background";
            btnChange.UseVisualStyleBackColor = false;
            btnChange.Click += btnChange_Click;
            // 
            // lbxControls
            // 
            lbxControls.BackColor = SystemColors.ActiveCaption;
            lbxControls.Font = new Font("Swis721 Blk BT", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lbxControls.FormattingEnabled = true;
            lbxControls.ItemHeight = 21;
            lbxControls.Items.AddRange(new object[] { "Controls:", "W = Walk forwards", "S = Walk backwards", "A = Strafe left", "D = Strafe right", "Shift = Run", "Space = Jump", "G = toggle graphics quality", "Right-Arrow = Decrease render distance", "Left-Arrow = Increase render distance", "E = Inventory", "ESC = Pause/Exit menu/Close game from main menu", "Right mouse button = destroy block / pick up half of the content in an inventory slot", "Left mouse button = place block / pick up the content in an inventory slot" });
            lbxControls.Location = new Point(382, 187);
            lbxControls.Name = "lbxControls";
            lbxControls.Size = new Size(857, 340);
            lbxControls.TabIndex = 5;
            // 
            // btnControls
            // 
            btnControls.BackColor = SystemColors.ActiveCaption;
            btnControls.BackgroundImageLayout = ImageLayout.Zoom;
            btnControls.FlatStyle = FlatStyle.Flat;
            btnControls.Font = new Font("Swis721 Blk BT", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnControls.Location = new Point(52, 266);
            btnControls.Name = "btnControls";
            btnControls.Size = new Size(324, 61);
            btnControls.TabIndex = 6;
            btnControls.Text = "Controls";
            btnControls.UseVisualStyleBackColor = false;
            btnControls.Click += btnControls_Click;
            // 
            // cbxInvertY
            // 
            cbxInvertY.AutoSize = true;
            cbxInvertY.BackColor = SystemColors.ActiveCaption;
            cbxInvertY.Location = new Point(622, 231);
            cbxInvertY.Name = "cbxInvertY";
            cbxInvertY.Size = new Size(134, 29);
            cbxInvertY.TabIndex = 7;
            cbxInvertY.Text = "Invert Y-axis";
            cbxInvertY.UseVisualStyleBackColor = false;
            // 
            // lightupdates
            // 
            lightupdates.Interval = 1000;
            lightupdates.Tick += lightupdates_Tick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DeepSkyBlue;
            ClientSize = new Size(800, 450);
            Controls.Add(cbxInvertY);
            Controls.Add(btnControls);
            Controls.Add(lbxControls);
            Controls.Add(btnChange);
            Controls.Add(pbxLogo);
            Controls.Add(button1);
            Controls.Add(lblPickedBlockAmountIndicator);
            Controls.Add(lblPlayerAngle);
            DoubleBuffered = true;
            KeyPreview = true;
            Name = "Form1";
            Text = "Form1";
            Shown += LoadMenu;
            Click += tickSpeed_Tick;
            KeyDown += KeyDownEvent;
            KeyUp += Form1_KeyUp;
            MouseClick += Form1_MouseClick;
            MouseDown += Form1_MouseDown;
            MouseUp += Form1_MouseUp;
            ((System.ComponentModel.ISupportInitialize)pbxLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.Timer lightupdates;
    }
}