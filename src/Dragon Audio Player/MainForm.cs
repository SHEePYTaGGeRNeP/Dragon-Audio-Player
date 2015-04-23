// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="DoubleDutch Dragons">
//   © 2014 DoubleDutch Dragons
// </copyright>
// <summary>
//   Defines the MainForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Dragon_Audio_Player.Classes;
using Dragon_Audio_Player.Properties;
using NAudio.Wave;

namespace Dragon_Audio_Player
{
    //      ---------------------------------------------
    //      |   Product:    Dragon Audio Player         |
    //      |   By:         SHEePYTaGGeRNeP             |
    //      |   Date:       28/03/2015                  |
    //      |   Version:    0.4                         |
    //      |   Copyright © Double Dutch Dragons 2015   |
    //      ---------------------------------------------

    public partial class MainForm : Form
    {

        /// TODO: https://docs.google.com/spreadsheets/d/1EPZA_mqK9Kh4qKLxEZsnqQuB8pNJWqQjR6zO2HlAzXQ/

        private bool _mouseDown;

        private readonly DrgnAudioPlayer _audioPlayer;

        private PlaybackState PlayState { get { return _audioPlayer.PlayingState; } }


        public MainForm()
        {
            try
            {
                InitializeComponent();
                tbarPlaying.MouseWheel += doNothing_MouseWheel;
                _audioPlayer = new DrgnAudioPlayer();
                _audioPlayer.OnNewSong += delegate
                {
                    ChangeTitleSong(_audioPlayer.CurrentlyPlaying); 
                };
                _audioPlayer.OnTimesPlayedIncrease += delegate(object pSender, AudioFile pFile)
                {
                    UpdateDataGridTimesPlayed(pFile);
                };

                _audioPlayer.LoadPlaylists(DrgnAudioPlayer.PlaylistFileName);
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region >< >< >< >< >< >< >< >< >< ><  F O R M   >< >< >< >< >< >< >< >< >< >< >< ><

        // ReSharper disable once InconsistentNaming
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                foreach (string lvS in _audioPlayer.GetPlaylistNames())
                    cbxmiPlaylistSelect.Items.Add(lvS);
                LoadUserInterface();
                LoadFromSettings();
                RefreshDataGrid();
                this.Cursor = Cursors.Default;
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserInterface()
        {
            try
            {
                micbxPrefencesPlayingModes.Items.Clear();
                micbxPrefencesPlayingModes.Items.AddRange(Enum.GetNames(typeof(DrgnAudioPlayer.EPlayingMode)));
                cbxmiPlaylistSelect.Items.Clear();
                cbxmiPlaylistSelect.Items.AddRange(_audioPlayer.GetPlaylistNames());
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadFromSettings()
        {
            try
            {
                this.Size = Settings.Default.FormSize;
                Console.WriteLine("Volume: {0}", Settings.Default.Volume);
                tbarVolume.Value = Settings.Default.Volume;
                //tbarVolume.Refresh();
                _audioPlayer.ChangeVolume(tbarVolume.Value);
                if (Settings.Default.PlayingMode == "")
                    // G = Displays the enumeration entry as a string value.
                    micbxPrefencesPlayingModes.SelectedIndex = micbxPrefencesPlayingModes.Items.IndexOf(DrgnAudioPlayer.EPlayingMode.smart.ToString("G"));
                else
                    micbxPrefencesPlayingModes.SelectedIndex = micbxPrefencesPlayingModes.Items.IndexOf(Settings.Default.PlayingMode);
                cbxmiPlaylistSelect.SelectedIndex = cbxmiPlaylistSelect.Items.IndexOf(Settings.Default.LastPlayinglist);

                _audioPlayer.SetPlaylist(cbxmiPlaylistSelect.Text);
            }
            catch (Exception lvEx)
            { MessageBox.Show("Error trying to load settings:\n" + lvEx.Message, "Loading error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void SetSettings()
        {
            try
            {
                Settings.Default.FormSize = this.Size;
                Settings.Default.Volume = tbarVolume.Value;
                Settings.Default.PlayingMode = micbxPrefencesPlayingModes.Text;
                Settings.Default.LastPlayinglist = cbxmiPlaylistSelect.Text;
                Settings.Default.SongOutLocation = tbxmiPreferencesWTFLocation.Text;
            }
            catch (Exception lvEx)
            { MessageBox.Show("Error trying to save settings:\n" + lvEx.Message, "Saving error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ChangeTitleSong(AudioFile pAf)
        {
            if (pAf != null)
            {
                ChangeTitle(String.Format("[{0}] {1} - {2} / {3}", _audioPlayer.PlayingState.ToString(),
                    pAf, _audioPlayer.CurrentTimeString, pAf.DurationString));
                tsslblStatus.Text = String.Format("{0} | {1} / {2} | {3} | {4}", pAf.FileType,
                    _audioPlayer.CurrentTimeString, pAf.DurationString, pAf.ToString(), pAf.FileLocation);
            }
            else
            {
                ChangeTitle("");
                tsslblStatus.Text = "Stopped";
            }
        }
        private void ChangeTitle(string pText)
        {
            try
            {
                if (pText != "")
                    this.Text = String.Format("{0} - ][ {1} ][", pText, AppInfo.AssemblyTitle);
                else
                    this.Text = AppInfo.AssemblyTitle;
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDataGrid()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                dgridSongs.Rows.Clear();
                foreach (AudioFile lvAf in _audioPlayer.CurrentPlaylist.Songs)
                {
                    dgridSongs.Rows.Add(lvAf.Title, lvAf.Artist, lvAf.Album, lvAf.Year, lvAf.DurationString, lvAf.TimesPlayed, lvAf.FileLocation);
                }

                this.Cursor = Cursors.Default;
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateDataGridTimesPlayed(AudioFile pAf)
        {
            try
            {
                foreach (DataGridViewRow lvRow in dgridSongs.Rows)
                {
                    if ((string)lvRow.Cells[6].Value == pAf.FileLocation)
                        lvRow.Cells[5].Value = pAf.TimesPlayed;
                }
            }
            catch (Exception lvEx)
            { MessageBox.Show("Error trying to update DataGrid:\n" + lvEx.Message, "Update error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void AddRowToGrid(AudioFile pAf)
        {
            try
            {
                dgridSongs.Rows.Add(pAf.Title, pAf.Artist, pAf.Album, pAf.Year, pAf.DurationString, pAf.TimesPlayed, pAf.FileLocation);
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ReSharper disable InconsistentNaming
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _audioPlayer.SavePlaylists(DrgnAudioPlayer.PlaylistFileName);
                SetSettings();
                Settings.Default.Save();
                _audioPlayer.Dispose();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dgridSongs_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete)
                {
                    if (dgridSongs.SelectedRows.Count > 0 && DialogResult.Yes == MessageBox.Show("Are you sure you want to delete this file(s)",
                        "Delete file(s)?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
                    {
                        foreach (DataGridViewRow lvRow in dgridSongs.SelectedRows)
                        {
                            var lvArtist = lvRow.Cells[1].Value.ToString();
                            var lvTitle = lvRow.Cells[0].Value.ToString();
                            AudioFile lvAf = _audioPlayer.CurrentPlaylist.GetSongByArtistTitle(lvArtist, lvTitle);
                            _audioPlayer.DeleteSongFromPlaylist(lvAf);
                            dgridSongs.Rows.Remove(lvRow);
                        }
                    }
                }
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ReSharper restore InconsistentNaming


        #endregion

        #region >< >< >< >< >< >< >< >< >< ><  M E N U   I T E M S  >< >< >< >< >< >< >< ><

        // ReSharper disable InconsistentNaming
        private void miFileAddFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string lvSkipped = String.Empty;
                FolderBrowserDialog lvFbd = new FolderBrowserDialog { Description = "Select a folder containing audio files" };
                if (DialogResult.OK == lvFbd.ShowDialog())
                {
                    this.Cursor = Cursors.WaitCursor;
                    lvSkipped = _audioPlayer.CurrentPlaylist.AddFolder(lvFbd.SelectedPath);
                    RefreshDataGrid();
                    this.Cursor = Cursors.Default;
                }
                if (lvSkipped != String.Empty)
                    MessageBox.Show(lvSkipped, "Skipped files", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void miFileAddFiles_Click(object sender, EventArgs e)
        {
            try
            {
                string lvSkipped = String.Empty;
                OpenFileDialog lvOfd = new OpenFileDialog
                {
                    Title = "Select one or more audio files",
                    Multiselect = true,
                    Filter = ".mp3 file(*.mp3)|*.mp3|.wav file(*.wav)|*.wav|.aac file(*.aac)|*.aac"
                             + "|.flac file(*.flac)|*.flac|.mp4 file(*.mp4)|*.mp4|.wma file(*.wma)|*.wma|All files(*.*)|*.*"
                };
                if (DialogResult.OK == lvOfd.ShowDialog())
                {
                    this.Cursor = Cursors.WaitCursor;
                    foreach (string lvS in lvOfd.FileNames)
                    {
                        string lvResult = _audioPlayer.CurrentPlaylist.AddFile(lvS, true);
                        if (lvResult != String.Empty)
                            lvSkipped += lvResult + "\n";
                        AddRowToGrid(_audioPlayer.LastAudioFile);
                    }
                    this.Cursor = Cursors.Default;
                }
                if (lvSkipped != String.Empty)
                    MessageBox.Show(lvSkipped, "Skipped files", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void miFileExit_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbmiPreferencesWriteToFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.Default.WriteToFile = cbmiPreferencesWriteToFile.Text;

            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void tbxmiPreferencesWTFLocation_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (tbxmiPreferencesWTFLocation.Text.Length > 0)
                    cbmiPreferencesWriteToFile.Enabled = true;
                else
                    cbmiPreferencesWriteToFile.Enabled = false;
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void micbxPrefencesPlayingModes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _audioPlayer.SetPlayingMode(micbxPrefencesPlayingModes.Text);
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void miPlaylistNewCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbxmiPlaylistNew.Text == "")
                    MessageBox.Show("Please enter a playlist name.", "Playlist name", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    // TODO: create new playlist.
                }
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cbxmiPlaylistSelect_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbxmiPlaylistSelect.Text != "")
                    _audioPlayer.SetPlaylist(cbxmiPlaylistSelect.Text);
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void miHelpAbout_Click(object sender, EventArgs e)
        {
            try
            {
                DrgnAboutBox ab = new DrgnAboutBox();
                ab.ShowDialog();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void miPlay_Click(object sender, EventArgs e)
        {
            try
            {
                if (PlayState == PlaybackState.Paused)
                {
                    _audioPlayer.Play(null);
                    ChangeTitleSong(_audioPlayer.CurrentlyPlaying);
                    if (!timer1s.Enabled)
                        timer1s.Start();
                }
                else if (dgridSongs.SelectedRows.Count == 1)
                {
                    string lvArtist = dgridSongs.SelectedRows[0].Cells[1].Value.ToString();
                    string lvTitle = dgridSongs.SelectedRows[0].Cells[0].Value.ToString();
                    AudioFile lvAf = _audioPlayer.CurrentPlaylist.GetSongByArtistTitle(lvArtist, lvTitle);
                    Play(lvAf.ToString());
                }
                else
                    PlayNext();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void miStop_Click(object sender, EventArgs e)
        {
            try
            {
                Stop();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void miPause_Click(object sender, EventArgs e)
        {
            try
            {
                Pause();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void miPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                Previous();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void miNext_Click(object sender, EventArgs e)
        {
            try
            {
                PlayNext();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void miPlaylistSavePlaylists_Click(object sender, EventArgs e)
        {
            try
            {
                _audioPlayer.SavePlaylists(DrgnAudioPlayer.PlaylistFileName);
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void miPlaylistLoadPlaylists_Click(object sender, EventArgs e)
        {
            try
            {
                _audioPlayer.LoadPlaylists(DrgnAudioPlayer.PlaylistFileName);
                RefreshDataGrid();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ReSharper restore InconsistentNaming


        #endregion


        #region >< >< >< >< >< >< >< >< >< ><  M E D I A   C O N T R O L S  >< >< >< >< ><

        private void tbarPlaying_MouseUp(object sender, MouseEventArgs e)
        { _mouseDown = false; }
        private void tbarPlaying_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;

            // Jump to the clicked location
            double lvDblValue = ((double)e.X / (double)tbarPlaying.Width) * (tbarPlaying.Maximum - tbarPlaying.Minimum);
            tbarPlaying.Value = Convert.ToInt32(lvDblValue);
        }
        private void tbarPlaying_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                _mouseDown = true;
        }
        private void tbarPlaying_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                _mouseDown = false;
        }

        private void tbarPlaying_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (_audioPlayer.CurrentlyPlaying != null)
                {
                    if (tbarPlaying.Value >= (int)_audioPlayer.CurrentlyPlaying.Duration.TotalSeconds)
                        try
                        { _audioPlayer.PlayNext(); }
                        catch (Exception lvEx)
                        {
                            MessageBox.Show("Error trying to play next song:\n" + lvEx.Message, "Next song error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    else
                    {
                        if (_mouseDown)
                        {
                            long lvSeekValue = (long)TimeSpan.FromSeconds(tbarPlaying.Value).TotalMilliseconds;
                            Seek(lvSeekValue);
                        }
                    }
                }
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void doNothing_MouseWheel(object sender, EventArgs e)
        {
            HandledMouseEventArgs lvEe = (HandledMouseEventArgs)e;
            lvEe.Handled = true;
        }

        private void tbarVolume_Scroll(object sender, EventArgs e)
        {
        }

        private void tbarVolume_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                _audioPlayer.ChangeVolume(tbarVolume.Value);
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Play(string pString)
        {
            try
            {
                _audioPlayer.Play(_audioPlayer.CurrentPlaylist.GetSongByString(pString));
                tbarPlaying.Value = 0;
                ChangeTitleSong(_audioPlayer.CurrentlyPlaying);
                AfterPlay();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PlayNext()
        {
            try
            {
                _audioPlayer.PlayNext();
                AfterPlay();
            }
            catch (Exception lvEx)
            { MessageBox.Show("Error trying to play next song:\n" + lvEx.Message, "Next error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void AfterPlay()
        {
            try
            {
                // Rounds up always, 170.1 = 171
                if (timer1s.Enabled == false)
                    timer1s.Start();
                ChangeTitleSong(_audioPlayer.CurrentlyPlaying);
            }
            catch (Exception lvEx)
            { MessageBox.Show("Error trying to set controls to next song:\n" + lvEx.Message, "Control error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Stop()
        {
            try
            {
                _audioPlayer.Stop();
                timer1s.Stop();
                ChangeTitleSong(null);
            }
            catch (Exception lvEx)
            { MessageBox.Show("Error trying to stop audio:\n" + lvEx.Message, "Stopping error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void Pause()
        {
            try
            {
                _audioPlayer.Pause();
                ChangeTitleSong(_audioPlayer.CurrentlyPlaying);
                timer1s.Stop();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Seek(long pMilliseconds)
        {
            try
            { _audioPlayer.Seek(pMilliseconds, SeekOrigin.Begin); }
            catch (Exception lvEx)
            {
                MessageBox.Show("Error trying to Seek to value: " + pMilliseconds + " milliseconds\n" + lvEx.Message, "Seek error",
                  MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Previous()
        {
            try
            {
                _audioPlayer.Previous();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion


        #region >< >< >< >< >< >< >< >< >< ><  O T H E R  >< >< >< >< >< >< >< >< ><

        private void dgridSongs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex > -1)
                    if (dgridSongs.SelectedRows.Count == 1)
                    {
                        string lvArtist = dgridSongs.SelectedRows[0].Cells[1].Value.ToString();
                        string lvTitle = dgridSongs.SelectedRows[0].Cells[0].Value.ToString();
                        AudioFile lvAf = _audioPlayer.CurrentPlaylist.GetSongByArtistTitle(lvArtist, lvTitle);
                        Play(lvAf.ToString());
                    }
            }
            catch (Exception lvEx)
            {
                MessageBox.Show("An unexpected error occurred:\n" + lvEx.Message, "Unexpected error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1s_Tick(object sender, EventArgs e)
        {
            try
            {
                tbarPlaying.Maximum = Convert.ToInt32(Math.Ceiling((double)_audioPlayer.CurrentlyPlaying.Duration.TotalSeconds));
                int lvSec = Convert.ToInt32(Math.Round(_audioPlayer.CurrentTime.TotalSeconds));
                tbarPlaying.Value = lvSec;
                if (cbmiPreferencesWriteToFile.SelectedIndex == cbmiPreferencesWriteToFile.Items.IndexOf("Every second")
                   && tbxmiPreferencesWTFLocation.Text != "")
                    WriteSongInfo();
            }
            catch
            { tbarPlaying.Value = tbarPlaying.Maximum; }
            ChangeTitleSong(_audioPlayer.CurrentlyPlaying);
        }
        private void WriteSongInfo()
        {
            try
            {
                if (tbxmiPreferencesWTFLocation.Text != "" && tbxmiPreferencesWTFLocation.Text.EndsWith(".txt"))
                {
                    switch (cbmiPreferencesWriteToFile.SelectedIndex)
                    {
                        // 0 = dont write
                        // Write on new song
                        case 1: StaticClass.WriteToFile(tbxmiPreferencesWTFLocation.Text,
                            String.Format("                 [{0}] {1} - {2}", _audioPlayer.PlayingState.ToString(),
                            _audioPlayer.CurrentlyPlaying.Artist, _audioPlayer.CurrentlyPlaying.Title)); break;
                        // Write every second
                        case 2: StaticClass.WriteToFile(tbxmiPreferencesWTFLocation.Text,
                            String.Format("                 [{0}] {1} - {2} {3} / {4}", _audioPlayer.PlayingState.ToString(),
                            _audioPlayer.CurrentlyPlaying.Artist, _audioPlayer.CurrentlyPlaying.Title, _audioPlayer.CurrentTimeString,
                            _audioPlayer.CurrentlyPlaying.DurationString)); break;
                    }
                }
            }
            catch (Exception lvEx)
            { MessageBox.Show("Error trying to write song to textfile:\n" + lvEx.Message, "Writing error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }


        #endregion



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
            if (_audioPlayer != null)
                _audioPlayer.Dispose();
            base.Dispose(disposing);
        }

        private void miPlaylistResetTimesPlayed_Click(object sender, EventArgs e)
        {
            _audioPlayer.CurrentPlaylist.ResetTimesPlayed();
            RefreshDataGrid();
        }



















    }



}
