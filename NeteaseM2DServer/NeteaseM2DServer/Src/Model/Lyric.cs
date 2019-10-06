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
            // Console.WriteLine(lrcString);
            LyricPage ret = new LyricPage();
            ret.Lines = new List<LyricLine>();
            foreach (String line in lrcString.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)) {
                
                // (\[\d{1,2}:\d{1,2}.\d{1,3}\])+.*
                // (\[\d{1,2}:\d{1,2}\])+.*
                Regex reg = new Regex("(\\[\\d{1,2}:\\d{1,2}(.\\d{1,3})*\\])+.*");
                if (!reg.IsMatch(line)) continue;

                List<LyricLine> ls = LyricLine.parseLyricLine(line);
                if (ls != null && ls.Count != 0)
                    ret.Lines.AddRange(ls);
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
        /// LrcLine -> List(of LyricLine)
        /// </summary>
        public static List<LyricLine> parseLyricLine(string lrcLineString) {

            try {
                List<LyricLine> ret = new List<LyricLine>();
                List<string> timeStrs = new List<string>();

                string[] sp = lrcLineString.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);

                // Lyric Content
                if (sp.Length == 1)
                    return null;
                else if (sp.Length == 2) { // [1]2
                    timeStrs.Add(sp[0]); // time
                    
                    LyricLine line = new LyricLine();
                    line.Lyric = sp[1].Trim(); // content
                    if (line.Lyric == "")
                        return null;

                    ret.Add(line);
                } else { // [1][2][3]4
                    for (int i = 0; i < sp.Length - 1; i++) {
                        timeStrs.Add(sp[i]); // time
                       
                        LyricLine line = new LyricLine();
                        line.Lyric = sp[sp.Length - 1].Trim(); // content
                        if (line.Lyric == "")
                            continue;

                        ret.Add(line);
                    }
                }

                // Time
                for (int i = 0; i < timeStrs.Count; i++) {
                    string[] tis = timeStrs.ElementAt(i).Split(new string[] { ":", "." }, StringSplitOptions.RemoveEmptyEntries);

                    if (tis.Length == 2) // [00:00]
                        ret.ElementAt(i).timeMilliSecond = 0;
                    else if (tis.Length == 3) { // [00:00.00]
                        if (tis[2].Length == 1)
                            ret.ElementAt(i).timeMilliSecond = int.Parse(tis[2] + "0"); // [00:00.0]
                        else
                            ret.ElementAt(i).timeMilliSecond = int.Parse(tis[2].Substring(0, 2));  // [00:00.00] [00:00.000]
                    } else
                        return null;

                    ret.ElementAt(i).timeMinute = int.Parse(tis[0]);
                    ret.ElementAt(i).timeSecond = int.Parse(tis[1]);
                }

                return ret;

            } catch (Exception ex) {
                // Error
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
