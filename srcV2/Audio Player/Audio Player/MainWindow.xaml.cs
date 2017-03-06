using Audio_Player.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Audio_Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModelPlaylists vm = new ViewModelPlaylists();

        private List<FileHandler> fileHandlers = new List<FileHandler>();

        public MainWindow()
        {
            InitializeComponent();
            this.vm.AddPlaylist("All");
            this.vm.Playlists[0].Directories.Add(@"D:\Muziek");
            this.lvPlaylistAll.ItemsSource = this.vm.Playlists[0].Songs;
            //this.CreateTabs();
        }

        private void CreateTabs()
        {
            foreach (Playlist pl in this.vm.Playlists)
            {
                if (pl.Name == "All") continue;
                // so difficult.
                TabItem ti = new TabItem() { Header = pl.Name, Name = "tabPlaylist" + pl.Name };
                ListView lv = new ListView() { Name = "lvPlaylist" + pl.Name };
                GridView gv = new GridView();
                this.tabControlPlaylists.Items.Add(ti);
            }
        }


        private void StartReading()
        {
            Thread t = new Thread(() =>
            {
                // TODO: add parameter to show which playlist
                Parallel.ForEach(this.vm.Playlists[0].Directories, StartReading);
            })
            {
                Name = "StartReading"
            };
            t.Start();
        }
        private void StartReading(string dir)
        {
            FileHandler fh = new FileHandler();
            fh.OnNewSongs += OnNewSongs;
            fh.OnFinishedReading += OnFinishedReading;
            this.fileHandlers.Add(fh);
            fh.Start(dir);
        }

        private void OnFinishedReading(FileHandler fh)
        {
            fh.OnNewSongs -= OnNewSongs;
            fh.OnFinishedReading -= OnFinishedReading;
            this.fileHandlers.Remove(fh);
        }

        private void OnNewSongs(Data.Song[] songs)
        {
            for (int i = 0; i < songs.Length; i++)
                this.vm.Playlists[0].Songs.Add(new Data.PlaylistSong(songs[i]));
            Dispatcher.BeginInvoke((Action)(() =>
            {
                this.lvPlaylistAll.Items.Refresh();
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for (int i = this.fileHandlers.Count - 1; i > 0; i--)
                this.fileHandlers[i].cancel = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.StartReading();
        }
    }
}
