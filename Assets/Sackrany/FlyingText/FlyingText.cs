using System;

using Sackrany.CustomRandom.Global;
using Sackrany.Extensions;
using Sackrany.Utils.Pool.Abstracts;
using Sackrany.Utils.TextColorSystem.Extensions;
using Sackrany.Variables.Numerics;

using TMPro;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Sackrany.FlyingText
{
    public class FlyingText : MonoBehaviour, IPoolable
    {
        [Header("Refs")]
        public TMP_Text Text;

        [Header("Timings")]
        public Vector2 LifeTime = new(0.8f, 1.2f);
        public float ImpactDuration = 0.12f;
        public float FadeOutDuration = 0.3f;

        [Header("Motion")]
        public Vector2 PushDistance = new(0.6f, 1.2f);
        public float ImpactSpeed = 18f;

        [Header("Impact FX")]
        public float ImpactScale = 1.6f;
        public float ImpactRotation = 20f;

        [Header("Noise (spawn only)")]
        public float NoiseStrength = 0.15f;
        public float NoiseFrequency = 14f;


        public Vector3 Pos;
        public Vector3 Dir;
        public bool IsRecreated;
        public Effect CurrentEffect;
        
        private void Awake()
        {

        }

        public FlyingText Initialize(string text, Vector3 position, Vector3 direction, Effect effect = Effect.Default)
        {
            transform.position = position;
            Text.text = text;
            Text.color = Text.color.SetAlpha(0f);
            
            Pos = position;
            Dir = direction;
            IsRecreated = true;
            CurrentEffect = effect;
            
            return this;
        }
        public FlyingText Initialize(float num, Vector3 position, Vector3 direction, Effect effect = Effect.Default) =>
            Initialize(num.ToString("F1"), position, direction, effect);
        public FlyingText Initialize(BigNumber num, Vector3 position, Vector3 direction, Effect effect = Effect.Default, TextType type = TextType.Default)
        {
            Text.color = FlyingTextManager.GetBigNumberColor(num);
            return Initialize(num.ToString(1).Sprite(type).Colored(type), position, direction, effect);
        }

        public void OnPooled()
        {
            gameObject.SetActive(true);
        }

        public void OnReleased()
        {
            CurrentEffect = Effect.Default;
            IsRecreated = false;
            gameObject.SetActive(false);
        }
    }

    public enum Effect
    {
        Default,
        Crit,
    }
}
