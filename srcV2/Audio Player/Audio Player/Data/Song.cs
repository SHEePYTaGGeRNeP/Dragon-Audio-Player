using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace Audio_Player.Data
{
    class Song
    {
        public static readonly string[] AudioFileTypes = { ".mp3", ".wav", "aac", ".mp4", ".wma", "flac" };
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string FileLocation { get; set; }
        public uint Year { get; set; }
        public TimeSpan Duration { get; set; }

        public Song(string fileLocation)
        {
            this.FileLocation = fileLocation;
            this.UpdateInfoFromTag();
        }

        public void UpdateInfoFromTag()
        {
            File lvTagFile = File.Create(FileLocation);
            Title = lvTagFile.Tag.Title;
            GetArtistFromTag(lvTagFile);
            Album = lvTagFile.Tag.Album;
            Year = lvTagFile.Tag.Year;
            Duration = lvTagFile.Properties.Duration;
        }

        private void GetArtistFromTag(File pTag)
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
        /// <summary>
        /// String.Format("{0} - [{1}] {2}", Artist, Album, Title);
        /// </summary>
        /// <returns>String.Format("{0} - [{1}] {2}", Artist, Album, Title);</returns>
        public override string ToString()
        {
            if (Artist != null && Album != null && Title != null)
                return String.Format("{0} - [{1}] {2}", Artist, Album, Title);
            if (Artist == null && Album != null && Title != null)
                return String.Format("[{0}] {1}", Album, Title);
            if (Artist != null && Album == null && Title != null)
                return String.Format("{0} - {1}", Artist, Title);
            return "Empty Song Title !! Please fixerino";
        }
    }
}
