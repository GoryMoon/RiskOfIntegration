using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

namespace RoIActions
{
    public class SpawnMonster: BaseAction<SpawnMonster>
    {
        [DefaultValue("Beetle")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "name")]
        private string _name;
        
        [DefaultValue("5")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amount")]
        private string _amount;
        
        [DefaultValue(10)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "min_distance")]
        private int _minDistance;
        
        [DefaultValue(20)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "max_distance")]
        private int _maxDistance;
        
        [DefaultValue("None")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "elite")]
        private string _elite;
        
        [DefaultValue("Monster")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "team")]
        private string _team;
        
        protected override SpawnMonster Process(SpawnMonster action, string username, string from, Dictionary<string, object> parameters)
        {
            action._amount = StringToFloat(_amount, float.MinValue, parameters).ToString(CultureInfo.InvariantCulture);
            return base.Process(action, username, from, parameters);
        }
    }
}