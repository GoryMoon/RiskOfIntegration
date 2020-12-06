using Newtonsoft.Json;
using R2API.Utils;
using RoR2;

namespace RiskOfIntegration.Actions
{
    public class HealPlayer: BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amount")]
        private float _amount;

        public override ActionResponse Handle()
        {
            if (Utils.GetPlayer(out _, out var player))
            {
                player.healthComponent.Heal(_amount, new ProcChainMask());
                if (_amount > 0)
                {
                    ChatMessage.SendColored($"{From} healed you for {_amount:N} health!", "#00b894");
                }
                else
                {
                    ChatMessage.SendColored($"{From} damaged you for {_amount:N} health!", "#d63031");
                }
                
                return ActionResponse.Done;
            }

            return ActionResponse.Retry;
        }
    }
}