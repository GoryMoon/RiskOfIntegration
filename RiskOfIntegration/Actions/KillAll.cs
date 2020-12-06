using R2API.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfIntegration.Actions
{
    public class KillAll: BaseAction
    {
        public override ActionResponse Handle()
        {
            foreach (var cm in Object.FindObjectsOfType<CharacterMaster>())
            {
                if (cm.teamIndex == TeamIndex.Monster)
                {
                    var cb = cm.GetBody();
                    if (cb)
                    {
                        if (cb.healthComponent)
                        {
                            cb.healthComponent.Suicide();
                        }
                    }
                }
            }
            ChatMessage.SendColored($"{From} killed all monsters for you!", "#00b894");

            return ActionResponse.Done;
        }
    }
}