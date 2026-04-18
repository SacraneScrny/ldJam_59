#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Linq;
using System.Text;

public class SceneEnumGenerator : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    private const string OutputPath = "Assets/_Generated/SceneNames.cs";
    private static readonly string[] ManualScenes = { "SystemScene", "UIScene" };

    [MenuItem("Sackrany/Scenes/Generate Scene Names")]
    public static void Generate()
    {
        var scenes = EditorBuildSettings.scenes
            .Select(s => Path.GetFileNameWithoutExtension(s.path))
            .Where(name => !ManualScenes.Contains(name));

        var sb = new StringBuilder();
        sb.AppendLine("// AUTO-GENERATED — не редактировать вручную");
        sb.AppendLine("namespace Sackrany.Scenes");
        sb.AppendLine("{");
        sb.AppendLine("    public static partial class SceneNames");
        sb.AppendLine("    {");

        foreach (var name in scenes)
        {
            var constName = name.ToUpper().Replace(" ", "_").Replace("-", "_");
            sb.AppendLine($"        public const string {constName} = \"{name}\";");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));
        File.WriteAllText(OutputPath, sb.ToString());
        AssetDatabase.Refresh();
    }

    public void OnPreprocessBuild(BuildReport report) => Generate();
}
#endif