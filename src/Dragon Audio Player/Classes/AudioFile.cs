using System;
using System.IO;
using File = TagLib.File;

namespace Dragon_Audio_Player.Classes
{
    public class AudioFile
    {
        //      ---------------------------------------------
        //      |   Product:    Dragon Audio Player         |
        //      |   By:         SHEePYTaGGeRNeP             |
        //      |   Date:       28/03/2015                  |
        //      |   Version:    0.4                         |
        //      |   Copyright © Double Dutch Dragons 2015   |
        //      ---------------------------------------------



        public string Title { get; set; }
        public string FileLocation { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public uint Year { get; set; }
        public TimeSpan Duration { get; set; }
        public string FileType { get; set; }
        public string DurationString { get { return StaticClass.GetTimeString(Duration); } }
        public int TimesPlayed { get; set; }

        /// <summary>
        /// Used for JSON
        /// </summary>
        public AudioFile()
        {
            
        }

        public AudioFile(string pFileLocation)
        {
            FileLocation = pFileLocation;
            FileType = Path.GetExtension(pFileLocation);
            UpdateInfoFromTag();
        }

        /// <summary>
        /// Only use for testing!
        /// </summary>
        /// <param name="pLoc"></param>
        /// <param name="pTitle"></param>
        /// <param name="pArtist"></param>
        /// <param name="pAlbum"></param>
        public AudioFile(string pLoc, string pTitle, string pArtist, string pAlbum)
        {
            FileLocation = pLoc;
            Title = pTitle;
            Artist = pArtist;
            Album = pAlbum;
        }

        //public AudioFile(string pLoc, string pTitle, string pArtist, string pAlbum, uint pYear,
        //    TimeSpan pDur)
        //{
        //    FileLocation = pLoc;
        //    Title = pTitle;
        //    Artist = pArtist;
        //    Album = pAlbum;
        //    Year = pYear;
        //    Duration = pDur;
        //    FileType = Path.GetExtension(pLoc);
        //}

        public void UpdateInfoFromTag()
        {
            File lvTagFile = File.Create(FileLocation);
            Title = lvTagFile.Tag.Title;
            GetArtistFromTag(lvTagFile);
            Album = lvTagFile.Tag.Album;
            Year = lvTagFile.Tag.Year;
            Duration = lvTagFile.Properties.Duration;
        }

        public void GetArtistFromTag(File pTag)
        {
            Artist = pTag.Tag.FirstAlbumArtist;
            if (Artist != null) return;
            Artist = pTag.Tag.FirstPerformer;
            if (Artist != null) return;
            Artist = pTag.Tag.JoinedAlbumArtists;
            if (Artist != null) return;
            try
            { Artist = pTag.Tag.AlbumArtists[0]; }
            catch
            { throw new Exception("Cannot find artist. Complain to developer please."); }
        }
        public override string ToString()
        {
            return String.Format("{0} - [{1}] {2}", Artist, Album, Title);
        }

    }
}