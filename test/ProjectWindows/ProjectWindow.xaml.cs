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
using test.Analitycs;
using test.HelperClasses;
using test.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace test
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectWindow : Window
    {

        private ProjectDescription projectDescription;
        private Root messages;
        private List<PersonStats> personStats;

        public ProjectWindow(ProjectDescription projectDescription)
        {
            InitializeComponent();
            this.projectDescription = projectDescription;
            LoadMessages();
            StartAnalitics();

            this.projectDescription = projectDescription;
            AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            SetupNavbar();

            WindowHelper.Maximize(this);

            NavBar.Header = messages.title;
        }

        private void SetupNavbar()
        {
            List<string> personNames = messages.participants.Select(x => x.name).ToList();
            NavBar.MenuItems.Add(new NavigationViewItemSeparator());
            foreach (var name in personNames)
            {
                var item = new NavigationViewItem
                {
                    Content = name,
                    Tag = $"Person_{name}",
                    Icon = new SymbolIcon(Symbol.Contact),

                };
                NavBar.MenuItems.Add(item);
            }
        }

        private void LoadMessages()
        {
            string mainJSONPath = Path.Combine(projectDescription.ProjectFolderLocation,projectDescription.MergedFile);
            messages = Root.LoadJson(mainJSONPath);
        }

        private void StartAnalitics()
        {
            var stats = PersonStatsAnalyzer.Analize(messages);
            personStats = stats;
        }

        private void SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                string tag = (string)selectedItem.Tag;

                if (tag != null && tag.StartsWith("Person_"))
                {
                    string personName = tag["Person_".Length..];
                    var seguageData = new
                    {
                        Sats = personStats,
                        Messages = messages,
                        PersonName = personName

                    };
                    contentFrame.Navigate(typeof(PersonPage), seguageData);
                }
                else
                {
                    if (tag == "full")
                    {
                        var seguageData = new
                        {
                            Sats = personStats,
                            Messages = messages,
                        };
                        contentFrame.Navigate(typeof(FullAnalytics), seguageData);
                        
                    }
                }
            }
        }
    }
}
