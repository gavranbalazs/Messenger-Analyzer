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
using test.ProjectWindows;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace test.Components
{
    public sealed partial class TimeOfDayStats : UserControl
    {
        TimeOfDayStatsViewModel TimeOfDayStatsViewModel { get; set; }

        public TimeOfDayStats()
        {
            InitializeComponent();
        }

        public void SetupComponent(Root messages)
        {
            TimeOfDayStatsViewModel viewModel = new TimeOfDayStatsViewModel(messages);
            this.DataContext = viewModel;
            TimeOfDayStatsViewModel = viewModel;
            LoadComboBoxItems();
        }

        private void LoadComboBoxItems()
        {
            // Clear existing items
            YearFilter.Items.Clear();
            // Add the "Összes" item
            YearFilter.Items.Add(new ComboBoxItem { Content = "Összes" });
            // Get distinct years from messages and add them to the ComboBox
            var years = TimeOfDayStatsViewModel.messages.messages.Select(m => m.Timestamp.Year).Distinct().ToList();
            years.Sort();
            foreach (var year in years)
            {
                YearFilter.Items.Add(year);
            }
        }

        private void YearFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            //check if the tag is all value
            if (YearFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                string content = selectedItem.Content.ToString();

                if (content == "Összes")
                {
                    TimeOfDayStatsViewModel.ApplyYearFilter(-1); // vagy a megfelelõ logika
                }
            }
            else
            {
                int selectedYear = (int)YearFilter.SelectedItem;
                TimeOfDayStatsViewModel.ApplyYearFilter(selectedYear);
            }

        }
    }
}
