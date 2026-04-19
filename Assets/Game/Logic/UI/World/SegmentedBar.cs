using System;
using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sackrany.Extensions;

using UnityEngine;
using UnityEngine.UI;

using Object = UnityEngine.Object;

namespace Game.Logic.UI.World
{
    public class SegmentedBar : MonoBehaviour
    {
        public GameObject SegmentPrefab;
        public RectTransform Content;
        public RectTransform Line;
        public RectTransform LineTo;
        
        public Color ActiveColor = Color.red;
        public Color InactiveColor = Color.black.SetAlpha(0.5f);
        
        List<Segment> _segments = new List<Segment>();
        Transform _lineConnection;
        int _lastSegment;
        bool _inverse;
        CancellationToken _ct;
        
        void Awake()
        {
            _ct = gameObject.GetCancellationTokenOnDestroy();
            SegmentPrefab.SetActive(false);
            Line.gameObject.SetActive(false);
        }

        public void Clear()
        {
            _inverse = false;
            foreach (var segment in _segments)
                Object.Destroy(segment.GO);
            _segments.Clear();
        }
        public void ConnectLine(Transform t)
        {
            Line.gameObject.SetActive(true);
            _lineConnection = t;
        }
        public void Inverse() => _inverse = true;
        public void Spawn(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var go = Instantiate(SegmentPrefab, Content);
                go.SetActive(true);
                var s = new Segment();
                var children = go.GetComponentsInChildren<Image>();
                s.GO = go;
                s.Main = children[0];
                s.Splash = children[1];
                s.Splash.color = s.Splash.color.SetAlpha(0);
                s.Main.color = !_inverse ? ActiveColor : InactiveColor;
                _segments.Add(s);
            }

            _lastSegment = _segments.Count - 1;
            if (_inverse) _lastSegment = 0;
        }
        void LateUpdate()
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);

            if (_lineConnection != null)
            {
                Line.localPosition = LineTo.localPosition;
                var relPos = Line.parent.InverseTransformPoint(_lineConnection.position);
                Vector2 dir = (relPos - Line.position);

                float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Line.transform.rotation = Quaternion.Euler(0, 0, targetAngle);
                Line.sizeDelta = new Vector2(dir.magnitude, 1f);
            }
        }
        public void Hit()
        {
            if (_lastSegment < 0 && !_inverse) return;
            if (_lastSegment >= _segments.Count && _inverse) return;
            
            var segment = _segments[_lastSegment];
            segment.Main.color = _inverse ? ActiveColor : InactiveColor;
            Animation(segment.Splash, _ct).Forget();
            
            _lastSegment += _inverse ? 1 : -1;
        }
        async UniTask Animation(Image splash, CancellationToken ct)
        {
            var initialScale = splash.transform.localScale;
            float t = 0f;
            try
            {
                while (t < 1f)
                {
                    if (splash == null || splash.gameObject == null) return;
                    t = Mathf.Min(t + Time.deltaTime * 3f, 1f);
                    splash.transform.localScale = Vector3.Lerp(initialScale, Vector3.one * 3f, t);
                    splash.color = splash.color.SetAlpha(Mathf.Sin(Mathf.Pow(t, 0.3f) * Mathf.PI) * 0.6f);
                    await UniTask.Yield(PlayerLoopTiming.Update, ct);
                }
            }
            finally
            {
                if (splash != null && splash.gameObject != null)
                    splash.color = splash.color.SetAlpha(0);
            }
        }
    }

    [Serializable]
    public class Segment
    {
        public GameObject GO;
        public Image Main;
        public Image Splash;
    }
}