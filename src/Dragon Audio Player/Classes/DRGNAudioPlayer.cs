// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DRGNAudioPlayer.cs" company="DoubleDutch Dragons">
//   © 2014 DoubleDutch Dragons
// </copyright>
// <summary>
//   Defines the DrgnAudioPlayer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Wave;
using File = TagLib.File;

namespace Dragon_Audio_Player.Classes
{
    //      ---------------------------------------------
    //      |   Product:    Dragon Audio Player         |
    //      |   By:         SHEePYTaGGeRNeP             |
    //      |   Date:       28/03/2015                  |
    //      |   Version:    0.4                         |
    //      |   Copyright © Double Dutch Dragons 2015   |
    //      ---------------------------------------------


    public class DrgnAudioPlayer : IDisposable
    {

        /// TODO: https://docs.google.com/spreadsheets/d/1EPZA_mqK9Kh4qKLxEZsnqQuB8pNJWqQjR6zO2HlAzXQ/

        public enum EPlayingMode { Smart, Random, Normal };
        private const string _FILE_NAME = "Playlists.txt";
        
        public EPlayingMode PlayingMode { get; private set; }
        private IWavePlayer _waveOutDevice;
        private MediaFoundationReader _mediaFoundationReader;


        private bool disposed = false;

        public List<Playlist> Playlists { get; private set; }
        public Playlist CurrentPlaylist { get; private set; }
        public AudioFile CurrentlyPlaying { get; private set; }
        public TimeSpan CurrentTime { get { return _mediaFoundationReader.CurrentTime; } }
        public string CurrentTimeString { get { return StaticClass.GetTimeString(CurrentTime); } }
        public PlaybackState PlayingState { get { return _waveOutDevice.PlaybackState; } }
        public float Volume { get { return _waveOutDevice.Volume; } }


        // for previous song
        public List<AudioFile> FinishedSongs { get; private set; }

        public string SavePlaylistsDirectory { get; set; }

        
        public AudioFile LastAudioFile
        {
            get
            {
                if (Playlists[0].Songs.Count > 0)
                    return Playlists[0].Songs[Playlists[0].Songs.Count - 1];
                return null;
            }
        }

        public DrgnAudioPlayer()
        {
            _waveOutDevice = new WaveOut();
            _waveOutDevice.PlaybackStopped += PlayBackEnds;
            Playlists = new List<Playlist>();
            Playlists.Add(new Playlist("All"));
            FinishedSongs = new List<AudioFile>();
            CurrentPlaylist = Playlists[0];
            //if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player")))
            //{
            //    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player"));
            //}
        }



        public void SetPlayingMode(string pText)
        {
            PlayingMode = (EPlayingMode)Enum.Parse(typeof(EPlayingMode), pText);
        }

        /// <summary>
        /// Changes the volume of the waveOutDevice between 0.0 and 0.5
        /// </summary>
        /// <param name="pVolume">Between 0 and 1000</param>
        public void ChangeVolume(int pVolume)
        {
            double lvVolume = Convert.ToDouble((double)pVolume / 2000);
            _waveOutDevice.Volume = (float)lvVolume;
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

        #region >< >< >< >< >< >< >< >< >< ><  P L A Y   N E X T  >< >< >< >< >< >< >< ><
        public void PlayNext()
        {
            if (CurrentlyPlaying != null)
            {
                CurrentlyPlaying.TimesPlayed++;
                FinishedSongs.Add(CurrentlyPlaying);
            }
            switch (PlayingMode)
            {
                case EPlayingMode.Normal:
                    PlayNextNormal();
                    break;
                case EPlayingMode.Random:
                    PlayNextRandom();
                    break;
                case EPlayingMode.Smart:
                    PlayNextSmart();
                    break;
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
            Random lvR = new Random();
            int lvI = lvR.Next(CurrentPlaylist.Songs.Count - 1);
            Play(CurrentPlaylist.Songs[lvI]);


        }
        private void PlayNextSmart()
        {
            AudioFile[] lvSongs = CurrentPlaylist.GetLowestTimesPlayed();
            Random lvR = new Random();
            string lvString = lvSongs[lvR.Next(lvSongs.Length - 1)].ToString();
            Play(CurrentPlaylist.GetSongByString(lvString));

        }

        // FIX  position = 175K  Length = 5M
        private void PlayBackEnds(object sender, StoppedEventArgs e)
        {
            if (_mediaFoundationReader.Position >= _mediaFoundationReader.Length)
                PlayNext();
        }

        #endregion



        #region >< >< >< >< >< >< >< >< >< ><  M E D I A   C O N T R O L S  >< >< >< >< >< >< >< ><
        public void Play(AudioFile pFile)
        {
            try
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
                        _mediaFoundationReader = new MediaFoundationReader(pFile.FileLocation);
                        WaveChannel32 lvWc = new WaveChannel32(_mediaFoundationReader) { PadWithZeroes = false };
                        _waveOutDevice = new WaveOut();
                        _waveOutDevice.Init(lvWc);
                        _waveOutDevice.PlaybackStopped += PlayBackEnds;
                    }
                    _waveOutDevice.Play();
                    CurrentlyPlaying = pFile;
                }
                else if (PlayingState == PlaybackState.Paused && CurrentlyPlaying != null)
                    _waveOutDevice.Play();
                else
                    PlayNext();
            }
            catch (Exception lvEx)
            {
                Console.WriteLine(lvEx.Message + "  " + pFile.ToString());
                throw;
            }

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
            if (_mediaFoundationReader != null)
            {
                _waveOutDevice.Pause();
                _mediaFoundationReader.CurrentTime =
                    TimeSpan.FromMilliseconds(_mediaFoundationReader.Seek(pMilliseconds, pSeekOrigin));
                _waveOutDevice.Play();
            }
            else
                throw new ArgumentNullException("MediaFoundationReader is null");
        }

        #endregion



        #region >< >< >< >< >< >< >< >< >< ><  P L A Y L I S T S  >< >< >< >< >< >< >< ><
        public Playlist GetPlaylist(string pName)
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
            string lvText = "To view this easier go to http://jsonlint.com/ \n" +
                            StaticClass.PlaylistsListToJson(Playlists);
            if (SavePlaylistsDirectory == null)
            {
                SavePlaylistsDirectory =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "Dragon Audio Player");
                SavePlaylistsDirectory = Path.Combine(SavePlaylistsDirectory, _FILE_NAME);
            }
            StaticClass.WriteToFile(SavePlaylistsDirectory, lvText);
        }
        public void LoadPlaylists()
        {
            if (SavePlaylistsDirectory == null)
            {
                SavePlaylistsDirectory =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "Dragon Audio Player");
                SavePlaylistsDirectory = Path.Combine(SavePlaylistsDirectory, _FILE_NAME);
            }
            if (System.IO.File.Exists(SavePlaylistsDirectory))
            {
                try
                {
                    List<Playlist> lvTemp =
                        StaticClass.JsonToPlaylistsList(System.IO.File.ReadAllText(SavePlaylistsDirectory));
                }
                catch
                {
                    return;
                } // file is empty
                Playlists = StaticClass.JsonToPlaylistsList(System.IO.File.ReadAllText(SavePlaylistsDirectory));
                if (Playlists.Count == 0)
                    throw new Exception("playlists.Playlists.Count is 0.");
                CurrentPlaylist = Playlists[0];
            }
        }

        #endregion

        
        public void Dispose()
        {
            Dispose(true);
        }
        ~DrgnAudioPlayer()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool pDisposing)
        {
            if (!this.disposed)
            {
                if (pDisposing)
                {
                    if (_waveOutDevice != null)
                    {
                        _waveOutDevice.Stop();
                        _waveOutDevice.Dispose();
                        _waveOutDevice = null;
                    }
                    if (_mediaFoundationReader != null)
                    {
                        _mediaFoundationReader.Dispose();
                        _mediaFoundationReader = null;
                    }
                    GC.SuppressFinalize(this);
                }
                // No unmanaged resources to release otherwise they'd go here.
            }
            disposed = true;
        }
    }
}
