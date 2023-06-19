namespace _4Gewinnt__nicht_
{
    partial class MainMenu
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
            this.StartButton = new System.Windows.Forms.Button();
            this.MainMenuGrid = new System.Windows.Forms.TableLayoutPanel();
            this.ExitButton = new System.Windows.Forms.Button();
            this.turtorialButton = new System.Windows.Forms.Button();
            this.MainMenuGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.StartButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StartButton.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StartButton.ForeColor = System.Drawing.SystemColors.Desktop;
            this.StartButton.Location = new System.Drawing.Point(430, 147);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(422, 149);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = false;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // MainMenuGrid
            // 
            this.MainMenuGrid.AutoSize = true;
            this.MainMenuGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainMenuGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MainMenuGrid.ColumnCount = 3;
            this.MainMenuGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.MainMenuGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.MainMenuGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.MainMenuGrid.Controls.Add(this.StartButton, 1, 1);
            this.MainMenuGrid.Controls.Add(this.ExitButton, 1, 3);
            this.MainMenuGrid.Controls.Add(this.turtorialButton, 1, 2);
            this.MainMenuGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainMenuGrid.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.MainMenuGrid.Location = new System.Drawing.Point(0, 0);
            this.MainMenuGrid.Name = "MainMenuGrid";
            this.MainMenuGrid.RowCount = 5;
            this.MainMenuGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.30502F));
            this.MainMenuGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20.69498F));
            this.MainMenuGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.MainMenuGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.MainMenuGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.MainMenuGrid.Size = new System.Drawing.Size(1284, 751);
            this.MainMenuGrid.TabIndex = 1;
            // 
            // ExitButton
            // 
            this.ExitButton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ExitButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExitButton.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ExitButton.ForeColor = System.Drawing.Color.Maroon;
            this.ExitButton.Location = new System.Drawing.Point(430, 452);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(422, 144);
            this.ExitButton.TabIndex = 2;
            this.ExitButton.Text = "Exit";
            this.ExitButton.UseVisualStyleBackColor = false;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // turtorialButton
            // 
            this.turtorialButton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.turtorialButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.turtorialButton.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.turtorialButton.Location = new System.Drawing.Point(430, 302);
            this.turtorialButton.Name = "turtorialButton";
            this.turtorialButton.Size = new System.Drawing.Size(422, 144);
            this.turtorialButton.TabIndex = 3;
            this.turtorialButton.Text = "\"Turtorial\"";
            this.turtorialButton.UseVisualStyleBackColor = false;
            this.turtorialButton.Click += new System.EventHandler(this.turtorialButton_Click);
            // 
            // MainMenu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1284, 751);
            this.Controls.Add(this.MainMenuGrid);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connect4";
            this.MainMenuGrid.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Button StartButton;
        private TableLayoutPanel MainMenuGrid;
        private Button ExitButton;
        private Button turtorialButton;
    }
}