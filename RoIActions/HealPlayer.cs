﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

namespace RoIActions
{
    public class HealPlayer: BaseAction<HealPlayer>
    {
        [DefaultValue("100")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amount")]
        private string _amount;

        protected override HealPlayer Process(HealPlayer action, string username, string from, Dictionary<string, object> parameters)
        {
            action._amount = StringToFloat(_amount, float.MinValue, parameters).ToString(CultureInfo.InvariantCulture);
            return base.Process(action, username, from, parameters);
        }
    }
}