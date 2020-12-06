using System.Collections;
using System.Collections.Generic;
using R2API.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfIntegration.Misc
{
    public class TimeScaleManager: MonoBehaviour
    {
        private readonly Queue<TimeScale> _timeScales = new Queue<TimeScale>();
        private bool _running;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Queue(float scale, float duration, string from)
        {
            _timeScales.Enqueue(new TimeScale(scale, duration, from));
            if (RiskOfIntegration.TimeScaleQueue.Value)
            {
                if (!_running)
                {
                    StartCoroutine(UpdateTimeScale());
                }
            }
            else
            {
                _running = true;
                StopAllCoroutines();
                StartCoroutine(UpdateTimeScale());
            }
        }
        
        private IEnumerator UpdateTimeScale()
        {
            while (_timeScales.Count > 0)
            {
                var timeScale = _timeScales.Dequeue();
                _running = true;
                
                SetTimeScale(timeScale.Scale);
                ChatMessage.SendColored($"{timeScale.From} set timescale to {timeScale.Scale:N} for {timeScale.Duration} seconds!", "#d63031");
                yield return new WaitForSeconds(timeScale.Duration);
                if (_timeScales.Count <= 0)
                {
                    SetTimeScale(1);
                    ChatMessage.SendColored("Timescale reset to normal!" ,"#00b894");
                }
            }

            _running = false;
        }

        private static void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
            TimescaleNet.Invoke(scale);
        }
        
        private readonly struct TimeScale
        {
            public float Scale { get; }
            public float Duration { get; }
            public string From { get; }

            public TimeScale(float scale, float duration, string from)
            {
                Scale = scale;
                Duration = duration;
                From = from;
            }
        }
        
        public static void InitRPC()
        {
            NetworkManager.RoIComponents.AddComponent<TimescaleNet>();
        }
        
        
        // ReSharper disable once ClassNeverInstantiated.Global
        // ReSharper disable once MemberCanBeMadeStatic.Local
        // ReSharper disable once UnusedMember.Local
        public class TimescaleNet : NetworkBehaviour
        {
            private static TimescaleNet _instance;
        
            private void Awake()
            {
                _instance = this;
            }

            internal static void Invoke(float scale)
            {
                _instance.RpcApplyTimescale(scale);
            }
        
            [ClientRpc]
            private void RpcApplyTimescale(float scale)
            {
                Time.timeScale = scale;
            }
        }
    }
}