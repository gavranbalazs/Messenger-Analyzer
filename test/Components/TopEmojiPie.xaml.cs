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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace test.Components
{
    public sealed partial class TopEmojiPie : UserControl
    {
        public TopEmojiPie()
        {
            InitializeComponent();
        }

        public void SetupComponent(Root messages)
        {
            var vm = new TopEmojisViewModel();
            this.DataContext = vm;
        }
    }
}
