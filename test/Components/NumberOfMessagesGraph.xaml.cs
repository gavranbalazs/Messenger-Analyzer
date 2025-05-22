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
    public sealed partial class NumberOfMessagesGraph : UserControl
    {
        public Root messages;
        public SummaryViewModel summaryViewModel { get; set; }

        
        public NumberOfMessagesGraph()
        {
            InitializeComponent();
        }

        public void SetupComponent(Root messages, List<PersonStats> personStats)
        {
            this.messages = messages;
            summaryViewModel = new SummaryViewModel(personStats, messages);
            this.DataContext = summaryViewModel;
            LoadComboBoxItems();
            summaryViewModel.ApplyYearFilter(-1);
        }

        private void LoadComboBoxItems()
        {
            
            var years = messages.messages.Select(m => m.Timestamp.Year).Distinct().ToList();
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
                    summaryViewModel.ApplyYearFilter(-1); // vagy a megfelelõ logika
                }
            }
            else
            {
                int selectedYear = (int)YearFilter.SelectedItem;
                summaryViewModel.ApplyYearFilter(selectedYear);
            }

        }
    }
}
