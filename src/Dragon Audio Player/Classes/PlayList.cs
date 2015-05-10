using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagLib;

namespace Dragon_Audio_Player.Classes
{
    public class Playlist
    {
        //      ---------------------------------------------
        //      |   Product:    Dragon Audio Player         |
        //      |   By:         SHEePYTaGGeRNeP             |
        //      |   Date:       28/03/2015                  |
        //      |   Version:    0.4                         |
        //      |   Copyright © Double Dutch Dragons 2015   |
        //      ---------------------------------------------

        public List<AudioFile> Songs { get; set; }
        private readonly List<string> _ignoredSongs;
        public List<String> Folders { get; set; }
        public string Name { get; set; }


        public Playlist(string pName)
        {
            Songs = new List<AudioFile>();
            _ignoredSongs = new List<string>();
            Folders = new List<string>();
            Name = pName;
        }


        public AudioFile GetLastAddedSong()
        {
            if (Songs.Count > 0)
            {
                return Songs[Songs.Count - 1];
            }
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

        public AudioFile GetSongByString(string pString)
        {
            return Songs.Find(pAf => String.Equals(pAf.ToString(), pString, StringComparison.CurrentCultureIgnoreCase));
        }

        public AudioFile GetSongByArtistTitle(string pArtist, string pTitle)
        {
            return Songs.Find(pAf => String.Equals(pAf.Artist, pArtist, StringComparison.CurrentCultureIgnoreCase)
                && String.Equals(pAf.Title, pTitle, StringComparison.CurrentCultureIgnoreCase));
        }

        public AudioFile GetSongByPath(string pPath)
        {
            return Songs.Find(pAf => String.Equals(pAf.FileLocation, pPath, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        ///  Returns empty when no files have been removed
        /// </summary>
        /// <returns></returns>
        public string FixPlaylist()
        {
            try
            {
                string lvReturnMessage = String.Empty;
                List<AudioFile> lvList = this.Songs.Where(pAf => pAf == null || pAf.FileLocation == null
                    || System.IO.File.Exists(pAf.FileLocation) == false).ToList();
                foreach (AudioFile lvAf in lvList)
                {
                    Songs.Remove(lvAf);
                    if (lvAf != null)
                        lvReturnMessage += String.Format("Playlist: {0} - Removed: {1}\n", Name, lvAf.ToString());
                }
                foreach (string lvFolder in Folders)
                {
                    if (Directory.Exists(lvFolder))
                    {
                        foreach (string lvFile in Directory.GetFiles(lvFolder))
                        {
                            if (!_ignoredSongs.Contains(lvFile.ToLower(), StringComparer.CurrentCultureIgnoreCase))
                                // Empty = successfully added
                                if (AddFile(lvFile, false) == String.Empty)
                                    lvReturnMessage += String.Format("Playlist: {0} - Added: {1}\n", Name, lvFile);
                        }
                    }
                }
                return lvReturnMessage;
            }
            catch (Exception lvEx)
            {
                throw new Exception(lvEx.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPath"></param>
        /// <returns>String.Empty if song was added. Warnings / errors if not empty</returns>
        public string AddFolder(string pPath)
        {
            string lvReturn = String.Empty;
            string[] lvFiles = Directory.GetFiles(pPath);
            foreach (string lvS in lvFiles)
            {
                string lvExt = Path.GetExtension(lvS.ToLower());
                if (!StaticClass.AudioFileTypes.Contains(lvExt)) continue;
                string lvResult = this.AddFile(lvS, false);
                if (lvResult != String.Empty)
                    lvReturn += lvResult + "\n";
            }
            Folders.Add(pPath);
            return lvReturn;
        }

        /// <summary>
        /// Add song to playlist.
        /// </summary>
        /// <param name="pFileLocation"></param>
        /// <param name="pAddIfExists"></param>
        /// <returns>String.Empty if song was added. Warnings / errors if not empty</returns>
        public string AddFile(string pFileLocation, bool pAddIfExists)
        {
            if (!StaticClass.AudioFileTypes.Contains(Path.GetExtension(pFileLocation.ToLower())))
                return "Unsupported file extension: " + Path.GetExtension(pFileLocation);
            AudioFile lvFoundSong = GetSongByPath(pFileLocation);
            if (pAddIfExists == true || lvFoundSong == null)
            {
                Songs.Add(new AudioFile(pFileLocation));
                return "";
            }
            return "This song already exists in this playlist: " + Path.GetFileName(pFileLocation);
        }

        public void ResetTimesPlayed()
        {
            foreach (AudioFile lvAf in Songs)
                lvAf.TimesPlayed = 0;
        }

        public void RemoveSong(AudioFile pAf)
        {
            if (pAf == null)
                throw new ArgumentNullException();
            Songs.Remove(pAf);
            _ignoredSongs.Add(pAf.FileLocation);
        }

    }
}