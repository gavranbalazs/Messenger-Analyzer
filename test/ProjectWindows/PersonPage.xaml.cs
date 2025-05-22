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

namespace test
{
   
    public sealed partial class PersonPage : Page
    {
        Root messages { get; set; }

        List<Message> personMessages { get; set; }

        public PersonPage()
        {
            InitializeComponent();
            PersonName.Text = "Default Name"; // Set a default name or handle it in the OnNavigatedTo method
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            dynamic data = e.Parameter;
            List<PersonStats> stats = data.Sats;
            Root msgs = data.Messages;
            string personName = data.PersonName;

            PersonName.Text = personName;

            personMessages = msgs.messages.Where(x => x.sender_name == personName).ToList();

        }
    }
}
