using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeteaseM2DServer.Src.Model {

    class Global {

        public static bool isListening { get; set; }
        public static long stateUpdateMS { get; set; }
        public static long currentPos { get; set; }

        public static Metadata currentSong { get; set; }
        public static PlaybackState currentState { get; set; }

        public static long MusicId { get; set; }

        public static double LyricWinOpacity = 0.40;

        public static LyricPage MusicLyricPage { get; set; }

        public delegate void MainFormTimerDelegate();
        public delegate void LyricFormTimerDelegate();

        public static MainFormTimerDelegate MainFormTimer;
        public static LyricFormTimerDelegate LyricFormTimer;
    }
}
