// Original free temporary-extraction launcher. No installer, elevation or console.
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

internal static class PortableLauncher
{
    const string Title = "ILABAN ANG TAWA DEVELOPED BY CHRISTINE TAMAYO";
    const string PayloadHash = "__PAYLOAD_HASH__";
    static readonly string Log = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "IlabanPortable", "launcher-last.log");

    static void Record(string text)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Log));
        File.AppendAllText(Log, DateTime.UtcNow.ToString("o") + " " + text + Environment.NewLine);
    }
    static string Hash(Stream stream)
    {
        using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
    }
    static string Quote(string value)
    {
        var result = new StringBuilder("\""); int slashes = 0;
        foreach (char c in value)
        {
            if (c == '\\') { slashes++; continue; }
            result.Append('\\', c == '"' ? slashes * 2 + 1 : slashes);
            result.Append(c); slashes = 0;
        }
        return result.Append('\\', slashes * 2).Append('"').ToString();
    }
    static string SafePath(string root, string entry)
    {
        string target = Path.GetFullPath(Path.Combine(root, entry.Replace('/', Path.DirectorySeparatorChar)));
        if (!target.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Hindi wasto ang talaksan.");
        return target;
    }
    [STAThread]
    static int Main(string[] args)
    {
        string folder = Path.Combine(Path.GetTempPath(), "IlabanPortable-" + Guid.NewGuid().ToString("N"));
        bool completed = false;
        try
        {
            Record("START payload=" + PayloadHash);
            Application.EnableVisualStyles();
            using (var progress = new Form())
            {
                progress.Text = Title; progress.StartPosition = FormStartPosition.CenterScreen;
                progress.ClientSize = new Size(560, 150); progress.FormBorderStyle = FormBorderStyle.FixedDialog;
                progress.ControlBox = false; progress.BackColor = Color.FromArgb(35, 32, 27);
                var label = new Label { Text = "Inihahanda ang laro...", ForeColor = Color.FromArgb(245, 229, 194), AutoSize = false, Location = new Point(24, 22), Size = new Size(510, 44), Font = new Font("Segoe UI", 16) };
                var bar = new ProgressBar { Location = new Point(24, 88), Size = new Size(510, 22), Minimum = 0, Maximum = 100 };
                progress.Controls.Add(label); progress.Controls.Add(bar); progress.Show(); Application.DoEvents();
                var assembly = Assembly.GetExecutingAssembly();
                using (var payload = assembly.GetManifestResourceStream("game.zip"))
                {
                    if (payload == null || Hash(payload) != PayloadHash) throw new InvalidDataException("Hindi kumpleto ang sipi ng laro.");
                    payload.Position = 0; Directory.CreateDirectory(folder);
                    using (var zip = new ZipArchive(payload, ZipArchiveMode.Read))
                    {
                        int count = 0;
                        foreach (var entry in zip.Entries)
                        {
                            string target = SafePath(folder, entry.FullName);
                            if (entry.FullName.EndsWith("/")) Directory.CreateDirectory(target);
                            else
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(target));
                                using (var input = entry.Open()) using (var output = File.Create(target))
                                {
                                    var buffer = new byte[1024 * 1024]; int read;
                                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0) { output.Write(buffer, 0, read); Application.DoEvents(); }
                                }
                            }
                            bar.Value = ++count * 80 / zip.Entries.Count; Application.DoEvents();
                        }
                    }
                }
                label.Text = "Sinusuri ang mga talaksan...";
                using (var manifest = new StreamReader(assembly.GetManifestResourceStream("files.sha256"), Encoding.UTF8))
                {
                    string line; int checkedFiles = 0;
                    while ((line = manifest.ReadLine()) != null)
                    {
                        if (line.Length < 67) throw new InvalidDataException("Hindi wasto ang talaan.");
                        string file = SafePath(folder, line.Substring(66));
                        using (var input = File.OpenRead(file)) if (Hash(input) != line.Substring(0, 64)) throw new InvalidDataException("Hindi tugma ang talaksan: " + Path.GetFileName(file));
                        checkedFiles++; Application.DoEvents();
                    }
                    Record("VERIFIED files=" + checkedFiles);
                }
                bar.Value = 100; label.Text = "Binubuksan ang laro..."; Application.DoEvents(); progress.Close();
            }
            string gameFolder = Path.Combine(folder, Title);
            var quoted = new string[args.Length]; for (int i = 0; i < args.Length; i++) quoted[i] = Quote(args[i]);
            using (var child = Process.Start(new ProcessStartInfo(Path.Combine(gameFolder, Title + ".exe"), string.Join(" ", quoted)) { WorkingDirectory = gameFolder, UseShellExecute = false }))
            {
                if (child == null) throw new IOException("Hindi mabuksan ang laro.");
                Record("GAME pid=" + child.Id); child.WaitForExit(); Record("EXIT code=" + child.ExitCode);
                completed = child.ExitCode == 0; return child.ExitCode;
            }
        }
        catch (Exception error)
        {
            Record("ERROR " + error);
            MessageBox.Show("Hindi mabuksan ang laro.\n" + error.Message + "\n\nTingnan ang tala: " + Log, Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 1;
        }
        finally
        {
            // Delete only the unique directory created by this invocation, after the child exits.
            string prefix = Path.Combine(Path.GetTempPath(), "IlabanPortable-");
            if (completed && Path.GetFullPath(folder).StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                for (int retry = 0; retry < 3; retry++)
                {
                    try { if (Directory.Exists(folder)) Directory.Delete(folder, true); Record("CLEANUP complete"); break; }
                    catch (IOException) { Thread.Sleep(500); }
                    catch (UnauthorizedAccessException) { Record("CLEANUP deferred"); break; }
                }
            }
        }
    }
}
