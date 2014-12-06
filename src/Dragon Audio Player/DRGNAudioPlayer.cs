using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Wave;
using File = TagLib.File;

namespace Dragon_Audio_Player
{
    //      ---------------------------------------------
    //      |   Product:    Dragon Audio Player         |
    //      |   By:         SHEePYTaGGeRNeP             |
    //      |   Date:       06/12/2014                  |
    //      |   Version:    0.3                         |
    //      |   Copyright © Double Dutch Dragons 2014   |
    //      ---------------------------------------------


    public class DrgnAudioPlayer
    {
        private const string _FILE_NAME = "Playlists.txt";

        public enum EPlayingMode { Smart, Random, Normal };
        public EPlayingMode PlayingMode { get; set; }
        private IWavePlayer _waveOutDevice;
        private AudioFileReader _audioFileReader;

        public List<PlayList> Playlists { get; private set; }
        public PlayList CurrentPlaylist { get; private set; }
        public AudioFile CurrentlyPlaying { get; private set; }
        public TimeSpan CurrentTime { get { return _audioFileReader.CurrentTime; } }
        public string CurrentTimeString { get { return StaticClass.GetTimeString(CurrentTime); } }
        public PlaybackState PlayingState { get { return _waveOutDevice.PlaybackState; } }
        public float Volume { get { return _waveOutDevice.Volume; } }


        // for previous song
        public List<AudioFile> FinishedSongs = new List<AudioFile>();

        public string SavePlaylistsDirectory { get; set; }



        public DrgnAudioPlayer()
        {
            try
            {
                _waveOutDevice = new WaveOut();
                _waveOutDevice.PlaybackStopped += new EventHandler<StoppedEventArgs>(PlayBackEnds);
                Playlists = new List<PlayList>();
                Playlists.Add(new PlayList("All", null));
                CurrentPlaylist = Playlists[0];
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
                if (Playlists[0].Songs.Count > 0)
                    return Playlists[0].Songs[Playlists[0].Songs.Count - 1];
                else return null;

            }
        }


        public void SetPlayingMode(string pText)
        {
            
            string m_text = pText.ToLower();
            if (m_text.Contains("normal"))
                PlayingMode = EPlayingMode.Normal;
            else if (m_text.Contains("random"))
                PlayingMode = EPlayingMode.Random;
            else
                PlayingMode = EPlayingMode.Smart;
        }
        public void PlayNext()
        {
            try
            {
                if (CurrentlyPlaying != null)
                {
                    CurrentlyPlaying.TimesPlayed++;
                    FinishedSongs.Add(CurrentlyPlaying);
                }
                switch (PlayingMode)
                {
                    case EPlayingMode.Normal: PlayNextNormal(); break;
                    case EPlayingMode.Random: PlayNextRandom(); break;
                    case EPlayingMode.Smart: PlayNextSmart(); break;
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
                int m_index = CurrentPlaylist.Songs.IndexOf(CurrentlyPlaying);
                if (m_index >= CurrentPlaylist.Songs.Count - 1)
                    Play(CurrentPlaylist.Songs[0]);
                else
                    Play(CurrentPlaylist.Songs[m_index + 1]);
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
                int i = r.Next(CurrentPlaylist.Songs.Count - 1);
                Play(CurrentPlaylist.Songs[i]);
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
                AudioFile[] m_songs = CurrentPlaylist.GetLowestTimesPlayed();
                Random r = new Random();
                string m_string = m_songs[r.Next(m_songs.Length - 1)].ToString();
                Play(CurrentPlaylist.GetSongByString(m_string));
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
            if (_audioFileReader.Position >= _audioFileReader.Length)
                PlayNext();
        }

        public AudioFile GetFileByString(string pString)
        {
            try
            {
                foreach (AudioFile af in Playlists[0].Songs)
                    if (af.ToString().ToLower() == pString.ToLower())
                        return af;
            }
            catch (Exception ex)
            { throw ex; }
            return null;
        }
        public AudioFile GetFileByPath(string pPath)
        {
            try
            {
                foreach (AudioFile af in Playlists[0].Songs)
                    if (af != null && af.FileLocation != null && af.FileLocation.ToLower() == pPath.ToLower())
                        return af;
            }
            catch (Exception ex)
            { throw ex; }
            return null;
        }

        public void Play(AudioFile pFile)
        {
            if (pFile != null)
            {
                try
                {
                    if (PlayingState == PlaybackState.Playing)
                        if (CurrentlyPlaying.FileLocation != pFile.FileLocation)
                            Stop();
                        else
                        {
                            Pause();
                            return;
                        }
                    if (PlayingState != PlaybackState.Paused)
                    {
                        _audioFileReader = new AudioFileReader(pFile.FileLocation);
                        WaveChannel32 wc = new WaveChannel32(_audioFileReader) {PadWithZeroes = false};
                        _waveOutDevice = new WaveOut();
                        _waveOutDevice.Init(wc);
                        _waveOutDevice.PlaybackStopped += new EventHandler<StoppedEventArgs>(PlayBackEnds);
                    }
                    _waveOutDevice.Play();
                    CurrentlyPlaying = pFile;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Play: " + ex.Message);
                    throw ex;
                }
            }
            else
                if (PlayingState == PlaybackState.Paused && CurrentlyPlaying != null)
                    _waveOutDevice.Play();
                else
                    PlayNext();
        }
        public void Stop()
        {
            if (_waveOutDevice != null)
            {
                try
                {
                    _waveOutDevice.Stop();
                    CurrentlyPlaying = null;
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
            if (_waveOutDevice != null)
                try { _waveOutDevice.Pause(); }
                catch (Exception ex)
                {
                    Console.WriteLine("Pause: " + ex.Message);
                    throw ex;
                }
        }
        public void Seek(long pMilliseconds, SeekOrigin pSeekOrigin)
        {
            if (_audioFileReader != null)
            {
                try
                {
                    _waveOutDevice.Pause();
                    _audioFileReader.CurrentTime = TimeSpan.FromMilliseconds(_audioFileReader.Seek(pMilliseconds, pSeekOrigin));
                    _waveOutDevice.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Seek: " + ex.Message);
                    throw ex;
                }
            }
            else
                throw new ArgumentNullException("Cannot seek audio to " + pMilliseconds + ":\naudioFileReader is null");
        }
        public void ChangeVolume(float pVolume)
        {
            try { _waveOutDevice.Volume = pVolume; }
            catch (Exception ex)
            {
                Console.WriteLine("Volume: " + ex.Message);
                throw ex;
            }
        }


        public void AddFolder(string pPath)
        {
            try
            {
                string[] m_files = Directory.GetFiles(pPath);
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
        public void AddFile(string pFileLocation, bool pAddIfExists)
        {
            if (GetFileByPath(pFileLocation) != null && pAddIfExists == false)
            { }
            else
            {
                try
                {
                    File tagFile = File.Create(pFileLocation);
                    string m_title = tagFile.Tag.Title;
                    string m_artist = StaticClass.GetArtist(tagFile);
                    string m_album = tagFile.Tag.Album;
                    uint m_year = tagFile.Tag.Year;
                    TimeSpan m_duration = tagFile.Properties.Duration;

                    CurrentPlaylist.Songs.Add(new AudioFile(pFileLocation, m_title, m_artist, m_album, m_year, m_duration));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AddFile: " + ex.Message);
                    throw ex;
                }
            }
        }

        public PlayList GetPlaylist(string pName)
        {
            return Playlists.FirstOrDefault(p => String.Equals(p.Name, pName, StringComparison.CurrentCultureIgnoreCase));
        }

        public string[] GetPlaylistNames()
        {
            return Playlists.Select(p => p.Name).ToArray();
        }
        public void SetPlaylist(string pName)
        {
            if (string.IsNullOrEmpty(pName))
            { throw new ArgumentNullException("Playlist name cannot be null or empty."); }
            try
            { CurrentPlaylist = GetPlaylist(pName); }
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
                string m_text = "To view this easier go to http://jsonlint.com/ \n" + StaticClass.PlaylistsListToJSON(Playlists);
                if (SavePlaylistsDirectory == null)
                {
                    SavePlaylistsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player");
                    SavePlaylistsDirectory = Path.Combine(SavePlaylistsDirectory, _FILE_NAME);
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
                    SavePlaylistsDirectory = Path.Combine(SavePlaylistsDirectory, _FILE_NAME);
                }
                if (System.IO.File.Exists(SavePlaylistsDirectory))
                {
                    try
                    { List<PlayList> m_temp = StaticClass.JSONToPlaylistsList(System.IO.File.ReadAllText(SavePlaylistsDirectory)); }
                    catch { return; }   // file is empty
                    Playlists = StaticClass.JSONToPlaylistsList(System.IO.File.ReadAllText(SavePlaylistsDirectory));
                    if (Playlists.Count == 0)
                        throw new Exception("playlists.Playlists.Count is 0.");
                    CurrentPlaylist = Playlists[0];
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
            if (_waveOutDevice != null)
            {
                _waveOutDevice.Stop();
            }
            if (_audioFileReader != null)
            {
                _audioFileReader.Dispose();
                _audioFileReader = null;
            }
            if (_waveOutDevice != null)
            {
                _waveOutDevice.Dispose();
                _waveOutDevice = null;
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


        public AudioFile(string pLoc, string pTitle, string pArtist, string pAlbum, uint pYear,
            TimeSpan pDur)
        {
            FileLocation = pLoc;
            Title = pTitle;
            Artist = pArtist;
            Album = pAlbum;
            Year = pYear;
            Duration = pDur;
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

        public PlayList(string pName, List<AudioFile> pSongs)
        {
            Songs = new List<AudioFile>();
            Name = pName;
            if (pSongs != null)
                foreach (AudioFile af in pSongs)
                    Songs.Add(af);
        }
        public AudioFile GetLastAddedSong(AudioFile pAf)
        {
            if (Songs.Count > 0)
            {
                return Songs[Songs.Count - 1];
            }
            else
                return null;
        }
        public AudioFile GetSongByString(string pString)
        {
            foreach (AudioFile af in Songs)
                if (af.ToString() == pString)
                    return af;
            return null;
        }
        public AudioFile[] GetLowestTimesPlayed()
        {
            int m_lowest = int.MaxValue;
            foreach (AudioFile af in Songs)
                if (af.TimesPlayed < m_lowest)
                    m_lowest = af.TimesPlayed;

            return Songs.Where(pAf => pAf.TimesPlayed == m_lowest).ToArray();
        }

    }

}
