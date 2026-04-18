using System;

using ModifiableVariable;
using ModifiableVariable.Comparers;
using ModifiableVariable.Stages;
using ModifiableVariable.Stages.StageFactory;

using UnityEngine;

namespace Sackrany.Variables.Numerics
{
    public static class BigNumberStageArithmeticsUnityBoostrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            StageArithmetic<BigNumber>.Register(StageOpKind.Add, (a, b) => a + b);
            StageArithmetic<BigNumber>.Register(StageOpKind.Subtract, (a, b) => a - b);
            StageArithmetic<BigNumber>.Register(StageOpKind.Multiply, (a, b) => a * b);
            StageArithmetic<BigNumber>.Register(StageOpKind.Divide, (a, b) => a / b);
            StageArithmetic<BigNumber>.Register(StageOpKind.Min, (a, b) => BigNumber.Min(a, b));
            StageArithmetic<BigNumber>.Register(StageOpKind.Max, (a, b) => BigNumber.Max(a, b));
            
            ComparerFactory.Register(new BigNumberNumericComparer());
        }
    }
}