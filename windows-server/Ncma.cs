using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Numerics;
using Newtonsoft.Json;

namespace NcmlAtwServer {

    // https://github.com/GEEKiDoS/NeteaseMuiscApi/blob/master/NeteaseCloudMuiscApi.cs

    class Ncma {

        // General
        private readonly string _MODULUS = "00e0b509f6259df8642dbc35662901477df22677ec152b5ff68ace615bb7b725152b3ab17a876aea8a5aa76d2e417629ec4ee341f56135fccf695280104e0312ecbda92557c93870114af6c9d05c4f7f0c3685b7a46bee255932575cce10b424d813cfe4875d3e82047b97ddef52741d546b8e289dc6935b3ece0462db0a22b8e7";
        private readonly string _NONCE = "0CoJUm6Qyw8W8jud";
        private readonly string _PUBKEY = "010001";
        private readonly string _VI = "0102030405060708";
        private readonly string[] _USERAGENTS = {
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1",
            "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36",
            "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36",
            "Mozilla/5.0 (Linux; Android 5.1.1; Nexus 6 Build/LYZ28E) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_2 like Mac OS X) AppleWebKit/603.2.4 (KHTML, like Gecko) Mobile/14F89;GameHelper",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/603.2.4 (KHTML, like Gecko) Version/10.1.1 Safari/603.2.4",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 10_0 like Mac OS X) AppleWebKit/602.1.38 (KHTML, like Gecko) Version/10.0 Mobile/14A300 Safari/602.1",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.12; rv:46.0) Gecko/20100101 Firefox/46.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:46.0) Gecko/20100101 Firefox/46.0",
            "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)",
            "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0)",
            "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",
            "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Win64; x64; Trident/6.0)",
            "Mozilla/5.0 (Windows NT 6.3; Win64, x64; Trident/7.0; rv:11.0) like Gecko",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/13.10586",
            "Mozilla/5.0 (iPad; CPU OS 10_0 like Mac OS X) AppleWebKit/602.1.38 (KHTML, like Gecko) Version/10.0 Mobile/14A300 Safari/602.1"
        };
        private readonly string _COOKIE = "os=pc;osver=Microsoft-Windows-10-Professional-build-16299.125-64bit;appver=2.0.3.131777;channel=netease;__remember_me=true";
        private readonly string _REFERER = "http://music.163.com/";
        private readonly string _NUIDRAND = "0123456789abcdefghijklmnopqrstuvwxyz";
        private readonly Random _RANDOM = new Random();
        // use keygen in c#
        private readonly string _secretKey;
        private readonly string _encSecKey;

        public Ncma() {
            _secretKey = CreateSecretKey(16);
            _encSecKey = RSAEncode(_secretKey);
        }

        private string CreateSecretKey(int length) {
            var str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var r = "";
            var rnd = new Random();
            for (int i = 0; i < length; i++) {
                r += str[rnd.Next(0, str.Length)];
            }
            return r;
        }

        private Dictionary<string, string> Prepare(string raw) {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["params"] = AESEncode(raw, _NONCE);
            data["params"] = AESEncode(data["params"], _secretKey);
            data["encSecKey"] = _encSecKey;

            return data;
        }

        // encrypt mod
        private string RSAEncode(string text) {
            string srtext = new string(text.Reverse().ToArray()); ;
            var a = BCHexDec(BitConverter.ToString(Encoding.Default.GetBytes(srtext)).Replace("-", ""));
            var b = BCHexDec(_PUBKEY);
            var c = BCHexDec(_MODULUS);
            var key = BigInteger.ModPow(a, b, c).ToString("x");
            key = key.PadLeft(256, '0');
            if (key.Length > 256)
                return key.Substring(key.Length - 256, 256);
            else
                return key;
        }

        private BigInteger BCHexDec(string hex) {
            BigInteger dec = new BigInteger(0);
            int len = hex.Length;
            for (int i = 0; i < len; i++) {
                dec += BigInteger.Multiply(new BigInteger(Convert.ToInt32(hex[i].ToString(), 16)), BigInteger.Pow(new BigInteger(16), len - i - 1));
            }
            return dec;
        }

        private string AESEncode(string secretData, string secret = "TA3YiYCfY2dDJQgg") {
            byte[] encrypted;
            byte[] IV = Encoding.UTF8.GetBytes(_VI);

            using (var aes = Aes.Create()) {
                aes.Key = Encoding.UTF8.GetBytes(secret);
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                using (var encryptor = aes.CreateEncryptor()) {
                    using (var stream = new MemoryStream()) {
                        using (var cstream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write)) {
                            using (var sw = new StreamWriter(cstream)) {
                                sw.Write(secretData);
                            }
                            encrypted = stream.ToArray();
                        }
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        // fake curl
        private string CURL(string url, Dictionary<string, string> parms, string method = "POST") {
            string result;
            var nuid = new string(Enumerable.Repeat(_NUIDRAND, 32).Select(s => s[_RANDOM.Next(s.Length)]).ToArray());
            using (var wc = new WebClient()) {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                wc.Headers.Add(HttpRequestHeader.Referer, _REFERER);
                wc.Headers.Add(HttpRequestHeader.UserAgent, _USERAGENTS[_RANDOM.Next(_USERAGENTS.Length)]);
                wc.Headers.Add(HttpRequestHeader.Cookie, $"_iuqxldmzr_=32;_ntes_nnid=${nuid},${DateTimeOffset.Now.ToUnixTimeMilliseconds()};_ntes_nuid=${nuid};{_COOKIE}");
                var reqparm = new System.Collections.Specialized.NameValueCollection();
                foreach (var keyPair in parms) {
                    reqparm.Add(keyPair.Key, keyPair.Value);
                }

                byte[] responsebytes = wc.UploadValues(url, method, reqparm);
                result = Encoding.UTF8.GetString(responsebytes);
            }
            return result;
        }

        // api start
        private class SearchJson {
            public string s;
            public int type;
            public int limit;
            public string total = "true";
            public int offset;
            public string csrf_token = "";
        }

        public enum SearchType {
            Song = 1,
            Album = 10,
            Artist = 100,
            PlayList = 1000,
            User = 1002,
            Radio = 1009,
        }

        public SearchResult Search(string keyword, SearchType type = SearchType.Song, int limit = 30, int offset = 0) {
            var url = "http://music.163.com/weapi/cloudsearch/get/web";
            var data = new SearchJson {
                s = keyword,
                type = (int) type,
                limit = limit,
                offset = offset,
            };

            string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));

            var DeserialedObj = JsonConvert.DeserializeObject<SearchResult>(raw);

            return DeserialedObj;
        }


        public ArtistResult Artist(long artist_id) {
            var url = "http://music.163.com/weapi/v1/artist/" + artist_id.ToString() + "?csrf_token=";
            var data = new Dictionary<string, string>
            {
                {"csrf_token",""}
            };
            var raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));

            var deserialedObj = JsonConvert.DeserializeObject<ArtistResult>(raw);
            return deserialedObj;
        }

        public AlbumResult Album(long album_id) {
            string url = "http://music.163.com/weapi/v1/album/" + album_id.ToString() + "?csrf_token=";
            var data = new Dictionary<string, string> {
                { "csrf_token","" },
            };
            string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));
            var deserialedObj = JsonConvert.DeserializeObject<AlbumResult>(raw);
            return deserialedObj;
        }

        public DetailResult Detail(long song_id) {
            string url = "http://music.163.com/weapi/v3/song/detail?csrf_token=";
            var data = new Dictionary<string, string> {
                { "c",
                    "[" + JsonConvert.SerializeObject(new Dictionary<string, string> { //神tm 加密的json里套json mdzz (说不定一次可以查多首歌?)
                        { "id", song_id.ToString() }
                    }) + "]"
                },
                {"csrf_token",""},
            };
            string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));

            var deserialedObj = JsonConvert.DeserializeObject<DetailResult>(raw);
            return deserialedObj;
        }

        private class GetSongUrlJson {
            public long[] ids;
            public long br;
            public string csrf_token = "";
        }

        public SongUrls GetSongsUrl(long[] song_id, long bitrate = 999000) {
            string url = "http://music.163.com/weapi/song/enhance/player/url?csrf_token=";


            var data = new GetSongUrlJson {
                ids = song_id,
                br = bitrate
            };

            string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));

            var deserialedObj = JsonConvert.DeserializeObject<SongUrls>(raw);
            return deserialedObj;
        }



        public PlayListResult Playlist(long playlist_id) {
            string url = "http://music.163.com/weapi/v3/playlist/detail?csrf_token=";
            var data = new Dictionary<string, string> {
                { "id",playlist_id.ToString() },
                { "n" , "1000" },
                { "csrf_token" , "" },
            };
            string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));

            var deserialedObj = JsonConvert.DeserializeObject<PlayListResult>(raw);
            return deserialedObj;
        }

        public LyricResult Lyric(long song_id) {
            string url = "http://music.163.com/weapi/song/lyric?csrf_token=";
            var data = new Dictionary<string, string> {
                { "id",song_id.ToString()},
                { "os","pc" },
                { "lv","-1" },
                { "kv","-1" },
                { "tv","-1" },
                { "csrf_token","" }
            };

            string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));
            var deserialedObj = JsonConvert.DeserializeObject<LyricResult>(raw);
            return deserialedObj;
        }

        public MVResult MV(int mv_id) {
            string url = "http://music.163.com/weapi/mv/detail?csrf_token=";
            var data = new Dictionary<string, string> {
                { "id",mv_id.ToString() },
                { "csrf_token","" },
            };
            string raw = CURL(url, Prepare(JsonConvert.SerializeObject(data)));
            var deserialedObj = JsonConvert.DeserializeObject<MVResult>(
                raw.Replace("\"720\"", "\"the720\"")
                   .Replace("\"480\"", "\"the480\"")
                   .Replace("\"240\"", "\"the240\"")); //不能解析数字key的解决方案
            return deserialedObj;
        }

        //static url encrypt, use for pic

        public string Id2Url(int id) {
            byte[] magic = Encoding.ASCII.GetBytes("3go8&8*3*3h0k(2)2");
            byte[] song_id = Encoding.ASCII.GetBytes(id.ToString());

            for (int i = 0; i < song_id.Length; i++)
                song_id[i] = Convert.ToByte(song_id[i] ^ magic[i % magic.Length]);

            string result;

            using (var md5 = MD5.Create()) {
                md5.ComputeHash(song_id);
                result = Convert.ToBase64String(md5.Hash);
            }

            result = result.Replace("/", "_");
            result = result.Replace("+", "-");
            return result;
        }
    }
}
