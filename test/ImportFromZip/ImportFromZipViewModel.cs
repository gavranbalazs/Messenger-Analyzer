using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using test.HelperClasses;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace test.ImportFromZip
{
    public partial class ImportFromZipViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial ObservableCollection<string> zipFiles { get; set; } = new();

        [ObservableProperty]
        public partial ObservableCollection<ConversationCandidate> conversationCandidates { get; set; } = new();

        [ObservableProperty]
        public partial ObservableCollection<ConversationCandidate> filteredCandidates { get; set; } = new();





        public async void AddZipFiles(Window window)
        {
            var picker = new FileOpenPicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(window));
            picker.FileTypeFilter.Add(".zip");

            var files = await picker.PickMultipleFilesAsync();
            foreach (var file in files)
            {
                if (!zipFiles.Contains(file.Path))
                    zipFiles.Add(file.Path);
            }
        }

        public async Task AnalyzeZipsAsync(Window window)
        {
            
            conversationCandidates.Clear();

            var candidates = new List<ConversationCandidate>();

            await Task.Run(() =>
            {
                foreach (var zipPath in zipFiles)
                {
                    string tempFolder = Path.Combine(Path.GetTempPath(), "MessengerImport", Guid.NewGuid().ToString());
                    Directory.CreateDirectory(tempFolder);

                    try
                    {
                        ZipFile.ExtractToDirectory(zipPath, tempFolder);

                        var messageFiles = Directory.GetFiles(tempFolder, "message*.json", SearchOption.AllDirectories);
                        foreach (var messageFile in messageFiles)
                        {
                            string jsonText = File.ReadAllText(messageFile);
                            string utf8String = FixUnicodeEncoding(jsonText);

                            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(utf8String);

                            if (!string.IsNullOrWhiteSpace(json.title) &&
                                !candidates.Any(c => c.DisplayName == json.title))
                            {
                                candidates.Add(new ConversationCandidate
                                {
                                    DisplayName = json.title,
                                    IsSelected = false
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        
                        Window.Current.DispatcherQueue.TryEnqueue(async () =>
                        {
                            ContentDialog dialog = new()
                            {
                                Title = "Hiba történt",
                                Content = $"Nem sikerült feldolgozni: {zipPath}\n\n{ex.Message}",
                                CloseButtonText = "OK",
                                XamlRoot = window.Content.XamlRoot
                            };
                            await dialog.ShowAsync();
                        });
                    }
                }
            });

            
            foreach (var c in candidates)
            {
                conversationCandidates.Add(c);
            }

            filteredCandidates.Clear();
            foreach (var candidate in conversationCandidates)
            {
                filteredCandidates.Add(candidate);
            }

        }

        public void UpdateFilteredCandidates(string query)
        {
            
            if (string.IsNullOrEmpty(query))
            {
                filteredCandidates.Clear();
                foreach (var candidate in conversationCandidates)
                {
                    filteredCandidates.Add(candidate);
                }
                return;
            }

            filteredCandidates.Clear();

            foreach (var candidate in conversationCandidates
                         .Where(c => c.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase)))
            {
                filteredCandidates.Add(candidate);
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

        internal void UpdateSelectedCandidate(ConversationCandidate candidate)
        {

            foreach (var candidate1 in filteredCandidates)
            {
                if (candidate1.DisplayName == candidate.DisplayName)
                {
                    candidate1.IsSelected = candidate.IsSelected;
                }
            }

            foreach (var candidate1 in conversationCandidates)
            {
                if (candidate1.DisplayName == candidate.DisplayName)
                {
                    candidate1.IsSelected = candidate.IsSelected;
                }
            }
        }
    }

    public class ConversationCandidate
    {
        public string DisplayName { get; set; } = "";
        public bool IsSelected { get; set; }

        public override string ToString() => DisplayName;
    }

}
