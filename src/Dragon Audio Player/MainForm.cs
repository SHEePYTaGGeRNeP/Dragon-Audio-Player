using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Dragon_Audio_Player
{
    //      ---------------------------------------------
    //      |   Product:    Dragon Audio Player         |
    //      |   By:         SHEePYTaGGeRNeP             |
    //      |   Date:       02/5/2014                   |
    //      |   Version:    0.1                         |
    //      |   Copyright © Double Dutch Dragons 2014   |
    //      ---------------------------------------------

    public partial class MainForm : Form
    {

        /// <BUGS>
        /// 
        /// TODO:
        /// Check start up volume / properties
        /// Of Wolf And Man - End Stuck
        /// Delete songs
        /// Use JSONLint to edit
        /// Save column width
        /// Reset playcount
        /// time text / max length
        /// 
        /// </summary>

        bool mouseDown = false;

        DrgnAudioPlayer audioPlayer;

        NAudio.Wave.PlaybackState PlayState { get { return audioPlayer.PlayingState; } }


        #region >< >< >< >< >< >< >< >< >< ><  F O R M   >< >< >< >< >< >< >< >< >< >< >< ><

        public MainForm()
        {
            InitializeComponent();
            tbarPlaying.MouseWheel += new MouseEventHandler(doNothing_MouseWheel);
            audioPlayer = new DrgnAudioPlayer();
            audioPlayer.LoadPlaylists();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string s in audioPlayer.GetPlaylistNames())
                cbxmiPlaylistSelect.Items.Add(s);
            loadFromSettings();
            refreshDataGrid();
        }
        private void loadFromSettings()
        {
            try 
            {

                this.Size = Properties.Settings.Default.FormSize;
                double m_vol = Convert.ToDouble(Properties.Settings.Default.Volume);
                Console.WriteLine("Volume: " + m_vol.ToString());
                tbarVolume.Value = Convert.ToInt32(Convert.ToDouble(Properties.Settings.Default.Volume) * 100);
                if (Properties.Settings.Default.PlayingMode == "")
                    cbxmiPreferencesMode.SelectedIndex = 2;
                else
                    cbxmiPreferencesMode.SelectedIndex = cbxmiPreferencesMode.Items.IndexOf(Properties.Settings.Default.PlayingMode);
                cbxmiPlaylistSelect.SelectedIndex = cbxmiPlaylistSelect.Items.IndexOf(Properties.Settings.Default.LastPlayinglist);
                audioPlayer.SetPlaylist(cbxmiPlaylistSelect.Text);
            }
            catch (Exception ex)
            { MessageBox.Show("Error trying to load settings:\n" + ex.Message, "Loading error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            audioPlayer.SavePlaylists();
            setSettings();
            Properties.Settings.Default.Save();
            audioPlayer.CloseWaveOut();
        }
        private void setSettings()
        {
            try
            {
                Properties.Settings.Default.FormSize = this.Size;
                Properties.Settings.Default.Volume = Convert.ToInt32(audioPlayer.Volume);
                Properties.Settings.Default.PlayingMode = cbxmiPreferencesMode.Text;
                Properties.Settings.Default.LastPlayinglist = cbxmiPlaylistSelect.Text;
                Properties.Settings.Default.SongOutLocation = tbxmiPreferencesWTFLocation.Text;
            }
            catch (Exception ex)
            { MessageBox.Show("Error trying to save settings:\n" + ex.Message, "Saving error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void changeTitleSong(AudioFile p_af)
        {
            if (p_af != null)
                changeTitle(String.Format("[{0}] {1} - {2} / {3}", audioPlayer.PlayingState.ToString(),
                    p_af.ToString(), audioPlayer.CurrentTimeString, p_af.DurationString));
        }
        private void changeTitle(string p_text)
        {
            if (p_text != "")
                this.Text = p_text + " - " + AppInfo.AssemblyTitle;
            else
                this.Text = AppInfo.AssemblyTitle;
        }

        private void refreshDataGrid()
        {
            dgridSongs.Rows.Clear();
            foreach (AudioFile af in audioPlayer.CurrentPlaylist.Songs)
            {
                dgridSongs.Rows.Add(af.Title, af.Artist, af.Album, af.Year, af.DurationString, af.TimesPlayed, af.FileLocation);
            }
        }
        private void updateDataGrid(AudioFile p_af)
        {
            try
            {
                foreach (DataGridViewRow row in dgridSongs.Rows)
                {
                    if ((string)row.Cells[6].Value == p_af.FileLocation)
                        row.Cells[5].Value = p_af.TimesPlayed;
                }
            }
            catch (Exception ex)
            { MessageBox.Show("Error trying to update DataGrid:\n" + ex.Message, "Update error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void addAudioToGrid(AudioFile p_af)
        {
            dgridSongs.Rows.Add(p_af.Title, p_af.Artist, p_af.Album, p_af.Year, p_af.DurationString, p_af.TimesPlayed, p_af.FileLocation);
        }



        #endregion

        #region >< >< >< >< >< >< >< >< >< ><  M E N U   I T E M S  >< >< >< >< >< >< >< ><

        private void miFileAddFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select a folder containing audio files";
            if (DialogResult.OK == fbd.ShowDialog())
            {
                audioPlayer.AddFolder(fbd.SelectedPath);
                refreshDataGrid();
            }
        }
        private void miFileAddFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select one or more audio files";
            ofd.Multiselect = true;
            ofd.Filter = ".mp3 file(*.mp3)|*.mp3|.wav file(*.wav)|*.wav|.aac file(*.aac)|*.aac"
                + "|.flac file(*.flac)|*.flac|.mp4 file(*.mp4)|*.mp4|.wma file(*.wma)|*.wma|All files(*.*)|*.*";
            if (DialogResult.OK == ofd.ShowDialog())
            {
                foreach (string s in ofd.FileNames)
                {
                    audioPlayer.AddFile(s, true);
                    addAudioToGrid(audioPlayer.LastAudioFile);
                }
            }
        }
        private void miFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbmiPreferencesWriteToFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.WriteToFile = cbmiPreferencesWriteToFile.Text;

        }
        private void cbxmiPreferencesMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            audioPlayer.SetPlayingMode(cbxmiPreferencesMode.Text);
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
                audioPlayer.SetPlaylist(cbxmiPlaylistSelect.Text);
        }

        private void miHelpAbout_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.ShowDialog();
        }


        private void miPlay_Click(object sender, EventArgs e)
        {
            if (PlayState == NAudio.Wave.PlaybackState.Paused)
                audioPlayer.Play(null);
            else if (dgridSongs.SelectedRows.Count == 1)
                play(dgridSongs.SelectedRows[0].Cells[1].Value + " - " + dgridSongs.SelectedRows[0].Cells[0].Value);
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


        #region >< >< >< >< >< >< >< >< >< ><  M E D I A   C O N T R O L S  >< >< >< >< ><

        private void tbarPlaying_MouseUp(object sender, MouseEventArgs e)
        { mouseDown = false; }
        private void tbarPlaying_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            double dblValue;

            // Jump to the clicked location
            dblValue = ((double)e.X / (double)tbarPlaying.Width) * (tbarPlaying.Maximum - tbarPlaying.Minimum);
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
            if (audioPlayer.CurrentlyPlaying != null)
            {
                if (tbarPlaying.Value >= (int)audioPlayer.CurrentlyPlaying.Duration.TotalSeconds)
                    try
                    { audioPlayer.PlayNext(); }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error trying to play next song:\n" + ex.Message, "Next song error",
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                else
                {
                    if (mouseDown)
                    {
                        long m_seekValue = (long)TimeSpan.FromSeconds(tbarPlaying.Value).TotalMilliseconds;
                        seek(m_seekValue);
                    }
                }
            }
        }
        private void doNothing_MouseWheel(object sender, EventArgs e)
        {
            HandledMouseEventArgs ee = (HandledMouseEventArgs)e;
            ee.Handled = true;
        }

        private void tbarVolume_Scroll(object sender, EventArgs e)
        {
            float m_vol = (float)tbarVolume.Value / 100;
            audioPlayer.ChangeVolume(m_vol);
            Properties.Settings.Default.Volume = m_vol;
        }
        private void play(string p_string)
        {
            audioPlayer.Play(audioPlayer.GetFileByString(p_string));
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
            { MessageBox.Show("Error trying to play next song:\n" + ex.Message, "Next error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
            { MessageBox.Show("Error trying to set controls to next song:\n" + ex.Message, "Control error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
            { MessageBox.Show("Error trying to stop audio:\n" + ex.Message, "Stopping error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
            { audioPlayer.Seek(p_milliseconds, System.IO.SeekOrigin.Begin); }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to Seek to value: " + p_milliseconds + " milliseconds\n" + ex.Message, "Seek error",
                  MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void previous()
        {

        }

        #endregion


        #region >< >< >< >< >< >< >< >< >< ><  O T H E R  >< >< >< >< >< >< >< >< ><

        private void dgridSongs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
                if (dgridSongs.SelectedRows.Count == 1)
                    play(dgridSongs.SelectedRows[0].Cells[1].Value + " - " + dgridSongs.SelectedRows[0].Cells[0].Value);
        }

        private void timer1s_Tick(object sender, EventArgs e)
        {
            try
            {
                tbarPlaying.Maximum = Convert.ToInt32(Math.Ceiling((double)audioPlayer.CurrentlyPlaying.Duration.TotalSeconds));
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
                if (cbmiPreferencesWriteToFile.SelectedIndex == 2 && tbxmiPreferencesWTFLocation.Text != "")
                    writeSongInfo();
            }
            catch
            { tbarPlaying.Value = tbarPlaying.Maximum; }
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
                        case 1: StaticClass.WriteToFile(tbxmiPreferencesWTFLocation.Text,
                            String.Format("                 [{0}] {1} - {2}", audioPlayer.PlayingState.ToString(),
                            audioPlayer.CurrentlyPlaying.Artist, audioPlayer.CurrentlyPlaying.Title)); break;
                        // Write every second
                        case 2: StaticClass.WriteToFile(tbxmiPreferencesWTFLocation.Text,
                            String.Format("                 [{0}] {1} - {2} {3} / {4}", audioPlayer.PlayingState.ToString(),
                            audioPlayer.CurrentlyPlaying.Artist, audioPlayer.CurrentlyPlaying.Title, audioPlayer.CurrentTimeString,
                            audioPlayer.CurrentlyPlaying.DurationString)); break;
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show("Error trying to write song to textfile:\n" + ex.Message, "Writing error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }


        #endregion















    }



}
