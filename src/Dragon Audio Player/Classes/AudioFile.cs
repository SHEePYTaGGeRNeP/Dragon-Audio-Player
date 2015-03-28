using System;
using System.IO;

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


        public AudioFile(string pLoc, string pTitle, string pArtist, string pAlbum, uint pYear,
            TimeSpan pDur)
        {
            FileLocation = pLoc;
            Title = pTitle;
            Artist = pArtist;
            Album = pAlbum;
            Year = pYear;
            Duration = pDur;
            FileType = Path.GetExtension(pLoc);
        }


        public override string ToString()
        {
            return String.Format("{0} - [{1}] {2}", Artist, Album, Title);
        }

    }
}