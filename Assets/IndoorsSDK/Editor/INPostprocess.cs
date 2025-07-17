#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Diagnostics;


public static class INPostprocess
{
      [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
            return;

        string podfilePath = Path.Combine(path, "Podfile");

        // Создание Podfile если его нет
        if (!File.Exists(podfilePath))
        {
            File.WriteAllText(podfilePath,
@"platform :ios, '12.0'
use_frameworks!

install! 'cocoapods', :warn_for_unused_master_specs_repo => false

target 'UnityFramework' do
  pod 'IndoorsCoreSDK', '~> 3.3.9'
end

target 'Unity-iPhone' do
  pod 'IndoorsCoreSDK', '~> 3.3.9'
end
");
        }
        else
        {
            // Добавить pod если уже есть Podfile (не строго обязательно)
            var content = File.ReadAllText(podfilePath);
            if (!content.Contains("IndoorsCoreSDK"))
            {
                content = content.Replace("use_frameworks!",
@"use_frameworks!
  pod 'IndoorsCoreSDK', '~> 3.3.9'");
                File.WriteAllText(podfilePath, content);
            }
        }

        // Выполнить pod install (если cocoapods установлен)
        RunCommand("/opt/homebrew/bin/pod", "install", path);
    }

static void RunCommand(string command, string args, string workingDir)
{
    if (!File.Exists(command))
    {
        UnityEngine.Debug.LogError($"[pod install error] Command not found: {command}");
        return;
    }

    var proc = new Process();
    proc.StartInfo.FileName = command;
    proc.StartInfo.Arguments = args;
    proc.StartInfo.WorkingDirectory = workingDir;
    proc.StartInfo.RedirectStandardOutput = true;
    proc.StartInfo.RedirectStandardError = true;
    proc.StartInfo.UseShellExecute = false;
    proc.StartInfo.CreateNoWindow = true;

    // Установка переменной LANG вручную
    proc.StartInfo.EnvironmentVariables["LANG"] = "en_US.UTF-8";

    UnityEngine.Debug.Log($"[pod install] Running: {command} {args} in {workingDir}");

    proc.Start();

    string output = proc.StandardOutput.ReadToEnd();
    string error = proc.StandardError.ReadToEnd();
    proc.WaitForExit();

    UnityEngine.Debug.Log("[pod install output] " + output);

    if (!string.IsNullOrEmpty(error))
        UnityEngine.Debug.LogError("[pod install error] " + error);

    if (proc.ExitCode != 0)
        UnityEngine.Debug.LogError($"[pod install] exited with code {proc.ExitCode}");
}
}
#endif

