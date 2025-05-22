using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.Components;
using test.HelperClasses;

namespace test.Analitycs
{
    public partial class DailyMessagesCountViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial ISeries[] DailyMessageSeries { get; set; }
        public ICartesianAxis[] DailyMessageXAxes { get; set; } = [
        new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("yy-MMM-dd"))
            ];

        public void LoadDailyMessageChart(Root messages)
        {
            var DailyMessageCount = messages.messages
                .GroupBy(m => m.Timestamp.Date)
                .Select(g => new DateTimePoint
                {
                    DateTime = g.Key,
                    Value = g.Count()
                })
                .OrderBy(g => g.DateTime)
                .ToList();

            DailyMessageSeries = new ISeries[]
            {
                new ColumnSeries<DateTimePoint>
                {
                    Values = DailyMessageCount
                }
            };
        }
        public void FilterChart(Root messages, int year)
        {
            var DailyMessageCount = messages.messages
                .Where(m => m.Timestamp.Year == year)
                .GroupBy(m => m.Timestamp.Date)
                .Select(g => new DateTimePoint
                {
                    DateTime = g.Key,
                    Value = g.Count()
                })
                .OrderBy(g => g.DateTime)
                .ToList();
            DailyMessageSeries = new ISeries[]
            {
                new ColumnSeries<DateTimePoint>
                {
                    Values = DailyMessageCount
                }
            };
        }

    }



}
