using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeteaseM2DServer.Src.Api;
using NeteaseM2DServer.Src.Model;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace NeteaseM2DServer.Src.Util {

    static class SearchHelper {

        /// <summary>
        /// 判断列表中搜索到的歌曲是否正确
        /// 专辑 -> 歌手 -> 标题
        /// </summary>
        private static bool CheckSongCorrect(Song searchRet, Metadata trueRet) {

            // 专辑
            if (!searchRet.Al.Name.Equals(trueRet.album)) return false;

            // 歌手
            List<string> searchArs = new List<string>();
            foreach (Api.Ar ar in searchRet.Ar)
                searchArs.Add(ar.Name);
            List<string> trueArs = trueRet.artist.Split(new String[] { "/", "|", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (string trueAr in trueArs) {
                // trueAr 不在 searchArs 内
                if (searchArs.IndexOf(trueAr.Trim()) == -1)
                    return false;
            }

            // 标题
            if (searchRet.Name.IndexOf("(") != -1) {
                // 存在括号
                if (trueRet.title.IndexOf("(") == -1) return false;

                string[] searchToken = searchRet.Name.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                string[] trueToken = trueRet.title.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);

                // xxx1 (xxx2)

                // xxx1
                if (!searchToken[0].Trim().Equals(trueToken[0].Trim())) return false;
                // xxx2
                if (!searchToken[1].Trim().Equals(trueToken[1].Trim())) return false;

                else return true;
            } else {
                // 不存在括号
                return searchRet.Name.Trim().Equals(new Regex("\\(.*\\)").Replace(trueRet.title, "").Trim());
            }
        }

        /// <summary>
        /// 判断列表中搜索到的专辑是否匹配
        /// 专辑名 -> 歌手
        /// </summary>
        private static bool CheckAlbumCorrect(Album searchAl, string trueAlName, List<string> trueArs) {
            // 专辑名
            if (!searchAl.Name.Trim().Equals(trueAlName.Trim())) return false;

            // 歌手
            List<string> searchArs = new List<string>();
            searchArs.Clear();
            foreach (Api.Artist ar in searchAl.Artists)
                searchArs.Add(ar.Name);

            foreach (string trueAr in trueArs)
                if (searchArs.IndexOf(trueAr) == -1)
                    return false;

            return true;
        }

        /// <summary>
        /// 列表内搜索 歌曲或专辑
        /// </summary>
        /// <param name="isSong">歌曲还是专辑</param>
        /// <returns>-1: 404</returns>
        private static Object SearchIdInList(SearchResult searchRet, Metadata trueRet, bool isSong) {

            if (isSong) {
                // 搜歌曲列表
                if (searchRet.Result == null || searchRet.Result.Songs == null || searchRet.Result.Songs.Count == 0) return null;
                for (int i = 0; i < searchRet.Result.Songs.Count; i++) { // <<< 不能用 searchRet.Result.SongCount
                    Api.Song searchSong = searchRet.Result.Songs.ElementAt(i);
                    if (CheckSongCorrect(searchSong, trueRet)) {
                        Console.WriteLine("> Search Song ID: " + i);
                        return searchSong;
                    }
                }
            } else {
                // 搜专辑列表
                if (searchRet.Result == null || searchRet.Result.Albums == null || searchRet.Result.Albums.Count == 0) return null;
                List<string> trueArs = trueRet.artist.Split(new String[] { "/", "|", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                for (int i = 0; i < searchRet.Result.Albums.Count; i++) {
                    Api.Album searchAl = searchRet.Result.Albums.ElementAt(i);
                    if (CheckAlbumCorrect(searchAl, trueRet.album, trueArs)) {
                        Console.WriteLine("> Song Album Id: " + i);
                        return searchAl;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 从 MetaData 搜索 Song
        /// </summary>
        /// <returns>null: 未搜索到</returns>
        public static Song Search(Metadata currSong) {
            var api = new NeteaseMusicAPI();

            // 几种歌曲名方案搜索
            string title_WithPh = currSong.title;                                                         // 标题
            string title_WithoutPh = new Regex("\\(|\\)").Replace(title_WithPh, " ");                               // 去括号的标题保留内容
            string title_WithoutPh_WithoutToken = new Regex("\\(.*\\)").Replace(title_WithPh, " ");                 // 去括号和括号内容的标题
            string[] searchToken = {
                title_WithPh,                                                                                       // 标题
                title_WithoutPh,
                title_WithoutPh_WithoutToken,

                title_WithPh + " " + currSong.artist,                                                     // 标题 + 歌手
                title_WithoutPh + " " + currSong.artist,
                title_WithoutPh_WithoutToken + " " + currSong.artist,

                title_WithPh + " " + currSong.artist + " " + currSong.album,                    // 标题 + 歌手 + 专辑
                title_WithoutPh + " " + currSong.artist + " " + currSong.album,
                title_WithoutPh_WithoutToken + " " + currSong.artist + " " + currSong.album
            };
            // 去重
            searchToken = searchToken.GroupBy(p => p).Select(p => p.Key).ToArray();

            ///////////////////////////////////////////////////////////////////////////////

            // 搜索结果返回
            SearchResult sRet = null;

            // 列表搜索结果
            Object CorrectItemInList = null;

            // 先搜索歌曲
            foreach (var stk in searchToken) {
                Console.WriteLine("> Search Song Token: " + stk);
                sRet = api.Search(stk);
                if (sRet.Code == 200 && ((CorrectItemInList = SearchIdInList(sRet, currSong, true)) != null))
                    break;
            }

            // 歌曲搜索成功
            if (CorrectItemInList != null)
                return (Song)CorrectItemInList;

            /////////

            // 歌曲搜索不到，搜索专辑
            sRet = api.Search(currSong.album, NeteaseMusicAPI.SearchType.Album);
            if (sRet.Code == 200 && ((CorrectItemInList = SearchIdInList(sRet, currSong, false)) != null)) {
                AlbumResult alRet = api.Album(((Api.Album) CorrectItemInList).Id);
                if (alRet.Code == 200 && alRet.Songs != null) {

                    // 搜索专辑内每一首歌
                    foreach (Song song in alRet.Songs) {
                        // 对每首歌名判断
                        foreach (string stk in searchToken) {
                            // 歌曲搜索成功
                            if (song.Name.Equals(stk))
                                return song;
                        }
                    }
                }
            }

            return null;
        }
    }
}
