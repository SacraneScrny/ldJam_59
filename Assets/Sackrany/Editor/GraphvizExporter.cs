// Editor/GraphvizExporter.cs
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

public static class GraphvizExporter
{
    [MenuItem("Tools/Export Graphviz")]
    static void Export()
    {
        var gameAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp");

        if (gameAssembly == null) { UnityEngine.Debug.LogError("Assembly not found"); return; }

        var types = gameAssembly.GetTypes()
            .Where(t => t.Namespace?.StartsWith("Sackrany.") == true)
            .Where(t =>
                !t.Name.StartsWith("<") &&
                !t.Name.StartsWith("__") &&
                !t.IsNested ||
                (t.IsNested && !t.Name.Contains("<"))
            )
            .Where(t => char.IsUpper(t.Name[0]))
            .ToList();
        var typeSet = new HashSet<string>(types.Select(t => t.Name));

        var sb = new StringBuilder();
        sb.AppendLine("digraph Unity {");
        sb.AppendLine("  rankdir=LR;");
        sb.AppendLine("  node [shape=box];");

        var grouped = types.GroupBy(t => t.Namespace ?? "Global");

        foreach (var group in grouped)
        {
            sb.AppendLine($"  subgraph \"cluster_{Sanitize(group.Key)}\" {{");
            sb.AppendLine($"    label=\"{group.Key}\";");
            sb.AppendLine($"    style=rounded; color=gray;");
            foreach (var t in group)
                sb.AppendLine($"    \"{t.Name}\";");
            sb.AppendLine("  }");
        }
        foreach (var t in types)
        {
            if (t.BaseType != null && typeSet.Contains(t.BaseType.Name))
                sb.AppendLine($"  \"{t.Name}\" -> \"{t.BaseType.Name}\" [label=\"extends\"];");

            foreach (var iface in t.GetInterfaces().Where(i => typeSet.Contains(i.Name)))
                sb.AppendLine($"  \"{t.Name}\" -> \"{iface.Name}\" [style=dashed, label=\"impl\"];");
        }
        
        sb.AppendLine("}");

        File.WriteAllText(Application.dataPath + "/graph.dot", sb.ToString());
        UnityEngine.Debug.Log($"Saved: {Application.dataPath + "/graph.dot"}");
    }
    
    static string Sanitize(string name)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c)) sb.Append(c);
            else sb.Append('_');
        }
        return sb.ToString();
    }
}