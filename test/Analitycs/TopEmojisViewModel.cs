using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test.Analitycs
{
    public partial class TopEmojisViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial IEnumerable<ISeries> EmojiSeries { get; set; }

        public TopEmojisViewModel()
        {
            // Dummy data, helyettesítsd saját feldolgozással
            int _index = 0;
            string[] _names = ["Maria", "Susan", "Charles", "Fiona", "George"];

            EmojiSeries = new[] { 8, 6, 5, 3, 3 }.AsPieSeries((value, series) =>
             {
                 series.Name = _names[_index++ % _names.Length];
                 series.DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer;
                 series.DataLabelsSize = 15;
                 series.DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30));
                 series.DataLabelsFormatter =
                    point =>
                        $"This slide takes {point.Coordinate.PrimaryValue} " +
                        $"out of {point.StackedValue!.Total} parts";
                 series.ToolTipLabelFormatter = point => $"{point.StackedValue!.Share:P2}";
             });


        }
    }
}
