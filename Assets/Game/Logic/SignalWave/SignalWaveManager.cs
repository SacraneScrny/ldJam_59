using System;
using System.Collections.Generic;

using Game.Logic.Level.Components;

using Sackrany.Extensions;
using Sackrany.GameInput;
using Sackrany.Utils;

using UnityEngine;

namespace Game.Logic.SignalWave
{
    public class SignalWaveManager : AManager<SignalWaveManager>
    {
        public float Dumping = 1;
        public int MaxBounces = 25;
        public float StartSpeed = 5;
        public float ParticleSize = 0.04f;
        public float ParticleScaleToVel = 0.5f;
        public Color DangerColor = Color.red;
        public Color GoodColor = Color.green;

        int _maxElements = 2048;
        readonly List<SignalWaveElement> elements = new List<SignalWaveElement>(2048);
        public IReadOnlyList<SignalWaveElement> Elements { get { return elements; } }
        
        Mesh _mesh;
        Material _material;

        Vector3[] _verts;
        Color[] _colors;
        int[] _indices;
        
        void Start()
        {
            _mesh = new Mesh();
            _mesh.MarkDynamic();
            _material = new Material(Shader.Find("Sprites/Default"));

            int max = _maxElements;
            _verts = new Vector3[max * 4];
            _colors = new Color[max * 4];
            _indices = new int[max * 6];

            for (int i = 0; i < max; i++)
            {
                int vi = i * 4;
                int ii = i * 6;
                _indices[ii + 0] = vi + 0;
                _indices[ii + 1] = vi + 1;
                _indices[ii + 2] = vi + 2;
                _indices[ii + 3] = vi + 0;
                _indices[ii + 4] = vi + 2;
                _indices[ii + 5] = vi + 3;
            }
            _mesh.vertices = _verts;
            _mesh.SetIndices(_indices, MeshTopology.Triangles, 0);
        }
        
        float _angle;
        void Update()
        {
            if (InputManager.PlayerCache.Attack)
            {
                _angle += Time.deltaTime * 180;
                Vector2 dir = new Vector2(Mathf.Cos(_angle * Mathf.Deg2Rad), Mathf.Sin(_angle * Mathf.Deg2Rad));
                
                var point = UnityEngine.Camera.main.ScreenToWorldPoint(InputManager.CurrentPointer.ScreenPosition);
                Spawn(point.With(z: 0), dir, StartSpeed);
            }    
        }
        void FixedUpdate()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                var el = elements[i];
                
                el.Collision();
                el.Move(Time.deltaTime, Dumping, LevelGenerator.Instance.LevelRect);

                if (el.IsDead(Time.deltaTime))
                {
                    RemoveFast(elements, i);
                    i--;
                }
                else
                    elements[i] = el;
            }
        }
        void LateUpdate()
        {
            int count = elements.Count;

            for (int i = 0; i < count; i++)
            {
                var el = elements[i];
                float scale = el.Scale;
                
                var p = el.Position;
                var c = el.IsDangerous ? DangerColor : GoodColor;
                int vi = i * 4;

                _verts[vi + 0] = new Vector3(p.x - scale, p.y - scale, p.z);
                _verts[vi + 1] = new Vector3(p.x + scale, p.y - scale, p.z);
                _verts[vi + 2] = new Vector3(p.x + scale, p.y + scale, p.z);
                _verts[vi + 3] = new Vector3(p.x - scale, p.y + scale, p.z);

                _colors[vi + 0] = _colors[vi + 1] = _colors[vi + 2] = _colors[vi + 3] = c;
            }

            for (int i = count * 4; i < _verts.Length; i++)
                _verts[i] = Vector3.zero;

            _mesh.vertices = _verts;
            _mesh.colors = _colors;

            Graphics.DrawMesh(_mesh, Matrix4x4.identity, _material, 0);
        }
        
        public static void RemoveFast<T>(List<T> list, int index)
        {
            int lastIndex = list.Count - 1;
            list[index] = list[lastIndex];
            list.RemoveAt(lastIndex);
        }
        public static void Spawn(Vector3 position, Vector3 direction, float speed)
        {
            if (Instance.elements.Count >= Instance._maxElements) return;
            Instance.elements.Add(new SignalWaveElement(position, direction.normalized * speed, Instance.MaxBounces, Instance.ParticleSize, Instance.ParticleScaleToVel));
        }
        public static void Kill(int num)
        {
            var el = Instance.elements[num];
            el.MarkDead();
            Instance.elements[num] = el;
        }
        void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            foreach (var element in elements)
            {
                Gizmos.color = element.IsDangerous ? Color.red : Color.white;
                Gizmos.DrawWireSphere(element.Position, 0.1f);
            }
            #endif
        }
    }

    public struct SignalWaveElement
    {
        public float Scale;
        public Vector3 Position;
        public Vector3 Velocity;
        public bool IsDangerous;
        public int BounceCount;
        int _collisionSave;
        float _scaleToVel;
        bool _markDead;

        public SignalWaveElement(Vector3 position, Vector3 velocity, int maxBounces, float scaleFactor, float scaleToVelFactor)
        {
            Position = position;
            Velocity = velocity;
            IsDangerous = true;
            BounceCount = maxBounces;
            _collisionSave = 0;

            Scale = scaleFactor;
            _scaleToVel = scaleToVelFactor;
            _markDead = false;
        }

        public void Move(float deltaTime, float dumping, Rect safeZone)
        {
            float mgn = Velocity.magnitude;
            if (!safeZone.Contains(Position)) dumping += (1f / mgn) * 10f;
            
            Position += Velocity * deltaTime;
            Velocity *= Mathf.Exp(-dumping * deltaTime);
            Scale += mgn * _scaleToVel * deltaTime;
        }
        public void Collision()
        {
            if (_collisionSave != 0)
            {
                _collisionSave--;
                return;
            }
            
            if (!LevelGenerator.IsInWall(Position)) return;
            var normal = LevelGenerator.GetNormal(Position);
            Velocity = normal * Velocity.magnitude;
            IsDangerous = !IsDangerous;

            BounceCount--;
            _collisionSave = 1;
        }

        public bool IsDead(float deltaTime)
        {
            if (_markDead) return true;
            if (BounceCount <= 0) return true;
            return Velocity.sqrMagnitude <= deltaTime;
        }
        public void MarkDead() => _markDead = true;
    }
}