using System.Diagnostics;
using UnityEngine;

public class RunPythonScript : MonoBehaviour
{
    void Start()
    {
        RunPython();
    }

    void RunPython()
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python"; // 確保 Python 在環境變數 PATH 中
        start.Arguments = Application.dataPath + "/PythonScripts/HandTrackToUnity.py"; // 指定腳本的路徑
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.CreateNoWindow = true;

        Process process = new Process();
        process.StartInfo = start;
        process.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data);
        process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError(args.Data);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }
}
