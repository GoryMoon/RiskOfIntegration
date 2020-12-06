using System;
using Humanizer;
using Newtonsoft.Json;
using R2API.Utils;
using RoR2;

namespace RiskOfIntegration.Actions
{
    public class ChangeRuntime: BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amount")]
        private float _amount;
        
        public override ActionResponse Handle()
        {
            Run.instance.SetRunStopwatch(Math.Max(0, Run.instance.GetRunStopwatch() + _amount));
            if (_amount > 0)
            {
                ChatMessage.SendColored($"{From} added {TimeSpan.FromSeconds(_amount).Humanize(2)} to the run!", "#d63031");
            }
            else
            {
                ChatMessage.SendColored($"{From} removed {TimeSpan.FromSeconds(_amount).Humanize(2)} from the run!", "#00b894");
            }
            
            return ActionResponse.Done;
        }
    }
}