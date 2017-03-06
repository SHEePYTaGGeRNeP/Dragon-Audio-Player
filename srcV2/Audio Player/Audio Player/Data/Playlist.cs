using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio_Player.Data
{
    class Playlist : NotifyBase
    {
        public string Name { get; set; }

        public Playlist() { }
        public Playlist(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// Song, times played
        /// </summary>        
        public List<PlaylistSong> Songs { get; set; } = new List<PlaylistSong>();
        public void SetPlayCount(Song s, int count)
        {
            PlaylistSong ps = Songs.Find(x => x.Song == s);
            if (ps == default(PlaylistSong)) throw new ArgumentException("Song does not exist in playlist");
            ps.TimesPlayed = count;
        }
        public List<string> Directories { get; set; } = new List<string>();
    }
    class PlaylistSong : NotifyBase
    {
        public Song Song { get; set; }
        private int _timesPlayed;

        public int TimesPlayed
        {
            get { return _timesPlayed; }
            set
            {
                _timesPlayed = value;
                this.OnPropertyChanged("TimesPlayed");
            }
        }
        public PlaylistSong(Song s, int timesplayed = 0)
        {
            this.Song = s;
            this.TimesPlayed = timesplayed;
            this.OnPropertyChanged("Song");
        }
    }
}
