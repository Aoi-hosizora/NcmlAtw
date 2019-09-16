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
    }
}
