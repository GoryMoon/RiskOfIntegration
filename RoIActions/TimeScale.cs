using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

namespace RoIActions
{
    public class TimeScale: BaseAction<TimeScale>
    {
        [DefaultValue("0.5")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "scale")]
        private string _scale;
        
        [DefaultValue("5")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "duration")]
        private int _duration;

        protected override TimeScale Process(TimeScale action, string username, string from, Dictionary<string, object> parameters)
        {
            action._scale = StringToFloat(_scale, float.MinValue, parameters).ToString(CultureInfo.InvariantCulture);
            return base.Process(action, username, from, parameters);
        }
    }
}