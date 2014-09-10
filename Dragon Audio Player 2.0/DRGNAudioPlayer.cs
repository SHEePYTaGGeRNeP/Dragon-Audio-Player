using System;
using System.Collections.Generic;
using System.Text;
using NAudio;
using NAudio.Wave;
using System.IO;
using TagLib;
using System.Windows.Forms;
using System.Linq;


namespace Dragon_Audio_Player
{
    //      ---------------------------------------------
    //      |   Product:    Dragon Audio Player         |
    //      |   By:         SHEePYTaGGeRNeP             |
    //      |   Date:       26/06/2014                  |
    //      |   Version:    0.2                         |
    //      |   Copyright © Double Dutch Dragons 2014   |
    //      ---------------------------------------------


    public class DRGNAudioPlayer
    {
        private const string FileName = "Playlists.txt";

        private List<string> LogMessages = new List<string>();

        public enum ePlayingMode
        {
            Smart,
            Random,
            Normal
        };

        public ePlayingMode PlayingMode { get; set; }
        private IWavePlayer waveOutDevice;
        private AudioFileReader audioFileReader;

        private List<PlayList> playlists;
        private PlayList currentPlaylist;
        private AudioFile currentyPlaying;

        public List<PlayList> Playlists
        {
            get { return playlists; }
        }

        public PlayList CurrentPlaylist
        {
            get { return currentPlaylist; }
        }

        public AudioFile CurrentlyPlaying
        {
            get { return currentyPlaying; }
        }

        public TimeSpan CurrentTime
        {
            get { return audioFileReader.CurrentTime; }
        }

        public string CurrentTimeString
        {
            get { return StaticClass.GetTimeString(CurrentTime); }
        }

        public PlaybackState PlayingState
        {
            get { return waveOutDevice.PlaybackState; }
        }

        public float Volume
        {
            get { return waveOutDevice.Volume; }
        }

        public List<AudioFile> FinishedSongs = new List<AudioFile>();

        public string SavePlaylistsDirectory { get; set; }


        public DRGNAudioPlayer()
        {
            try
            {
                waveOutDevice = new WaveOut();
                waveOutDevice.PlaybackStopped += new EventHandler<StoppedEventArgs>(PlayBackEnds);
                playlists = new List<PlayList>();
                playlists.Add(new PlayList("All", null, null, null, false, true));
                currentPlaylist = playlists[0];
                if (
                    !Directory.Exists(Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player")))
                {
                    Directory.CreateDirectory(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Dragon Audio Player"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DRGNAudioPlayer: " + ex.Message);
                MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public AudioFile LastAudioFile
        {
            get
            {
                if (playlists[0].Songs.Count > 0)
                    return playlists[0].Songs[playlists[0].Songs.Count - 1];
                else return null;
            }
        }


        public void SetPlayingMode(string p_text)
        {
            string m_text = p_text.ToLower();
            if (m_text.Contains("normal"))
                PlayingMode = ePlayingMode.Normal;
            else if (m_text.Contains("random"))
                PlayingMode = ePlayingMode.Random;
            else
                PlayingMode = ePlayingMode.Smart;
        }

        public void PlayNext()
        {
            try
            {
                if (currentPlaylist.Songs.Count > 0)
                {
                    if (currentyPlaying != null)
                    {
                        currentyPlaying.TimesPlayed++;
                        FinishedSongs.Add(CurrentlyPlaying);
                    }
                    switch (PlayingMode)
                    {
                        case ePlayingMode.Normal:
                            PlayNextNormal();
                            break;
                        case ePlayingMode.Random:
                            PlayNextRandom();
                            break;
                        case ePlayingMode.Smart:
                            PlayNextSmart();
                            break;
                    }
                }
                else
                    MessageBox.Show("Playlist is empty.", "Empty playlist", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine("PlayNext: " + ex.Message);
                MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PlayNextNormal()
        {
            try
            {
                int m_index = currentPlaylist.Songs.IndexOf(currentyPlaying);
                if (m_index >= currentPlaylist.Songs.Count - 1)
                    Play(currentPlaylist.Songs[0]);
                else
                    Play(currentPlaylist.Songs[m_index + 1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("PlayNextNormal: " + ex.Message);
                MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PlayNextRandom()
        {
            try
            {
                Random r = new Random();
                int i = r.Next(currentPlaylist.Songs.Count - 1);
                Play(currentPlaylist.Songs[i]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("PlayNextRandom: " + ex.Message);
                MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ;
            }
        }

        private void PlayNextSmart()
        {
            try
            {
                AudioFile[] m_songs = currentPlaylist.GetLowestTimesPlayed();
                Random r = new Random();
                Play(currentPlaylist.GetSongByLocation(m_songs[r.Next(m_songs.Length - 1)].FileLocation));
            }
            catch (Exception ex)
            {
                Console.WriteLine("PlayNextSmart: " + ex.Message);
                MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ;
            }
        }

        // FIX  position = 175K  Length = 5M
        private void PlayBackEnds(object sender, StoppedEventArgs e)
        {
            if (audioFileReader.Position >= audioFileReader.Length)
                PlayNext();
        }


        public void Play(AudioFile p_file)
        {
            if (p_file != null)
            {
                try
                {
                    if (PlayingState == PlaybackState.Playing)
                        if (currentyPlaying.FileLocation != p_file.FileLocation)
                            Stop();
                        else
                        {
                            Pause();
                            return;
                        }
                    if (PlayingState != PlaybackState.Paused)
                    {
                        audioFileReader = new AudioFileReader(p_file.FileLocation);
                        WaveChannel32 wc = new NAudio.Wave.WaveChannel32(audioFileReader);
                        wc.PadWithZeroes = false;
                        waveOutDevice = new WaveOut();
                        waveOutDevice.Init(wc);
                        waveOutDevice.PlaybackStopped += new EventHandler<StoppedEventArgs>(PlayBackEnds);
                    }
                    waveOutDevice.Play();
                    currentyPlaying = p_file;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Play: " + ex.Message);
                    MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ;
                }
            }
            else if (PlayingState == PlaybackState.Paused && currentyPlaying != null)
                waveOutDevice.Play();
            else
                PlayNext();
        }

        public void Stop()
        {
            if (waveOutDevice != null)
            {
                try
                {
                    waveOutDevice.Stop();
                    currentyPlaying = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Stop: " + ex.Message);
                    MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ;
                }
            }
        }

        public void Pause()
        {
            if (waveOutDevice != null)
                try
                {
                    waveOutDevice.Pause();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Pause: " + ex.Message);
                    MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.");
                    ;
                }
        }

        public void Seek(long pMilliseconds, SeekOrigin p_seekOrigin)
        {
            if (audioFileReader != null)
            {
                try
                {
                    waveOutDevice.Pause();
                    audioFileReader.CurrentTime =
                        TimeSpan.FromMilliseconds(audioFileReader.Seek(pMilliseconds, p_seekOrigin));
                    waveOutDevice.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Seek: " + ex.Message);
                    MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ;
                }
            }
            else
                throw new ArgumentNullException("Cannot seek audio to " + pMilliseconds + ":\naudioFileReader is null");
        }

        public void ChangeVolume(float p_volume)
        {
            try
            {
                waveOutDevice.Volume = p_volume;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Volume: " + ex.Message);
                MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ;
            }
        }


        public void AddFolder(string p_path, bool p_autoreset)
        {
            try
            {
                string[] m_files = Directory.GetFiles(p_path);
                foreach (string s in m_files)
                    if (s.EndsWith(".mp3") || s.EndsWith(".wav") || s.EndsWith(".wma") || s.EndsWith(".flac")
                        || s.EndsWith(".mp4") || s.EndsWith(".aac"))
                        AddFile(s, false, false);
                if (!currentPlaylist.Directories.Contains(p_path))
                    currentPlaylist.Directories.Add(p_path);
                if (p_autoreset)
                    currentPlaylist.ResetTimesPlayed();
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddFolder: " + ex.Message);
                MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ;
            }
        }

        public void AddFile(string p_fileLocation, bool p_addIfExists, bool p_autoreset)
        {
            if (currentPlaylist.GetSongByLocation(p_fileLocation) != null && p_addIfExists == false)
            {
            }
            else
            {
                try
                {
                    TagLib.File tagFile = TagLib.File.Create(p_fileLocation);
                    string m_title = tagFile.Tag.Title;
                    string m_artist = StaticClass.GetArtist(tagFile);
                    string m_album = tagFile.Tag.Album;
                    uint m_year = tagFile.Tag.Year;
                    TimeSpan m_duration = tagFile.Properties.Duration;

                    currentPlaylist.Songs.Add(new AudioFile(p_fileLocation, m_title, m_artist, m_album, m_year,
                        m_duration));
                    if (currentPlaylist.AutoResetOnSync && p_autoreset)
                        currentPlaylist.ResetTimesPlayed();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AddFile: " + ex.Message);
                    MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ;
                }
            }
        }

        public void RemoveSong(string p_fileLocation)
        {
            try
            {
                AudioFile m_af = currentPlaylist.GetSongByLocation(p_fileLocation);
                if (currentyPlaying == m_af)
                    Stop();
                currentPlaylist.Songs.Remove(m_af);
                currentPlaylist.Ignoredfiles.Add(m_af.FileLocation);
            }
            catch (Exception ex)
            {
                Console.WriteLine("RemoveSong: " + ex.Message);
                MessageBox.Show("An error occurred:\n" + ex.Message + "\nReport this error.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public PlayList GetPlaylist(string p_name)
        {
            foreach (PlayList p in Playlists)
                if (p.Name.ToLower() == p_name.ToLower())
                    return p;
            return null;
        }

        public string[] GetPlaylistNames()
        {
            List<string> m_names = new List<string>();
            foreach (PlayList p in Playlists)
                m_names.Add(p.Name);
            return m_names.ToArray();
        }

        // CHANGED PLAYLIST = TRUE
        public bool SetPlaylist(string p_name)
        {
            if (p_name == null || p_name == "")
            {
                throw new ArgumentNullException("Playlist name cannot be null or empty.");
            }
            try
            {
                PlayList m_pl = GetPlaylist(p_name);
                if (m_pl == currentPlaylist)
                    return false;
                else
                {
                    currentPlaylist = GetPlaylist(p_name);
                    Stop();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetPlaylist: " + ex.Message);
                MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ;
                return false;
            }
        }


        public void SavePlaylists()
        {
            try
            {
                string m_text = "To view this easier go to http://jsonlint.com/ \n" +
                                StaticClass.PlaylistsListToJSON(playlists);
                if (SavePlaylistsDirectory == null)
                {
                    SavePlaylistsDirectory =
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Dragon Audio Player");
                    SavePlaylistsDirectory = Path.Combine(SavePlaylistsDirectory, FileName);
                }
                StaticClass.WriteToFile(SavePlaylistsDirectory, m_text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SavePlayLists(): " + ex.Message);
                MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ;
            }
        }

        public void LoadPlaylists()
        {
            try
            {
                if (SavePlaylistsDirectory == null)
                {
                    SavePlaylistsDirectory =
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Dragon Audio Player");
                    SavePlaylistsDirectory = Path.Combine(SavePlaylistsDirectory, FileName);
                }
                if (System.IO.File.Exists(SavePlaylistsDirectory))
                {
                    try
                    {
                        List<PlayList> m_temp =
                            StaticClass.JSONToPlaylistsList(System.IO.File.ReadAllText(SavePlaylistsDirectory));
                    }
                    catch
                    {
                        return;
                    } // file is empty
                    playlists = StaticClass.JSONToPlaylistsList(System.IO.File.ReadAllText(SavePlaylistsDirectory));
                    if (playlists.Count == 0)
                        throw new Exception("playlists.Playlists.Count is 0.");
                    else
                        currentPlaylist = playlists[0];
                    foreach (PlayList pl in playlists)
                    {
                        UpdatePlaylist(pl);
                    }
                    if (LogMessages.Count > 0)
                    {
                        string m_message = "";
                        foreach (string s in LogMessages)
                            m_message += s + "\n";
                        MessageBox.Show(m_message, "Playlists have been updated", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        LogMessages.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadPlaylists: " + ex.Message);
                MessageBox.Show("An error occurred:\n" + ex.Message + " \nReport this error.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePlaylist(PlayList p_pl)
        {
            try
            {
                if (p_pl.Sync == true)
                {
                    bool m_added = false;
                    foreach (string s in p_pl.Directories)
                        foreach (string s2 in Directory.GetFiles(s))
                            if (StaticClass.EndsWithAudioFileType(s2) && !p_pl.SongExistsInPlaylist(s2) &&
                                !p_pl.SongExistsInIgnoredFiles(s2))
                            {
                                m_added = true;
                                AddFile(s2, false, p_pl.AutoResetOnSync);
                                LogMessages.Add("Added " + Path.GetFileName(s2) + " to playlist: " + p_pl.Name + ".");
                            }
                    if (m_added)
                        p_pl.ResetTimesPlayed();
                }
                List<AudioFile> m_remove = new List<AudioFile>();
                foreach (AudioFile af in p_pl.Songs)
                    if (!System.IO.File.Exists(af.FileLocation))
                    {
                        m_remove.Add(af);
                        LogMessages.Add("Removed " + Path.GetFileName(af.FileLocation) + " from playlist: " + p_pl.Name +
                                        ".");
                    }
                foreach (AudioFile af in m_remove)
                    p_pl.Songs.Remove(af);
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdatePlaylist: " + ex.Message);
                try
                {
                    MessageBox.Show("Error updating playlist " + p_pl.Name + ":\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                    // in case p_pl.name = null
                catch
                {
                    MessageBox.Show("Error updating playlist:\n" + ex.Message, "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        public void CloseWaveOut()
        {
            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();
            }
            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }
            if (waveOutDevice != null)
            {
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }
        }
    }


    public class AudioFile
    {
        public string Title { get; set; }
        public string FileLocation { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public uint Year { get; set; }
        public TimeSpan Duration { get; set; }

        public string DurationString
        {
            get { return StaticClass.GetTimeString(Duration); }
        }

        public int TimesPlayed { get; set; }


        public AudioFile(string p_loc, string p_title, string p_artist, string p_album, uint p_year,
            TimeSpan p_dur)
        {
            FileLocation = p_loc;
            Title = p_title;
            Artist = p_artist;
            Album = p_album;
            Year = p_year;
            Duration = p_dur;
        }


        public override string ToString()
        {
            return Artist + " - " + Title;
        }
    }

    public class PlayList
    {
        public string Name { get; set; }
        public bool Sync = false;
        public bool AutoResetOnSync = true;
        public List<string> Ignoredfiles { get; set; }
        public List<string> Directories { get; set; }
        public List<AudioFile> Songs { get; set; }


        public PlayList(string p_name, List<AudioFile> p_songs, List<string> p_directories, List<string> p_ignoredfiles,
            bool p_sync, bool p_autoresetonsync)
        {
            Name = p_name;
            Sync = p_sync;
            AutoResetOnSync = p_autoresetonsync;
            Ignoredfiles = new List<string>();
            if (p_ignoredfiles != null)
                foreach (string s in p_ignoredfiles)
                    Ignoredfiles.Add(s);
            Directories = new List<string>();
            if (p_directories != null)
                foreach (string s in p_directories)
                    Directories.Add(s);
            Songs = new List<AudioFile>();
            if (p_songs != null)
                foreach (AudioFile af in p_songs)
                    Songs.Add(af);
        }


        public AudioFile GetSongByLocation(string p_fileLocation)
        {
            foreach (AudioFile af in Songs)
                if (af.FileLocation.ToLower() == p_fileLocation.ToLower())
                    return af;
            return null;
        }

        public AudioFile[] GetLowestTimesPlayed()
        {
            int m_lowest = int.MaxValue;
            foreach (AudioFile af in Songs)
                if (af.TimesPlayed < m_lowest)
                    m_lowest = af.TimesPlayed;

            List<AudioFile> m_list = new List<AudioFile>();
            foreach (AudioFile af in Songs)
                if (af.TimesPlayed == m_lowest)
                    m_list.Add(af);
            return m_list.ToArray();
        }

        public AudioFile GetLastSong()
        {
            if (Songs.Count > 0)
                return Songs[Songs.Count - 1];
            else
                return null;
        }

        public bool SongExistsInPlaylist(string p_path)
        {
            foreach (AudioFile af in Songs)
                if (af.FileLocation.ToLower() == p_path.ToLower())
                    return true;
            return false;
        }

        public bool SongExistsInIgnoredFiles(string p_path)
        {
            foreach (string s in Ignoredfiles)
                if (s.ToLower() == p_path.ToLower())
                    return true;
            return false;
        }

        public void ResetTimesPlayed()
        {
            foreach (AudioFile af in Songs)
                af.TimesPlayed = 0;
        }
    }
}