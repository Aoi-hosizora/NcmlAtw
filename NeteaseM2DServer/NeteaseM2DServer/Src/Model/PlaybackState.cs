using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace NeteaseM2DServer.Src.Model {

    [JsonObject]
    class PlaybackState {
        public bool isPlay { get; set; }
        public double currentPosSecond { get; set; }

        public static PlaybackState parseJson(string json) {
            return JsonConvert.DeserializeObject<PlaybackState>(json);
        }

        public override string ToString() {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}