using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi
{
    public class Case
    {
        [JsonProperty(PropertyName = "id")]
        public int id { get; set; }


        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }


        [JsonProperty(PropertyName = "info")]
        public string info { get; set; }
    }
}