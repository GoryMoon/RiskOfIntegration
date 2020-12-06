using Newtonsoft.Json;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfIntegration.Actions
{
    public class HealPlayer: BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amount_min")]
        private float _amountMin;
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amount_max")]
        private float _amountMax;

        public override ActionResponse Handle()
        {
            if (Utils.GetPlayer(out _, out var player))
            {
                var amount = Random.Range(_amountMin, _amountMax);
                player.healthComponent.Heal(amount, new ProcChainMask());
                if (amount > 0)
                {
                    ChatMessage.SendColored($"{From} healed you for {amount:N} health!", "#00b894");
                }
                else
                {
                    ChatMessage.SendColored($"{From} damaged you for {amount:N} health!", "#d63031");
                }
                
                return ActionResponse.Done;
            }

            return ActionResponse.Retry;
        }
    }
}