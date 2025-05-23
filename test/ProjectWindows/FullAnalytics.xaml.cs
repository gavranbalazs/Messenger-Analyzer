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
using test.Components;
using test.HelperClasses;
using test.ProjectWindows;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FullAnalytics : Page
    {
        private List<PersonStats> personStats;
        private Root messages;

        public FullAnalytics()
        {
            InitializeComponent();
            //this.Summary.Text = "�sszegz�s";
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            dynamic data = e.Parameter;
            personStats = data.Sats;
            Root msgs = data.Messages;
            messages = msgs;
            NumberOfMessagesGraph.SetupComponent(messages, personStats);
            DailyMessageCount.SetupComponent(messages);
            TimeOfDaysStats.SetupComponent(messages);
        }
    }
}
