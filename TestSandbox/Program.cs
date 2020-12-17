using System;
using System.Numerics;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics;
using SSHProtocol;

namespace TestSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "powershell.exe";
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            proc.StandardInput.WriteLine(@"ls C:\Users");
            proc.StandardInput.Flush();
            proc.StandardInput.Close();
            proc.WaitForExit();
            Console.WriteLine(proc.StandardOutput.ReadToEnd());
        }
    }
}
