﻿using System.Windows.Forms;
namespace Dragon_Audio_Player 
{
    partial class Form1 : Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlPlaying = new System.Windows.Forms.Panel();
            this.splitterPlaying = new System.Windows.Forms.Splitter();
            this.tbarPlaying = new System.Windows.Forms.TrackBar();
            this.tbarVolume = new System.Windows.Forms.TrackBar();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.miFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.miFileAddFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileAddFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.miPlaylist = new System.Windows.Forms.ToolStripDropDownButton();
            this.miPlaylistNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tbxmiPlaylistNew = new System.Windows.Forms.ToolStripTextBox();
            this.miPlaylistNewCreate = new System.Windows.Forms.ToolStripMenuItem();
            this.miPlaylistSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.cbxmiPlaylistSelect = new System.Windows.Forms.ToolStripComboBox();
            this.miSelectedPlaylist = new System.Windows.Forms.ToolStripMenuItem();
            this.miSelectPlaylistSync = new System.Windows.Forms.ToolStripMenuItem();
            this.cbxmiPlaylistSPSync = new System.Windows.Forms.ToolStripComboBox();
            this.miPlaylistsSPAutoReset = new System.Windows.Forms.ToolStripMenuItem();
            this.cbxmiPlaylistSPAutoReset = new System.Windows.Forms.ToolStripComboBox();
            this.miSelectedPlaylistResetTimesPlayed = new System.Windows.Forms.ToolStripMenuItem();
            this.miPreferences = new System.Windows.Forms.ToolStripDropDownButton();
            this.playingModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbxmiPreferencesMode = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cbmiPreferencesWriteToFile = new System.Windows.Forms.ToolStripComboBox();
            this.tbxmiPreferencesWTFLocation = new System.Windows.Forms.ToolStripTextBox();
            this.miHelp = new System.Windows.Forms.ToolStripDropDownButton();
            this.miHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.miPlay = new System.Windows.Forms.ToolStripButton();
            this.miPause = new System.Windows.Forms.ToolStripButton();
            this.miStop = new System.Windows.Forms.ToolStripButton();
            this.miPrevious = new System.Windows.Forms.ToolStripButton();
            this.miNext = new System.Windows.Forms.ToolStripButton();
            this.dgridSongs = new System.Windows.Forms.DataGridView();
            this.cxmsGrid = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxmiPlaycount = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxmitbxPlaycount = new System.Windows.Forms.ToolStripTextBox();
            this.ctxmibtnPlaycountOK = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxmibtnDeleteFromPlaylist = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1s = new System.Windows.Forms.Timer(this.components);
            this.clmTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmArtist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmAlbum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmTimesPlayed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlPlaying.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbarPlaying)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarVolume)).BeginInit();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgridSongs)).BeginInit();
            this.cxmsGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPlaying
            // 
            this.pnlPlaying.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPlaying.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlPlaying.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPlaying.Controls.Add(this.splitterPlaying);
            this.pnlPlaying.Controls.Add(this.tbarPlaying);
            this.pnlPlaying.Controls.Add(this.tbarVolume);
            this.pnlPlaying.Location = new System.Drawing.Point(0, 28);
            this.pnlPlaying.Name = "pnlPlaying";
            this.pnlPlaying.Size = new System.Drawing.Size(597, 34);
            this.pnlPlaying.TabIndex = 2;
            // 
            // splitterPlaying
            // 
            this.splitterPlaying.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.splitterPlaying.Location = new System.Drawing.Point(109, 0);
            this.splitterPlaying.Margin = new System.Windows.Forms.Padding(0);
            this.splitterPlaying.Name = "splitterPlaying";
            this.splitterPlaying.Size = new System.Drawing.Size(5, 32);
            this.splitterPlaying.TabIndex = 2;
            this.splitterPlaying.TabStop = false;
            // 
            // tbarPlaying
            // 
            this.tbarPlaying.BackColor = System.Drawing.Color.White;
            this.tbarPlaying.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbarPlaying.Location = new System.Drawing.Point(109, 0);
            this.tbarPlaying.Name = "tbarPlaying";
            this.tbarPlaying.Size = new System.Drawing.Size(486, 32);
            this.tbarPlaying.TabIndex = 1;
            this.tbarPlaying.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbarPlaying.ValueChanged += new System.EventHandler(this.tbarPlaying_ValueChanged);
            this.tbarPlaying.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbarPlaying_KeyDown);
            this.tbarPlaying.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbarPlaying_KeyUp);
            this.tbarPlaying.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbarPlaying_MouseDown);
            this.tbarPlaying.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tbarPlaying_MouseUp);
            // 
            // tbarVolume
            // 
            this.tbarVolume.BackColor = System.Drawing.Color.White;
            this.tbarVolume.Dock = System.Windows.Forms.DockStyle.Left;
            this.tbarVolume.Location = new System.Drawing.Point(0, 0);
            this.tbarVolume.Maximum = 100;
            this.tbarVolume.Name = "tbarVolume";
            this.tbarVolume.Size = new System.Drawing.Size(109, 32);
            this.tbarVolume.TabIndex = 0;
            this.tbarVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbarVolume.Value = 50;
            this.tbarVolume.Scroll += new System.EventHandler(this.tbarVolume_Scroll);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miPlaylist,
            this.miPreferences,
            this.miHelp,
            this.toolStripSeparator1,
            this.miPlay,
            this.miPause,
            this.miStop,
            this.miPrevious,
            this.miNext});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(597, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // miFile
            // 
            this.miFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileAddFolder,
            this.miFileAddFiles,
            this.miFileExit});
            this.miFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miFile.Name = "miFile";
            this.miFile.Size = new System.Drawing.Size(38, 22);
            this.miFile.Text = "File";
            // 
            // miFileAddFolder
            // 
            this.miFileAddFolder.Name = "miFileAddFolder";
            this.miFileAddFolder.Size = new System.Drawing.Size(147, 22);
            this.miFileAddFolder.Text = "Add Directory";
            this.miFileAddFolder.ToolTipText = "Setting Sync to \"Yes\" means you won\'t have to add songs manually anywhere. You ca" +
    "n just place them in the  selected directory and they will be added automaticall" +
    "y.";
            this.miFileAddFolder.Click += new System.EventHandler(this.miFileAddFolder_Click);
            // 
            // miFileAddFiles
            // 
            this.miFileAddFiles.Name = "miFileAddFiles";
            this.miFileAddFiles.Size = new System.Drawing.Size(147, 22);
            this.miFileAddFiles.Text = "Add Files";
            this.miFileAddFiles.Click += new System.EventHandler(this.miFileAddFiles_Click);
            // 
            // miFileExit
            // 
            this.miFileExit.Name = "miFileExit";
            this.miFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.miFileExit.Size = new System.Drawing.Size(147, 22);
            this.miFileExit.Text = "Exit";
            this.miFileExit.Click += new System.EventHandler(this.miFileExit_Click);
            // 
            // miPlaylist
            // 
            this.miPlaylist.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miPlaylist.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miPlaylistNew,
            this.miPlaylistSelect,
            this.miSelectedPlaylist});
            this.miPlaylist.Image = ((System.Drawing.Image)(resources.GetObject("miPlaylist.Image")));
            this.miPlaylist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miPlaylist.Name = "miPlaylist";
            this.miPlaylist.Size = new System.Drawing.Size(57, 22);
            this.miPlaylist.Text = "Playlist";
            // 
            // miPlaylistNew
            // 
            this.miPlaylistNew.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbxmiPlaylistNew,
            this.miPlaylistNewCreate});
            this.miPlaylistNew.Name = "miPlaylistNew";
            this.miPlaylistNew.Size = new System.Drawing.Size(158, 22);
            this.miPlaylistNew.Text = "Create New";
            // 
            // tbxmiPlaylistNew
            // 
            this.tbxmiPlaylistNew.Name = "tbxmiPlaylistNew";
            this.tbxmiPlaylistNew.Size = new System.Drawing.Size(100, 23);
            this.tbxmiPlaylistNew.ToolTipText = "Enter the name for the playlist.";
            // 
            // miPlaylistNewCreate
            // 
            this.miPlaylistNewCreate.Name = "miPlaylistNewCreate";
            this.miPlaylistNewCreate.Size = new System.Drawing.Size(160, 22);
            this.miPlaylistNewCreate.Text = "Create";
            this.miPlaylistNewCreate.Click += new System.EventHandler(this.miPlaylistNewCreate_Click);
            // 
            // miPlaylistSelect
            // 
            this.miPlaylistSelect.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbxmiPlaylistSelect});
            this.miPlaylistSelect.Name = "miPlaylistSelect";
            this.miPlaylistSelect.Size = new System.Drawing.Size(158, 22);
            this.miPlaylistSelect.Text = "Select Playlist";
            // 
            // cbxmiPlaylistSelect
            // 
            this.cbxmiPlaylistSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxmiPlaylistSelect.Name = "cbxmiPlaylistSelect";
            this.cbxmiPlaylistSelect.Size = new System.Drawing.Size(121, 23);
            this.cbxmiPlaylistSelect.SelectedIndexChanged += new System.EventHandler(this.cbxmiPlaylistSelect_SelectedIndexChanged);
            // 
            // miSelectedPlaylist
            // 
            this.miSelectedPlaylist.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSelectPlaylistSync,
            this.miPlaylistsSPAutoReset,
            this.miSelectedPlaylistResetTimesPlayed});
            this.miSelectedPlaylist.Name = "miSelectedPlaylist";
            this.miSelectedPlaylist.Size = new System.Drawing.Size(158, 22);
            this.miSelectedPlaylist.Text = "Selected Playlist";
            // 
            // miSelectPlaylistSync
            // 
            this.miSelectPlaylistSync.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbxmiPlaylistSPSync});
            this.miSelectPlaylistSync.Name = "miSelectPlaylistSync";
            this.miSelectPlaylistSync.Size = new System.Drawing.Size(198, 22);
            this.miSelectPlaylistSync.Text = "Sync";
            this.miSelectPlaylistSync.ToolTipText = "Set this to \"Yes\" to add files to the playlist when they are added to one of the " +
    "directories you added in this playlist.";
            // 
            // cbxmiPlaylistSPSync
            // 
            this.cbxmiPlaylistSPSync.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxmiPlaylistSPSync.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cbxmiPlaylistSPSync.Name = "cbxmiPlaylistSPSync";
            this.cbxmiPlaylistSPSync.Size = new System.Drawing.Size(121, 23);
            this.cbxmiPlaylistSPSync.ToolTipText = "When files get added to one of the playlist\'s folders it gets automatically added" +
    " to the playlist.";
            this.cbxmiPlaylistSPSync.SelectedIndexChanged += new System.EventHandler(this.cbxmiPlaylistSPSync_SelectedIndexChanged);
            // 
            // miPlaylistsSPAutoReset
            // 
            this.miPlaylistsSPAutoReset.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbxmiPlaylistSPAutoReset});
            this.miPlaylistsSPAutoReset.Name = "miPlaylistsSPAutoReset";
            this.miPlaylistsSPAutoReset.Size = new System.Drawing.Size(198, 22);
            this.miPlaylistsSPAutoReset.Text = "Auto reset times played";
            // 
            // cbxmiPlaylistSPAutoReset
            // 
            this.cbxmiPlaylistSPAutoReset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxmiPlaylistSPAutoReset.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cbxmiPlaylistSPAutoReset.Name = "cbxmiPlaylistSPAutoReset";
            this.cbxmiPlaylistSPAutoReset.Size = new System.Drawing.Size(121, 23);
            this.cbxmiPlaylistSPAutoReset.ToolTipText = "Set \"Yes\" if you want to reset the times played on all songs when a new song is a" +
    "dded to the playlist.";
            this.cbxmiPlaylistSPAutoReset.SelectedIndexChanged += new System.EventHandler(this.cbxmiPlaylistSPAutoReset_SelectedIndexChanged);
            // 
            // miSelectedPlaylistResetTimesPlayed
            // 
            this.miSelectedPlaylistResetTimesPlayed.Name = "miSelectedPlaylistResetTimesPlayed";
            this.miSelectedPlaylistResetTimesPlayed.Size = new System.Drawing.Size(198, 22);
            this.miSelectedPlaylistResetTimesPlayed.Text = "Reset times played";
            this.miSelectedPlaylistResetTimesPlayed.Click += new System.EventHandler(this.miSelectedPlaylistResetTimesPlayed_Click);
            // 
            // miPreferences
            // 
            this.miPreferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miPreferences.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playingModeToolStripMenuItem,
            this.toolStripMenuItem1});
            this.miPreferences.Image = ((System.Drawing.Image)(resources.GetObject("miPreferences.Image")));
            this.miPreferences.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miPreferences.Name = "miPreferences";
            this.miPreferences.Size = new System.Drawing.Size(81, 22);
            this.miPreferences.Text = "Preferences";
            // 
            // playingModeToolStripMenuItem
            // 
            this.playingModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbxmiPreferencesMode});
            this.playingModeToolStripMenuItem.Name = "playingModeToolStripMenuItem";
            this.playingModeToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.playingModeToolStripMenuItem.Text = "Playing Mode";
            // 
            // cbxmiPreferencesMode
            // 
            this.cbxmiPreferencesMode.Items.AddRange(new object[] {
            "Normal Mode",
            "Random Mode",
            "Smart Mode"});
            this.cbxmiPreferencesMode.Name = "cbxmiPreferencesMode";
            this.cbxmiPreferencesMode.Size = new System.Drawing.Size(121, 23);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbmiPreferencesWriteToFile,
            this.tbxmiPreferencesWTFLocation});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(147, 22);
            this.toolStripMenuItem1.Text = "Write to file";
            // 
            // cbmiPreferencesWriteToFile
            // 
            this.cbmiPreferencesWriteToFile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbmiPreferencesWriteToFile.Items.AddRange(new object[] {
            "On new song",
            "Every second"});
            this.cbmiPreferencesWriteToFile.Name = "cbmiPreferencesWriteToFile";
            this.cbmiPreferencesWriteToFile.Size = new System.Drawing.Size(121, 23);
            this.cbmiPreferencesWriteToFile.SelectedIndexChanged += new System.EventHandler(this.cbmiPreferencesWriteToFile_SelectedIndexChanged);
            // 
            // tbxmiPreferencesWTFLocation
            // 
            this.tbxmiPreferencesWTFLocation.Name = "tbxmiPreferencesWTFLocation";
            this.tbxmiPreferencesWTFLocation.Size = new System.Drawing.Size(100, 23);
            this.tbxmiPreferencesWTFLocation.ToolTipText = "Fill in the location of the file you want to save the current status to - ending " +
    "with .txt.";
            // 
            // miHelp
            // 
            this.miHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miHelpAbout});
            this.miHelp.Image = ((System.Drawing.Image)(resources.GetObject("miHelp.Image")));
            this.miHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miHelp.Name = "miHelp";
            this.miHelp.Size = new System.Drawing.Size(45, 22);
            this.miHelp.Text = "Help";
            // 
            // miHelpAbout
            // 
            this.miHelpAbout.Name = "miHelpAbout";
            this.miHelpAbout.Size = new System.Drawing.Size(107, 22);
            this.miHelpAbout.Text = "About";
            this.miHelpAbout.Click += new System.EventHandler(this.miHelpAbout_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // miPlay
            // 
            this.miPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miPlay.Image = ((System.Drawing.Image)(resources.GetObject("miPlay.Image")));
            this.miPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miPlay.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
            this.miPlay.Name = "miPlay";
            this.miPlay.Size = new System.Drawing.Size(33, 22);
            this.miPlay.Text = "Play";
            this.miPlay.Click += new System.EventHandler(this.miPlay_Click);
            // 
            // miPause
            // 
            this.miPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miPause.Image = ((System.Drawing.Image)(resources.GetObject("miPause.Image")));
            this.miPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miPause.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
            this.miPause.Name = "miPause";
            this.miPause.Size = new System.Drawing.Size(42, 22);
            this.miPause.Text = "Pause";
            this.miPause.Click += new System.EventHandler(this.miPause_Click);
            // 
            // miStop
            // 
            this.miStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miStop.Image = ((System.Drawing.Image)(resources.GetObject("miStop.Image")));
            this.miStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miStop.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
            this.miStop.Name = "miStop";
            this.miStop.Size = new System.Drawing.Size(35, 22);
            this.miStop.Text = "Stop";
            this.miStop.Click += new System.EventHandler(this.miStop_Click);
            // 
            // miPrevious
            // 
            this.miPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miPrevious.Image = ((System.Drawing.Image)(resources.GetObject("miPrevious.Image")));
            this.miPrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miPrevious.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
            this.miPrevious.Name = "miPrevious";
            this.miPrevious.Size = new System.Drawing.Size(56, 22);
            this.miPrevious.Text = "Previous";
            this.miPrevious.Click += new System.EventHandler(this.miPrevious_Click);
            // 
            // miNext
            // 
            this.miNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miNext.Image = ((System.Drawing.Image)(resources.GetObject("miNext.Image")));
            this.miNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.miNext.Margin = new System.Windows.Forms.Padding(0, 1, 10, 2);
            this.miNext.Name = "miNext";
            this.miNext.Size = new System.Drawing.Size(35, 22);
            this.miNext.Text = "Next";
            this.miNext.Click += new System.EventHandler(this.miNext_Click);
            // 
            // dgridSongs
            // 
            this.dgridSongs.AllowUserToAddRows = false;
            this.dgridSongs.AllowUserToDeleteRows = false;
            this.dgridSongs.AllowUserToResizeRows = false;
            this.dgridSongs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgridSongs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgridSongs.BackgroundColor = System.Drawing.Color.Black;
            this.dgridSongs.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
            this.dgridSongs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmTitle,
            this.clmArtist,
            this.clmAlbum,
            this.clmYear,
            this.clmDuration,
            this.clmTimesPlayed,
            this.clmLocation});
            this.dgridSongs.ContextMenuStrip = this.cxmsGrid;
            this.dgridSongs.Location = new System.Drawing.Point(0, 62);
            this.dgridSongs.Name = "dgridSongs";
            this.dgridSongs.RowHeadersVisible = false;
            this.dgridSongs.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.DodgerBlue;
            this.dgridSongs.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgridSongs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgridSongs.Size = new System.Drawing.Size(597, 266);
            this.dgridSongs.TabIndex = 4;
            this.dgridSongs.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgridSongs_CellDoubleClick);
            this.dgridSongs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgridSongs_KeyDown);
            this.dgridSongs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgridSongs_MouseDown);
            // 
            // cxmsGrid
            // 
            this.cxmsGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxmiPlaycount,
            this.ctxmibtnDeleteFromPlaylist});
            this.cxmsGrid.Name = "cxmsGrid";
            this.cxmsGrid.Size = new System.Drawing.Size(177, 48);
            // 
            // ctxmiPlaycount
            // 
            this.ctxmiPlaycount.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxmitbxPlaycount,
            this.ctxmibtnPlaycountOK});
            this.ctxmiPlaycount.Name = "ctxmiPlaycount";
            this.ctxmiPlaycount.Size = new System.Drawing.Size(176, 22);
            this.ctxmiPlaycount.Text = "Set times played";
            // 
            // ctxmitbxPlaycount
            // 
            this.ctxmitbxPlaycount.Name = "ctxmitbxPlaycount";
            this.ctxmitbxPlaycount.Size = new System.Drawing.Size(100, 23);
            // 
            // ctxmibtnPlaycountOK
            // 
            this.ctxmibtnPlaycountOK.Name = "ctxmibtnPlaycountOK";
            this.ctxmibtnPlaycountOK.Size = new System.Drawing.Size(160, 22);
            this.ctxmibtnPlaycountOK.Text = "OK";
            this.ctxmibtnPlaycountOK.Click += new System.EventHandler(this.ctxmibtnPlaycountOK_Click);
            // 
            // ctxmibtnDeleteFromPlaylist
            // 
            this.ctxmibtnDeleteFromPlaylist.Name = "ctxmibtnDeleteFromPlaylist";
            this.ctxmibtnDeleteFromPlaylist.Size = new System.Drawing.Size(176, 22);
            this.ctxmibtnDeleteFromPlaylist.Text = "Delete from playlist";
            this.ctxmibtnDeleteFromPlaylist.Click += new System.EventHandler(this.ctxmibtnDeleteFromPlaylist_Click);
            // 
            // timer1s
            // 
            this.timer1s.Interval = 1000;
            this.timer1s.Tick += new System.EventHandler(this.timer1s_Tick);
            // 
            // clmTitle
            // 
            this.clmTitle.HeaderText = "Title";
            this.clmTitle.Name = "clmTitle";
            this.clmTitle.ReadOnly = true;
            // 
            // clmArtist
            // 
            this.clmArtist.HeaderText = "Artist";
            this.clmArtist.Name = "clmArtist";
            this.clmArtist.ReadOnly = true;
            // 
            // clmAlbum
            // 
            this.clmAlbum.HeaderText = "Album";
            this.clmAlbum.Name = "clmAlbum";
            this.clmAlbum.ReadOnly = true;
            // 
            // clmYear
            // 
            this.clmYear.HeaderText = "Year";
            this.clmYear.Name = "clmYear";
            this.clmYear.ReadOnly = true;
            // 
            // clmDuration
            // 
            this.clmDuration.HeaderText = "Duration";
            this.clmDuration.Name = "clmDuration";
            this.clmDuration.ReadOnly = true;
            // 
            // clmTimesPlayed
            // 
            this.clmTimesPlayed.HeaderText = "Times Played";
            this.clmTimesPlayed.Name = "clmTimesPlayed";
            this.clmTimesPlayed.ReadOnly = true;
            // 
            // clmLocation
            // 
            this.clmLocation.HeaderText = "Location";
            this.clmLocation.Name = "clmLocation";
            this.clmLocation.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(597, 328);
            this.Controls.Add(this.dgridSongs);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.pnlPlaying);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Dragon Audio Player";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnlPlaying.ResumeLayout(false);
            this.pnlPlaying.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbarPlaying)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbarVolume)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgridSongs)).EndInit();
            this.cxmsGrid.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlPlaying;
        private System.Windows.Forms.Splitter splitterPlaying;
        private System.Windows.Forms.TrackBar tbarPlaying;
        private System.Windows.Forms.TrackBar tbarVolume;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton miFile;
        private System.Windows.Forms.ToolStripMenuItem miFileAddFolder;
        private System.Windows.Forms.ToolStripMenuItem miFileAddFiles;
        private System.Windows.Forms.ToolStripMenuItem miFileExit;
        private System.Windows.Forms.ToolStripDropDownButton miPlaylist;
        private System.Windows.Forms.ToolStripMenuItem miPlaylistNew;
        private System.Windows.Forms.ToolStripMenuItem miPlaylistSelect;
        private System.Windows.Forms.ToolStripComboBox cbxmiPlaylistSelect;
        private System.Windows.Forms.ToolStripDropDownButton miPreferences;
        private System.Windows.Forms.ToolStripDropDownButton miHelp;
        private System.Windows.Forms.ToolStripMenuItem miHelpAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton miPlay;
        private System.Windows.Forms.ToolStripButton miStop;
        private System.Windows.Forms.DataGridView dgridSongs;
        private System.Windows.Forms.ToolStripButton miPause;
        private System.Windows.Forms.ToolStripButton miPrevious;
        private System.Windows.Forms.ToolStripButton miNext;
        private System.Windows.Forms.Timer timer1s;
        private System.Windows.Forms.ToolStripTextBox tbxmiPlaylistNew;
        private System.Windows.Forms.ToolStripMenuItem miPlaylistNewCreate;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripComboBox cbmiPreferencesWriteToFile;
        private System.Windows.Forms.ToolStripTextBox tbxmiPreferencesWTFLocation;
        private ToolStripMenuItem miSelectedPlaylist;
        private ToolStripMenuItem miSelectPlaylistSync;
        private ToolStripComboBox cbxmiPlaylistSPSync;
        private ContextMenuStrip cxmsGrid;
        private ToolStripMenuItem ctxmiPlaycount;
        private ToolStripTextBox ctxmitbxPlaycount;
        private ToolStripMenuItem ctxmibtnPlaycountOK;
        private ToolStripMenuItem ctxmibtnDeleteFromPlaylist;
        private ToolStripMenuItem miSelectedPlaylistResetTimesPlayed;
        private ToolStripMenuItem miPlaylistsSPAutoReset;
        private ToolStripComboBox cbxmiPlaylistSPAutoReset;
        private ToolStripMenuItem playingModeToolStripMenuItem;
        private ToolStripComboBox cbxmiPreferencesMode;
        private DataGridViewTextBoxColumn clmTitle;
        private DataGridViewTextBoxColumn clmArtist;
        private DataGridViewTextBoxColumn clmAlbum;
        private DataGridViewTextBoxColumn clmYear;
        private DataGridViewTextBoxColumn clmDuration;
        private DataGridViewTextBoxColumn clmTimesPlayed;
        private DataGridViewTextBoxColumn clmLocation;
    }
}

