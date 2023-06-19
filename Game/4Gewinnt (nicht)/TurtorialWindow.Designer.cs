namespace _4Gewinnt__nicht_
{
    partial class TurtorialWindow
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
            this.MainMenuPanel = new System.Windows.Forms.Panel();
            this.backgroundPanel = new System.Windows.Forms.TableLayoutPanel();
            this.mainTurtorialPanel = new System.Windows.Forms.TableLayoutPanel();
            this.turtorialControlPanel = new System.Windows.Forms.Panel();
            this.previousButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.turtorialPicturebox = new System.Windows.Forms.PictureBox();
            this.BackButtonPanel = new System.Windows.Forms.Panel();
            this.backButton = new System.Windows.Forms.Button();
            this.MainMenuPanel.SuspendLayout();
            this.backgroundPanel.SuspendLayout();
            this.mainTurtorialPanel.SuspendLayout();
            this.turtorialControlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.turtorialPicturebox)).BeginInit();
            this.BackButtonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenuPanel
            // 
            this.MainMenuPanel.Controls.Add(this.backgroundPanel);
            this.MainMenuPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainMenuPanel.Location = new System.Drawing.Point(0, 0);
            this.MainMenuPanel.Name = "MainMenuPanel";
            this.MainMenuPanel.Size = new System.Drawing.Size(1284, 751);
            this.MainMenuPanel.TabIndex = 0;
            // 
            // backgroundPanel
            // 
            this.backgroundPanel.BackgroundImage = global::_4Gewinnt__nicht_.Properties.Resources.super;
            this.backgroundPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.backgroundPanel.ColumnCount = 2;
            this.backgroundPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.backgroundPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.backgroundPanel.Controls.Add(this.mainTurtorialPanel, 0, 1);
            this.backgroundPanel.Controls.Add(this.BackButtonPanel, 1, 1);
            this.backgroundPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundPanel.Location = new System.Drawing.Point(0, 0);
            this.backgroundPanel.Name = "backgroundPanel";
            this.backgroundPanel.RowCount = 2;
            this.backgroundPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.61518F));
            this.backgroundPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 61.38482F));
            this.backgroundPanel.Size = new System.Drawing.Size(1284, 751);
            this.backgroundPanel.TabIndex = 0;
            // 
            // mainTurtorialPanel
            // 
            this.mainTurtorialPanel.BackColor = System.Drawing.Color.Transparent;
            this.mainTurtorialPanel.ColumnCount = 1;
            this.mainTurtorialPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTurtorialPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainTurtorialPanel.Controls.Add(this.turtorialControlPanel, 0, 0);
            this.mainTurtorialPanel.Controls.Add(this.turtorialPicturebox, 0, 1);
            this.mainTurtorialPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTurtorialPanel.Location = new System.Drawing.Point(3, 293);
            this.mainTurtorialPanel.Name = "mainTurtorialPanel";
            this.mainTurtorialPanel.RowCount = 2;
            this.mainTurtorialPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.92308F));
            this.mainTurtorialPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 83.07692F));
            this.mainTurtorialPanel.Size = new System.Drawing.Size(636, 455);
            this.mainTurtorialPanel.TabIndex = 0;
            // 
            // turtorialControlPanel
            // 
            this.turtorialControlPanel.BackColor = System.Drawing.Color.Transparent;
            this.turtorialControlPanel.Controls.Add(this.previousButton);
            this.turtorialControlPanel.Controls.Add(this.nextButton);
            this.turtorialControlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.turtorialControlPanel.Location = new System.Drawing.Point(3, 3);
            this.turtorialControlPanel.Name = "turtorialControlPanel";
            this.turtorialControlPanel.Size = new System.Drawing.Size(630, 71);
            this.turtorialControlPanel.TabIndex = 0;
            // 
            // previousButton
            // 
            this.previousButton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.previousButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.previousButton.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.previousButton.Location = new System.Drawing.Point(0, 0);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(159, 71);
            this.previousButton.TabIndex = 1;
            this.previousButton.Text = "Previous";
            this.previousButton.UseVisualStyleBackColor = false;
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.nextButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.nextButton.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.nextButton.Location = new System.Drawing.Point(459, 0);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(171, 71);
            this.nextButton.TabIndex = 0;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = false;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // turtorialPicturebox
            // 
            this.turtorialPicturebox.BackColor = System.Drawing.Color.Transparent;
            this.turtorialPicturebox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.turtorialPicturebox.Location = new System.Drawing.Point(3, 80);
            this.turtorialPicturebox.Name = "turtorialPicturebox";
            this.turtorialPicturebox.Size = new System.Drawing.Size(630, 372);
            this.turtorialPicturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.turtorialPicturebox.TabIndex = 1;
            this.turtorialPicturebox.TabStop = false;
            // 
            // BackButtonPanel
            // 
            this.BackButtonPanel.BackColor = System.Drawing.Color.Transparent;
            this.BackButtonPanel.Controls.Add(this.backButton);
            this.BackButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BackButtonPanel.Location = new System.Drawing.Point(645, 293);
            this.BackButtonPanel.Name = "BackButtonPanel";
            this.BackButtonPanel.Size = new System.Drawing.Size(636, 455);
            this.BackButtonPanel.TabIndex = 1;
            // 
            // backButton
            // 
            this.backButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.backButton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.backButton.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.backButton.ForeColor = System.Drawing.Color.Maroon;
            this.backButton.Location = new System.Drawing.Point(503, 394);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(130, 58);
            this.backButton.TabIndex = 0;
            this.backButton.Text = "Back";
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new System.EventHandler(this.backButton_Click_1);
            // 
            // TurtorialWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1284, 751);
            this.Controls.Add(this.MainMenuPanel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TurtorialWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connect4";
            this.MainMenuPanel.ResumeLayout(false);
            this.backgroundPanel.ResumeLayout(false);
            this.mainTurtorialPanel.ResumeLayout(false);
            this.turtorialControlPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.turtorialPicturebox)).EndInit();
            this.BackButtonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel mainTurtorialPanel;
        private Panel MainMenuPanel;
        private TableLayoutPanel backgroundPanel;
        private Panel turtorialControlPanel;
        private PictureBox turtorialPicturebox;
        private Button previousButton;
        private Button nextButton;
        private Panel BackButtonPanel;
        private Button backButton;
    }
}