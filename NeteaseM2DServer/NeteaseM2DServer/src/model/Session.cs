using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace NeteaseM2DServer.src.model {

    [JsonObject]
    class Session {
        public bool isDestroyed { get; set; }

        public static Session parseJson(string json) {
            return JsonConvert.DeserializeObject<Session>(json);
        }

        public override string ToString() {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}
