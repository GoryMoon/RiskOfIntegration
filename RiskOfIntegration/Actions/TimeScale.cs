using Newtonsoft.Json;
using RiskOfIntegration.Misc;
using UnityEngine;

namespace RiskOfIntegration.Actions
{
    public class TimeScale: BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "scale")]
        private float _scale;
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "duration")]
        private float _duration;

        private static TimeScaleManager _timeScaleManager;
        
        public override ActionResponse Handle()
        {
            if (_timeScaleManager == null)
            {
                var gameObject = new GameObject();
                _timeScaleManager = gameObject.AddComponent<TimeScaleManager>();
                Object.Instantiate(gameObject);
            }

            _timeScaleManager.Queue(_scale, _duration, From);
            return ActionResponse.Done;
        }
    }
}