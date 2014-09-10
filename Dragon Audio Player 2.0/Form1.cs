using System;
using System.IO;
using System.Windows.Forms;
using Dragon_Audio_Player.Properties;
using NAudio.Wave;

namespace Dragon_Audio_Player
{
    //      ---------------------------------------------
    //      |   Product:    Dragon Audio Player         |
    //      |   By:         SHEePYTaGGeRNeP             |
    //      |   Date:       26/06/2014                  |
    //      |   Version:    0.2                         |
    //      |   Copyright © Double Dutch Dragons 2014   |
    //      ---------------------------------------------

    /// BUGS
    /// 
    /// TODO:
    /// Check Writesong - "Never" - I removed array value in UI.
    /// Of Wolf And Man - End Stuck
    /// Save column width
    /// Reset playcount
    /// time text / max length
    public partial class Form1 : Form
    {
        private readonly DRGNAudioPlayer audioPlayer;
        private bool mouseDown;

        #region >< >< >< >< >< >< >< >< >< ><  F O R M   >< >< >< >< >< >< >< >< >< >< >< ><

        public Form1()
        {
            InitializeComponent();
            tbarPlaying.MouseWheel += doNothing_MouseWheel;
            audioPlayer = new DRGNAudioPlayer();
            audioPlayer.LoadPlaylists();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string s in audioPlayer.GetPlaylistNames())
                cbxmiPlaylistSelect.Items.Add(s);
            loadFromSettings();
            LoadPlaylistSettings();
            dgridSongs.MouseDown += dgridSongs_MouseDown;
            refreshDataGrid();
        }

        private void loadFromSettings()
        {
            try
            {
                Size = Settings.Default.FormSize;
                float m_vol = Settings.Default.Volume;
                Console.WriteLine("Volume: " + m_vol);
                tbarVolume.Value = Convert.ToInt32(Convert.ToDouble(Settings.Default.Volume)*100);
                if (Settings.Default.PlayingMode == "")
                    cbxmiPreferencesMode.SelectedIndex = 2;
                else
                    cbxmiPreferencesMode.SelectedIndex = cbxmiPreferencesMode.Items.IndexOf(Settings.Default.PlayingMode);
                cbxmiPlaylistSelect.SelectedIndex = cbxmiPlaylistSelect.Items.IndexOf(Settings.Default.LastPlayinglist);
                audioPlayer.SetPlaylist(cbxmiPlaylistSelect.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to load settings:\n" + ex.Message, "Loading error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadPlaylistSettings()
        {
            PlayList m_pl = audioPlayer.CurrentPlaylist;
            if (m_pl.Sync)
                cbxmiPlaylistSPSync.SelectedIndex = 0;
            else
                cbxmiPlaylistSPSync.SelectedIndex = 1;
            if (m_pl.AutoResetOnSync)
                cbxmiPlaylistSPAutoReset.SelectedIndex = 0;
            else
                cbxmiPlaylistSPAutoReset.SelectedIndex = 1;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (audioPlayer != null && audioPlayer.PlayingState == PlaybackState.Playing)
                if (DialogResult.Yes ==
                    MessageBox.Show("Are you sure you want to exit?", "Exit?", MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Information))
                {
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            audioPlayer.SavePlaylists();
            setSettings();
            Settings.Default.Save();
            audioPlayer.CloseWaveOut();
        }

        private void setSettings()
        {
            try
            {
                Settings.Default.FormSize = Size;
                Settings.Default.Volume = audioPlayer.Volume;
                Settings.Default.PlayingMode = cbxmiPreferencesMode.Text;
                Settings.Default.LastPlayinglist = cbxmiPlaylistSelect.Text;
                Settings.Default.SongOutLocation = tbxmiPreferencesWTFLocation.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to save settings:\n" + ex.Message, "Saving error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void changeTitleSong(AudioFile p_af)
        {
            if (p_af != null)
                changeTitle(String.Format("[{0}] {1} - {2} / {3}", audioPlayer.PlayingState,
                    p_af, audioPlayer.CurrentTimeString, p_af.DurationString));
        }

        private void changeTitle(string p_text)
        {
            if (p_text != "")
                Text = p_text + " - " + AppInfo.AssemblyTitle;
            else
                Text = AppInfo.AssemblyTitle;
        }

        private void refreshDataGrid()
        {
            dgridSongs.Rows.Clear();
            foreach (AudioFile af in audioPlayer.CurrentPlaylist.Songs)
            {
                dgridSongs.Rows.Add(af.Title, af.Artist, af.Album, af.Year, af.DurationString, af.TimesPlayed,
                    af.FileLocation);
            }
        }

        private void updateDataGrid(AudioFile p_af)
        {
            try
            {
                foreach (DataGridViewRow row in dgridSongs.Rows)
                {
                    if ((string) row.Cells[6].Value == p_af.FileLocation)
                        row.Cells[5].Value = p_af.TimesPlayed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to update DataGrid:\n" + ex.Message, "Update error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void addAudioToGrid(AudioFile p_af)
        {
            dgridSongs.Rows.Add(p_af.Title, p_af.Artist, p_af.Album, p_af.Year, p_af.DurationString, p_af.TimesPlayed,
                p_af.FileLocation);
        }

        #endregion

        #region >< >< >< >< >< >< >< >< >< ><  M E N U   I T E M S  >< >< >< >< >< >< >< ><

        private void miFileAddFolder_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            fbd.Description = "Select a folder containing audio files";
            if (DialogResult.OK == fbd.ShowDialog())
            {
                audioPlayer.AddFolder(fbd.SelectedPath, audioPlayer.CurrentPlaylist.AutoResetOnSync);
                refreshDataGrid();
            }
        }

        private void miFileAddFiles_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Select one or more audio files";
            ofd.Multiselect = true;
            ofd.Filter = ".mp3 file(*.mp3)|*.mp3|.wav file(*.wav)|*.wav|.aac file(*.aac)|*.aac"
                         + "|.flac file(*.flac)|*.flac|.mp4 file(*.mp4)|*.mp4|.wma file(*.wma)|*.wma|All files(*.*)|*.*";
            if (DialogResult.OK == ofd.ShowDialog())
            {
                foreach (string s in ofd.FileNames)
                {
                    audioPlayer.AddFile(s, true, true);
                    addAudioToGrid(audioPlayer.LastAudioFile);
                }
            }
        }

        private void miFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbmiPreferencesWriteToFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.WriteToFile = cbmiPreferencesWriteToFile.Text;
        }

        private void cbxmiPreferencesMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            audioPlayer.SetPlayingMode(cbxmiPreferencesMode.Text);
        }

        private void cbxmiPlaylistSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                audioPlayer.Stop();
                if (audioPlayer.SetPlaylist(cbxmiPlaylistSelect.Text))
                    refreshDataGrid();
                LoadPlaylistSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine("cbxmiPlaylistSelect_SelectedIndexChanged: " + ex.Message);
                MessageBox.Show(
                    "Something went horribly wrong report this - cbxmiPlaylistSelect_Click:\n" + ex.Message, "BIG ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void miPlaylistNewCreate_Click(object sender, EventArgs e)
        {
            if (tbxmiPlaylistNew.Text == "")
                MessageBox.Show("Please enter a playlist name.", "Playlist name", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            else
            {
                MessageBox.Show("Not implemented yet.");
            }
        }


        private void cbxmiPlaylistSPSync_SelectedIndexChanged(object sender, EventArgs e)
        {
            audioPlayer.CurrentPlaylist.Sync = cbxmiPlaylistSPSync.SelectedIndex == 0;
        }

        private void cbxmiPlaylistSPAutoReset_SelectedIndexChanged(object sender, EventArgs e)
        {
            audioPlayer.CurrentPlaylist.AutoResetOnSync = cbxmiPlaylistSPAutoReset.SelectedIndex == 0;
        }

        private void miSelectedPlaylistResetTimesPlayed_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes ==
                MessageBox.Show("Are you sure you want to set the times played on all songs in this playlist to 0?",
                    "Reset times played?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
            {
                audioPlayer.CurrentPlaylist.ResetTimesPlayed();
                foreach (DataGridViewRow dgvr in dgridSongs.Rows)
                    dgvr.Cells["clmTimesPlayed"].Value = 0;
            }
        }


        private void miHelpAbout_Click(object sender, EventArgs e)
        {
            var ab = new AboutBox1();
            ab.ShowDialog();
        }


        private void miPlay_Click(object sender, EventArgs e)
        {
            if (audioPlayer.PlayingState == PlaybackState.Paused)
                audioPlayer.Play(null);
            else if (dgridSongs.SelectedRows.Count == 1)
                play(dgridSongs.SelectedRows[0].Cells["clmLocation"].Value.ToString());
            else
                playNext();
        }

        private void miStop_Click(object sender, EventArgs e)
        {
            stop();
        }

        private void miPause_Click(object sender, EventArgs e)
        {
            pause();
        }

        private void miPrevious_Click(object sender, EventArgs e)
        {
            previous();
        }

        private void miNext_Click(object sender, EventArgs e)
        {
            playNext();
        }

        #endregion

        #region >< >< >< >< >< >< >< >< >< ><  M E D I A   U I  >< >< >< >< ><

        private void tbarPlaying_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void tbarPlaying_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            double dblValue;

            // Jump to the clicked location
            dblValue = (e.X/(double) tbarPlaying.Width)*(tbarPlaying.Maximum - tbarPlaying.Minimum);
            tbarPlaying.Value = Convert.ToInt32(dblValue);
        }

        private void tbarPlaying_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                mouseDown = true;
        }

        private void tbarPlaying_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                mouseDown = false;
        }

        private void tbarPlaying_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (audioPlayer.CurrentlyPlaying != null)
                {
                    if (tbarPlaying.Value >= (int) audioPlayer.CurrentlyPlaying.Duration.TotalSeconds)
                        try
                        {
                            audioPlayer.PlayNext();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error trying to play next song:\n" + ex.Message, "Next song error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    else
                    {
                        if (mouseDown)
                        {
                            var m_seekValue = (long) TimeSpan.FromSeconds(tbarPlaying.Value).TotalMilliseconds;
                            seek(m_seekValue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("tbarPlaying_ValueChanged(): " + ex.Message);
                MessageBox.Show("An error occurred while seeking in trackbak:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void doNothing_MouseWheel(object sender, EventArgs e)
        {
            var ee = (HandledMouseEventArgs) e;
            ee.Handled = true;
        }


        private void dgridSongs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
                if (dgridSongs.SelectedRows.Count == 1)
                    play(dgridSongs.SelectedRows[0].Cells["clmLocation"].Value.ToString());
        }

        private void dgridSongs_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hti = dgridSongs.HitTest(e.X, e.Y);
                dgridSongs.ClearSelection();
                dgridSongs.Rows[hti.RowIndex].Selected = true;
            }
        }

        private void dgridSongs_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                string m_message = "Are you sure you want to delete " + audioPlayer.CurrentPlaylist.GetSongByLocation(
                    dgridSongs.SelectedRows[0].Cells["clmLocation"].Value.ToString()) + "?";
                if (dgridSongs.SelectedRows.Count > 1)
                    m_message = "Are you sure you want to delete these " + dgridSongs.SelectedRows.Count + " files?";
                if (DialogResult.Yes ==
                    MessageBox.Show(m_message, "Delete file(s)?", MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Information))
                {
                    bool m_fromPC = false;
                    if (e.Modifiers == Keys.Shift)
                    {
                        DialogResult m_result = MessageBox.Show("Do also wish to delete these file(s) from your PC?",
                            "Remove from PC? ",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                        if (m_result == DialogResult.Yes)
                            m_fromPC = true;
                        else if (m_result == DialogResult.Cancel)
                            return;
                    }
                    foreach (DataGridViewRow dgvr in dgridSongs.SelectedRows)
                    {
                        audioPlayer.RemoveSong(dgvr.Cells["clmLocation"].Value.ToString());
                        if (m_fromPC)
                            File.Delete(dgvr.Cells["clmLocation"].Value.ToString());
                        dgridSongs.Rows.Remove(dgvr);
                    }
                }
            }
        }

        private void ctxmibtnDeleteFromPlaylist_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgridSongs.SelectedRows.Count == 1)
                {
                    AudioFile m_af = audioPlayer.CurrentPlaylist.GetSongByLocation
                        (dgridSongs.SelectedRows[0].Cells["clmLocation"].Value.ToString());
                    if (DialogResult.Yes ==
                        MessageBox.Show("Are you sure you want to delete: " + m_af + "?", "Delete song?",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
                    {
                        audioPlayer.RemoveSong(m_af.FileLocation);
                        dgridSongs.Rows.Remove(dgridSongs.SelectedRows[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteFromPlaylist(): " + ex.Message);
                MessageBox.Show("An error occurred while removing song:\n" + ex.Message, "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ctxmibtnPlaycountOK_Click(object sender, EventArgs e)
        {
            if (dgridSongs.SelectedRows.Count != 1)
                MessageBox.Show("FIX DEZE SHIT JONGEN");
            if (ctxmitbxPlaycount.Text == "")
                MessageBox.Show("Fill in a number.", "Text cannot be empty", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            else
            {
                try
                {
                    int m_result = 0;
                    bool m_ok = Int32.TryParse(ctxmitbxPlaycount.Text, out m_result);
                    if (m_ok)
                    {
                        AudioFile m_af =
                            audioPlayer.CurrentPlaylist.GetSongByLocation(
                                dgridSongs.SelectedRows[0].Cells["clmLocation"].Value.ToString());
                        m_af.TimesPlayed = m_result;
                        dgridSongs.SelectedRows[0].Cells["clmTimesPlayed"].Value = m_result;
                    }
                    else
                        MessageBox.Show("Fill in a correct number.", "Cannot convert to number", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ctxmibtnPlaycountOK_Click(): " + ex.Message);
                    MessageBox.Show("An error occurred while setting playcount:\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tbarVolume_Scroll(object sender, EventArgs e)
        {
            float m_vol = (float) tbarVolume.Value/100;
            audioPlayer.ChangeVolume(m_vol);
            Settings.Default.Volume = m_vol;
        }

        #endregion

        #region >< >< >< >< >< >< >< >< >< ><  M E D I A   C O N T R O L S  >< >< >< >< ><

        private void play(string p_string)
        {
            audioPlayer.Play(audioPlayer.CurrentPlaylist.GetSongByLocation(p_string));
            changeTitleSong(audioPlayer.CurrentlyPlaying);
            afterPlay();
        }

        private void playNext()
        {
            try
            {
                audioPlayer.PlayNext();
                afterPlay();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to play next song:\n" + ex.Message, "Next error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void afterPlay()
        {
            try
            {
                tbarPlaying.Value = 0;
                // Rounds up always, 170.1 = 171
                if (timer1s.Enabled == false)
                    timer1s.Start();
                changeTitleSong(audioPlayer.CurrentlyPlaying);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to set controls to next song:\n" + ex.Message, "Control error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stop()
        {
            try
            {
                audioPlayer.Stop();
                timer1s.Stop();
                changeTitle("");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to stop audio:\n" + ex.Message, "Stopping error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void pause()
        {
            audioPlayer.Pause();
            changeTitleSong(audioPlayer.CurrentlyPlaying);
            timer1s.Stop();
        }

        private void seek(long p_milliseconds)
        {
            try
            {
                audioPlayer.Seek(p_milliseconds, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to Seek to value: " + p_milliseconds + " milliseconds\n" + ex.Message,
                    "Seek error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void previous()
        {
        }

        #endregion

        #region >< >< >< >< >< >< >< >< >< ><  O T H E R  >< >< >< >< >< >< >< >< ><

        private void timer1s_Tick(object sender, EventArgs e)
        {
            try
            {
                tbarPlaying.Maximum = Convert.ToInt32(Math.Ceiling(audioPlayer.CurrentlyPlaying.Duration.TotalSeconds));
                int m_sec = Convert.ToInt32(Math.Round(audioPlayer.CurrentTime.TotalSeconds));
                tbarPlaying.Value = m_sec;
                if (audioPlayer.FinishedSongs.Count > 0)
                {
                    while (audioPlayer.FinishedSongs.Count > 0)
                    {
                        updateDataGrid(audioPlayer.FinishedSongs[0]);
                        audioPlayer.FinishedSongs.RemoveAt(0);
                    }
                    writeSongInfo();
                }
                if (cbmiPreferencesWriteToFile.SelectedIndex == 1 && tbxmiPreferencesWTFLocation.Text != "")
                    writeSongInfo();
            }
            catch
            {
                if (audioPlayer.CurrentlyPlaying == null)
                    timer1s.Stop();
                tbarPlaying.Value = tbarPlaying.Maximum;
            }
            changeTitleSong(audioPlayer.CurrentlyPlaying);
        }

        private void writeSongInfo()
        {
            try
            {
                if (tbxmiPreferencesWTFLocation.Text != "" && tbxmiPreferencesWTFLocation.Text.EndsWith(".txt"))
                {
                    switch (cbmiPreferencesWriteToFile.SelectedIndex)
                    {
                            // 0 = dont write
                            // Write on new song
                        case 0:
                            StaticClass.WriteToFile(tbxmiPreferencesWTFLocation.Text,
                                String.Format("                 [{0}] {1} - {2}", audioPlayer.PlayingState,
                                    audioPlayer.CurrentlyPlaying.Artist, audioPlayer.CurrentlyPlaying.Title));
                            break;
                            // Write every second
                        case 1:
                            StaticClass.WriteToFile(tbxmiPreferencesWTFLocation.Text,
                                String.Format("                 [{0}] {1} - {2} {3} / {4}", audioPlayer.PlayingState,
                                    audioPlayer.CurrentlyPlaying.Artist, audioPlayer.CurrentlyPlaying.Title,
                                    audioPlayer.CurrentTimeString,
                                    audioPlayer.CurrentlyPlaying.DurationString));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to write song to textfile:\n" + ex.Message, "Writing error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        //NAudio.Wave.PlaybackState PlayState { get { return audioPlayer.PlayingState; } }
    }
}