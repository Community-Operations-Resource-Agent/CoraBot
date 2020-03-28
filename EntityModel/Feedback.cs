using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace EntityModel
{
    public class Feedback : Model
    {
        public string CreatedById { get; set; }

        public string Text { get; set; }
    }
}
