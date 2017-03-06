using Audio_Player.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio_Player
{
    class ViewModelPlaylists : NotifyBase
    {

        public List<Playlist> Playlists { get; set; } = new List<Playlist>();
        
        public void AddPlaylist(string name)
        {
            this.Playlists.Add(new Playlist(name));
            this.OnPropertyChanged("Playlists");
        }
    }
}
