using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using R2API.Utils;
using RiskOfIntegration.Misc;

namespace RiskOfIntegration
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin("se.gory_moon.risk_of_integration", "Stream Integration", "1.4.0")]
    public class RiskOfIntegration: BaseUnityPlugin
    {
        public ManualLogSource Log => Logger;
        
        public IntegrationManager IntegrationManager { get; private set; }
        public ActionManager ActionManager { get; private set; }
        public static RiskOfIntegration Instance { get; private set; }

        public static ConfigEntry<bool> TimeScaleQueue;
        public static ConfigEntry<float> PingTime;
        
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
            PingTime = Config.Bind("General","PingTime", 1000.0f, "The time to show the monster ping time for, value is in seconds");
            
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
            if (!Utils.AnySpawned) return;
            IntegrationManager.Update();
            ActionManager.Update();
        }

        private void OnDestroy() => IntegrationManager.Close();
        private void OnApplicationQuit() => OnDestroy();
    }
}