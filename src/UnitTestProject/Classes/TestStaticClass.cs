using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Dragon_Audio_Player.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject.Classes
{
    [TestClass]
    public class TestStaticClass
    {
        //      ---------------------------------------------
        //      |   Product:    Dragon Audio Player         |
        //      |   By:         SHEePYTaGGeRNeP             |
        //      |   Date:       28/03/2015                  |
        //      |   Version:    0.4                         |
        //      |   Copyright © Double Dutch Dragons 2015   |
        //      ---------------------------------------------

        private const string _PLAYLIST_JSON =
            "{\"Playlists\":[{\"Songs\":[{\"Title\":\"test\",\"FileLocation\":\"test\",\"Artist\":\"test\",\"Album\":\"\",\"Year\":0,\"Duration\":\"00:01:00\",\"FileType\":\"\",\"DurationString\":\"01:00\",\"TimesPlayed\":0},{\"Title\":\"test2\",\"FileLocation\":\"test2\",\"Artist\":\"test2\",\"Album\":\"\",\"Year\":0,\"Duration\":\"00:01:00\",\"FileType\":\"\",\"DurationString\":\"01:00\",\"TimesPlayed\":0}],\"Name\":\"test\"}]}";

        [TestMethod]
        public void TestEndsWithAudioFileType()
        {
            Assert.IsTrue(StaticClass.EndsWithAudioFileType(".wma"));
            Assert.IsFalse(StaticClass.EndsWithAudioFileType(".mp"));
            Assert.IsTrue(StaticClass.EndsWithAudioFileType(".wav"));
        }

        [TestMethod]
        public void TestPlaylistsListToJson()
        {
            List<Playlist> lvPlaylists = new List<Playlist>();
            Playlist lvPlaylist = new Playlist("test");
            AudioFile lvAf = new AudioFile("test", "test", "test", "", 0, TimeSpan.FromMinutes(1));
            lvPlaylist.Songs.Add(lvAf);
            AudioFile lvAf2 = new AudioFile("test2", "test2", "test2", "", 0, TimeSpan.FromMinutes(1));
            lvPlaylist.Songs.Add(lvAf2);
            lvPlaylists.Add(lvPlaylist);

            string lvResult = StaticClass.PlaylistsListToJson(lvPlaylists);
            Assert.AreEqual(_PLAYLIST_JSON, lvResult);
        }

        [TestMethod]
        public void TestJsonToPlaylistsList()
        {
            List<Playlist> lvPlaylists = new List<Playlist>();
            Playlist lvPlaylist = new Playlist("test");
            AudioFile lvAf = new AudioFile("test", "test", "test", "", 0, TimeSpan.FromMinutes(1));
            lvPlaylist.Songs.Add(lvAf);
            AudioFile lvAf2 = new AudioFile("test2", "test2", "test2", "", 0, TimeSpan.FromMinutes(1));
            lvPlaylist.Songs.Add(lvAf2);
            lvPlaylists.Add(lvPlaylist);

            List<Playlist> lvResult = StaticClass.JsonToPlaylistsList(_PLAYLIST_JSON);

            for (int i = 0; i < lvPlaylists[0].Songs.Count; i++)
                Assert.AreEqual(lvPlaylists[0].Songs[i].ToString(), lvResult[0].Songs[i].ToString());
        }

        [TestMethod]
        public void TestFixPlaylist()
        {
            Playlist lvPlaylist = new Playlist("test");
            AudioFile lvAf = new AudioFile("test", "test", "test", "", 0, TimeSpan.FromMinutes(1));
            lvPlaylist.Songs.Add(lvAf);
            AudioFile lvAf2 = new AudioFile("test2", "test2", "test2", "", 0, TimeSpan.FromMinutes(1));
            lvPlaylist.Songs.Add(lvAf2);
            lvPlaylist.Songs.Add(null);
            lvPlaylist = StaticClass.FixPlaylist(lvPlaylist);
            Assert.AreEqual(2, lvPlaylist.Songs.Count);
        }

        [TestMethod]
        public void TestGetTimeString()
        {
            Assert.AreEqual("10:00:00", StaticClass.GetTimeString(new TimeSpan(10, 0, 0)));
            Assert.AreEqual("11.10:00:00", StaticClass.GetTimeString(new TimeSpan(10, 10, 0, 0)));
            Assert.AreEqual("10:00", StaticClass.GetTimeString(new TimeSpan(0, 10, 0)));
        }

        [TestMethod]
        public void TestWriteToFile()
        {
            try
            {
                StaticClass.WriteToFile("", "test");
                Assert.Fail("Cannot write to this directory");
            }
            catch (Exception lvEx)
            {
                Assert.AreEqual("Cannot write to empty directory", lvEx.Message);
            }
            try
            {
                string lvSaveDirectory =
                       Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                           "Dragon Audio Player");
                lvSaveDirectory = Path.Combine(lvSaveDirectory, "test.txt");
                StaticClass.WriteToFile(lvSaveDirectory, "test");
                if (File.Exists(lvSaveDirectory))
                    File.Delete(lvSaveDirectory);
            }
            catch (Exception lvEx)
            {
                Assert.Fail("Failed writing file: " + lvEx.Message);
            }
        }
    }
}
