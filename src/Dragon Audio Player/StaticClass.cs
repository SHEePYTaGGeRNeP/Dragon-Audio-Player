using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
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

    public static class StaticClass
    {
        private static readonly string[] AudioFileTypes = {".mp3", ".wav", "aac", "flac", ".mp4", ".wma"};

        private class PlayListsList
        {
            public readonly List<PlayList> Playlists;

            public PlayListsList()
            {
                Playlists = new List<PlayList>();
            }
        }

        public static bool EndsWithAudioFileType(string pFileLocation)
        {
            return AudioFileTypes.Any(s => pFileLocation.ToLower().EndsWith(s));
        }

        public static List<PlayList> JSONToPlaylistsList(string pText)
        {
            try
            {
                string lvMText = pText;
                if (!pText.ToLower().StartsWith("{"))
                    lvMText = pText.Remove(0, lvMText.IndexOf("{"));
                PlayListsList lvMList = JsonConvert.DeserializeObject<PlayListsList>(lvMText);
                return lvMList.Playlists.Select(FixPlaylist).ToList();
            }
            catch (Exception lvEx)
            {
                throw new Exception(lvEx.Message);
            }
        }

        public static string PlaylistsListToJSON(List<PlayList> pLists)
        {
            try
            {
                PlayListsList lvMList = new PlayListsList();
                foreach (PlayList p in pLists)
                    lvMList.Playlists.Add(p);
                var lvMJson = JsonConvert.SerializeObject(lvMList);
                return lvMJson.ToString();
            }
            catch (Exception lvEx)
            {
                throw new Exception(lvEx.Message);
            }
        }

        public static PlayList FixPlaylist(PlayList pPl)
        {
            try
            {
                List<AudioFile> lvMList = new List<AudioFile>();
                foreach (AudioFile lvAf in pPl.Songs)
                {
                    if (lvAf == null || lvAf.FileLocation == null)
                        lvMList.Add(lvAf);
                    else if (lvAf.Artist == null || lvAf.Title == null || lvAf.Duration == null || lvAf.Duration == TimeSpan.Zero)
                    {
                        File lvTagFile = File.Create(lvAf.FileLocation);
                        lvAf.Title = lvTagFile.Tag.Title;
                        lvAf.Artist = GetArtist(lvTagFile);
                        lvAf.Album = lvTagFile.Tag.Album;
                        lvAf.Year = lvTagFile.Tag.Year;
                        lvAf.Duration = lvTagFile.Properties.Duration;
                    }
                }
                foreach (AudioFile lvAf in lvMList)
                    pPl.Songs.Remove(lvAf);
                return pPl;
            }
            catch (Exception lvEx)
            {
                throw new Exception(lvEx.Message);
            }
        }

        public static string GetArtist(File pTag)
        {
            try
            {
                string lvMArtist = pTag.Tag.FirstAlbumArtist;
                if (lvMArtist != null) return lvMArtist;
                lvMArtist = pTag.Tag.FirstPerformer;
                if (lvMArtist != null) return lvMArtist;
                lvMArtist = pTag.Tag.JoinedAlbumArtists;
                if (lvMArtist != null) return lvMArtist;
                try
                {
                    lvMArtist = pTag.Tag.AlbumArtists[0];
                }
                catch
                {
                    throw new Exception("Cannot find artist. Complain to developer please.");
                }
                return lvMArtist;
            }
            catch (Exception lvEx)
            {
                Console.WriteLine("getArtist: " + lvEx.Message);
                throw lvEx;
            }
        }

        public static string GetTimeString(TimeSpan pSpan)
        {
            string lvMReturn = "";
            if (pSpan.Days > 0)
                lvMReturn = new DateTime(pSpan.Ticks).ToString("dd.HH:mm:ss");
            else if (pSpan.Hours > 0)
                lvMReturn = new DateTime(pSpan.Ticks).ToString("HH:mm:ss");
            else
                lvMReturn = new DateTime(pSpan.Ticks).ToString("mm:ss");
            return lvMReturn;
        }

        public static void WriteToFile(string pLoc, string pText)
        {
            if (string.IsNullOrEmpty(pLoc))
                throw new Exception("Cannot write to empty directory");
            if (!Directory.Exists(Path.GetDirectoryName(pLoc)))
                Directory.CreateDirectory(pLoc);
            System.IO.File.WriteAllText(pLoc, pText);
        }
    }
}