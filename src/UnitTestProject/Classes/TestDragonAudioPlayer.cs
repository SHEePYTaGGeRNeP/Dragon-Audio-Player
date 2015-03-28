﻿using System;
using System.IO;
using Dragon_Audio_Player.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject.Classes
{
    [TestClass]
    public class TestDragonAudioPlayer
    {
        //      ---------------------------------------------
        //      |   Product:    Dragon Audio Player         |
        //      |   By:         SHEePYTaGGeRNeP             |
        //      |   Date:       28/03/2015                  |
        //      |   Version:    0.4                         |
        //      |   Copyright © Double Dutch Dragons 2015   |
        //      ---------------------------------------------


        private static DrgnAudioPlayer _audioPlayer;
        // PROBABLY NEEDS TO BE CHANGED.
        private  string _AUDIO_1_PATH = @"E:\GITHUB\Dragon-Audio-Player\src\UnitTestProject\bin\Debug\AudioTestFiles\audio1.mp3";


        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void TestsInitialize()
        {
            _audioPlayer = new DrgnAudioPlayer();
        }


        [TestMethod]
        public void TestSetPlayingMode()
        {
            _audioPlayer.SetPlayingMode("smart");
            Assert.AreEqual(_audioPlayer.PlayingMode, DrgnAudioPlayer.EPlayingMode.smart);
            _audioPlayer.SetPlayingMode("normal");
            Assert.AreEqual(_audioPlayer.PlayingMode, DrgnAudioPlayer.EPlayingMode.normal);
            _audioPlayer.SetPlayingMode("random");
            Assert.AreEqual(_audioPlayer.PlayingMode, DrgnAudioPlayer.EPlayingMode.random);
        }

        [TestMethod]
        public void TestChangeVolume()
        {
            _audioPlayer.ChangeVolume(1000);
            Assert.AreEqual(0.5f, _audioPlayer.Volume);
            _audioPlayer.ChangeVolume(0);
            Assert.AreEqual(0.0f, _audioPlayer.Volume);
            _audioPlayer.ChangeVolume(200);
            Assert.AreEqual(0.1f, _audioPlayer.Volume);
        }

        [TestMethod]
        public void TestAddFolder()
        {
            try
            {
                _audioPlayer.CurrentPlaylist.AddFolder("test1231b1b1b");
            }
            catch (IOException)
            { }
            catch (Exception lvEx)
            { Assert.Fail(lvEx.Message); }
        }

        [TestMethod]
        public void TestAddFile()
        {
            Assert.AreEqual(String.Empty,_audioPlayer.CurrentPlaylist.AddFile(_AUDIO_1_PATH, true));
            Assert.AreNotEqual(String.Empty, _audioPlayer.CurrentPlaylist.AddFile(_AUDIO_1_PATH, false));
            Assert.AreEqual(String.Empty, _audioPlayer.CurrentPlaylist.AddFile(_AUDIO_1_PATH, true));
            Assert.AreEqual(2, _audioPlayer.CurrentPlaylist.Songs.Count);
        }

        [TestMethod]
        public void TestPlay()
        {
            AudioFile lvAf = new AudioFile(_AUDIO_1_PATH, "test", "test", "test");
            _audioPlayer.CurrentPlaylist.Songs.Add(lvAf);
            _audioPlayer.Play(lvAf);
            Assert.AreEqual(lvAf, _audioPlayer.CurrentlyPlaying);
            _audioPlayer.Stop();
            Assert.IsNull(_audioPlayer.CurrentlyPlaying);
        }

        [TestMethod]
        public void TestGetPlaylist()
        {
            Playlist lvPlaylist = new Playlist("test1");
            _audioPlayer.Playlists.Add(lvPlaylist);
            Assert.AreEqual(lvPlaylist, _audioPlayer.GetPlaylist("test1"));
            Assert.IsNull(_audioPlayer.GetPlaylist("test2"));
        }

        [TestMethod]
        public void TestGetPlaylistNames()
        {
            string[] lvNames = new string[2];
            // Default playlist = All
            lvNames[0] = "All";
            lvNames[1] = "test";
            _audioPlayer.Playlists.Add(new Playlist("test"));
            CollectionAssert.AreEqual(lvNames, _audioPlayer.GetPlaylistNames());
            _audioPlayer.Playlists.Clear();
            Assert.AreEqual(0, _audioPlayer.GetPlaylistNames().Length);
        }

        [TestMethod]
        public void TestSetPlaylist()
        {
            Playlist lvPlaylist = new Playlist("test");
            _audioPlayer.Playlists.Add(lvPlaylist);
            _audioPlayer.SetPlaylist("test");
            Assert.AreEqual(lvPlaylist, _audioPlayer.CurrentPlaylist);
            try
            { _audioPlayer.SetPlaylist(""); }
            catch (ArgumentNullException)
            { }
            catch (Exception lvEx)
            { Assert.Fail(lvEx.Message); }

            try
            { _audioPlayer.SetPlaylist("test2"); }
            catch (ArgumentException)
            { }
            catch (Exception lvEx)
            { Assert.Fail(lvEx.Message); }
        }

        [TestMethod]
        public void TestSavePlaylists()
        {
            _audioPlayer.CurrentPlaylist.Songs.Add(new AudioFile(_AUDIO_1_PATH,"test","",""));
            try
            {
                _audioPlayer.SavePlaylists(UtStaticClass.PlaylistFileName);
                Assert.IsTrue(File.Exists(UtStaticClass.PlaylistFileName));
            }
            catch (IOException)
            {}
            catch (Exception lvEx)
            {
                Assert.Fail(lvEx.Message);
            }
        }

        [TestMethod]
        public void TestLoadPlaylists()
        {
            TestSavePlaylists();
            AudioFile lvAf  = new AudioFile(_AUDIO_1_PATH, "test", "", "");
            _audioPlayer.LoadPlaylists(UtStaticClass.PlaylistFileName);
            Assert.AreEqual(lvAf.ToString(),_audioPlayer.CurrentPlaylist.Songs[0].ToString());
        }
    }

}
