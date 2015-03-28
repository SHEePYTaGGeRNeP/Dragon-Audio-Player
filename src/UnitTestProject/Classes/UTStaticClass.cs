using System.IO;
using Dragon_Audio_Player.Classes;

namespace UnitTestProject.Classes
{
    public class UtStaticClass
    {

        #region Private fields

        #endregion


        #region Public Properties
        public static string PlaylistFileName
        {
            get { return Path.Combine(StaticClass.AppDataFolder, "UTPlaylists.txt"); }
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

         
    }
}