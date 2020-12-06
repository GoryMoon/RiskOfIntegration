using Newtonsoft.Json;
using R2API.Utils;
using UnityEngine;

namespace RiskOfIntegration.Actions
{
    public class MovePlayer: BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "horizontal_force")]
        private float _force;
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "vertical_force")]
        private float _forceUp;
        
        public override ActionResponse Handle()
        {
            if (Utils.GetPlayer(out _, out var player))
            {
                var force = Vector3.forward * Random.Range(-_force, _force) + Vector3.right * Random.Range(-_force, _force) +
                              Vector3.up * _forceUp;
                var characterMotor = player.characterMotor;
                characterMotor.velocity += force;
                characterMotor.Motor.ForceUnground();
                characterMotor.disableAirControlUntilCollision = true;
                ChatMessage.SendColored($"{From} forced you to move! :O", "#d63031");
                return ActionResponse.Done;
            }

            return ActionResponse.Done;
        }
    }
}