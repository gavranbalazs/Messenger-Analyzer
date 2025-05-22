using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.HelperClasses;

namespace test.Analitycs
{
    public partial class TimeOfDayStatsViewModel : ObservableObject
    {

        [ObservableProperty]
        public partial ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        public Root messages { get; set; }

        public TimeOfDayStatsViewModel(Root messages)
        {
            this.messages = messages;
            Dictionary<string, int> data = new Dictionary<string, int>();
            for (int i = 0; i < 24; i++)
            {
                data.Add(i.ToString("00") + ":00", 0); // Initialize with 0 messages for each hour
            }

            messages.messages.ForEach(m =>
            {
                // Get the hour of the message timestamp
                var hour = m.Timestamp.Hour;
                // Increment the count for that hour
                data[hour.ToString("00") + ":00"]++;
            });

            Series = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = new ObservableCollection<int>(data.Values),
                    Name = "Üzenetek száma"
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = new List<string>(data.Keys),
                    LabelsRotation = 15
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Üzenetszám"
                }
            };
        }

        public void ApplyYearFilter(int year)
        {
            // Create a dictionary to hold the count of messages for each hour of the day
            Dictionary<string, int> data = new Dictionary<string, int>();
            for (int i = 0; i < 24; i++)
            {
                data.Add(i.ToString("00") + ":00", 0); // Initialize with 0 messages for each hour
            }
            var a = messages.messages;
            if (year != -1)
            {
                a = messages.messages.Where(m => m.Timestamp.Year == year).ToList();
            }

            a.ForEach(m =>
            {
                // Get the hour of the message timestamp
                var hour = m.Timestamp.Hour;
                // Increment the count for that hour
                data[hour.ToString("00") + ":00"]++;
            });
            Series = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = new ObservableCollection<int>(data.Values),
                    Name = "Üzenetek száma"
                }
            };
            
        }

    }
}
