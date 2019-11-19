using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FixBlazorStaticFiles
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var mainDir = new DirectoryInfo(args.Length > 0 ? args[0] : Environment.CurrentDirectory);
            await FixBootJson(mainDir);
            await FixWebAssemblyJs(mainDir);
        }

        static async Task FixBootJson(DirectoryInfo mainDir)
        {
            var fullName = mainDir.GetFiles("blazor.boot.json").Single().FullName;
            var content = new StringBuilder(await File.ReadAllTextAsync(fullName));
            Fix(mainDir.GetDirectories("_bin").Single().GetFiles(), content);
            await File.WriteAllTextAsync(fullName, content.ToString());
        }

        static async Task FixWebAssemblyJs(DirectoryInfo mainDir)
        {
            var fullName = mainDir.GetFiles("blazor.webassembly.js").Single().FullName;
            var content = new StringBuilder(await File.ReadAllTextAsync(fullName));
            Fix(mainDir.GetDirectories("wasm").Single().GetFiles(), content, "_framework/wasm/");
            Fix(mainDir.GetFiles("blazor.boot.json"), content, "_framework/");
            await File.WriteAllTextAsync(fullName, content.ToString());
        }

        private static void Fix(FileInfo[] files, StringBuilder content, string baseUrl = "")
        {
            var binFiles = files
                .Select(e => new {e.Name, Md5 = ComputeMd5(e.FullName)})
                .AsParallel()
                .ToList();
            foreach (var file in binFiles)
                content.Replace($"\"{baseUrl}{file.Name}\"", $"\"{baseUrl}{file.Name}?v={file.Md5}\"");
        }

        static string ComputeMd5(string filename)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filename);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}