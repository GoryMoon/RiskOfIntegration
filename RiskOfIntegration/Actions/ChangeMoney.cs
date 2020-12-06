using System;
using System.Globalization;
using Newtonsoft.Json;
using R2API.Utils;

namespace RiskOfIntegration.Actions
{
    public class ChangeMoney: BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amount")]
        private int _amount;

        private static readonly CultureInfo UsCulture = CultureInfo.CreateSpecificCulture("en-US");
        
        public override ActionResponse Handle()
        {
            if (Utils.GetPlayer(out _, out var player))
            {
                player.master.money = (uint) Math.Max(0, player.master.money + _amount);
                if (_amount > 0)
                {
                    ChatMessage.SendColored($"{From} gave you {_amount.ToString("C", UsCulture)}!", "#00b894");
                }
                else
                {
                    ChatMessage.SendColored($"{From} took {Math.Abs(_amount).ToString("C", UsCulture)} from you!", "#d63031");
                }

                return ActionResponse.Done;
            }
            return ActionResponse.Retry;
        }
    }
}