using System;
using Dragon_Audio_Player.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject.Classes
{
    [TestClass]
    public class TestPlaylist
    {
        //      ---------------------------------------------
        //      |   Product:    Dragon Audio Player         |
        //      |   By:         SHEePYTaGGeRNeP             |
        //      |   Date:       28/03/2015                  |
        //      |   Version:    0.4                         |
        //      |   Copyright © Double Dutch Dragons 2015   |
        //      ---------------------------------------------

        private Playlist _playlist;

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _playlist = new Playlist("testPlaylist");

        }

        [TestMethod]
        public void TestConstructor()
        {
            Assert.AreEqual("testPlaylist",_playlist.Name, "Constructor is not correct");
        }

        [TestMethod]
        public void TestGetLastAddedSong()
        {
            AudioFile lvAf = new AudioFile("", "test1", "test1","");
            _playlist.Songs.Add(lvAf);
            Assert.AreEqual(lvAf, _playlist.GetLastAddedSong());
            AudioFile lvAf2 = new AudioFile("", "test2", "test2","");
            _playlist.Songs.Add(lvAf2);
            Assert.AreEqual(lvAf2, _playlist.GetLastAddedSong());
        }

        [TestMethod]
        public void TestGetLowestTimesPlayed()
        {
            AudioFile lvAf = new AudioFile("", "test1", "test1", "");
            lvAf.TimesPlayed = 1;
            _playlist.Songs.Add(lvAf);
            AudioFile lvAf2 = new AudioFile("", "test2", "test2", "");
            _playlist.Songs.Add(lvAf2);
            AudioFile[] lvTimesPlayed = _playlist.GetLowestTimesPlayed();
            Assert.AreEqual(lvAf2,lvTimesPlayed[0]);
        }

        [TestMethod]
        public void TestGetSongByString()
        {
            AudioFile lvAf = new AudioFile("", "test1", "test1", "");
            _playlist.Songs.Add(lvAf);
            AudioFile lvAf2 = new AudioFile("", "test2", "test2", "");
            _playlist.Songs.Add(lvAf2);
            AudioFile lvSongByString = _playlist.GetSongByString(lvAf2.ToString());
            Assert.AreEqual(lvAf2,lvSongByString);
        }

        [TestMethod]
        public void TestGetSongByPath()
        {
            AudioFile lvAf = new AudioFile("test1L", "test1", "test1","");
            _playlist.Songs.Add(lvAf);
            AudioFile lvAf2 = new AudioFile("test2L", "test2", "test2", "");
            _playlist.Songs.Add(lvAf2);
            AudioFile lvSongByPath = _playlist.GetSongByPath("test2L");
            Assert.AreEqual(lvAf2,lvSongByPath);
        }

    }
}