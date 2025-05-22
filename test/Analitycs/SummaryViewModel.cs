using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using test.Analitycs;
using test.HelperClasses;

namespace test.ProjectWindows
{

    public partial class SummaryViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        private List<PersonStats> personStats { get; set; }
        private Root Messages { get; set; }


        public SummaryViewModel(List<PersonStats> stats, Root root)
        {
            this.personStats = stats;
            var names = stats.Select(p => p.Name).ToArray();
            var messageCounts = stats.Select(p => (double)p.MessageCount).ToArray();
            this.Messages = root;

            ApplyYearFilter(-1);



            YAxes = new Axis[]
            {
            new Axis
            {
                Name = "Üzenetek száma"
            }
            };
        }

        public void ApplyYearFilter(int year = -1)
        {
            var messageCounts = personStats.Select(p => (double)p.MessageCount).ToArray();
            var names = personStats.Select(p => p.Name).ToArray();
            if (year != -1)
            {
                var a = Messages.participants.Select(p => new
                {
                    Name = p.name,
                    MessageCount = Messages.messages.Where(m => m.sender_name == p.name && m.Timestamp.Year == year).Count()
                });
                names = a.Select(p => p.Name).ToArray();
                messageCounts = a.Select(p => (double)p.MessageCount).ToArray();

            }
            //sort by message count
            var sorted = messageCounts.Select((value, index) => new { value, index })
                .OrderByDescending(x => x.value)
                .ToList();
            messageCounts = sorted.Select(x => x.value).ToArray();
            names = sorted.Select(x => names[x.index]).ToArray();


            XAxes = new Axis[]
            {
            new Axis
            {
                Labels = names,
                LabelsRotation = 6,
                
            }
            };

            Series = new ISeries[]
                {
                    new ColumnSeries<double>
                    {
                    Values = messageCounts,
                    Name = "Üzenetek",
                    DataLabelsSize = 12
                    }
                };

        }
    }
}
