using System;

namespace Dragon_Audio_Player.Classes
{
    public static class CustomExceptions
    {
        [Serializable]
        public class SongAlreadyExistsInPlaylistException : Exception
        {
            //
            // For guidelines regarding the creation of new exception types, see
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
            // and
            //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
            //
        }
    }
}
