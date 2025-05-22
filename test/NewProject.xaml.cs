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
using System.Text;
using System.Threading.Tasks;
using test.HelperClasses;
using test.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using static System.Net.Mime.MediaTypeNames;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace test
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewProjectWindow : Window
    {
        private List<StorageFile>? selectedFiles;

        private Root? Messages { get; set; }

        private string MagicName { get; set; }

        public NewProjectWindow()
        {
            InitializeComponent();
            AppWindow.Resize(new Windows.Graphics.SizeInt32(550,700));
        }

        private void SelectFiles(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".json");
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(this));
            var files = picker.PickMultipleFilesAsync().AsTask().Result;

            if (files != null && files.Any())
            {
                selectedFiles = files.ToList();
                FileListView.ItemsSource = selectedFiles.Select(f => f.Name).Order();

            }

            ParseFile();
        }

        private void ParseFile()
        {
            List<Root> parsedFiles = new List<Root>();
            if (selectedFiles == null || selectedFiles.Count == 0)
            {
                return;
            }

            //loadbar
            LoadingBar.IsIndeterminate = true;
            LoadingBar.Visibility = Visibility.Visible;


            foreach (var file in selectedFiles)
            {
                
                try
                {
                    string jsonText = File.ReadAllText(file.Path);
                    //fix unicode encoding
                    string utf8String = FixUnicodeEncoding(jsonText);

                    var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(utf8String);
                        if (json != null)
                        {
                            parsedFiles.Add(json);
                        }
                        else
                        {
                            ContentDialog dialog = new()
                            {
                                Title = "Hiba",
                                Content = $"A {file.Name} nem érvényes JSON formátumú.",
                                CloseButtonText = "OK",
                                XamlRoot = this.Content.XamlRoot
                            };
                        _ = dialog.ShowAsync();
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            }
            if (parsedFiles.Count > 1)
            {
                for (int i = 1; i < parsedFiles.Count; i++)
                {
                    parsedFiles[0].MergeMessages(parsedFiles[i]);
                }
            }
            Messages = parsedFiles[0];
            MagicName = Messages.title;
            
            if (IsAutomaticName.IsChecked == true) ProjectNameTextBox.Text = MagicName;

            if (Messages != null)
            {
                RemoveFile.IsEnabled = true;
                //calculate file size
                long size = 0;
                foreach (var file in selectedFiles)
                {
                    var fileInfo = new FileInfo(file.Path);
                    size += fileInfo.Length;
                }
                //file size in MB
                size = size / 1024 / 1024;
                //fájl méret kerekítése kiíráskor
                FileInfoTB.Text = $"{parsedFiles.Count} fájl ({size} MB)";
            }
            else
            {
                RemoveFile.IsEnabled = false;
            }
            //hide loadbar
            LoadingBar.IsIndeterminate = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        private void Cancle(object sender, RoutedEventArgs e)
        {
            //set all properties to null, so they will be garbage collected
            Messages = null;
            selectedFiles = null;
            FileListView.ItemsSource = null;
            ProjectNameTextBox.Text = string.Empty;
            ProjectDescriptionTextBox.Text = string.Empty;

            this.Close();
        }

        private async void CreateProject(object sender, RoutedEventArgs e)
        {
            string projectName = ProjectNameTextBox.Text.Trim();
            string projectDescription = ProjectDescriptionTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(projectName) || selectedFiles.Count == 0)
            {
                ContentDialog dialog = new()
                {
                    Title = "Hiba",
                    Content = "Kérlek add meg a projekt nevét és válassz ki legalább egy fájlt.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            
            await SaveProjectAsync(projectName, projectDescription);
        }

        private async Task SaveProjectAsync(string projectName, string projectDescription)
        {
            var projectsRoot = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Projects", CreationCollisionOption.OpenIfExists);
            var projectFolder = await projectsRoot.CreateFolderAsync(projectName, CreationCollisionOption.ReplaceExisting);

            // Fájlok másolása
            foreach (var file in selectedFiles)
            {
                await file.CopyAsync(projectFolder);
            }

            ProjectConfig projectConfig = new()
            {
                //windows language
                Language = "hu",
                IsAnonymous = false,
                Nicnames = []
            };

            // Projekt leírás létrehozása
            var projectDescriptionfile = new ProjectDescription
            {
                Name = projectName,
                Description = projectDescription, 
                Creator = Environment.UserName,
                CreationDate = DateTime.Now,
                RawFiles = selectedFiles.Select(f => f.Name).ToArray(),
                MergedFile = "messages.json",
                StatisticsFile = [],
                ProjectFolderLocation = projectFolder.Path
            };

            projectDescriptionfile.WriteToJson(Path.Combine(projectFolder.Path, "project.mproj"));

            Messages.SaveJson(Path.Combine(projectFolder.Path, "messages.json"));

            //set all properties to null, so they will be garbage collected
            projectDescriptionfile = null;
            projectConfig = null;
            Messages = null;
            selectedFiles = null;
            FileListView.ItemsSource = null;


            this.Close();
        }

        private void AutomaticNameChanged(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                if (checkBox.IsChecked == true)
                {
                    ProjectNameTextBox.IsEnabled = false;
                    (MagicName, ProjectNameTextBox.Text) = (ProjectNameTextBox.Text, MagicName);
                }
                else
                {
                    ProjectNameTextBox.IsEnabled = true;
                }
            }
        }

        private void RemoveSelectedFiles(object sender, RoutedEventArgs e)
        {
            foreach (var item in FileListView.SelectedItems)
            {
                //remove selected files
                var fileName = item.ToString();
                var file = selectedFiles.FirstOrDefault(f => f.Name == fileName);
                if (file != null)
                {
                    selectedFiles.Remove(file);
                    
                }
            }
            FileListView.ItemsSource = selectedFiles.Select(f => f.Name);
            Messages = null;
            ParseFile();

            if (Messages == null)
            {
                RemoveFile.IsEnabled = false;
                FileInfoTB.Text = "0 fájl (0 MB)";
            }
        }


        public static string FixUnicodeEncoding(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            try
            {
                // Decode Unicode escape sequences
                string decoded = System.Text.RegularExpressions.Regex.Replace(
                    input,
                    @"\\u([0-9A-Fa-f]{4})",
                    match => {
                        int codePoint = Convert.ToInt32(match.Groups[1].Value, 16);
                        return ((char)codePoint).ToString();
                    }
                );

                // Fix double-encoded UTF-8
                byte[] bytes = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(decoded);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return input;
            }
        }
    }
}
