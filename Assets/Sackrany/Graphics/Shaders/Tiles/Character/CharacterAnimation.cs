using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Sackrany.Graphics.Shaders.Tiles.Character {
    [CreateAssetMenu(menuName = "Character/Create Character Animation", fileName = "New Character Animation")]
    public class CharacterAnimation : ScriptableObject
    {
        public Texture2D[] AlbedoTextures;
        public Texture2D[] NormalTextures;

        #if UNITY_EDITOR
        // Вызов из редактора: вручную заставить ассет просканировать папку
        public void PopulateFromContainingFolder()
        {
            string path = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(path))
                return;

            string folder = Path.GetDirectoryName(path).Replace("\\", "/");
            CharacterAnimationEditorHelpers.PopulateAssetFromFolder(folder, this);
        }
        #endif
    }

    #if UNITY_EDITOR
    static class CharacterAnimationEditorHelpers
    {
        public static void PopulateAssetFromFolder(string folderPath, CharacterAnimation asset)
        {
            if (string.IsNullOrEmpty(folderPath) || asset == null)
                return;

            // Убедимся, что путь начинается с Assets
            if (!folderPath.StartsWith("Assets"))
                return;

            // Albedo
            string albedoFolder = folderPath + "/Albedo";
            if (AssetDatabase.IsValidFolder(albedoFolder))
            {
                var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { albedoFolder });
                var list = new List<Texture2D>();
                foreach (var g in guids)
                {
                    var p = AssetDatabase.GUIDToAssetPath(g);
                    var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(p);
                    if (tex != null) list.Add(tex);
                }
                asset.AlbedoTextures = list.OrderBy(t => t.name).ToArray();
            }

            // Normal
            string normalFolder = folderPath + "/Normal";
            if (AssetDatabase.IsValidFolder(normalFolder))
            {
                var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { normalFolder });
                var list = new List<Texture2D>();
                foreach (var g in guids)
                {
                    var p = AssetDatabase.GUIDToAssetPath(g);
                    var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(p);
                    if (tex != null) list.Add(tex);
                }
                asset.NormalTextures = list.OrderBy(t => t.name).ToArray();
            }

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        public static void ConfigureTextureImportSettings(Texture2D tex, bool isNormal)
        {
            if (tex == null)
                return;

            string path = AssetDatabase.GetAssetPath(tex);
            if (string.IsNullOrEmpty(path))
                return;

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                return;

            if (isNormal)
            {
                importer.textureType = TextureImporterType.NormalMap; // Normal map
                importer.alphaIsTransparency = false;
            }
            else
            {
                importer.textureType = TextureImporterType.Default; // Albedo as default
                importer.alphaIsTransparency = true; // AlphaIsTransparency true
            }
        
            // Общие настройки
            importer.npotScale = TextureImporterNPOTScale.None; // Non-Power of 2 None
            importer.mipmapEnabled = false; // Generate Mip maps false
            importer.filterMode = FilterMode.Point; // Filter mode point
            importer.textureCompression = TextureImporterCompression.Uncompressed; // compression none

            // max size -> выставляем по реальному размеру текстуры
            int maxSize = Mathf.Max(tex.width, tex.height);
            // Unity позволяет только заранее выбранные размеры, но указываем ближайший доступный
            importer.maxTextureSize = GetClosestMaxTextureSize(maxSize);


            // Сохраняем и перезагружаем
            importer.SaveAndReimport();
        }

        static int GetClosestMaxTextureSize(int size)
        {
            // Unity поддерживает фиксированные опции: 32,64,128,256,512,1024,2048,4096,8192
            int[] options = new int[] { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
            int best = options[0];
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] >= size)
                {
                    best = options[i];
                    break;
                }
            }
            return best;
        }
    }

// Автоматически заполняем только что созданный ассет того типа
    class CharacterAnimationAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets)
            {
                if (!path.EndsWith(".asset"))
                    continue;

                var asset = AssetDatabase.LoadAssetAtPath<CharacterAnimation>(path);
                if (asset == null)
                    continue;

                // Если массивы пустые — заполняем из папки
                string folder = Path.GetDirectoryName(path).Replace("\\", "/");
                bool needFill = (asset.AlbedoTextures == null || asset.AlbedoTextures.Length == 0) && (asset.NormalTextures == null || asset.NormalTextures.Length == 0);
                if (needFill)
                {
                    CharacterAnimationEditorHelpers.PopulateAssetFromFolder(folder, asset);
                }
            }
        }
    }

    [CustomEditor(typeof(CharacterAnimation))]
    class CharacterAnimationEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var targ = (CharacterAnimation)target;

            GUILayout.Space(6);

            if (GUILayout.Button("Rescan folder and fill textures"))
            {
                string path = AssetDatabase.GetAssetPath(targ);
                if (!string.IsNullOrEmpty(path))
                {
                    string folder = Path.GetDirectoryName(path).Replace("\\", "/");
                    CharacterAnimationEditorHelpers.PopulateAssetFromFolder(folder, targ);
                }
            }

            if (GUILayout.Button("Configure textures (Albedo / Normal)"))
            {
                // Конфигурируем все текстуры из массивов
                if (targ.AlbedoTextures != null)
                {
                    foreach (var t in targ.AlbedoTextures)
                    {
                        CharacterAnimationEditorHelpers.ConfigureTextureImportSettings(t, false);
                    }
                }
                if (targ.NormalTextures != null)
                {
                    foreach (var t in targ.NormalTextures)
                    {
                        CharacterAnimationEditorHelpers.ConfigureTextureImportSettings(t, true);
                    }
                }

                // Обновим базу
                AssetDatabase.Refresh();
            }
        }
    }
    #endif
}