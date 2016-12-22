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
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }

        #region >< >< >< >< >< >< >< >< >< ><  F O R M   >< >< >< >< >< >< >< >< >< >< >< ><

        // ReSharper disable once InconsistentNaming
        private void Form1_Load(object sender, EventArgs pE)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                LoadPlaylists();
                foreach (string lvS in _audioPlayer.GetPlaylistNames())
                    cbxmiPlaylistSelect.Items.Add(lvS);
                this.FillComboBoxes();
                LoadFromSettings();
                RefreshDataGrid();
                this.Cursor = Cursors.Default;
                dgridSongs.ClearSelection();
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }

        private void FillComboBoxes()
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }
        private void LoadFromSettings()
        {
            try
            {
                this.Size = Settings.Default.FormSize;
                Console.WriteLine(Resources.Volume___0_, Settings.Default.Volume);
                tbarVolume.Value = Settings.Default.Volume;
                //tbarVolume.Refresh();
                _audioPlayer.ChangeVolume(tbarVolume.Value);
                if (Settings.Default.PlayingMode == "")
                    // G = Displays the enumeration entry as a string value.
                    micbxPrefencesPlayingModes.SelectedIndex = micbxPrefencesPlayingModes.Items.IndexOf(DrgnAudioPlayer.EPlayingMode.smart.ToString("G"));
                else
                    micbxPrefencesPlayingModes.SelectedIndex = micbxPrefencesPlayingModes.Items.IndexOf(Settings.Default.PlayingMode);
                if (!String.IsNullOrEmpty(Settings.Default.LastPlayinglist) &&
                    _audioPlayer.GetPlaylist(Settings.Default.LastPlayinglist) != null)
                {
                    this.SetPlaylist(Settings.Default.LastPlayinglist);
                }
                if (Settings.Default.WriteToFile != null)
                    tbxmiPreferencesWTFLocation.Text = Settings.Default.WriteToFile;
                this.LoadColumnSizesFromSettings();
            }
            catch (Exception lvEx)
            {
                MessageBox.Show(Resources.Error_MainForm_LoadFromSettings + lvEx.Message,
                  Resources.MainForm_LoadFromSettings_Loading_error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadColumnSizesFromSettings()
        {
            dgridSongs.Columns["clmAlbum"].Width = Settings.Default.ColumnWidthAlbum;
            dgridSongs.Columns["clmArtist"].Width = Settings.Default.ColumnWidthArtist;
            dgridSongs.Columns["clmDuration"].Width = Settings.Default.ColumnWidthDuration;
            dgridSongs.Columns["clmLocation"].Width = Settings.Default.ColumnWidthLocation;
            dgridSongs.Columns["clmTimesPlayed"].Width = Settings.Default.ColumnWidthTimesPlayed;
            dgridSongs.Columns["clmTitle"].Width = Settings.Default.ColumnWidthTitle;
            dgridSongs.Columns["clmYear"].Width = Settings.Default.ColumnWidthYear;
        }
        private void SetSettings()
        {
            try
            {
                Settings.Default.FormSize = this.Size;
                Settings.Default.Volume = tbarVolume.Value;
                Settings.Default.PlayingMode = micbxPrefencesPlayingModes.Text;
                Settings.Default.LastPlayinglist = cbxmiPlaylistSelect.Text;
                Settings.Default.WriteToFile = tbxmiPreferencesWTFLocation.Text;
                Settings.Default.ColumnWidthAlbum = dgridSongs.Columns["clmAlbum"].Width;
                Settings.Default.ColumnWidthArtist = dgridSongs.Columns["clmArtist"].Width;
                Settings.Default.ColumnWidthDuration = dgridSongs.Columns["clmDuration"].Width;
                Settings.Default.ColumnWidthLocation = dgridSongs.Columns["clmLocation"].Width;
                Settings.Default.ColumnWidthTimesPlayed = dgridSongs.Columns["clmTimesPlayed"].Width;
                Settings.Default.ColumnWidthTitle = dgridSongs.Columns["clmTitle"].Width;
                Settings.Default.ColumnWidthYear = dgridSongs.Columns["clmYear"].Width;

            }
            catch (Exception lvEx)
            {
                MessageBox.Show(Resources.Error_MainForm_SetSettings + lvEx.Message,
                  Resources.MainForm_SetSettings_Saving_error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                tsslblStatus.Text = Resources.PlayingStatus_Stopped;
            }
        }
        private void ChangeTitle(string pText)
        {
            try
            {
                if (!String.IsNullOrEmpty(pText))
                    this.Text = String.Format("{0} - ][ {1} ][", pText, AppInfo.AssemblyTitle);
                else
                    this.Text = AppInfo.AssemblyTitle;
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }

        private void WriteSongInfo()
        {
            try
            {
                if (this.tbxmiPreferencesWTFLocation.Text == ""
                    || !this.tbxmiPreferencesWTFLocation.Text.EndsWith(".txt"))
                {
                    return;
                }
                switch (this.cbmiPreferencesWriteToFile.SelectedIndex)
                {
                    // 0 = dont write
                    // Write on new song
                    case 1: StaticClass.WriteToFile(this.tbxmiPreferencesWTFLocation.Text,
                        String.Format("                 [{0}] {1} - {2}", this._audioPlayer.PlayingState.ToString(),
                            this._audioPlayer.CurrentlyPlaying.Artist, this._audioPlayer.CurrentlyPlaying.Title)); break;
                    // Write every second
                    case 2:
                        if (_audioPlayer.CurrentlyPlaying != null)
                        {
                            StaticClass.WriteToFile(this.tbxmiPreferencesWTFLocation.Text,
                                String.Format("                 [{0}] {1} - {2} {3} / {4}",
                                    this._audioPlayer.PlayingState.ToString(),
                                    this._audioPlayer.CurrentlyPlaying.Artist, this._audioPlayer.CurrentlyPlaying.Title,
                                    this._audioPlayer.CurrentTimeString,
                                    this._audioPlayer.CurrentlyPlaying.DurationString));
                        }
                        else
                        {
                            StaticClass.WriteToFile(this.tbxmiPreferencesWTFLocation.Text,
                                String.Format("                 [Stopped]"));
                        }
                        break;
                }
            }
            catch (Exception lvEx)
            {
                MessageBox.Show(String.Format("Error trying to write song to textfile located in :{0}\n\n{1}",
                  tbxmiPreferencesWTFLocation.Text, lvEx.Message), Resources.Writing_error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void RefreshDataGrid()
        {
            try
            {
                // TODO: Multithread this shit.
                this.Cursor = Cursors.WaitCursor;
                dgridSongs.Rows.Clear();
                if (this._audioPlayer.CurrentPlaylist != null)
                {
                    foreach (AudioFile lvAf in _audioPlayer.CurrentPlaylist.Songs)
                    {
                        dgridSongs.Rows.Add(lvAf.Title, lvAf.Artist, lvAf.Album, lvAf.Year, lvAf.DurationString,
                            lvAf.TimesPlayed, lvAf.FileLocation);
                    }
                }
                this.Cursor = Cursors.Default;
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }
        private void AddRowToGrid(AudioFile pAf)
        {
            try
            {
                dgridSongs.Rows.Add(pAf.Title, pAf.Artist, pAf.Album, pAf.Year, pAf.DurationString, pAf.TimesPlayed, pAf.FileLocation);
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }

        // ReSharper disable InconsistentNaming
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _audioPlayer.Stop();
                _audioPlayer.SavePlaylists(DrgnAudioPlayer.PlaylistFileName);
                SetSettings();
                Settings.Default.Save();
                WriteSongInfo();
                _audioPlayer.Dispose();
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }
        private void dgridSongs_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Delete) return;
                if (this.dgridSongs.SelectedRows.Count <= 0
                    || DialogResult.Yes
                    != MessageBox.Show(
                        Resources.Are_you_sure_you_want_to_delete_these_file_s_,
                        Resources.Delete_file_s__,
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Information))
                {
                    return;
                }
                foreach (DataGridViewRow lvRow in this.dgridSongs.SelectedRows)
                {
                    var lvArtist = lvRow.Cells[1].Value.ToString();
                    var lvTitle = lvRow.Cells[0].Value.ToString();
                    AudioFile lvAf = this._audioPlayer.CurrentPlaylist.GetSongByArtistTitle(lvArtist, lvTitle);
                    this._audioPlayer.DeleteSongFromPlaylist(lvAf);
                    this.dgridSongs.Rows.Remove(lvRow);
                }
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }

        private void timer1s_Tick(object sender, EventArgs e)
        {
            try
            {
                tbarPlaying.Maximum = Convert.ToInt32(Math.Ceiling(this._audioPlayer.CurrentlyPlaying.Duration.TotalSeconds));
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
        // ReSharper restore InconsistentNaming

        private void ShowUnexpectedErrorMessage(string pMethodname, string pExceptionMessage)
        {
            MessageBox.Show(Resources.MainForm_ShowUnexpectedErrorMessage_An_unexpected_error_occurred_in_ +
                   pMethodname + ":\n" + pExceptionMessage, Resources.MainForm_ShowUnexpectedErrorMessage_Unexpected_error,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region >< >< >< >< >< >< >< >< >< ><  M E N U   I T E M S  >< >< >< >< >< >< >< ><

        // ReSharper disable InconsistentNaming
        private void miFileAddFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string lvSkipped = String.Empty;
                FolderBrowserDialog lvFbd = new FolderBrowserDialog { Description = Resources.Click_Select_a_folder_containing_audio_files };
                if (DialogResult.OK == lvFbd.ShowDialog())
                {
                    this.Cursor = Cursors.WaitCursor;
                    lvSkipped = _audioPlayer.CurrentPlaylist.AddFolder(lvFbd.SelectedPath);
                    RefreshDataGrid();
                    this.Cursor = Cursors.Default;
                }
                if (lvSkipped != String.Empty)
                    MessageBox.Show(lvSkipped, Resources.Skipped_files, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
                    MessageBox.Show(lvSkipped, Resources.Skipped_files, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }

        private void miPlaylistNewCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(tbxmiPlaylistNew.Text.Trim()))
                    MessageBox.Show(Resources.miPlaylistNewCreate_Please_enter_a_playlist_name_, Resources.Playlist_name,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    if (_audioPlayer.GetPlaylist(tbxmiPlaylistNew.Text) != null)
                        MessageBox.Show(Resources.Playlist_with_this_name_already_exists,
                            Resources.Playlist_with_this_name_already_exists, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    else
                    {
                        // TODO: create new playlist.
                        _audioPlayer.Playlists.Add(new Playlist(tbxmiPlaylistNew.Text));
                        MessageBox.Show(tbxmiPlaylistNew.Text + Resources.Playlist_created,
                            Resources.New_playlist_created, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.SetPlaylist(tbxmiPlaylistNew.Text);
                        tbxmiPlaylistNew.Text = String.Empty;

                        cbxmiPlaylistSelect.Items.Clear();
                        cbxmiPlaylistSelect.Items.AddRange(_audioPlayer.GetPlaylistNames());
                    }
                }
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }
        private void cbxmiPlaylistSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.SetPlaylist(cbxmiPlaylistSelect.Text);
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }
        private void miPlaylistDeleteCurrent_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes ==
                    MessageBox.Show(String.Format("Are you sure you want to delete the current playlist: {0}?",
                        _audioPlayer.CurrentPlaylist.Name), Resources.Delete_current_playlist_, MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question))
                {
                    bool currentPlaylist = _audioPlayer.CurrentPlaylist.Name == cbxmiPlaylistSelect.Text;
                    _audioPlayer.DeleteCurrentPlaylist();
                    cbxmiPlaylistSelect.Items.Remove(cbxmiPlaylistSelect.SelectedItem);
                    if (currentPlaylist && cbxmiPlaylistSelect.Items.Count > 0)
                        this.SetPlaylist(cbxmiPlaylistSelect.Items[0].ToString());
                    else
                        this.RefreshDataGrid();
                }
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }
        private void miPlaylistLoadPlaylists_Click(object sender, EventArgs e)
        {
            try
            {
                LoadPlaylists();
                cbxmiPlaylistSelect.Items.Clear();
                cbxmiPlaylistSelect.Items.AddRange(_audioPlayer.GetPlaylistNames());
                if (cbxmiPlaylistSelect.Items.Count > 0)
                    this.SetPlaylist(cbxmiPlaylistSelect.Items[0].ToString());
                else
                    this.RefreshDataGrid();
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }
        private void miPlaylistResetTimesPlayed_Click(object sender, EventArgs e)
        {
            _audioPlayer.CurrentPlaylist.ResetTimesPlayed();
            RefreshDataGrid();
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }

        // ReSharper restore InconsistentNaming
        #endregion


        #region >< >< >< >< >< >< >< >< >< ><  M E D I A   C O N T R O L S  >< >< >< >< ><

        #region >< >< >< >< >< >< >< >< >< ><  E V E N T   H A N D L E R S >< >< >< >< ><
        // ReSharper disable InconsistentNaming
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
                if (this._audioPlayer.CurrentlyPlaying == null) return;
                if (this.tbarPlaying.Value >= (int)this._audioPlayer.CurrentlyPlaying.Duration.TotalSeconds)
                    try
                    { this._audioPlayer.PlayNext(); }
                    catch (Exception lvEx)
                    {
                        MessageBox.Show(Resources.MainForm_tbarPlaying_ValueChanged_NextSongError
                            + lvEx.Message, Resources.MainForm_tbarPlaying_ValueChanged_Next_song_error,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                else
                {
                    if (!this._mouseDown) return;
                    long lvSeekValue = (long)TimeSpan.FromSeconds(this.tbarPlaying.Value).TotalMilliseconds;
                    this.Seek(lvSeekValue);
                }
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }
        private void doNothing_MouseWheel(object sender, EventArgs e)
        {
            HandledMouseEventArgs lvEe = (HandledMouseEventArgs)e;
            lvEe.Handled = true;
        }

        private void tbarVolume_Scroll(object sender, EventArgs e)
        {
            try
            {
                _audioPlayer.ChangeVolume(tbarVolume.Value);
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }


        private void tbarVolume_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                _audioPlayer.ChangeVolume(tbarVolume.Value);
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }

        // ReSharper restore InconsistentNaming
        #endregion

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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
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
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }
        private void AfterPlay()
        {
            try
            {
                // Rounds up always, 170.1 = 171
                if (timer1s.Enabled == false) timer1s.Start();
                ChangeTitleSong(_audioPlayer.CurrentlyPlaying);
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
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
            { MessageBox.Show(Resources.MainForm_Stop_ + lvEx.Message, Resources.Stopping_error, MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }
        private void Seek(long pMilliseconds)
        {
            try
            { _audioPlayer.Seek(pMilliseconds, SeekOrigin.Begin); }
            catch (Exception lvEx)
            {
                MessageBox.Show(Resources.MainForm_Seek_Error_trying_to_Seek_to_value__ + pMilliseconds +
                    Resources.MainForm_Seek_Milliseconds + lvEx.Message, Resources.MainForm_Seek_Seek_error,
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
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }

        #endregion


        private void LoadPlaylists()
        {
            try
            {
                string lvReturnMessage = _audioPlayer.LoadPlaylists(DrgnAudioPlayer.PlaylistFileName);
                if (!String.IsNullOrEmpty(lvReturnMessage))
                {
                    PlaylistUpdate lvPu = new PlaylistUpdate(lvReturnMessage);
                    lvPu.ShowDialog();
                }
            }
            catch (Exception lvEx)
            {
                this.ShowUnexpectedErrorMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, lvEx.Message);
            }
        }
        private void SetPlaylist(string pPlaylistName)
        {
            if (!String.IsNullOrEmpty(pPlaylistName))
            {
                _audioPlayer.SetPlaylist(pPlaylistName);
                tsslblPlaylist.Text = _audioPlayer.CurrentPlaylist.Name;
                cbxmiPlaylistSelect.SelectedIndex =
                    cbxmiPlaylistSelect.Items.IndexOf(_audioPlayer.CurrentPlaylist.Name);
                RefreshDataGrid();
                Properties.Settings.Default.LastPlayinglist = _audioPlayer.CurrentPlaylist.Name;
            }
        }



        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="pDisposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && (components != null))
            {
                components.Dispose();
            }
            if (_audioPlayer != null)
                _audioPlayer.Dispose();
            base.Dispose(pDisposing);
        }























    }



}
