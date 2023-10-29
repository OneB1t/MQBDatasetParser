using System.IO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DatasetParser
{
    /// <summary>
    /// Interakční logika pro HexEditor.xaml
    /// </summary>
    public partial class HexEditor : Window
    {

        public HexEditor(string tempFilePath)
        {
            InitializeComponent();
            // Create an instance of the view model
            MyViewModel viewModel = new MyViewModel();
            viewModel.FileNamePath = tempFilePath;

            // Set the data context of the window
            this.DataContext = viewModel;
        }
    }
}
