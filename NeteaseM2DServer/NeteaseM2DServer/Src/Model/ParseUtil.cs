using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeteaseM2DServer.Src.Model
{
    class ParseUtil {

        /*
         * PlaybackState: {"isPlay":false,"currentPosSecond":19.648}
         * Metadata: {"title":"ユーフォリアム","artist":"鈴湯","album":"アストラエアの白き永遠","duration":285.213}
         */

        public enum Type {
            PlaybackState, Metadata
        }

        public static Type checkType(string json) {
            if (json.StartsWith("{\"isPlay\":")) 
                return Type.PlaybackState;
            else 
                return Type.Metadata;
        }
    }
}
