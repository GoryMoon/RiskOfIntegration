using System;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace RiskOfIntegration.Misc
{
    public class NetworkManager
    {
        internal static GameObject RoIComponents;
        private static GameObject _roIComponentsSpawned;

        internal static void Init()
        {
            var roi = new GameObject();
            roi.AddComponent<NetworkIdentity>();
            RoIComponents = roi.InstantiateClone("RiskOfIntegrationComponentsNetworked");
            Object.Destroy(roi);

            TimeScaleManager.InitRPC();

            ApplyHook();
        }

        private static void ApplyHook()
        {
            On.RoR2.SceneDirector.Start += EnsureRoINetwork;
        }

        internal static void UndoHook()
        {
            On.RoR2.SceneDirector.Start -= EnsureRoINetwork;
        }

        private static void EnsureRoINetwork(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            try
            {
                orig(self);
            }
            catch (Exception e)
            {
                RiskOfIntegration.Instance.Log.LogWarning($"Vanilla Exception: {e}");
            }

            if (!_roIComponentsSpawned)
            {
                _roIComponentsSpawned = Object.Instantiate(RoIComponents);
                NetworkServer.Spawn(_roIComponentsSpawned);
            }
        }
    }
}