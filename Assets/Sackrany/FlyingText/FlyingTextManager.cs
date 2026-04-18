using System.Collections.Generic;

using Sackrany.Actor.Traits.Damage;
using Sackrany.Utils;
using Sackrany.Utils.Pool.Extensions;
using Sackrany.Variables.Numerics;

using UnityEngine;

namespace Sackrany.FlyingText
{
    public class FlyingTextManager : AManager<FlyingTextManager>
    {
        public float TextRadius = 0.04f;
        public Vector2Int ExponentToColorGradient;
        public Gradient BigNumberToColorGradient;
        public GameObject TextPrefab;
        
        protected override void OnManagerAwake()
        {
            
        }
        public static Color GetBigNumberColor(BigNumber number)
        {
            if (number.Exponent < Instance.ExponentToColorGradient.x) return Instance.BigNumberToColorGradient.Evaluate(0);
            if (number.Exponent >= Instance.ExponentToColorGradient.y) return Instance.BigNumberToColorGradient.Evaluate(1f);
            return Instance.BigNumberToColorGradient.Evaluate((number.Exponent + Instance.ExponentToColorGradient.x + 1) / (float)Instance.ExponentToColorGradient.y);
        }

        public static void SpawnDamage(BigDamageInfo info, Vector3? positionOffset = null, Vector3? forceOffset = null)
        {
            Instance.TextPrefab.POOL().GetComponent<FlyingText>().Initialize(
                info.Damage,
                info.HitPosition + positionOffset ?? Vector3.zero,
                -info.Direction + forceOffset ?? Vector3.zero
                //type: info.damageType
                );
        }
    }
}