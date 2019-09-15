using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeteaseM2DServer.Src.Model
{
    class Global {

        public static bool isListening { get; set; }
        public static long stateUpdateMS { get; set; }

        public static Metadata currentSong { get; set; }
        public static PlaybackState currentState { get; set; }

        public static long MusicId { get; set; }
        public static string MusicLrc { get; set; }
    }
}
