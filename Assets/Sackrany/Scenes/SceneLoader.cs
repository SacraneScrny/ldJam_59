using System.Collections.Generic;
using System.IO;
using System.Linq;

using Cysharp.Threading.Tasks;

using Sackrany.ConfigSystem;
using Sackrany.SerializableData;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sackrany.Scenes
{
    public static class SceneLoader
    {
        static readonly Dictionary<string, SceneData> _scenes = new ();
        static string _currentDynamicScene;
        static bool _isTransitioning;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void EarlyInit()
        {
            if (SceneManager.GetActiveScene().name == SceneNames.SYSTEM) return;
            var systemScene = SceneManager.GetSceneByName(SceneNames.SYSTEM);
            if (systemScene.isLoaded) return;

            SceneManager.LoadScene(SceneNames.SYSTEM, LoadSceneMode.Additive);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            if (SceneManager.sceneCountInBuildSettings == 0) return;
            
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                var path = SceneUtility.GetScenePathByBuildIndex(i);
                var name = Path.GetFileNameWithoutExtension(path);
                _scenes[name] = new SceneData(name);
            }

            if (_scenes.TryGetValue(SceneNames.UI, out var uiScene))
                uiScene.Load().Forget();

            var firstScene = _scenes.Values.FirstOrDefault(x => x.IsLoaded && !IsStaticScene(x.SceneName));
            _currentDynamicScene = firstScene?.SceneName;
            
            LoadDefaultScene().Forget();
        }

        static async UniTask LoadDefaultScene()
        {
            #if !UNITY_EDITOR
            var defaultScene = ConfigGet<SceneConfig>.Value.DefaultScene;
            if (!_scenes.TryGetValue(defaultScene, out var sceneData)) return;
            if (sceneData.IsLoaded) return;

            foreach (var scene in _scenes.Values.Where(x => x.IsLoaded && !IsStaticScene(x.SceneName)))
                await scene.Unload();
            await sceneData.Load();
            #endif
        }
        
        static bool IsStaticScene(string name) =>
            name == SceneNames.SYSTEM || name == SceneNames.UI;

        public static void Load(string sceneName)
        {
            InternalLoadScene(sceneName).Forget();
        }
        static async UniTask InternalLoadScene(string sceneName)
        {
            if (_isTransitioning) return;
            if (string.IsNullOrEmpty(sceneName)) return;
            if (!_scenes.TryGetValue(sceneName, out var next)) return;
            if (sceneName == _currentDynamicScene) return;
            
            _isTransitioning = true;
            
            if (_currentDynamicScene != null && _scenes.TryGetValue(_currentDynamicScene, out var current))
                await current.WaitForLoad();
            
            var currentScene = _scenes[_currentDynamicScene];
            DataManager.SerializeScene(currentScene.Scene);
            
            await currentScene.Unload();
            await next.Load();

            _currentDynamicScene = sceneName;
            _isTransitioning = false;
        }
    }
}