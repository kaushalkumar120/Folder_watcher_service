using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Project7_FolderWatcherService;

public class Form1 : Form
{
    TextBox txtPath = new();
    Button browse = new(), start = new(), stop = new();
    ListBox log = new();
    FileSystemWatcher? watcher;
    readonly string logFile = "folder_log.txt";

    public Form1()
    {
        Text = "Folder Watcher Service";
        Size = new Size(800, 550);
        StartPosition = FormStartPosition.CenterScreen;

        txtPath.SetBounds(20, 20, 500, 30);
        browse.Text = "Browse"; browse.SetBounds(530, 20, 90, 30); browse.Click += Browse_Click;
        start.Text = "Start"; start.SetBounds(630, 20, 60, 30); start.Click += Start_Click;
        stop.Text = "Stop"; stop.SetBounds(700, 20, 60, 30); stop.Click += Stop_Click;

        log.SetBounds(20, 70, 740, 400);

        Controls.AddRange(new Control[] { txtPath, browse, start, stop, log });
    }

    void Browse_Click(object? s, EventArgs e)
    {
        using FolderBrowserDialog d = new();
        if (d.ShowDialog() == DialogResult.OK)
            txtPath.Text = d.SelectedPath;
    }

    void Start_Click(object? s, EventArgs e)
    {
        if (!Directory.Exists(txtPath.Text))
        {
            MessageBox.Show("Select a valid folder.");
            return;
        }

        watcher?.Dispose();
        watcher = new FileSystemWatcher(txtPath.Text)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            IncludeSubdirectories = false
        };

        watcher.Created += (_, e2) => AddLog("Created: " + e2.Name);
        watcher.Deleted += (_, e2) => AddLog("Deleted: " + e2.Name);
        watcher.Renamed += (_, e2) => AddLog($"Renamed: {e2.OldName} -> {e2.Name}");
        watcher.EnableRaisingEvents = true;

        AddLog("Watcher started.");
    }

    void Stop_Click(object? s, EventArgs e)
    {
        if (watcher != null)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            watcher = null;
            AddLog("Watcher stopped.");
        }
    }

    void AddLog(string message)
    {
        string line = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] {message}";
        if (InvokeRequired)
        {
            BeginInvoke(() => AddLog(message));
            return;
        }
        log.Items.Add(line);
        File.AppendAllText(logFile, line + Environment.NewLine);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        watcher?.Dispose();
        base.OnFormClosing(e);
    }
}
