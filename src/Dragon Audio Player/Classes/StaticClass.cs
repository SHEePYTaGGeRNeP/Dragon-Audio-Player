// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticClass.cs" company="DoubleDutch Dragons">
//   © 2014 DoubleDutch Dragons
// </copyright>
// <summary>
//   Defines the StaticClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
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

    public static class StaticClass
    {
        private static readonly string[] AudioFileTypes = {".mp3", ".wav", "aac", "flac", ".mp4", ".wma"};

        private class PlayListsList
        {
            public readonly List<Playlist> Playlists;

            public PlayListsList()
            {
                Playlists = new List<Playlist>();
            }
        }

        public static bool EndsWithAudioFileType(string pFileLocation)
        {
            return AudioFileTypes.Any(pS => pFileLocation.ToLower().EndsWith(pS));
        }

        public static List<Playlist> JsonToPlaylistsList(string pText)
        {
            try
            {
                string lvMText = pText;
                if (!pText.ToLower().StartsWith("{"))
                    lvMText = pText.Remove(0, lvMText.IndexOf("{", StringComparison.Ordinal));
                PlayListsList lvMList = JsonConvert.DeserializeObject<PlayListsList>(lvMText);
                return lvMList.Playlists.Select(FixPlaylist).ToList();
            }
            catch (Exception lvEx)
            {
                throw new Exception(lvEx.Message);
            }
        }

        public static string PlaylistsListToJson(List<Playlist> pLists)
        {
            try
            {
                PlayListsList lvMList = new PlayListsList();
                foreach (Playlist lvPlayList in pLists)
                    lvMList.Playlists.Add(lvPlayList);
                var lvMJson = JsonConvert.SerializeObject(lvMList);
                return lvMJson;
            }
            catch (Exception lvEx)
            {
                throw new Exception(lvEx.Message);
            }
        }

        public static Playlist FixPlaylist(Playlist pPl)
        {
            try
            {
                List<AudioFile> lvMList = new List<AudioFile>();
                foreach (AudioFile lvAf in pPl.Songs)
                {
                    if (lvAf == null || lvAf.FileLocation == null)
                        lvMList.Add(lvAf);
                    else if (lvAf.Artist == null || lvAf.Title == null || lvAf.Duration == TimeSpan.Zero)
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
                string lvArtist = pTag.Tag.FirstAlbumArtist;
                if (lvArtist != null) return lvArtist;
                lvArtist = pTag.Tag.FirstPerformer;
                if (lvArtist != null) return lvArtist;
                lvArtist = pTag.Tag.JoinedAlbumArtists;
                if (lvArtist != null) return lvArtist;
                try
                {
                    lvArtist = pTag.Tag.AlbumArtists[0];
                }
                catch
                {
                    throw new Exception("Cannot find artist. Complain to developer please.");
                }
                return lvArtist;
        }

        public static string GetTimeString(TimeSpan pSpan)
        {
            string lvMReturn;
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