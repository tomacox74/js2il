using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;



namespace Js2IL.Tests.Utilities
{
    internal class AssemblyToText
    {
        public static string ConvertToTextUsingIlSpyCmd(string assemblyPath)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"tool run ilspycmd -il {assemblyPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            process!.WaitForExit();

            string stdOut = process.StandardOutput.ReadToEnd();
            string stdErr = process.StandardError.ReadToEnd();

            if (process.ExitCode != 0)
            {
                throw new Exception($"ilspycmd failed:\n{stdErr}");
            }

            return stdOut;
        }

        public static string ConvertToText(string assemblyPath)
        {
            //return ConvertToTextUsingIlSpyCmd(assemblyPath);
            return ILSpyBasedDisassembler.DisassembleIL(assemblyPath);
        }
    }

    public static class ILSpyBasedDisassembler
    {
        public static string DisassembleIL(string dllPath)
        {
            using var peFile = new PEFile(dllPath); 
            var stringWriter = new StringWriter();
            var output = new PlainTextOutput(stringWriter);

            output.WriteLine($"// IL code: {peFile.Name}");

            var disassembler = new ReflectionDisassembler(output, CancellationToken.None)
            {
                ShowSequencePoints = true,
            };

            disassembler.WriteModuleContents(peFile);

            return output.ToString();
        }
    }
}


