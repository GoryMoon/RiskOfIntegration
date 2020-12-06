using System;
using Humanizer;
using Newtonsoft.Json;
using R2API.Utils;
using RoR2;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RiskOfIntegration.Actions
{
    public class SpawnMonster: BaseAction
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "name")]
        private string _name;
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "amount")]
        private int _amount;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "min_distance")]
        private int _minDistance;
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "max_distance")]
        private int _maxDistance;
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "elite")]
        private string _elite;
        
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "team")]
        private string _team;
        
        public override ActionResponse Handle()
        {
            if (Utils.GetPlayer(out _, out var player))
            {
                var spawnCard = Utils.GetSpawnCard(_name);
                if (spawnCard == null)
                {
                    ChatMessage.SendColored($"ERROR: Can't find monster with name \"{_name}\"", "#d63031");
                    return ActionResponse.Done;
                }

                var director = Object.FindObjectOfType<CombatDirector>();
                var rng = director.GetFieldValue<Xoroshiro128Plus>("rng");

                for (var i = 0; i < _amount; i++)
                {
                    if (!Enum.TryParse(_team, out TeamIndex teamIndex) || teamIndex == TeamIndex.None || teamIndex == TeamIndex.Count)
                    {
                        teamIndex = TeamIndex.Neutral;
                    }

                    var placementRule = new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                        spawnOnTarget = player.transform,
                        preventOverhead = false,
                        minDistance = _minDistance,
                        maxDistance = _maxDistance
                    };
                    
                    DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placementRule, rng)
                    {
                        teamIndexOverride = teamIndex,
                        ignoreTeamMemberLimit = true,
                        onSpawnedServer = OnCardSpawned
                    });
                }
                ChatMessage.SendColored($"{From} spawned {_amount}x {_name.Replace("Master", string.Empty).Humanize()} on you!", "#d63031");

                return ActionResponse.Done;
            }

            return ActionResponse.Retry;
        }

        private void OnCardSpawned(SpawnCard.SpawnResult result)
        {
            var master = result.spawnedInstance.GetComponent<CharacterMaster>();
            if (Enum.TryParse<EliteIndex>(_elite, out var eliteIndex) && eliteIndex != EliteIndex.None && eliteIndex != EliteIndex.Count)
            {
                master.inventory.SetEquipmentIndex(EliteCatalog.GetEliteDef(eliteIndex).eliteEquipmentIndex);
                master.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((GetTierDef(eliteIndex).healthBoostCoefficient -1) * 10));
                master.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((GetTierDef(eliteIndex).damageBoostCoefficient -1) * 10));
            }
        }

        private static CombatDirector.EliteTierDef GetTierDef(EliteIndex index)
        {
            var tier = 0;
            var tierdefs = typeof(CombatDirector).GetFieldValue<CombatDirector.EliteTierDef[]>("eliteTiers");
            if ((int)index > (int)EliteIndex.None && (int)index < (int)EliteIndex.Count)
            {
                for (var i = 0; i < tierdefs.Length; i++)
                {
                    foreach (var t in tierdefs[i].eliteTypes)
                    {
                        if (t == index)
                        {
                            tier = i;
                        }
                    }
                }
            }
            return tierdefs[tier];
        }
    }
}