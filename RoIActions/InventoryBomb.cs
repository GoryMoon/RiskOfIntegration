using System.ComponentModel;
using Newtonsoft.Json;

namespace RoIActions
{
    public class InventoryBomb: BaseAction<InventoryBomb>
    {
        [DefaultValue("60")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "spread")]
        private float _spread;
    }
}