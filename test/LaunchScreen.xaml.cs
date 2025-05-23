using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using test.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace test
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private List<StorageFile> recentProjectFiles;
        private List<ProjectDescription> recentProjectDescriptions;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            AppWindow.Resize(new Windows.Graphics.SizeInt32(800, 600));
            LoadRecentProjects();
            CleanOldTempFolders();
        }

        private void LoadRecentProjects()
        {
            recentProjectFiles = new List<StorageFile>();
            var folders = ApplicationData.Current.LocalFolder.GetFoldersAsync().AsTask().Result;

            foreach (var folder in folders)
            {
                if (folder.Name == "Projects")
                {
                    // Fõmappa fájljai
                    var files = folder.GetFilesAsync().AsTask().Result;
                    recentProjectFiles.AddRange(files.Where(f => f.Name.EndsWith(".mproj")));

                    // Almappák rekurzív vizsgálata
                    var subfolders = folder.GetFoldersAsync().AsTask().Result;
                    foreach (var subfolder in subfolders)
                    {
                        var filesInSubfolder = subfolder.GetFilesAsync().AsTask().Result;
                        foreach (var subfile in filesInSubfolder)
                        {
                            if (subfile.Name.EndsWith(".mproj"))
                            {
                                recentProjectFiles.Add(subfile);
                            }
                        }
                    }

                    break;
                }
            }

            recentProjectDescriptions = new List<ProjectDescription>();
            foreach (var file in recentProjectFiles)
            {
                var projectDescription = new ProjectDescription(file.Path);
                recentProjectDescriptions.Add(projectDescription);
            }
            RecentProjectsList.ItemsSource = recentProjectDescriptions;
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewProjectWindow newProjectWindow = new NewProjectWindow();
            var (x, y) = WindowHelper.GetWindowPosition(this);
            var appWindow = WindowHelper.GetAppWindow(newProjectWindow);
            appWindow.Move(new Windows.Graphics.PointInt32(x + 50, y + 50));
            newProjectWindow.Activate();
        }

        private void OpenProject(object sender, DoubleTappedRoutedEventArgs e)
        {
            
            if (sender is ListView list)
            {
                ProjectWindow projectWindow = new ProjectWindow(recentProjectDescriptions[list.SelectedIndex]);
                projectWindow.Title = recentProjectDescriptions[list.SelectedIndex].Name;
                projectWindow.Activate();
                this.Close();
            }
            
        }

        private void ImportFromZip_Click(object sender, RoutedEventArgs e)
        {
            ImportFromZipWindow newProjectWindow = new ImportFromZipWindow();
            var (x, y) = WindowHelper.GetWindowPosition(this);
            var appWindow = WindowHelper.GetAppWindow(newProjectWindow);
            appWindow.Move(new Windows.Graphics.PointInt32(x + 50, y + 50));
            newProjectWindow.Activate();
        }

        void CleanOldTempFolders()
        {
            string baseTemp = Path.Combine(Path.GetTempPath(), "MessengerImport");
            if (Directory.Exists(baseTemp))
            {
                foreach (var dir in Directory.GetDirectories(baseTemp))
                {
                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch { }
                }
            }
        }
    }
}
