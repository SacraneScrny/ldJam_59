using System;
using System.Collections;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Sackrany.SerializableData.Components
{
    public class AutoSave : MonoBehaviour
    {
        public float IntervalMinutes = 2;
        IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(IntervalMinutes * 60);
                DataManager.SaveAllData(true).Forget();
            }
        }
    }
}