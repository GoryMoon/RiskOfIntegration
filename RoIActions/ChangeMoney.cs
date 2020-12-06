using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

namespace RoIActions
{
    public class ChangeMoney: BaseAction<ChangeMoney>
    {
        [DefaultValue("5")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amount")]
        private string _amount;
        
        protected override ChangeMoney Process(ChangeMoney action, string username, string from, Dictionary<string, object> parameters)
        {
            action._amount = StringToInt(_amount, int.MinValue, parameters).ToString(CultureInfo.InvariantCulture);
            return base.Process(action, username, from, parameters);
        }
    }
}