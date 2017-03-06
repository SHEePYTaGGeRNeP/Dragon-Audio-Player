using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Audio_Player.Data;

namespace Audio_Player
{
    class FileHandler
    {
        const int _UPDATE_GUI_TIME = 200;

        public delegate void NewSongsHandler(Song[] fileLoc);
        public event NewSongsHandler OnNewSongs;
        public delegate void FinishedReadingHandler(FileHandler fh);
        public event FinishedReadingHandler OnFinishedReading;

        public bool cancel;
        public void Start(string location)
        {
            List<Song> result = new List<Song>();
            DateTime startTime = DateTime.Now;
            DateTime lastUpdate = DateTime.Now;
            foreach (string file in Directory.EnumerateFiles(location))
            {
                if (cancel) break;
                if (Data.Song.AudioFileTypes.Contains(Path.GetExtension(file.ToLower())))
                    result.Add(new Song(file));
                if ((DateTime.Now - lastUpdate).TotalMilliseconds >_UPDATE_GUI_TIME)
                {
                    OnNewSongs(result.ToArray());
                    result.Clear();
                    lastUpdate = DateTime.Now;
                    Console.WriteLine(lastUpdate + " updated files");
                }
            }
            OnNewSongs(result.ToArray());
            Console.WriteLine(DateTime.Now + " done with " + location + " in " + (DateTime.Now - startTime));
            this.OnFinishedReading?.Invoke(this);
        }

    }
}
