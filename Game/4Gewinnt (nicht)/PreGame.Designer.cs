namespace _4Gewinnt__nicht_
{
    partial class PreGame
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
            this.MainMenuGrid = new System.Windows.Forms.TableLayoutPanel();
            this.StartButton = new System.Windows.Forms.Button();
            this.PlayerPanel = new System.Windows.Forms.Panel();
            this.Backbutton = new System.Windows.Forms.Button();
            this.chatPlane = new System.Windows.Forms.Panel();
            this.sendButton = new System.Windows.Forms.Button();
            this.chatTextBox = new System.Windows.Forms.TextBox();
            this.PlayerControlPanel = new System.Windows.Forms.Panel();
            this.portTextbox = new System.Windows.Forms.TextBox();
            this.ipTextbox = new System.Windows.Forms.TextBox();
            this.startServerButton = new System.Windows.Forms.Button();
            this.JoinButton = new System.Windows.Forms.Button();
            this.playerNameTextBox = new System.Windows.Forms.TextBox();
            this.serverNameLabel = new System.Windows.Forms.Label();
            this.AddPlayerButton = new System.Windows.Forms.Button();
            this.chatOutputTextBox = new System.Windows.Forms.TextBox();
            this.MainMenuGrid.SuspendLayout();
            this.chatPlane.SuspendLayout();
            this.PlayerControlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenuGrid
            // 
            this.MainMenuGrid.AutoSize = true;
            this.MainMenuGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainMenuGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MainMenuGrid.ColumnCount = 3;
            this.MainMenuGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.04361F));
            this.MainMenuGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.28661F));
            this.MainMenuGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.74766F));
            this.MainMenuGrid.Controls.Add(this.StartButton, 0, 4);
            this.MainMenuGrid.Controls.Add(this.PlayerPanel, 1, 2);
            this.MainMenuGrid.Controls.Add(this.Backbutton, 2, 4);
            this.MainMenuGrid.Controls.Add(this.chatPlane, 1, 4);
            this.MainMenuGrid.Controls.Add(this.PlayerControlPanel, 2, 2);
            this.MainMenuGrid.Controls.Add(this.chatOutputTextBox, 0, 2);
            this.MainMenuGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainMenuGrid.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.MainMenuGrid.Location = new System.Drawing.Point(0, 0);
            this.MainMenuGrid.Name = "MainMenuGrid";
            this.MainMenuGrid.RowCount = 5;
            this.MainMenuGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.258322F));
            this.MainMenuGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.125166F));
            this.MainMenuGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.84421F));
            this.MainMenuGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.471816F));
            this.MainMenuGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.92693F));
            this.MainMenuGrid.Size = new System.Drawing.Size(1765, 958);
            this.MainMenuGrid.TabIndex = 1;
            // 
            // StartButton
            // 
            this.StartButton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.StartButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StartButton.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StartButton.Location = new System.Drawing.Point(8, 822);
            this.StartButton.Margin = new System.Windows.Forms.Padding(8);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(266, 128);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = false;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // PlayerPanel
            // 
            this.PlayerPanel.AutoScroll = true;
            this.PlayerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlayerPanel.Location = new System.Drawing.Point(285, 120);
            this.PlayerPanel.Name = "PlayerPanel";
            this.PlayerPanel.Size = new System.Drawing.Size(1286, 630);
            this.PlayerPanel.TabIndex = 1;
            // 
            // Backbutton
            // 
            this.Backbutton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Backbutton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Backbutton.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Backbutton.ForeColor = System.Drawing.Color.Maroon;
            this.Backbutton.Location = new System.Drawing.Point(1582, 822);
            this.Backbutton.Margin = new System.Windows.Forms.Padding(8);
            this.Backbutton.Name = "Backbutton";
            this.Backbutton.Size = new System.Drawing.Size(175, 128);
            this.Backbutton.TabIndex = 3;
            this.Backbutton.Text = "Back";
            this.Backbutton.UseVisualStyleBackColor = false;
            this.Backbutton.Click += new System.EventHandler(this.Backbutton_Click);
            // 
            // chatPlane
            // 
            this.chatPlane.Controls.Add(this.sendButton);
            this.chatPlane.Controls.Add(this.chatTextBox);
            this.chatPlane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatPlane.Location = new System.Drawing.Point(285, 817);
            this.chatPlane.Name = "chatPlane";
            this.chatPlane.Size = new System.Drawing.Size(1286, 138);
            this.chatPlane.TabIndex = 4;
            // 
            // sendButton
            // 
            this.sendButton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.sendButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.sendButton.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.sendButton.Location = new System.Drawing.Point(1138, 0);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(148, 107);
            this.sendButton.TabIndex = 1;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = false;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // chatTextBox
            // 
            this.chatTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.chatTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.chatTextBox.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chatTextBox.Location = new System.Drawing.Point(0, 107);
            this.chatTextBox.Name = "chatTextBox";
            this.chatTextBox.Size = new System.Drawing.Size(1286, 31);
            this.chatTextBox.TabIndex = 0;
            this.chatTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ChatTextBox_KeyPress);
            // 
            // PlayerControlPanel
            // 
            this.PlayerControlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PlayerControlPanel.Controls.Add(this.portTextbox);
            this.PlayerControlPanel.Controls.Add(this.ipTextbox);
            this.PlayerControlPanel.Controls.Add(this.startServerButton);
            this.PlayerControlPanel.Controls.Add(this.JoinButton);
            this.PlayerControlPanel.Controls.Add(this.playerNameTextBox);
            this.PlayerControlPanel.Controls.Add(this.serverNameLabel);
            this.PlayerControlPanel.Controls.Add(this.AddPlayerButton);
            this.PlayerControlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PlayerControlPanel.Location = new System.Drawing.Point(1577, 120);
            this.PlayerControlPanel.Name = "PlayerControlPanel";
            this.PlayerControlPanel.Size = new System.Drawing.Size(185, 630);
            this.PlayerControlPanel.TabIndex = 6;
            // 
            // portTextbox
            // 
            this.portTextbox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.portTextbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.portTextbox.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.portTextbox.Location = new System.Drawing.Point(0, 305);
            this.portTextbox.Name = "portTextbox";
            this.portTextbox.Size = new System.Drawing.Size(185, 30);
            this.portTextbox.TabIndex = 7;
            this.portTextbox.Text = "Port";
            // 
            // ipTextbox
            // 
            this.ipTextbox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ipTextbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.ipTextbox.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ipTextbox.Location = new System.Drawing.Point(0, 275);
            this.ipTextbox.Name = "ipTextbox";
            this.ipTextbox.Size = new System.Drawing.Size(185, 30);
            this.ipTextbox.TabIndex = 6;
            this.ipTextbox.Text = "IP";
            // 
            // startServerButton
            // 
            this.startServerButton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.startServerButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.startServerButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.startServerButton.Location = new System.Drawing.Point(0, 204);
            this.startServerButton.Name = "startServerButton";
            this.startServerButton.Size = new System.Drawing.Size(185, 71);
            this.startServerButton.TabIndex = 5;
            this.startServerButton.Text = "Start Server";
            this.startServerButton.UseVisualStyleBackColor = false;
            this.startServerButton.Click += new System.EventHandler(this.StartServerButton_Click);
            // 
            // JoinButton
            // 
            this.JoinButton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.JoinButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.JoinButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.JoinButton.Location = new System.Drawing.Point(0, 138);
            this.JoinButton.Name = "JoinButton";
            this.JoinButton.Size = new System.Drawing.Size(185, 66);
            this.JoinButton.TabIndex = 4;
            this.JoinButton.Text = "Join";
            this.JoinButton.UseVisualStyleBackColor = false;
            this.JoinButton.Click += new System.EventHandler(this.JoinButton_Click);
            // 
            // playerNameTextBox
            // 
            this.playerNameTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.playerNameTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.playerNameTextBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.playerNameTextBox.Location = new System.Drawing.Point(0, 104);
            this.playerNameTextBox.Name = "playerNameTextBox";
            this.playerNameTextBox.Size = new System.Drawing.Size(185, 34);
            this.playerNameTextBox.TabIndex = 3;
            this.playerNameTextBox.Text = "Name";
            // 
            // serverNameLabel
            // 
            this.serverNameLabel.AutoSize = true;
            this.serverNameLabel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.serverNameLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.serverNameLabel.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.serverNameLabel.Location = new System.Drawing.Point(0, 81);
            this.serverNameLabel.Name = "serverNameLabel";
            this.serverNameLabel.Size = new System.Drawing.Size(115, 23);
            this.serverNameLabel.TabIndex = 8;
            this.serverNameLabel.Text = "Online Name:";
            // 
            // AddPlayerButton
            // 
            this.AddPlayerButton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.AddPlayerButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.AddPlayerButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.AddPlayerButton.Location = new System.Drawing.Point(0, 0);
            this.AddPlayerButton.Name = "AddPlayerButton";
            this.AddPlayerButton.Size = new System.Drawing.Size(185, 81);
            this.AddPlayerButton.TabIndex = 2;
            this.AddPlayerButton.Text = "AddPlayer";
            this.AddPlayerButton.UseVisualStyleBackColor = false;
            this.AddPlayerButton.Click += new System.EventHandler(this.AddPlayerButton_Click);
            // 
            // chatOutputTextBox
            // 
            this.chatOutputTextBox.AcceptsReturn = true;
            this.chatOutputTextBox.AcceptsTab = true;
            this.chatOutputTextBox.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.chatOutputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chatOutputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatOutputTextBox.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chatOutputTextBox.Location = new System.Drawing.Point(3, 120);
            this.chatOutputTextBox.Multiline = true;
            this.chatOutputTextBox.Name = "chatOutputTextBox";
            this.chatOutputTextBox.ReadOnly = true;
            this.chatOutputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.chatOutputTextBox.Size = new System.Drawing.Size(276, 630);
            this.chatOutputTextBox.TabIndex = 7;
            // 
            // PreGame
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1765, 958);
            this.Controls.Add(this.MainMenuGrid);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PreGame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connect4";
            this.Load += new System.EventHandler(this.PreGame_Load);
            this.MainMenuGrid.ResumeLayout(false);
            this.MainMenuGrid.PerformLayout();
            this.chatPlane.ResumeLayout(false);
            this.chatPlane.PerformLayout();
            this.PlayerControlPanel.ResumeLayout(false);
            this.PlayerControlPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TableLayoutPanel MainMenuGrid;
        private Button StartButton;
        public Panel PlayerPanel;
        private Button AddPlayerButton;
        private Button Backbutton;
        private Panel chatPlane;
        private Button sendButton;
        private TextBox chatTextBox;
        private Panel PlayerControlPanel;
        private Button JoinButton;
        private TextBox playerNameTextBox;
        public TextBox chatOutputTextBox;
        private Button startServerButton;
        private TextBox portTextbox;
        private TextBox ipTextbox;
        private Label serverNameLabel;
    }
}