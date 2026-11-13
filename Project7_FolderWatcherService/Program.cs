using System;
using System.Windows.Forms;

namespace Project7_FolderWatcherService;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}
