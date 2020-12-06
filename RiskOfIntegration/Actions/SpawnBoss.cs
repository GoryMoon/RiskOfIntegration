using RoR2;
using UnityEngine;
using UnityEngine.Events;

namespace RiskOfIntegration.Actions
{
    public class SpawnBoss: BaseAction
    {
        public override ActionResponse Handle()
        {
            if (Utils.GetPlayer(out _, out var player))
            {
                var bossDirector = TeleporterInteraction.instance.bossDirector;
                bossDirector.enabled = true;
                bossDirector.monsterCredit += (int) (600.0 * Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 0.5f) * (1 + TeleporterInteraction.instance.shrineBonusStacks));
                bossDirector.currentSpawnTarget = player.gameObject;
                bossDirector.SetNextSpawnAsBoss();
                bossDirector.onSpawnedServer.AddListener(OnSpawned);

                var formatChatMessage = new Chat.SubjectFormatChatMessage
                {
                    subjectAsCharacterBody = player,
                    baseToken = "SHRINE_BOSS_USE_MESSAGE"
                };
                Chat.SendBroadcastChat(formatChatMessage);
                EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
                {
                    origin = player.transform.position,
                    rotation = Quaternion.identity,
                    scale = 1f,
                    color = new Color(0.7372549f, 0.9058824f, 0.945098f)
                }, true);

                void OnSpawned(GameObject masterObject)
                {
                    var body = masterObject.GetComponent<CharacterBody>();
                    if (body)
                    {
                        body.baseNameToken = From;
                    }
                    bossDirector.onSpawnedServer.RemoveListener(OnSpawned);
                }
                
                return ActionResponse.Done;
            }

            return ActionResponse.Retry;
        }
    }
}