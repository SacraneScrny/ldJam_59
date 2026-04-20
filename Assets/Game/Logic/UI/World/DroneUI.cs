using System;

using Game.Logic.Drone.Modules;
using Sackrany.Actor.UnitMono;

using UnityEngine;
using R3;

using Sackrany.Actor.Static;

using TMPro;

using Unit = Sackrany.Actor.UnitMono.Unit;

namespace Game.Logic.UI.World
{
    public class DroneUI : MonoBehaviour
    {
        public TMP_Text BatteryText;
        
        bool _isInited;
        Unit _drone;
        DroneSignalAffectModule _signal;
        
        void Awake()
        {
            _drone = GetComponentInParent<Unit>();
        }
        void Start()
        {
            _drone.MaybeAsync<DroneBatteryModule>((b) =>
            {
                _isInited = true;
                b.Battery.Subscribe(OnBatteryChanged);
            });
            _drone.MaybeAsync<DroneSignalAffectModule>((s) =>
            {
                _signal = s;
            });
        }
        void LateUpdate()
        {
            if (!_isInited) return;
            transform.rotation = Quaternion.identity;
            
            SignalAffect();
        }
        float _glitchTimer;
        float _restoreTimer;
        bool _isGlitching;
        int _lastBatteryValue;

        static readonly char[] GlitchChars = "!@#$%^&*?~<>[]{}|\\/-_=+".ToCharArray();

        void SignalAffect()
        {
            if (_signal == null) return;

            float penalty = _signal.CurrentPenalty;
            if (penalty <= 0f)
            {
                _isGlitching = false;
                return;
            }

            if (_isGlitching)
            {
                _restoreTimer -= Time.deltaTime;
                if (_restoreTimer <= 0f)
                {
                    _isGlitching = false;
                    BatteryText.text = $"{_lastBatteryValue}%";
                }
                else
                {
                    BatteryText.text = BuildGlitchText(penalty);
                }
                return;
            }

            _glitchTimer -= Time.deltaTime;
            float interval = Mathf.Lerp(2.0f, 0.1f, penalty);
            if (_glitchTimer <= 0f)
            {
                _glitchTimer = interval;
                _isGlitching = true;
                _restoreTimer = Mathf.Lerp(0.05f, 0.3f, penalty);
            }
        }

        string BuildGlitchText(float penalty)
        {
            int prefixLen = UnityEngine.Random.Range(1, 4);
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < prefixLen; i++)
                sb.Append(GlitchChars[UnityEngine.Random.Range(0, GlitchChars.Length)]);

            if (UnityEngine.Random.value < penalty)
                sb.Append(UnityEngine.Random.Range(0, 999)).Append('%');

            for (int i = 0; i < UnityEngine.Random.Range(0, 3); i++)
                sb.Append(GlitchChars[UnityEngine.Random.Range(0, GlitchChars.Length)]);

            return sb.ToString();
        }

        void OnBatteryChanged(int value)
        {
            _lastBatteryValue = value;
            if (!_isGlitching)
                BatteryText.text = $"{value}%";
        }
    }
}