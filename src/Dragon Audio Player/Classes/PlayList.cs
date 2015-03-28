using System.Collections.Generic;
using System.Linq;

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
        public string Name { get; set; }

        public Playlist(string pName)
        {
            Songs = new List<AudioFile>();
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
        public AudioFile GetSongByString(string pString)
        {
            foreach (AudioFile lvAf in Songs)
                if (lvAf.ToString() == pString)
                    return lvAf;
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

    }
}