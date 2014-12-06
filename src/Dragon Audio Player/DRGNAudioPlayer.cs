using System;
using System.Collections.Generic;
using System.Text;

using NAudio;
using NAudio.Wave;
using System.IO;
using Dragon_Audio_Player;
using TagLib;

namespace Dragon_Audio_Player
{
    //      ---------------------------------------------
    //      |   Product:    Dragon Audio Player         |
    //      |   By:         SHEePYTaGGeRNeP             |
    //      |   Date:       10/4/2014                   |
    //      |   Version:    0.1                         |
    //      |   Copyright © Double Dutch Dragons 2014   |
    //      ---------------------------------------------


    public class DrgnAudioPlayer
    {
        const string FileName = "Playlists.txt";

        public enum ePlayingMode { Smart, Random, Normal };
        public ePlayingMode PlayingMode { get; set; }
        private IWavePlayer waveOutDevice;
        private AudioFileReader audioFileReader;
        private List<PlayList> playlists;
        private PlayList currentPlaylist;
        private AudioFile currentyPlaying;

        public List<PlayList> Playlists { get { return playlists; } }
        public PlayList CurrentPlaylist { get { return currentPlaylist; } }
        public AudioFile CurrentlyPlaying { get { return currentyPlaying; } }
        public TimeSpan CurrentTime { get { return audioFileReader.CurrentTime; } }
        public string CurrentTimeString { get { return StaticClass.GetTimeString(CurrentTime); } }
        public PlaybackState PlayingState { get { return waveOutDevice.PlaybackState; } }
        public float Volume { get { return waveOutDevice.Volume; } }

        public List<AudioFile> FinishedSongs = new List<AudioFile>();

        public string SavePlaylistsDirectory { get; set; }



        public DrgnAudioPlayer()
        {
            try
            {
                waveOutDevice = new WaveOut();
                waveOutDevice.PlaybackStopped += new EventHandler<StoppedEventArgs>(PlayBackEnds);
                playlists = new List<PlayList>();
                playlists.Add(new PlayList("All", null));
                currentPlaylist = playlists[0];
                if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player")))
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DRGNAudioPlayer: " + ex.Message);
                throw ex;
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
                if (currentyPlaying != null)
                {
                    currentyPlaying.TimesPlayed++;
                    FinishedSongs.Add(CurrentlyPlaying);
                }
                switch (PlayingMode)
                {
                    case ePlayingMode.Normal: PlayNextNormal(); break;
                    case ePlayingMode.Random: PlayNextRandom(); break;
                    case ePlayingMode.Smart: PlayNextSmart(); break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("PlayNext: " + ex.Message);
                throw ex;
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
                throw ex;
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
                throw ex;
            }

        }
        private void PlayNextSmart()
        {
            try
            {
                AudioFile[] m_songs = currentPlaylist.GetLowestTimesPlayed();
                Random r = new Random();
                string m_string = m_songs[r.Next(m_songs.Length - 1)].ToString();
                Play(currentPlaylist.GetSongByString(m_string));
            }
            catch (Exception ex)
            {
                Console.WriteLine("PlayNextSmart: " + ex.Message);
                throw ex;
            }

        }
        // FIX  position = 175K  Length = 5M
        private void PlayBackEnds(object sender, StoppedEventArgs e)
        {
            if (audioFileReader.Position >= audioFileReader.Length)
                PlayNext();
        }

        public AudioFile GetFileByString(string p_string)
        {
            try
            {
                foreach (AudioFile af in playlists[0].Songs)
                    if (af.ToString().ToLower() == p_string.ToLower())
                        return af;
            }
            catch (Exception ex)
            { throw ex; }
            return null;
        }
        public AudioFile GetFileByPath(string p_path)
        {
            try
            {
                foreach (AudioFile af in playlists[0].Songs)
                    if (af != null && af.FileLocation != null && af.FileLocation.ToLower() == p_path.ToLower())
                        return af;
            }
            catch (Exception ex)
            { throw ex; }
            return null;
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
                    throw ex;
                }
            }
            else
                if (PlayingState == PlaybackState.Paused && currentyPlaying != null)
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
                    throw ex;
                }
            }
        }
        public void Pause()
        {
            if (waveOutDevice != null)
                try { waveOutDevice.Pause(); }
                catch (Exception ex)
                {
                    Console.WriteLine("Pause: " + ex.Message);
                    throw ex;
                }
        }
        public void Seek(long p_milliseconds, SeekOrigin p_seekOrigin)
        {
            if (audioFileReader != null)
            {
                try
                {
                    waveOutDevice.Pause();
                    audioFileReader.CurrentTime = TimeSpan.FromMilliseconds(audioFileReader.Seek(p_milliseconds, p_seekOrigin));
                    waveOutDevice.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Seek: " + ex.Message);
                    throw ex;
                }
            }
            else
                throw new ArgumentNullException("Cannot seek audio to " + p_milliseconds + ":\naudioFileReader is null");
        }
        public void ChangeVolume(float p_volume)
        {
            try { waveOutDevice.Volume = p_volume; }
            catch (Exception ex)
            {
                Console.WriteLine("Volume: " + ex.Message);
                throw ex;
            }
        }


        public void AddFolder(string p_path)
        {
            try
            {
                string[] m_files = Directory.GetFiles(p_path);
                foreach (string s in m_files)
                    if (s.EndsWith(".mp3") || s.EndsWith(".wav") || s.EndsWith(".wma") || s.EndsWith(".flac")
                        || s.EndsWith(".mp4") || s.EndsWith(".aac"))
                        AddFile(s, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddFolder: " + ex.Message);
                throw ex;
            }
        }
        public void AddFile(string p_fileLocation, bool p_addIfExists)
        {
            if (GetFileByPath(p_fileLocation) != null && p_addIfExists == false)
            { }
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

                    currentPlaylist.Songs.Add(new AudioFile(p_fileLocation, m_title, m_artist, m_album, m_year, m_duration));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AddFile: " + ex.Message);
                    throw ex;
                }
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
        public void SetPlaylist(string p_name)
        {
            if (p_name == null || p_name == "")
            { throw new ArgumentNullException("Playlist name cannot be null or empty."); }
            try
            { currentPlaylist = GetPlaylist(p_name); }
            catch (Exception ex)
            {
                Console.WriteLine("SetPlaylist: " + ex.Message);
                throw ex;
            }
        }

        public void SavePlaylists()
        {
            try
            {
                string m_text = "To view this easier go to http://jsonlint.com/ \n" + StaticClass.PlaylistsListToJSON(playlists);
                if (SavePlaylistsDirectory == null)
                {
                    SavePlaylistsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player");
                    SavePlaylistsDirectory = Path.Combine(SavePlaylistsDirectory, FileName);
                }
                StaticClass.WriteToFile(SavePlaylistsDirectory, m_text); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("SavePlayLists(): " + ex.Message);
                throw ex;
            }
        }
        public void LoadPlaylists()
        {
            try
            {
                if (SavePlaylistsDirectory == null)
                {
                    SavePlaylistsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player");
                    SavePlaylistsDirectory = Path.Combine(SavePlaylistsDirectory, FileName);
                }
                if (System.IO.File.Exists(SavePlaylistsDirectory))
                {
                    try
                    { List<PlayList> m_temp = StaticClass.JSONToPlaylistsList(System.IO.File.ReadAllText(SavePlaylistsDirectory)); }
                    catch { return; }   // file is empty
                    playlists = StaticClass.JSONToPlaylistsList(System.IO.File.ReadAllText(SavePlaylistsDirectory));
                    if (playlists.Count == 0)
                        throw new Exception("playlists.Playlists.Count is 0.");
                    else
                        currentPlaylist = playlists[0];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadPlaylists: " + ex.Message);
                throw ex;
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
        public string DurationString { get { return StaticClass.GetTimeString(Duration); } }
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
        public List<AudioFile> Songs { get; set; }
        public string Name { get; set; }

        public PlayList(string p_name, List<AudioFile> p_songs)
        {
            Songs = new List<AudioFile>();
            Name = p_name;
            if (p_songs != null)
                foreach (AudioFile af in p_songs)
                    Songs.Add(af);
        }
        public AudioFile GetLastAddedSong(AudioFile p_af)
        {
            if (Songs.Count > 0)
            {
                return Songs[Songs.Count - 1];
            }
            else
                return null;
        }
        public AudioFile GetSongByString(string p_string)
        {
            foreach (AudioFile af in Songs)
                if (af.ToString() == p_string)
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

    }

}
