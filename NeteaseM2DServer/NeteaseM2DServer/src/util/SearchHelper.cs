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
        /// 判断搜索列表中的歌曲是否正确 (1)
        /// 专辑 -> 歌手 -> 标题
        /// </summary>
        /// <param name="trueRet">要查找的歌曲</param>
        /// <param name="trueArs">要查找歌曲的艺术家集合</param>
        private static bool CheckSongInSongsCorrect(Song searchRet, Metadata trueRet, List<string> trueArs) {

            // 专辑
            if (!searchRet.Al.Name.Equals(trueRet.album)) return false;

            // 歌手
            List<string> searchArs = new List<string>();
            foreach (Api.Ar ar in searchRet.Ar)
                searchArs.Add(ar.Name);

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
        /// 判断搜索列表中的专辑是否匹配 (2.1)
        /// 专辑名 -> 歌手
        /// </summary>
        /// <param name="trueRet">要查找的歌曲</param>
        /// <param name="trueArs">要查找歌曲的艺术家集合</param>
        private static bool CheckAlbumInAlbumsCorrect(Album searchAl, Metadata trueRet, List<string> trueArs) {
            // 专辑名
            if (!searchAl.Name.Trim().Equals(trueRet.album.Trim())) return false;

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
        /// 判断专辑中的歌曲是否正确并返回 (2.2)
        /// 只查歌名就可以
        /// </summary>
        /// <returns>null: 专辑中没有匹配</returns>
        private static Song CheckSongInAlbumCorrect(Album album, Metadata trueRet) {
            AlbumResult alRet = api.Album(album.Id);
            if (alRet.Code == 200 && alRet.Songs != null) {

                // 搜索专辑内每一首歌
                foreach (Song song in alRet.Songs) {

                    // 对每首歌名判断
                    foreach (string stk in getSearchToken(trueRet)) {

                        // 歌曲搜索成功
                        if (song.Name.Trim().Equals(stk.Trim()))
                            return song;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 列表内搜索 歌曲或专辑 (0)
        /// -> CheckSongInSongsCorrect
        /// -> CheckAlbumInAlbumsCorrect -> CheckSongInAlbumCorrect
        /// </summary>
        /// <param name="isSong">歌曲还是专辑</param>
        /// <returns>-1: 404</returns>
        private static Song SearchSongInList(SearchResult searchRet, Metadata currSong, bool isSong) {

            // 匹配的艺术家
            List<string> trueArs = currSong.artist.Split(new String[] { "/", "|", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (isSong) {
                // 搜歌曲列表
                if (searchRet.Result == null || searchRet.Result.Songs == null || searchRet.Result.Songs.Count == 0) return null;

                for (int i = 0; i < searchRet.Result.Songs.Count; i++) { // <<< 不能用 searchRet.Result.SongCount
                    Api.Song searchSong = searchRet.Result.Songs.ElementAt(i);
                    if (CheckSongInSongsCorrect(searchSong, currSong, trueArs)) {

                        // 歌曲信息匹配
                        Console.WriteLine("> Search Song Idx: " + i);
                        return searchSong;
                    }
                }
            } else {
                // 搜专辑列表
                if (searchRet.Result == null || searchRet.Result.Albums == null || searchRet.Result.Albums.Count == 0) return null;

                for (int i = 0; i < searchRet.Result.Albums.Count; i++) {
                    Api.Album searchAl = searchRet.Result.Albums.ElementAt(i);
                    if (CheckAlbumInAlbumsCorrect(searchAl, currSong, trueArs)) {
                        
                        // 专辑信息匹配，搜索专辑内歌曲
                        Console.WriteLine("> Search Album Name: " + searchAl.Name);
                        Song retSong = null;
                        if ((retSong = CheckSongInAlbumCorrect(searchAl, currSong)) != null) {

                            // 歌曲信息匹配
                            Console.WriteLine("> Search Song Idx: " + i);
                            return retSong;
                        }
                    }
                }
            }
            return null;
        }

        private static NeteaseMusicAPI api = new NeteaseMusicAPI();

        /// <summary>
        /// 获得集中歌名搜索方案
        /// </summary>
        private static string[] getSearchToken(Metadata currSong) {
            string title_WithPh = currSong.title;                                                       // 标题
            string title_WithoutPh = new Regex("\\(|\\)").Replace(title_WithPh, " ");                   // 去括号的标题保留内容
            string title_WithoutPh_WithoutToken = new Regex("\\(.*\\)").Replace(title_WithPh, " ");     // 去括号和括号内容的标题
            string[] searchToken = {
                title_WithPh,                                                                           // 标题
                title_WithoutPh,
                title_WithoutPh_WithoutToken,

                title_WithPh + " " + currSong.artist,                                                   // 标题 + 歌手
                title_WithoutPh + " " + currSong.artist,
                title_WithoutPh_WithoutToken + " " + currSong.artist,

                title_WithPh + " " + currSong.artist + " " + currSong.album,                            // 标题 + 歌手 + 专辑
                title_WithoutPh + " " + currSong.artist + " " + currSong.album,
                title_WithoutPh_WithoutToken + " " + currSong.artist + " " + currSong.album
            };
            // 去重
            return searchToken.GroupBy(p => p).Select(p => p.Key).ToArray();
        }

        /// <summary>
        /// 从 MetaData 搜索 Song
        /// </summary>
        /// <returns>null: 未搜索到</returns>
        public static Song Search(Metadata currSong) {

            // 搜索结果返回
            SearchResult sRet = null;

            // 歌曲列表和专辑列表搜索结果
            Song CorrectItemInList = null;

            // 先搜索歌曲
            foreach (var stk in getSearchToken(currSong)) {
                Console.WriteLine("> Search Song Token: " + stk);
                sRet = api.Search(stk, NeteaseMusicAPI.SearchType.Song);
                if (sRet.Code == 200 && ((CorrectItemInList = SearchSongInList(sRet, currSong, true)) != null))
                    break;
            }

            /*
             * Search Song Token
             * Search Song Idx
             */

            // 歌曲搜索不到，搜索专辑
            if (CorrectItemInList == null) {
                sRet = api.Search(currSong.album, NeteaseMusicAPI.SearchType.Album);
                if (sRet.Code == 200)
                    CorrectItemInList = SearchSongInList(sRet, currSong, false);
            }

            /*
             * Search Song Token
             * Search Album Name
             * Search Song Idx
             */

            // 可能返回 Song 或 null
            return CorrectItemInList;
        }
    }
}
