using System;
using Humanizer;
using Newtonsoft.Json;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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
                if (!Enum.TryParse(_team, out TeamIndex teamIndex) || teamIndex == TeamIndex.None || teamIndex == TeamIndex.Count)
                {
                    teamIndex = TeamIndex.Monster;
                }
                
                var spawnCard = Utils.GetSpawnCard(_name);
                if (spawnCard == null)
                {
                    var masterName= Utils.GetMasterName(_name);
                    if (masterName == null)
                    {
                        ChatMessage.SendColored($"ERROR: Can't find monster with name \"{_name}\"", "#d63031");
                        return ActionResponse.Done;
                    }
                    SpawnMaster(player, masterName, teamIndex);
                }
                else
                {
                    SpawnCard(player, spawnCard, teamIndex);
                }

                ChatMessage.SendColored($"{From} spawned {_amount}x {_name.Replace("Master", string.Empty).Humanize()} on you!", "#d63031");

                return ActionResponse.Done;
            }

            return ActionResponse.Retry;
        }

        private void SpawnMaster(Component player, string masterName, TeamIndex teamIndex)
        {
            var masterPrefab = MasterCatalog.FindMasterPrefab(masterName);
            var body = masterPrefab.GetComponent<CharacterMaster>().bodyPrefab;
            var location = player.transform.position;
            
            for (var i = 0; i < _amount; i++)
            {
                var pos = Vector3.forward * Random.Range(-_minDistance, _maxDistance) + Vector3.right * Random.Range(-_minDistance, _maxDistance);
                var ray = new Ray(location + pos + Vector3.up * 200, Vector3.down);
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerIndex.defaultLayer.intVal))
                {
                    if (hit.collider != null)
                    {
                        pos.y = hit.point.y + 10;
                    }
                }
                var bodyGameObject = Object.Instantiate(masterPrefab, location + pos, Quaternion.identity);
                var master = bodyGameObject.GetComponent<CharacterMaster>();
                NetworkServer.Spawn(bodyGameObject);
                master.SpawnBody(body, bodyGameObject.transform.position, Quaternion.identity);
                HandleSpawned(master, 10);
                
                master.teamIndex = teamIndex;
                master.GetBody().teamComponent.teamIndex = teamIndex;
            }
        }
        
        private void SpawnCard(Component player, SpawnCard spawnCard, TeamIndex teamIndex)
        {
            var director = Object.FindObjectOfType<CombatDirector>();
            var rng = director.GetFieldValue<Xoroshiro128Plus>("rng");

            for (var i = 0; i < _amount; i++)
            {
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
        }

        private void OnCardSpawned(SpawnCard.SpawnResult result)
        {
            HandleSpawned(result.spawnedInstance.GetComponent<CharacterMaster>(), result.spawnRequest.spawnCard.directorCreditCost);
        }

        private void HandleSpawned(CharacterMaster master, int cost)
        {
            if (Enum.TryParse<EliteIndex>(_elite, out var eliteIndex) && eliteIndex != EliteIndex.None && eliteIndex != EliteIndex.Count)
            {
                master.inventory.SetEquipmentIndex(EliteCatalog.GetEliteDef(eliteIndex).eliteEquipmentIndex);
                master.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((GetTierDef(eliteIndex).healthBoostCoefficient -1) * 10));
                master.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((GetTierDef(eliteIndex).damageBoostCoefficient -1) * 10));
            }

            var deathRewards = master.GetComponent<DeathRewards>();
            if (deathRewards)
            {
                deathRewards.spawnValue = (int) Mathf.Max(1f, cost * 0.2f);
                deathRewards.expReward = (uint) ((double) cost * 0.2f * Run.instance.compensatedDifficultyCoefficient);
                deathRewards.goldReward = (uint) ((double) cost * 0.2f * 2.0 * Run.instance.compensatedDifficultyCoefficient);
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