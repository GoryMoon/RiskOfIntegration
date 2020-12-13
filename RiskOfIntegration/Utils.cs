using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoR2;
using UnityEngine;

namespace RiskOfIntegration
{
    public static class Utils
    {
        private static readonly Dictionary<string, string> NameCache = new Dictionary<string, string>();
        private static readonly List<SpawnCard> SpawnCard = new List<SpawnCard>();

        public static bool AnySpawned { get; private set; }

        public static void Init()
        {
            RiskOfIntegration.Instance.Log.LogMessage("Loading all CSC's");
            SpawnCard.AddRange(Resources.LoadAll<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards"));
            
            On.RoR2.Stage.RespawnCharacter += (orig, self, master) =>
            {
                orig(self, master);
                if (!self.usePod)
                {
                    AnySpawned = true;
                }
            };
            On.RoR2.Stage.Start += (orig, self) =>
            {
                AnySpawned = false;
                orig(self);
            };
            On.RoR2.SurvivorPodController.OnPassengerExit += (orig, self, passenger) =>
            {
                orig(self, passenger);
                if (passenger.GetComponent<CharacterBody>())
                {
                    AnySpawned = true;
                }
            };
            On.RoR2.Chat.AddMessage_string += (orig, message) =>
            {
                if (message.Contains("wants to attack:"))
                {
                    var msg = message.Replace(".</style>", "").Split(' ');
                    var name = msg[msg.Length - 1];
                    if (message.Contains($"{name} wants to attack: {name}"))
                    {
                        return;
                    }
                }
                orig(message);
            };
        }

        public static bool GetPlayer(out NetworkUser networkUser, out CharacterBody characterBody)
        {
            networkUser = LocalUserManager.GetFirstLocalUser()?.currentNetworkUser;
            characterBody = null;
            
            if (networkUser != null)
            {
                var master = networkUser.master;
                if (master && master.GetBody())
                {
                    characterBody = master.GetBody();
                    return true;
                }
            }
            return false;
        }

        public static string GetMasterName(string name)
        {
            if (NameCache.ContainsKey(name))
            {
                return NameCache[name];
            }

            foreach (var master in MasterCatalog.allAiMasters)
            {
                if (master.name.ToUpper().Equals(name.ToUpper()) || master.name.ToUpper().Replace("MASTER", string.Empty).Equals(name.ToUpper()))
                {
                    NameCache.Add(name, master.name);
                    return master.name;
                }
            }
            
            foreach (var master in MasterCatalog.allAiMasters)
            {
                var langName = Language.GetString(master.bodyPrefab.GetComponent<CharacterBody>().baseNameToken);
                if (langName.ToUpper().Equals(name.ToUpper()))
                {
                    NameCache.Add(name, master.name);
                    return master.name;
                }
            }

            return null;
        }

        public static SpawnCard GetSpawnCard(string name)
        {
            foreach (var card in SpawnCard.Where(card => card.name.ToUpper().Replace("CSC", string.Empty).Equals(name.ToUpper())))
            {
                return card;
            }

            name = GetMasterName(name).ToUpper();
            return SpawnCard.FirstOrDefault(card => card.prefab.name.ToUpper().Equals(name));
        }
        
        [ConCommand(commandName = "roi_log_masters", flags = ConVarFlags.None, helpText = "Logs all masters names")]
        public static void LogMasterNames(ConCommandArgs args)
        {
            var s = new StringBuilder();
            foreach (var master in MasterCatalog.allAiMasters)
            {
                var langName = Language.GetString(master.bodyPrefab.GetComponent<CharacterBody>().baseNameToken);
                s.AppendLine($"|{langName}|{master.name.Replace("Master", string.Empty)}|");
            }
            RiskOfIntegration.Instance.Log.LogDebug(s.ToString());
        }
        
        
        [ConCommand(commandName = "roi_log_csc", flags = ConVarFlags.None, helpText = "Logs all masters names")]
        public static void LogCscNames(ConCommandArgs args)
        {
            var s = new StringBuilder();
            foreach (var spawnCard in SpawnCard)
            {
                s.AppendLine($"|{spawnCard.name.Replace("CSC", string.Empty)}|{spawnCard.prefab.name}|");
            }
            RiskOfIntegration.Instance.Log.LogDebug(s.ToString());
        }
    }
}