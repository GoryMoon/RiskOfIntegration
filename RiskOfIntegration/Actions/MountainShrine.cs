using System;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfIntegration.Actions
{
    public class MountainShrine: BaseAction
    {
        public override ActionResponse Handle()
        {
            var instance = TeleporterInteraction.instance;
            if (instance.isCharging || instance.isCharged)
            {
                TryLater(TimeSpan.FromSeconds(10));
                return ActionResponse.Retry;
            }
            
            instance.AddShrineStack();
            if (Utils.GetPlayer(out _, out var player))
            {
                var component = player.GetComponent<CharacterBody>();
                var formatChatMessage = new Chat.SubjectFormatChatMessage
                {
                    subjectAsCharacterBody = component,
                    baseToken = "SHRINE_BOSS_USE_MESSAGE"
                };
                Chat.SendBroadcastChat(formatChatMessage);
                ChatMessage.SendColored($"{From} increased the difficulty of the next boss!", "#d63031");
                EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
                {
                    origin = player.transform.position,
                    rotation = Quaternion.identity,
                    scale = 1f,
                    color = new Color(0.7372549f, 0.9058824f, 0.945098f)
                }, true);
                return ActionResponse.Done;
            }
            return ActionResponse.Retry;
        }
    }
}