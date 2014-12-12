using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Dragon_Audio_Player.Properties;
using NAudio.Wave;

namespace Dragon_Audio_Player
{
    //      ---------------------------------------------
    //      |   Product:    Dragon Audio Player         |
    //      |   By:         SHEePYTaGGeRNeP             |
    //      |   Date:       06/12/2014                  |
    //      |   Version:    0.3                         |
    //      |   Copyright © Double Dutch Dragons 2014   |
    //      ---------------------------------------------

    public partial class MainForm : Form
    {

        /// TODO: https://docs.google.com/spreadsheets/d/1EPZA_mqK9Kh4qKLxEZsnqQuB8pNJWqQjR6zO2HlAzXQ/

        private bool _mouseDown;

        private readonly DrgnAudioPlayer _audioPlayer;

        private PlaybackState PlayState { get { return _audioPlayer.PlayingState; } }


        #region >< >< >< >< >< >< >< >< >< ><  F O R M   >< >< >< >< >< >< >< >< >< >< >< ><

        public MainForm()
        {
            InitializeComponent();
            tbarPlaying.MouseWheel += doNothing_MouseWheel;
            _audioPlayer = new DrgnAudioPlayer();
            _audioPlayer.LoadPlaylists();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string lvS in _audioPlayer.GetPlaylistNames())
                cbxmiPlaylistSelect.Items.Add(lvS);
            LoadUserInterface();
            LoadFromSettings();
            RefreshDataGrid();
        }

        private void LoadUserInterface()
        {
            micbxPrefencesPlayingModes.Items.AddRange(Enum.GetNames(typeof(DrgnAudioPlayer.EPlayingMode)));
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
                    // G = text / value
                    micbxPrefencesPlayingModes.SelectedIndex = micbxPrefencesPlayingModes.Items.IndexOf(DrgnAudioPlayer.EPlayingMode.Smart.ToString("G"));
                else
                    micbxPrefencesPlayingModes.SelectedIndex = micbxPrefencesPlayingModes.Items.IndexOf(Settings.Default.PlayingMode);
                cbxmiPlaylistSelect.SelectedIndex = cbxmiPlaylistSelect.Items.IndexOf(Settings.Default.LastPlayinglist);
                _audioPlayer.SetPlaylist(cbxmiPlaylistSelect.Text);
            }
            catch (Exception lvEx)
            { MessageBox.Show("Error trying to load settings:\n" + lvEx.Message, "Loading error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _audioPlayer.SavePlaylists();
            SetSettings();
            Settings.Default.Save();
            _audioPlayer.CloseWaveOut();
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
            if (pText != "")
                this.Text = String.Format("{0} - ][ {1} ][", pText, AppInfo.AssemblyTitle);
            else
                this.Text = AppInfo.AssemblyTitle;
        }

        private void RefreshDataGrid()
        {
            dgridSongs.Rows.Clear();
            foreach (AudioFile lvAf in _audioPlayer.CurrentPlaylist.Songs)
            {
                dgridSongs.Rows.Add(lvAf.Title, lvAf.Artist, lvAf.Album, lvAf.Year, lvAf.DurationString, lvAf.TimesPlayed, lvAf.FileLocation);
            }
        }
        private void UpdateDataGrid(AudioFile pAf)
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
        private void AddAudioToGrid(AudioFile pAf)
        {
            dgridSongs.Rows.Add(pAf.Title, pAf.Artist, pAf.Album, pAf.Year, pAf.DurationString, pAf.TimesPlayed, pAf.FileLocation);
        }



        #endregion

        #region >< >< >< >< >< >< >< >< >< ><  M E N U   I T E M S  >< >< >< >< >< >< >< ><

        private void miFileAddFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog lvFbd = new FolderBrowserDialog { Description = "Select a folder containing audio files" };
            if (DialogResult.OK == lvFbd.ShowDialog())
            {
                _audioPlayer.AddFolder(lvFbd.SelectedPath);
                RefreshDataGrid();
            }
        }
        private void miFileAddFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog lvOfd = new OpenFileDialog
            {
                Title = "Select one or more audio files",
                Multiselect = true,
                Filter = ".mp3 file(*.mp3)|*.mp3|.wav file(*.wav)|*.wav|.aac file(*.aac)|*.aac"
                         + "|.flac file(*.flac)|*.flac|.mp4 file(*.mp4)|*.mp4|.wma file(*.wma)|*.wma|All files(*.*)|*.*"
            };
            if (DialogResult.OK == lvOfd.ShowDialog())
            {
                foreach (string lvS in lvOfd.FileNames)
                {
                    _audioPlayer.AddFile(lvS, true);
                    AddAudioToGrid(_audioPlayer.LastAudioFile);
                }
            }
        }
        private void miFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbmiPreferencesWriteToFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.WriteToFile = cbmiPreferencesWriteToFile.Text;

        }
        private void tbxmiPreferencesWTFLocation_TextChanged(object sender, EventArgs e)
        {
            if (tbxmiPreferencesWTFLocation.Text.Length > 0)
                cbmiPreferencesWriteToFile.Enabled = true;
            else
                cbmiPreferencesWriteToFile.Enabled = false;
        }
        private void micbxPrefencesPlayingModes_SelectedIndexChanged(object sender, EventArgs e)
        {
            _audioPlayer.SetPlayingMode(micbxPrefencesPlayingModes.Text);
        }
        
        private void miPlaylistNewCreate_Click(object sender, EventArgs e)
        {
            if (tbxmiPlaylistNew.Text == "")
                MessageBox.Show("Please enter a playlist name.", "Playlist name", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {

            }
        }
        private void cbxmiPlaylistSelect_Click(object sender, EventArgs e)
        {
            if (cbxmiPlaylistSelect.Text != "")
                _audioPlayer.SetPlaylist(cbxmiPlaylistSelect.Text);
        }

        private void miHelpAbout_Click(object sender, EventArgs e)
        {
            DrgnAboutBox ab = new DrgnAboutBox();
            ab.ShowDialog();
        }


        private void miPlay_Click(object sender, EventArgs e)
        {
            if (PlayState == PlaybackState.Paused)
            {
                _audioPlayer.Play(null);
                ChangeTitleSong(_audioPlayer.CurrentlyPlaying);
                if (!timer1s.Enabled)
                    timer1s.Start();
            }
            else if (dgridSongs.SelectedRows.Count == 1)
                Play(dgridSongs.SelectedRows[0].Cells[1].Value + " - " + dgridSongs.SelectedRows[0].Cells[0].Value);
            else
                PlayNext();
        }
        private void miStop_Click(object sender, EventArgs e)
        {
            Stop();
        }
        private void miPause_Click(object sender, EventArgs e)
        {
            Pause();
        }
        private void miPrevious_Click(object sender, EventArgs e)
        {
            Previous();
        }
        private void miNext_Click(object sender, EventArgs e)
        {
            PlayNext();
        }




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
            _audioPlayer.ChangeVolume(tbarVolume.Value);
        }
        private void Play(string pString)
        {
            _audioPlayer.Play(_audioPlayer.GetFileByString(pString));
            tbarPlaying.Value = 0;
            ChangeTitleSong(_audioPlayer.CurrentlyPlaying);
            AfterPlay();
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
            _audioPlayer.Pause();
            ChangeTitleSong(_audioPlayer.CurrentlyPlaying);
            timer1s.Stop();
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

        }

        #endregion


        #region >< >< >< >< >< >< >< >< >< ><  O T H E R  >< >< >< >< >< >< >< >< ><

        private void dgridSongs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
                if (dgridSongs.SelectedRows.Count == 1)
                    Play(dgridSongs.SelectedRows[0].Cells[1].Value + " - " + dgridSongs.SelectedRows[0].Cells[0].Value);
        }

        private void timer1s_Tick(object sender, EventArgs e)
        {
            try
            {
                tbarPlaying.Maximum = Convert.ToInt32(Math.Ceiling((double)_audioPlayer.CurrentlyPlaying.Duration.TotalSeconds));
                int lvSec = Convert.ToInt32(Math.Round(_audioPlayer.CurrentTime.TotalSeconds));
                tbarPlaying.Value = lvSec;
                if (_audioPlayer.FinishedSongs.Count > 0)
                {
                    while (_audioPlayer.FinishedSongs.Count > 0)
                    {
                        UpdateDataGrid(_audioPlayer.FinishedSongs[0]);
                        _audioPlayer.FinishedSongs.RemoveAt(0);
                    }
                }
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


















    }



}
