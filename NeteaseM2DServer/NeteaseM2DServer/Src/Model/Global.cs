using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NeteaseM2DServer.Src.Model {

    class Global {

        public static Bitmap qrCodeImage { get; set; }
        public const string qrCodeMagic = "NETEASEM2D";

        public static bool isListening { get; set; }
        public static long stateUpdateMS { get; set; }
        public static long currentPos { get; set; }
        public static string durationStr { get; set; }

        public static Metadata currentSong { get; set; }
        public static PlaybackState currentState { get; set; }

        public static long MusicId { get; set; }

        public static LyricPage MusicLyricPage { get; set; }
        public static LyricState MusicLyricState { get; set; }

        public delegate void MainFormTimerDelegate();
        public delegate void LyricFormTimerDelegate();

        public static MainFormTimerDelegate MainFormTimer;
        public static LyricFormTimerDelegate LyricFormTimer;

        public enum LyricState {
            Found, NotFound, PureMusic
        }
    }
}
