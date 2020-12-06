using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfIntegration.Misc
{
    [RequireComponent(typeof(CombatDirector))]
    public class CombatShrineManager: MonoBehaviour
    {
        private DirectorCard _chosenDirectorCard;
        private CombatDirector _combatDirector;
        private int _actionvationCount;

        private void Awake()
        {
            if (!NetworkServer.active)
                return;
            _combatDirector = GetComponent<CombatDirector>();
        }

        public bool RunEffect(CharacterBody player, float credits)
        {
            if (!NetworkServer.active) return true;

            var monsterCredit = credits * Stage.instance.entryDifficultyCoefficient * (1.0f + _actionvationCount * (2 - 1.0f));
            _chosenDirectorCard = _combatDirector.SelectMonsterCardForCombatShrine(monsterCredit);
            
            _combatDirector.CombatShrineActivation(player.GetComponent<Interactor>(), monsterCredit, _chosenDirectorCard);
            EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ShrineUseEffect"), new EffectData
            {
                origin = transform.position,
                rotation = Quaternion.identity,
                scale = 1f,
                color = new Color(0.6661001f, 0.5333304f, 0.8018868f)
            }, true);
            
            ++_actionvationCount;
            return true;
        }
    }
}