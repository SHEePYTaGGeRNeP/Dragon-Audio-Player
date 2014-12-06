using System;
using System.Collections.Generic;
using System.IO;
using Dragon_Audio_Player;
using Newtonsoft.Json;
using File = TagLib.File;

namespace Dragon_Audio_Player
{
    //      ---------------------------------------------
    //      |   Product:    Dragon Audio Player         |
    //      |   By:         SHEePYTaGGeRNeP             |
    //      |   Date:       26/06/2014                  |
    //      |   Version:    0.2                         |
    //      |   Copyright © Double Dutch Dragons 2014   |
    //      ---------------------------------------------

    public static class StaticClass
    {
        private static string[] AUDIO_FILE_TYPES = {".mp3", ".wav", "aac", "flac", ".mp4", ".wma"};

        private class PlayListsList
        {
            public List<PlayList> Playlists;

            public PlayListsList()
            {
                Playlists = new List<PlayList>();
            }
        }

        public static bool EndsWithAudioFileType(string p_fileLocation)
        {
            foreach (string s in AUDIO_FILE_TYPES)
                if (p_fileLocation.ToLower().EndsWith(s))
                    return true;
            return false;
        }

        public static List<PlayList> JSONToPlaylistsList(string p_text)
        {
            try
            {
                string m_text = p_text;
                if (!p_text.ToLower().StartsWith("{"))
                    m_text = p_text.Remove(0, m_text.IndexOf("{"));
                PlayListsList m_list = JsonConvert.DeserializeObject<PlayListsList>(m_text);
                List<PlayList> m_return = new List<PlayList>();
                foreach (PlayList p in m_list.Playlists)
                {
                    m_return.Add(FixPlaylist(p));
                }
                return m_return;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string PlaylistsListToJSON(List<PlayList> p_lists)
        {
            try
            {
                PlayListsList m_list = new PlayListsList();
                foreach (PlayList p in p_lists)
                    m_list.Playlists.Add(p);
                var m_json = JsonConvert.SerializeObject(m_list);
                return m_json.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static PlayList FixPlaylist(PlayList p_pl)
        {
            try
            {
                List<AudioFile> m_list = new List<AudioFile>();
                foreach (AudioFile af in p_pl.Songs)
                {
                    if (af == null || af.FileLocation == null)
                        m_list.Add(af);
                    else if (af.Artist == null || af.Title == null || af.Duration == null || af.Duration == TimeSpan.Zero)
                    {
                        File tagFile = File.Create(af.FileLocation);
                        af.Title = tagFile.Tag.Title;
                        af.Artist = GetArtist(tagFile);
                        af.Album = tagFile.Tag.Album;
                        af.Year = tagFile.Tag.Year;
                        af.Duration = tagFile.Properties.Duration;
                    }
                }
                foreach (AudioFile af in m_list)
                    p_pl.Songs.Remove(af);
                return p_pl;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string GetArtist(File p_tag)
        {
            try
            {
                string m_artist = p_tag.Tag.FirstAlbumArtist;
                if (m_artist == null)
                {
                    m_artist = p_tag.Tag.FirstPerformer;
                    if (m_artist == null)
                    {
                        m_artist = p_tag.Tag.JoinedAlbumArtists;
                        if (m_artist == null)
                        {
                            try
                            {
                                m_artist = p_tag.Tag.AlbumArtists[0];
                            }
                            catch
                            {
                            }
                            if (m_artist == null)
                                m_artist = "";
                        }
                    }
                }
                return m_artist;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getArtist: " + ex.Message);
                throw ex;
            }
        }

        public static string GetTimeString(TimeSpan p_span)
        {
            string m_return = "";
            if (p_span.Days > 0)
                m_return = new DateTime(p_span.Ticks).ToString("dd.HH:mm:ss");
            else if (p_span.Hours > 0)
                m_return = new DateTime(p_span.Ticks).ToString("HH:mm:ss");
            else
                m_return = new DateTime(p_span.Ticks).ToString("mm:ss");
            return m_return;
        }

        public static void WriteToFile(string p_loc, string p_text)
        {
            if (!Directory.Exists(Path.GetDirectoryName(p_loc)))
                Directory.CreateDirectory(p_loc);
            System.IO.File.WriteAllText(p_loc, p_text);
        }
    }
}