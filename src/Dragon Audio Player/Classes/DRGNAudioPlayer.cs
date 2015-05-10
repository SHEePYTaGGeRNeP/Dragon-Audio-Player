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

namespace Dragon_Audio_Player.Classes
{
    //      ---------------------------------------------
    //      |   Product:    Dragon Audio Player         |
    //      |   By:         SHEePYTaGGeRNeP             |
    //      |   Date:       30/04/2015                  |
    //      |   Version:    0.4                         |
    //      |   Copyright © Double Dutch Dragons 2015   |
    //      ---------------------------------------------


    public class DrgnAudioPlayer : IDisposable
    {

        /// TODO: https://docs.google.com/spreadsheets/d/1EPZA_mqK9Kh4qKLxEZsnqQuB8pNJWqQjR6zO2HlAzXQ/

        // ReSharper disable InconsistentNaming
        // Lowercase because of comparing with string
        public enum EPlayingMode { smart, random, normal };
        // ReSharper restore InconsistentNaming

        public static string PlaylistFileName
        {
            get { return Path.Combine(StaticClass.AppDataFolder, "Playlists.txt"); }
        }

        public delegate void NewSongHandler(object pSender, EventArgs pE);
        // Fired when a new song is played
        public event NewSongHandler OnNewSong;

        public delegate void TimesPlayedIncreaseHandler(object pSender, AudioFile pFile);
        // Fired when PlayNext is called
        public event TimesPlayedIncreaseHandler OnTimesPlayedIncrease;

        public EPlayingMode PlayingMode { get; private set; }
        private IWavePlayer _waveOutDevice;
        private MediaFoundationReader _mediaFoundationReader;


        private bool _disposed;

        public List<Playlist> Playlists { get; private set; }
        public Playlist CurrentPlaylist { get; private set; }
        public AudioFile CurrentlyPlaying { get; private set; }
        public TimeSpan CurrentTime { get { return _mediaFoundationReader.CurrentTime; } }
        public string CurrentTimeString { get { return StaticClass.GetTimeString(CurrentTime); } }
        public PlaybackState PlayingState { get { return _waveOutDevice.PlaybackState; } }
        public float Volume { get { return _waveOutDevice.Volume; } }


        /// <summary>
        /// FileLocation
        /// </summary>
        public Stack<string> PreviouslyPlayedSongs { get; private set; }



        public AudioFile LastAudioFile
        {
            get
            {
                if (CurrentPlaylist != null && CurrentPlaylist.Songs.Count > 0)
                    return CurrentPlaylist.Songs[CurrentPlaylist.Songs.Count - 1];
                return null;
            }
        }

        public DrgnAudioPlayer()
        {
            _waveOutDevice = new WaveOut();
            _waveOutDevice.PlaybackStopped += PlayBackEnds;
            Playlists = new List<Playlist>();
            PreviouslyPlayedSongs = new Stack<string>();
        }



        public void SetPlayingMode(string pText)
        {
            PlayingMode = (EPlayingMode)Enum.Parse(typeof(EPlayingMode), pText.ToLower());
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




        #region >< >< >< >< >< >< >< >< >< ><  P L A Y   N E X T  >< >< >< >< >< >< >< ><
        public void PlayNext()
        {
            if (CurrentlyPlaying != null)
            {
                CurrentlyPlaying.TimesPlayed++;
                if (OnTimesPlayedIncrease != null)
                    OnTimesPlayedIncrease.Invoke(this, CurrentlyPlaying);
            }
            switch (PlayingMode)
            {
                case EPlayingMode.normal:
                    PlayNextNormal();
                    break;
                case EPlayingMode.random:
                    PlayNextRandom();
                    break;
                case EPlayingMode.smart:
                    PlayNextSmart();
                    break;
            }
        }

        private void PlayNextNormal()
        {
            if (CurrentPlaylist != null && CurrentPlaylist.Songs != null && CurrentPlaylist.Songs.Count > 0)
            {
                int lvIndex = CurrentPlaylist.Songs.IndexOf(CurrentlyPlaying);
                if (lvIndex >= CurrentPlaylist.Songs.Count - 1)
                    Play(CurrentPlaylist.Songs[0]);
                else
                    Play(CurrentPlaylist.Songs[lvIndex + 1]);
            }

        }
        private void PlayNextRandom()
        {
            if (CurrentPlaylist != null && CurrentPlaylist.Songs != null && CurrentPlaylist.Songs.Count > 0)
            {
                Random lvR = new Random();
                int lvI = lvR.Next(CurrentPlaylist.Songs.Count - 1);
                Play(CurrentPlaylist.Songs[lvI]);
            }

        }
        private void PlayNextSmart()
        {
            if (CurrentPlaylist != null && CurrentPlaylist.Songs != null && CurrentPlaylist.Songs.Count > 0)
            {
                AudioFile[] lvSongs = CurrentPlaylist.GetLowestTimesPlayed();
                Random lvR = new Random();
                string lvString = lvSongs[lvR.Next(lvSongs.Length - 1)].ToString();
                Play(CurrentPlaylist.GetSongByString(lvString));
            }
        }

        // FIX  position = 175K  Length = 5M
        private void PlayBackEnds(object pSender, StoppedEventArgs pE)
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
                        if (CurrentlyPlaying != pFile)
                            Stop();
                        else
                        {
                            Pause();
                            return;
                        }
                    if (PlayingState != PlaybackState.Paused || pFile != CurrentlyPlaying)
                    {
                        _mediaFoundationReader = new MediaFoundationReader(pFile.FileLocation);
                        WaveChannel32 lvWc = new WaveChannel32(_mediaFoundationReader) { PadWithZeroes = false };
                        _waveOutDevice = new WaveOut();
                        _waveOutDevice.Init(lvWc);
                        _waveOutDevice.PlaybackStopped += PlayBackEnds;
                    }
                    _waveOutDevice.Play();
                    CurrentlyPlaying = pFile;
                    if (OnNewSong != null)
                        OnNewSong.Invoke(this, new EventArgs());
                    PreviouslyPlayedSongs.Push(CurrentlyPlaying.FileLocation);
                }
                else if (PlayingState == PlaybackState.Paused && CurrentlyPlaying != null)
                    _waveOutDevice.Play();
                else
                    PlayNext();
            }
            catch (Exception lvEx)
            {
                Console.WriteLine(lvEx.Message + "\n" + pFile.ToString());
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
            if (CurrentlyPlaying != null)
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
                throw new NullReferenceException("MediaFoundationReader is null");
        }
        public void Previous()
        {
            if (PreviouslyPlayedSongs.Count > 1)
            {
                // Pop the currently playing song
                PreviouslyPlayedSongs.Pop();
                // Pop the previous played song
                string lvNewSong = PreviouslyPlayedSongs.Pop();
                AudioFile lvAf = CurrentPlaylist.GetSongByPath(lvNewSong);
                Play(lvAf);
            }
        }

        #endregion



        #region >< >< >< >< >< >< >< >< >< ><  P L A Y L I S T S  >< >< >< >< >< >< >< ><
        public Playlist GetPlaylist(string pName)
        {
            return Playlists.Find(p => String.Equals(p.Name, pName, StringComparison.CurrentCultureIgnoreCase));
        }
        public string[] GetPlaylistNames()
        {
            if (Playlists.Count == 0)
                return new string[0];
            return Playlists.Select(p => p.Name).ToArray();
        }
        public void SetPlaylist(string pName)
        {
            if (string.IsNullOrEmpty(pName))
                throw new ArgumentNullException(pName);
            Playlist lvPlaylist = GetPlaylist(pName);
            if (lvPlaylist != null)
                CurrentPlaylist = lvPlaylist;
            else
                throw new ArgumentException("Cannot find playlist: " + pName);
        }
        public void SavePlaylists(string pFileLocation)
        {
            string lvText = "To view this easier go to http://jsonlint.com/ \n" +
                            StaticClass.PlaylistsListToJson(Playlists);
            StaticClass.WriteToFile(pFileLocation, lvText);
        }
        public string LoadPlaylists(string pFileLocation)
        {
            if (File.Exists(pFileLocation))
            {
                try
                { StaticClass.JsonToPlaylistsList(File.ReadAllText(pFileLocation)); }
                catch
                {
                    // file is empty
                    return String.Empty;
                }
                Playlists = StaticClass.JsonToPlaylistsList(File.ReadAllText(pFileLocation));
                string lvReturnMessage = String.Empty;
                foreach (Playlist lvPlaylist in Playlists)
                    lvReturnMessage += lvPlaylist.FixPlaylist();
                if (Playlists.Count > 0)
                    CurrentPlaylist = Playlists[0];
                return lvReturnMessage;
            }
            return String.Empty;
        }
        public void DeleteSongFromPlaylist(AudioFile pAf)
        {
            if (pAf == null)
                throw new ArgumentNullException();
            if (CurrentlyPlaying == pAf)
                Stop();
            CurrentPlaylist.RemoveSong(pAf);
        }
        public void DeleteCurrentPlaylist()
        {
            if (CurrentPlaylist != null)
            {
                Playlists.Remove(CurrentPlaylist);
                if (Playlists.Count > 0)
                    CurrentPlaylist = Playlists[0];
                else
                    CurrentPlaylist = null;
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
            if (!this._disposed)
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
            this._disposed = true;
        }
    }

}
