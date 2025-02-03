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
        start.FileName = "python"; // �T�O Python �b�����ܼ� PATH ��
        start.Arguments = Application.dataPath + "/PythonScripts/HandTrackToUnity.py"; // ���w�}�������|
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
