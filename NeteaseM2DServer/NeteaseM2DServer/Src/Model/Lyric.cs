using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NeteaseM2DServer.Src.Model {

    class LyricPage {

        public List<LyricLine> Lines { get; set; }

        /// <summary>
        /// Lrc -> LyricPage
        /// </summary>
        public static LyricPage parseLrc(string lrcString) {
            LyricPage ret = new LyricPage();
            ret.Lines = new List<LyricLine>();
            foreach (String line in lrcString.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)) {
                Regex reg = new Regex("\\[\\d{1,2}:\\d{1,2}.\\d{1,3}\\].*");
                if (!reg.IsMatch(line)) continue;

                LyricLine l = LyricLine.parseLyricLine(line);
                if (l != null)
                    ret.Lines.Add(l);
            }
            if (ret.Lines.Count == 0) return null;
            ret.Lines.Sort();
            return ret;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (LyricLine line in Lines)
                sb.AppendLine(line.ToString());
            return sb.ToString();
        }
    }

    class LyricLine : IComparable<LyricLine> {

        // [00:00.00] xxxx

        public int timeMinute { get; set; }
        public int timeSecond { get; set; }
        public int timeMilliSecond { get; set; }

        public long timeDuration {
            get {
                // 01:02.03 -> 3 * 10 + 2 * 1000 + 1 * 60000
                return timeMilliSecond * 10 + timeSecond * 1000 + timeMinute * 60000;
            }
        }

        public string Lyric { set; get; }

        public override string ToString() {
            return "[" + timeMinute.ToString("00") + ":" + timeSecond.ToString("00") + "." + timeMilliSecond.ToString("000") + "]" + Lyric;
        }

        public int CompareTo(LyricLine other) {
            return this.timeDuration.CompareTo(other.timeDuration);
        }


        /// <summary>
        /// LrcLine -> LyricLine
        /// </summary>
        public static LyricLine parseLyricLine(string lrcLineString) {
            LyricLine ret = new LyricLine();

            string[] sp = lrcLineString.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

            // Lyric
            if (sp.Length == 1)
                return null;
            else if (sp.Length == 2) {
                ret.Lyric = sp[1].Trim();
                if (ret.Lyric == "") 
                    return null;
            } else
                return null;

            // Time
            string[] tis = sp[0].Split(new string[] { ":", "." }, StringSplitOptions.RemoveEmptyEntries);

            if (tis.Length == 2)
                ret.timeMilliSecond = int.Parse(tis[2]);
            else if (tis.Length == 3) {
                if (tis[2].Length == 1)
                    ret.timeMilliSecond = int.Parse(tis[2] + "0");
                else
                    ret.timeMilliSecond = int.Parse(tis[2].Substring(0, 2));
            } else
                return null;

            ret.timeMinute = int.Parse(tis[0]);
            ret.timeSecond = int.Parse(tis[1]);
            return ret;
        }
    }
}
