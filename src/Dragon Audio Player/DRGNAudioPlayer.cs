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
        public enum EPlayingMode { Smart, Random, Normal };
        private const string _FILE_NAME = "Playlists.txt";

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
            _waveOutDevice = new WaveOut();
            _waveOutDevice.PlaybackStopped += PlayBackEnds;
            Playlists = new List<PlayList>();
            Playlists.Add(new PlayList("All", null));
            CurrentPlaylist = Playlists[0];
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player"));
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

            string lvText = pText.ToLower();
            if (lvText.Contains("normal"))
                PlayingMode = EPlayingMode.Normal;
            else if (lvText.Contains("random"))
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
            catch (Exception lvEx)
            {
                Console.WriteLine("PlayNext: " + lvEx.Message);
                throw lvEx;
            }
        }
        private void PlayNextNormal()
        {
            int lvIndex = CurrentPlaylist.Songs.IndexOf(CurrentlyPlaying);
            if (lvIndex >= CurrentPlaylist.Songs.Count - 1)
                Play(CurrentPlaylist.Songs[0]);
            else
                Play(CurrentPlaylist.Songs[lvIndex + 1]);

        }
        private void PlayNextRandom()
        {
            try
            {
                Random lvR = new Random();
                int lvI = lvR.Next(CurrentPlaylist.Songs.Count - 1);
                Play(CurrentPlaylist.Songs[lvI]);
            }
            catch (Exception lvEx)
            {
                Console.WriteLine("PlayNextRandom: " + lvEx.Message);
                throw;
            }

        }
        private void PlayNextSmart()
        {
            try
            {
                AudioFile[] lvSongs = CurrentPlaylist.GetLowestTimesPlayed();
                Random lvR = new Random();
                string lvString = lvSongs[lvR.Next(lvSongs.Length - 1)].ToString();
                Play(CurrentPlaylist.GetSongByString(lvString));
            }
            catch (Exception lvEx)
            {
                Console.WriteLine("PlayNextSmart: " + lvEx.Message);
                throw;
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
            foreach (AudioFile lvAf in Playlists[0].Songs)
                if (lvAf.ToString().ToLower() == pString.ToLower())
                    return lvAf;

            return null;
        }
        public AudioFile GetFileByPath(string pPath)
        {
            foreach (AudioFile lvAf in Playlists[0].Songs)
                if (lvAf != null && lvAf.FileLocation != null && lvAf.FileLocation.ToLower() == pPath.ToLower())
                    return lvAf;

            return null;
        }

        public void Play(AudioFile pFile)
        {
            if (pFile != null)
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
                    WaveChannel32 lvWc = new WaveChannel32(_audioFileReader) { PadWithZeroes = false };
                    _waveOutDevice = new WaveOut();
                    _waveOutDevice.Init(lvWc);
                    _waveOutDevice.PlaybackStopped += PlayBackEnds;
                }
                _waveOutDevice.Play();
                CurrentlyPlaying = pFile;
            }
            else
                if (PlayingState == PlaybackState.Paused && CurrentlyPlaying != null)
                    _waveOutDevice.Play();
                else
                    PlayNext();
        }
        public void Stop()
        {
            if (_waveOutDevice == null) return;
            _waveOutDevice.Stop();
            CurrentlyPlaying = null;
        }

        public void Pause()
        {
            if (_waveOutDevice == null) return;
            _waveOutDevice.Pause();
        }

        public void Seek(long pMilliseconds, SeekOrigin pSeekOrigin)
        {
            if (_audioFileReader != null)
            {
                _waveOutDevice.Pause();
                _audioFileReader.CurrentTime = TimeSpan.FromMilliseconds(_audioFileReader.Seek(pMilliseconds, pSeekOrigin));
                _waveOutDevice.Play();
            }
            else
                throw new ArgumentNullException("AudioFileReader is null");
        }
        public void ChangeVolume(float pVolume)
        {
            _waveOutDevice.Volume = pVolume;
        }


        public void AddFolder(string pPath)
        {
            string[] lvFiles = Directory.GetFiles(pPath);
            foreach (string s in lvFiles)
                if (s.EndsWith(".mp3") || s.EndsWith(".wav") || s.EndsWith(".wma") || s.EndsWith(".flac")
                    || s.EndsWith(".mp4") || s.EndsWith(".aac"))
                    AddFile(s, false);
        }
        public void AddFile(string pFileLocation, bool pAddIfExists)
        {
            if (GetFileByPath(pFileLocation) != null && pAddIfExists == false)
            { }
            else
            {
                File lvTagFile = File.Create(pFileLocation);
                string lvTitle = lvTagFile.Tag.Title;
                string lvArtist = StaticClass.GetArtist(lvTagFile);
                string lvAlbum = lvTagFile.Tag.Album;
                uint lvYear = lvTagFile.Tag.Year;
                TimeSpan lvDuration = lvTagFile.Properties.Duration;

                CurrentPlaylist.Songs.Add(new AudioFile(pFileLocation, lvTitle, lvArtist, lvAlbum, lvYear, lvDuration));
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
                throw new ArgumentNullException(pName);
            CurrentPlaylist = GetPlaylist(pName);
        }

        public void SavePlaylists()
        {
            string lvText = "To view this easier go to http://jsonlint.com/ \n" + StaticClass.PlaylistsListToJSON(Playlists);
            if (SavePlaylistsDirectory == null)
            {
                SavePlaylistsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player");
                SavePlaylistsDirectory = Path.Combine(SavePlaylistsDirectory, _FILE_NAME);
            }
            StaticClass.WriteToFile(SavePlaylistsDirectory, lvText);
        }
        public void LoadPlaylists()
        {
            if (SavePlaylistsDirectory == null)
            {
                SavePlaylistsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player");
                SavePlaylistsDirectory = Path.Combine(SavePlaylistsDirectory, _FILE_NAME);
            }
            if (System.IO.File.Exists(SavePlaylistsDirectory))
            {
                try
                { List<PlayList> lvTemp = StaticClass.JSONToPlaylistsList(System.IO.File.ReadAllText(SavePlaylistsDirectory)); }
                catch { return; }   // file is empty
                Playlists = StaticClass.JSONToPlaylistsList(System.IO.File.ReadAllText(SavePlaylistsDirectory));
                if (Playlists.Count == 0)
                    throw new Exception("playlists.Playlists.Count is 0.");
                CurrentPlaylist = Playlists[0];
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
                foreach (AudioFile lvAf in pSongs)
                    Songs.Add(lvAf);
        }
        public AudioFile GetLastAddedSong(AudioFile pAf)
        {
            if (Songs.Count > 0)
            {
                return Songs[Songs.Count - 1];
            }
            return null;
        }
        public AudioFile GetSongByString(string pString)
        {
            foreach (AudioFile lvAf in Songs)
                if (lvAf.ToString() == pString)
                    return lvAf;
            return null;
        }
        public AudioFile[] GetLowestTimesPlayed()
        {
            int lvLowest = int.MaxValue;
            foreach (AudioFile lvAf in Songs)
                if (lvAf.TimesPlayed < lvLowest)
                    lvLowest = lvAf.TimesPlayed;

            return Songs.Where(pAf => pAf.TimesPlayed == lvLowest).ToArray();
        }

    }

}
