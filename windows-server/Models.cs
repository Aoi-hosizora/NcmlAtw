using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NcmlAtwServer {

    static class Global {
        public static bool IsListening { get; set; }
        public static double Offset { get; set; }

        public static Metadata CurrentMetadata { get; set; }
        public static PlaybackState CurrentState { get; set; }
        public static double LastUpdateTimestamp { get; set; }
        public static double CurrentPosition { get; set; }

        public static long CurrentMusicId { get; set; }
        public static LyricPage CurrentLyric { get; set; }
        public static LyricState CurrentLyricState { get; set; }
    }

    [JsonObject]
    class Metadata {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public double Duration { get; set; }

        public static Metadata FromJson(string json) {
            return JsonConvert.DeserializeObject<Metadata>(json, new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
        }

        public override bool Equals(object obj) {
            if (obj is Metadata data) {
                return Title == data.Title && Artist == data.Artist && Album == data.Album && Duration == data.Duration;
            }
            return false;
        }

        public override int GetHashCode() {
            return Title.GetHashCode() ^ Artist.GetHashCode() ^ Album.GetHashCode() ^ Duration.GetHashCode();
        }
    }

    [JsonObject]
    class PlaybackState {
        public bool IsPlaying { get; set; }
        public double CurrentPosition { get; set; }

        public static PlaybackState FromJson(string json) {
            return JsonConvert.DeserializeObject<PlaybackState>(json, new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            });
        }
    }

    enum LyricState {
        Found,
        NotFound,
        NoLyric,
    }

    class LyricPage {

        public List<LyricLine> Lines { get; set; }

        public override string ToString() {
            return string.Join("\n", Lines.Select(l => l.ToString()));
        }

        private static readonly Regex LYRIC_PAGE_REGEXP = new Regex(@"(\[\d{1,2}:\d{1,2}(.\d{1,3})*\])+.*");

        public static LyricPage ParseLyricPage(string s) {
            var result = new List<LyricLine>();
            foreach (var line in s.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)) {
                if (!LYRIC_PAGE_REGEXP.IsMatch(line)) {
                    continue;
                }
                var lines = LyricLine.ParseLyricLines(line);
                if (lines.Count > 0) {
                    result.AddRange(lines);
                }
            }
            result.Sort();
            return new LyricPage { Lines = result };
        }
    }

    class LyricLine : IComparable<LyricLine> {

        public int Minute { get; set; }
        public int Second { get; set; }
        public int Millisecond { get; set; }
        public string Lyric { set; get; }
        public double Duration { get => Millisecond / 100.0 + Second + Minute * 60; }

        public override string ToString() {
            return string.Format("[{0:00}:{1:00}:{2:000}] {3}", Minute, Second, Millisecond, Lyric);
        }

        public int CompareTo(LyricLine other) {
            return Duration.CompareTo(other.Duration);
        }

        public static List<LyricLine> ParseLyricLines(string s) {
            // parse content to lyric and timeStrings
            string lyric;
            var timeStrings = new List<string>();
            string[] sp = $"{s} ".Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
            if (sp.Length == 1) {
                return new List<LyricLine>();
            }
            if (sp.Length == 2) { // [1]2
                lyric = sp[1].Trim();
                timeStrings.Add(sp[0].Trim());
            } else { // [1][2][3]4
                lyric = sp[sp.Length - 1].Trim();
                for (int i = 0; i < sp.Length - 1; i++) {
                    timeStrings.Add(sp[i].Trim());
                }
            }

            // handle each lines
            var result = new List<LyricLine>();
            foreach (var timeString in timeStrings) {
                string[] tsp = timeString.Split(new string[] { ":", "." }, StringSplitOptions.RemoveEmptyEntries);
                string msString;
                if (tsp.Length == 2) { // [00:00]
                    msString = "0";
                } else if (tsp.Length == 3) {
                    if (tsp[2].Length == 1) { // [00:00.0]
                        msString = tsp[2] + "0";
                    } else { // [00:00.00] [00:00.000]
                        msString = tsp[2].Substring(0, 2);
                    }
                } else {
                    continue;
                }

                var line = new LyricLine { Lyric = lyric };
                try {
                    line.Minute = int.Parse(tsp[0]);
                    line.Second = int.Parse(tsp[1]);
                    line.Millisecond = int.Parse(msString);
                } catch {
                    continue;
                }
                result.Add(line);
            }

            return result;
        }
    }
}
