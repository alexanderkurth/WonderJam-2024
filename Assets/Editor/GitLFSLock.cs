using UnityEditor;
using UnityEngine;
using System.Diagnostics;

public class GitLFSLock : MonoBehaviour
{
    [MenuItem("Assets/Unlock Git LFS File")]
    static void UnlockGitLFSFile()
    {
        // Récupère l'asset sélectionné
        var selected = Selection.activeObject;
        if (selected == null)
        {
            UnityEngine.Debug.LogError("No asset selected.");
            return;
        }

        // Obtient le chemin complet de l'asset
        string assetPath = AssetDatabase.GetAssetPath(selected);
        string fullPath = Application.dataPath + assetPath.Substring("Assets".Length);

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c git lfs unlock \"{fullPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        Process process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            UnityEngine.Debug.Log($"Successfully unlocked {fullPath} with Git LFS.");
        }
        else
        {
            UnityEngine.Debug.LogError($"Failed to unlock {fullPath} with Git LFS. Output: {output}");
        }
    }
}
