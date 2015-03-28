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
        public static readonly string[] AudioFileTypes = { ".mp3", ".wav", "aac", ".mp4", ".wma" };
        public static string AppDataFolder { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dragon Audio Player"); } }


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
                string lvText = pText;
                if (!pText.ToLower().StartsWith("{"))
                    // Removes all text until "{" is found.
                    lvText = pText.Remove(0, lvText.IndexOf("{", StringComparison.Ordinal));
                PlayListsList lvList = JsonConvert.DeserializeObject<PlayListsList>(lvText);
                List<Playlist> lvPlaylists = new List<Playlist>();
                foreach (Playlist lvPl in lvList.Playlists)
                {
                    lvPl.FixPlaylist();
                    lvPlaylists.Add(lvPl);
                }
                return lvPlaylists;
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
            File.WriteAllText(pLoc, pText);
        }
    }
}