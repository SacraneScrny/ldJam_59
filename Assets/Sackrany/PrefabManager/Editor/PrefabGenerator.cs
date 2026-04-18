using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Sackrany.PrefabManager.Hash;

using UnityEditor;

using UnityEngine;

namespace Sackrany.PrefabManager.Editor
{
    public static class PrefabGenerator
    {
        const string OutputPath = "Assets/_Generated";

        static readonly string[] SearchRoots = { "Assets/Resources" };

        [MenuItem("Sackrany/Prefabs/Generate")]
        public static void Generate()
        {
            var entries = CollectEntries();
            Directory.CreateDirectory(OutputPath);

            WritePrefabsClass(entries);      // Prefabs.cs
            WritePrefabInterfaces(entries);  // PrefabInterfaces.cs
            WritePrefabWrappers(entries);    // PrefabWrappers.cs
            WriteManifestInstaller(entries); // PrefabManifestInstaller.cs

            AssetDatabase.Refresh();
            Debug.Log($"[Prefabs] Generated {entries.Count} entries");
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Collect — only .prefab
        // ─────────────────────────────────────────────────────────────────────

        static List<Entry> CollectEntries()
        {
            var entries = new List<Entry>();

            var resourceRoots = AssetDatabase
                .GetAllAssetPaths()
                .Where(p => SearchRoots.Any(p.StartsWith)
                            && (p.EndsWith("/Resources") || p == "Assets/Resources"))
                .ToList();

            foreach (var root in resourceRoots)
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { root }))
            {
                var fullPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(fullPath)) continue;

                var resourcePath = Path.ChangeExtension(fullPath[(root.Length + 1)..], null);
                var hash         = FNV1a.Compute(resourcePath);
                entries.Add(new Entry(resourcePath, hash));
            }

            return entries
                .GroupBy(e => e.Hash)
                .Select(g =>
                {
                    if (g.Count() > 1)
                        Debug.LogWarning($"[Prefabs] Hash collision: {string.Join(", ", g.Select(e => e.ResourcePath))}");
                    return g.First();
                })
                .OrderBy(e => e.ResourcePath)
                .ToList();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Prefabs.cs — dot-access tree
        //
        //  public static partial class Prefabs {
        //      public static partial class Enemies {
        //          public static partial class Goblins {
        //              public static readonly PrefabRef goblin01 = new(hash);
        //          }
        //      }
        //  }
        // ─────────────────────────────────────────────────────────────────────

        static void WritePrefabsClass(List<Entry> entries)
        {
            var root = BuildTree(entries);
            var sb   = new StringBuilder();
            sb.AppendLine("// AUTO-GENERATED — do not edit manually");
            sb.AppendLine("using Sackrany.PrefabManager;");
            sb.AppendLine("using Sackrany.PrefabManager.Entities;");
            sb.AppendLine();
            sb.AppendLine("public static partial class Prefabs");
            sb.AppendLine("{");
            WriteTreeChildren(sb, root, 1);
            sb.AppendLine("}");
            WriteFile("Prefabs.cs", sb);
        }

        static void WriteTreeChildren(StringBuilder sb, TreeNode node, int depth)
        {
            var pad = Indent(depth);
            foreach (var leaf in node.Leaves.OrderBy(l => l.Name))
            {
                sb.AppendLine($"{pad}/// <summary><c>{leaf.Entry.ResourcePath}</c></summary>");
                sb.AppendLine($"{pad}public static readonly PrefabRef {leaf.Name} = new({leaf.Entry.Hash});");
            }
            if (node.Leaves.Count > 0 && node.Children.Count > 0) sb.AppendLine();
            foreach (var child in node.Children.Values.OrderBy(c => c.Name))
            {
                sb.AppendLine($"{pad}public static partial class {child.Name}");
                sb.AppendLine($"{pad}{{");
                WriteTreeChildren(sb, child, depth + 1);
                sb.AppendLine($"{pad}}}");
                sb.AppendLine();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  PrefabInterfaces.cs — one interface per folder, all : IPrefabRef
        //
        //  public interface IPrefabsEnemies        : IPrefabRef { }
        //  public interface IPrefabsEnemiesGoblins : IPrefabsEnemies { }
        //
        //  Usage:
        //      [SerializeReference, SubclassSelector]
        //      public IPrefabsEnemiesGoblins[] goblins;
        //      // dropdown shows goblin01, goblin02, ...
        //
        //      [SerializeReference, SubclassSelector]
        //      public IPrefabsEnemies[] anyEnemy;
        //      // dropdown shows everything in Enemies/ recursively
        //
        //      [SerializeReference, SubclassSelector]
        //      public IPrefabRef anyPrefab;
        //      // dropdown shows all prefabs
        // ─────────────────────────────────────────────────────────────────────

        static void WritePrefabInterfaces(List<Entry> entries)
        {
            var folders = CollectFolders(entries);
            var sb      = new StringBuilder();
            sb.AppendLine("// AUTO-GENERATED — do not edit manually");
            sb.AppendLine("using Sackrany.PrefabManager;");
            sb.AppendLine("using Sackrany.PrefabManager.Entities;");
            sb.AppendLine();
            sb.AppendLine("// [SerializeReference, SubclassSelector] IPrefabsEnemiesGoblins[] goblins;");
            sb.AppendLine("// → dropdown: goblin01, goblin02, ...");
            sb.AppendLine("// → goblins[0].Load().POOL();");
            sb.AppendLine("// → goblins[0].Instantiate(pos, rot);");
            sb.AppendLine("// → goblins[0].InstantiateWithComponent<NavMeshAgent>(pos, rot);");
            sb.AppendLine();

            foreach (var folder in folders.OrderBy(f => f))
            {
                var iface       = FolderToInterface(folder);
                var parent      = GetParentFolder(folder);
                var parentIface = parent != null ? FolderToInterface(parent) : "IPrefabRef";

                sb.AppendLine($"/// <summary>Prefabs inside <c>{folder}/</c></summary>");
                sb.AppendLine($"public interface {iface} : {parentIface} {{ }}");
            }

            WriteFile("PrefabInterfaces.cs", sb);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  PrefabWrappers.cs — one [Serializable] class per prefab
        //
        //  [Serializable]
        //  public sealed class Prefab_Enemies_Goblins_goblin01 : IPrefabsEnemiesGoblins
        //  {
        //      public PrefabRef GetRef() => Prefabs.Enemies.Goblins.goblin01;
        //  }
        //
        //  Load/Instantiate/InstantiateWithComponent come from IPrefabRef default methods.
        // ─────────────────────────────────────────────────────────────────────

        static void WritePrefabWrappers(List<Entry> entries)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// AUTO-GENERATED — do not edit manually");
            sb.AppendLine("using System;");
            sb.AppendLine("using Sackrany.PrefabManager;");
            sb.AppendLine("using Sackrany.PrefabManager.Entities;");
            sb.AppendLine();

            foreach (var e in entries.OrderBy(e => e.ResourcePath))
            {
                var parts      = e.ResourcePath.Split('/');
                var className  = "Prefab_" + SanitizeName(e.ResourcePath.Replace('/', '_').Replace('-', '_'));
                var folderPath = parts.Length > 1 ? string.Join("/", parts.Take(parts.Length - 1)) : null;
                var iface      = folderPath != null ? FolderToInterface(folderPath) : "IPrefabRef";
                var dotAccess  = ToDotAccess(e.ResourcePath);

                sb.AppendLine("[Serializable]");
                sb.AppendLine($"public sealed class {className} : {iface}");
                sb.AppendLine("{");
                sb.AppendLine($"    public PrefabRef GetRef() => {dotAccess};");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            WriteFile("PrefabWrappers.cs", sb);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  PrefabManifestInstaller.cs
        // ─────────────────────────────────────────────────────────────────────

        static void WriteManifestInstaller(List<Entry> entries)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// AUTO-GENERATED — do not edit manually");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using Sackrany.PrefabManager;");
            sb.AppendLine("using Sackrany.PrefabManager.Entities;");
            sb.AppendLine();
            sb.AppendLine("internal static class PrefabManifestInstaller");
            sb.AppendLine("{");
            sb.AppendLine("    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]");
            sb.AppendLine("    static void Install()");
            sb.AppendLine("    {");
            sb.AppendLine("        PrefabManifest.Register(new Dictionary<int, string>");
            sb.AppendLine("        {");
            foreach (var e in entries)
                sb.AppendLine($"            [{e.Hash}] = \"{e.ResourcePath}\",");
            sb.AppendLine("        });");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            WriteFile("PrefabManifestInstaller.cs", sb);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────────────────────────────────

        // "Enemies/Goblins" → "IPrefabsEnemiesGoblins"
        static string FolderToInterface(string folder)
            => "IPrefabs" + string.Join("", folder.Split('/').Select(SanitizeName));

        static string GetParentFolder(string folder)
        {
            var idx = folder.LastIndexOf('/');
            return idx < 0 ? null : folder[..idx];
        }

        // "Enemies/Goblins/goblin01" → "Prefabs.Enemies.Goblins.goblin01"
        static string ToDotAccess(string resourcePath)
            => "Prefabs." + string.Join(".", resourcePath.Split('/').Select(SanitizeName));

        static HashSet<string> CollectFolders(List<Entry> entries)
        {
            var folders = new HashSet<string>();
            foreach (var e in entries)
            {
                var parts = e.ResourcePath.Split('/');
                for (int i = 1; i < parts.Length; i++)
                    folders.Add(string.Join("/", parts.Take(i)));
            }
            return folders;
        }

        static TreeNode BuildTree(List<Entry> entries)
        {
            var root = new TreeNode("Prefabs");
            foreach (var e in entries)
            {
                var parts = e.ResourcePath.Split('/');
                var node  = root;
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    var key = SanitizeName(parts[i]);
                    if (!node.Children.TryGetValue(key, out var child))
                        node.Children[key] = child = new TreeNode(key);
                    node = child;
                }
                node.Leaves.Add(new Leaf(SanitizeName(parts[^1]), e));
            }
            return root;
        }

        static string SanitizeName(string raw)
        {
            var sb = new StringBuilder();
            foreach (var c in raw) sb.Append(char.IsLetterOrDigit(c) ? c : '_');
            var s = sb.ToString();
            return char.IsDigit(s[0]) ? "_" + s : s;
        }

        static string Indent(int depth) => new(' ', depth * 4);

        static void WriteFile(string fileName, StringBuilder sb)
            => File.WriteAllText(Path.Combine(OutputPath, fileName), sb.ToString());

        readonly struct Entry
        {
            public readonly string ResourcePath;
            public readonly int    Hash;
            public Entry(string p, int h) { ResourcePath = p; Hash = h; }
        }

        class TreeNode
        {
            public string                       Name;
            public Dictionary<string, TreeNode> Children = new();
            public List<Leaf>                   Leaves   = new();
            public TreeNode(string name) => Name = name;
        }

        readonly struct Leaf
        {
            public readonly string Name;
            public readonly Entry  Entry;
            public Leaf(string name, Entry entry) { Name = name; Entry = entry; }
        }
    }
}