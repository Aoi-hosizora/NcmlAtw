using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace NeteaseM2DServer.Src.Model {

    [JsonObject]
    class Metadata {
        public string title { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public double duration { get; set; }

        public static Metadata parseJson(string json) {
            return JsonConvert.DeserializeObject<Metadata>(json);
        }

        public override string ToString() {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }

        public override bool Equals(object obj) {
            if (obj.GetType() != typeof(Metadata)) return false;
            return title.Equals((obj as Metadata).title) &&
                artist.Equals((obj as Metadata).artist) &&
                album.Equals((obj as Metadata).album) &&
                duration == (obj as Metadata).duration;
        }
    }
}
