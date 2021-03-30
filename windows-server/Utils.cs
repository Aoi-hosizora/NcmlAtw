using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NcmlAtwServer {

    static class Utils {

        public static List<string> GetNetworkInterfaces() {
            var result = new List<string>();
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces()) {
                if (ni.GetIPProperties().UnicastAddresses.Any(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork)) {
                    result.Add(ni.Description);
                }
            }
            return result;
        }

        public static string GetNetworkInterfaceIPv4(string description) {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces()) {
                if (ni.Description == description) {
                    foreach (var addr in ni.GetIPProperties().UnicastAddresses) {
                        if (addr.Address.AddressFamily == AddressFamily.InterNetwork) {
                            return addr.Address.ToString();
                        }
                    }
                }
            }
            return "unknown";
        }

        private const string QRCODE_MAGIC = "NCMLATW-";

        public static Bitmap GenerateAddressQrcode(string ip, int port, int pixelsPerModule = 10) {
            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode($"{QRCODE_MAGIC}{ip}:{port}", QRCodeGenerator.ECCLevel.Q);
            var code = new QRCode(data);
            return code.GetGraphic(pixelsPerModule);
        }

        private static Form _bitmapForm;

        public static void ShowBitmapForm(Bitmap bmp, string title, Form parent) {
            _bitmapForm?.Close();
            _bitmapForm = new Form {
                Text = title,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false,
                ShowIcon = false,
                StartPosition = FormStartPosition.CenterScreen,
                Size = new Size(bmp.Width, bmp.Height + (parent.RectangleToScreen(parent.ClientRectangle).Top - parent.Top - 8)),
            };

            var pictureBox = new PictureBox {
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = bmp,
                Dock = DockStyle.Fill,
            };
            _bitmapForm.Controls.Add(pictureBox);
            _bitmapForm.Show(parent);
        }

        public static void CloseBitmapForm() {
            _bitmapForm?.Close();
        }

        public static long GetCurrentTimestamp() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return long.Parse(Convert.ToInt64(ts.TotalMilliseconds).ToString());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Song SearchMusicFromNcma(Metadata metadata) {
            var api = new Ncma();

            // search song
            foreach (var keyword in GetSearchKeywords(metadata)) {
                Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>\n> search keyword: {keyword}");
                var searchSongResult = api.Search(keyword, Ncma.SearchType.Song);
                Console.WriteLine($"> search result code: {searchSongResult.Code}");
                if (searchSongResult.Code == 200) {
                    var result = FilterSongInSearchResult(searchSongResult, metadata, Ncma.SearchType.Song);
                    if (result != null) {
                        return result;
                    }
                }
            }

            // search album
            Console.WriteLine($"> Search keyword: {metadata.Album}");
            var searchAlbumResult = api.Search(metadata.Album, Ncma.SearchType.Album);
            if (searchAlbumResult.Code == 200) {
                var result = FilterSongInSearchResult(searchAlbumResult, metadata, Ncma.SearchType.Album);
                if (result != null) {
                    return result;
                }
            }

            return null; // not found
        }

        private static string[] GetSearchKeywords(Metadata metadata) {
            var title = metadata.Title.Trim();
            var artist = metadata.Artist.Trim();
            var album = metadata.Album.Trim();
            var title2 = new Regex(@"\(|\)").Replace(title, " ").Trim();
            var title3 = new Regex(@"\(.*\)").Replace(title, " ").Trim();
            var keywords = new string[] {
                title, title2, title3,
                $"{title} {artist}",  $"{title2} {artist}",  $"{title3} {artist}",
                $"{title} {artist} {album}",  $"{title2} {artist} {album}",  $"{title3} {artist} {album}",
            };
            return keywords.Select(d => d.Trim()).Distinct().ToArray();
        }

        private static Song FilterSongInSearchResult(SearchResult sr, Metadata metadata, Ncma.SearchType type) {
            var api = new Ncma();

            var artists = metadata.Artist.Split(new string[] { "/", "|", "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim());
            bool CheckCorrectSong(Song s) {
                // check album
                Console.WriteLine($"> check album: {s.Al.Name} | {metadata.Album}");
                if (s.Al.Name.Trim() != metadata.Album.Trim()) {
                    return false;
                }
                // check artists
                var ars = s.Ar.Select(a => a.Name.Trim());
                Console.WriteLine($"> check artists: {string.Join(",", ars)} | {string.Join(",", artists)}");
                if (artists.Any(a => !ars.Contains(a))) {
                    return false;
                }
                // check song title
                Console.WriteLine($"> check title: {s.Name} | {metadata.Title}");
                if (s.Name.Trim() == metadata.Title.Trim()) {
                    return true;
                }
                var titles1 = metadata.Title.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList();
                var titles2 = s.Name.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList();
                if (titles1.Count > 1 && titles2.Count > 1) {
                    return titles1[0] == titles2[0] && titles1[1] == titles2[1];
                }
                return titles1[0] == titles2[0];
            }
            bool CheckCorrectAlbum(Album al) {
                // check album name
                if (al.Name.Trim() != metadata.Title.Trim()) {
                    return false;
                }
                // check artists
                var ars = al.Artists.Select(a => a.Name.Trim());
                return artists.All(a => ars.Contains(a));
            }

            if (type == Ncma.SearchType.Song) {
                if ((sr?.Result?.Songs?.Count ?? 0) == 0) {
                    return null;
                }
                foreach (var song in sr.Result.Songs) { // for each songs
                    if (CheckCorrectSong(song)) {
                        return song;
                    }
                }
            } else if (type == Ncma.SearchType.Album) {
                if ((sr?.Result?.Albums?.Count ?? 0) == 0) {
                    return null;
                }
                foreach (var album in sr.Result.Albums) { // for each albums
                    if (CheckCorrectAlbum(album)) {
                        var albumResult = api.Album(album.Id);
                        if (albumResult.Code == 200 && albumResult.Songs != null) {
                            foreach (var song in albumResult.Songs) { // for each songs in album
                                if (CheckCorrectSong(song)) {
                                    return song;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Tuple<LyricPage, LyricState> SearchLyricFromNcma(long musicId) {
            var api = new Ncma();
            var lr = api.Lyric(musicId);
            if (lr.Code == 200) {
                if (lr.Nolyric) {
                    return new Tuple<LyricPage, LyricState>(null, LyricState.NoLyric);
                }
                var lp = LyricPage.ParseLyricPage(lr.Lrc?.Lyric ?? "");
                if (lp != null) {
                    return new Tuple<LyricPage, LyricState>(lp, LyricState.Found);
                }
            }
            return new Tuple<LyricPage, LyricState>(null, LyricState.NotFound);
        }

        public static string GetMusicLink(long musicId) {
            return $"https://music.163.com/#/song?id={musicId}";
        }

        private const uint WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_EXSTYLE = (-20);
        private const int LWA_ALPHA = 0x2;

        [DllImport("user32")]
        public static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32")]
        public static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32")]
        public static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

        public static void SetWindowCrossOver(Form form, double opacity, bool isCross) {
            // uint intExTemp = GetWindowLong(form.Handle, GWL_EXSTYLE);
            if (isCross) {
                SetWindowLong(form.Handle, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED);
            } else {
                SetWindowLong(form.Handle, GWL_EXSTYLE, WS_EX_LAYERED);
            }
            SetLayeredWindowAttributes(form.Handle, 0, (int) (opacity * 255), LWA_ALPHA);
        }
    }
}
