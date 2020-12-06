using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using R2API.Utils;
using RiskOfIntegration.Misc;
using RoR2;

namespace RiskOfIntegration
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin("se.gory_moon.risk_of_integration", "Stream Integration", "1.0.0")]
    public class RiskOfIntegration: BaseUnityPlugin
    {
        public ManualLogSource Log => Logger;
        
        public IntegrationManager IntegrationManager { get; private set; }
        public ActionManager ActionManager { get; private set; }
        public static RiskOfIntegration Instance { get; private set; }

        public static ConfigEntry<bool> TimeScaleQueue;
        
        public RiskOfIntegration()
        {
            Instance = this;

            var harmony = new Harmony(Info.Metadata.GUID);
            harmony.PatchAll();
        }

        private void Awake()
        {
            Logger.LogDebug("Starting plugin");
            TimeScaleQueue = Config.Bind("General","TimeScaleQueue", true, "If the timescale action should queue or override previous timescale action");
            
            IntegrationManager = new IntegrationManager(this, "RiskOfRain");
            ActionManager = new ActionManager(Logger);
            
            NetworkManager.Init();
            Utils.Init();
            
            CommandHelper.AddToConsoleWhenReady();
        }

        private void Start()
        {
            IntegrationManager.Start();
            InvokeRepeating(nameof(UpdateIntegration), 0, 0.1f);
        }

        private void UpdateIntegration()
        {
            if (!Run.instance) return;
            IntegrationManager.Update();
            ActionManager.Update();
        }

        private void OnDestroy() => IntegrationManager.Close();
        private void OnApplicationQuit() => OnDestroy();
    }
}