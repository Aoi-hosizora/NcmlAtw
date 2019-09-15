using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeteaseM2DServer.Src.Model
{
    class Global {

        public static Metadata currentSong { get; set; }
        public static PlaybackState currentState { get; set; }
        public static bool isListening { get; set; } 
    }
}
