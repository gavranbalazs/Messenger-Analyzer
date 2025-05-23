using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using test.HelperClasses;
using test.Helpers;
using test.ImportFromZip;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace test;

public sealed partial class ImportFromZipWindow : Window
{
    public ImportFromZipViewModel ViewModel { get; } = new();

    public ImportFromZipWindow()
    {
        InitializeComponent();
        WindowHelper.CenterWindow(this);
        AnalyzerSpinner.IsActive = false;
    }

    private void ImportSelected_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void AddZipFiles_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.AddZipFiles(this);
    }

    private async void AnalyzeZips_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.zipFiles.Any() == false)
        {
            var dialog = new ContentDialog
            {
                Title = "Hiba",
                Content = "Nincs kiválasztott zip fájl. Az analizáláshoz adj hozzá legalább egyet! ",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();
            return;
        }


        await ActivateSpinnerAsync(AnalyzerSpinner);


        try
        {
            await ViewModel.AnalyzeZipsAsync(this);
        }
        catch (Exception ex)
        {
            // Pl. hibaüzenet megjelenítése
            var dialog = new ContentDialog
            {
                Title = "Hiba",
                Content = ex.Message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
        finally
        {
            AnalyzerSpinner.IsActive = false;
            AnalyzerSpinner.IsIndeterminate = false;
            AnalyzerSpinner.Visibility = Visibility.Collapsed;
        }
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel.UpdateFilteredCandidates(sender.Text);
        }
    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is ConversationCandidate candidate)
        {
            candidate.IsSelected = true;
            //notify the view model about the change
            ViewModel.UpdateSelectedCandidate(candidate);
            ViewModel.UpdateFilteredCandidates(sender.Text);
        }
    }

    private async Task ActivateSpinnerAsync(ProgressRing ring)
    {
        ring.IsIndeterminate = false;
        ring.IsActive = false;
        ring.Visibility = Visibility.Visible;
        await Task.Delay(100);
        ring.IsIndeterminate = true;
        ring.IsActive = true;
    }

    private void Grid_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Link;

        e.DragUIOverride.Caption = "ZIP fájl(ok) hozzáadása";
        var color = new SolidColorBrush(Colors.Gray);
        color.Opacity = 0.5;
        MainGrid.Background = color;
    }

    private void Grid_Drop(object sender, DragEventArgs e)
    {
        MainGrid.Background = new SolidColorBrush(Colors.Transparent);

        ViewModel.AddZipfilesFromDrop(e);
    }

    private void Grid_DragLeave(object sender, DragEventArgs e)
    {
        MainGrid.Background = new SolidColorBrush(Colors.Transparent);
    }

    private void WindowClosed(object sender, WindowEventArgs args)
    {
        ViewModel.CleanTempFolders();
    }
}
