using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

namespace RoIActions
{
    public class CombatShrine: BaseAction<CombatShrine>
    {
        [DefaultValue("100")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "base_credits")]
        private string _baseCredits;
        
        protected override CombatShrine Process(CombatShrine action, string username, string from, Dictionary<string, object> parameters)
        {
            action._baseCredits = StringToFloat(_baseCredits, float.MinValue, parameters).ToString(CultureInfo.InvariantCulture);
            return base.Process(action, username, from, parameters);
        }
    }
}