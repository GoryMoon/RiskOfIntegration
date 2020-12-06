using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

namespace RoIActions
{
    public class ChangeRuntime: BaseAction<ChangeRuntime>
    {
        [DefaultValue("1")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amount")]
        private string _amount;

        protected override ChangeRuntime Process(ChangeRuntime action, string username, string from, Dictionary<string, object> parameters)
        {
            action._amount = StringToFloat(_amount, float.MinValue, parameters).ToString(CultureInfo.InvariantCulture);
            return base.Process(action, username, from, parameters);
        }
    }
}