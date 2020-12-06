using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

namespace RoIActions
{
    public class HealPlayer: BaseAction<HealPlayer>
    {
        [DefaultValue("100")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amount_min")]
        private string _amountMin;

        [DefaultValue("100")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amount_max")]
        private string _amountMax;
        
        protected override HealPlayer Process(HealPlayer action, string username, string from, Dictionary<string, object> parameters)
        {
            action._amountMin = StringToFloat(_amountMin, float.MinValue, parameters).ToString(CultureInfo.InvariantCulture);
            action._amountMax = StringToFloat(_amountMax, float.MinValue, parameters).ToString(CultureInfo.InvariantCulture);
            return base.Process(action, username, from, parameters);
        }
    }
}