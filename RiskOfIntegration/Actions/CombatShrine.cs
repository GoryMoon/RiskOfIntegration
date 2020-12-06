using System;
using Newtonsoft.Json;
using RiskOfIntegration.Misc;
using RoR2;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RiskOfIntegration.Actions
{
    public class CombatShrine: BaseAction
    {
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "base_credits")]
        private int _baseCredits;
        
        private static CombatShrineManager _combatShrineManager;
        public override ActionResponse Handle()
        {
            if (Utils.GetPlayer(out _, out var player))
            {
                if (_combatShrineManager == null)
                {
                    var gameObject = new GameObject();
                    gameObject.SetActive(false);
                    var combatDirector = gameObject.AddComponent<CombatDirector>();
                    combatDirector.moneyWaveIntervals = new[] {new RangeFloat {min = 1, max = 1}};
                    gameObject.SetActive(true);
                    _combatShrineManager = gameObject.AddComponent<CombatShrineManager>();
                    Object.Instantiate(gameObject);
                }

                if (_combatShrineManager.RunEffect(player, _baseCredits)) return ActionResponse.Done;
                
                TryLater(TimeSpan.FromSeconds(5));
                return ActionResponse.Retry;
            }

            return ActionResponse.Retry;
        }
    }
}