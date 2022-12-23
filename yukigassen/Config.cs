using System;
using TShockAPI;
using Newtonsoft.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.IO;

namespace yukigassen
{
    [JsonObject("雪合戦ルール")]
    public class Config
    {
        [JsonProperty("mode")]
        public int Mode { get; private set; } = 0;

        [JsonProperty("与えるバフのid")]
        public int BuffId { get; private set; } = 63;

        [JsonProperty("与えるバフの効果時間")]
        public int BuffDuration { get; private set; } = 500;

        [JsonProperty("一度に増えるポイントの量")]
        public int IncPoint { get; private set; } = 2;

        [JsonProperty("一度に減るポイントの量")]
        public int DecPoint { get; private set; } = 5;
    }
}
