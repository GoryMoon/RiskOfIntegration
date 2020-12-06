using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

namespace RoIActions
{
    public class MovePlayer: BaseAction<MovePlayer>
    {
        [DefaultValue("40")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "horizontal_force")]
        private string _force;
        
        [DefaultValue("30")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "vertical_force")]
        private string _forceUp;

        protected override MovePlayer Process(MovePlayer action, string username, string from, Dictionary<string, object> parameters)
        {
            action._force = StringToFloat(_force, float.MinValue, parameters).ToString(CultureInfo.InvariantCulture);
            action._forceUp = StringToFloat(_forceUp, float.MinValue, parameters).ToString(CultureInfo.InvariantCulture);
            return base.Process(action, username, from, parameters);
        }
    }
}