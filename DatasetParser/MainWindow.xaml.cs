using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Xml.Serialization;
using System.Xml;
using Xml2CSharp;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using DiffPlex;
using DiffPlex.DiffBuilder.Model;
using DiffPlex.Model;
using DiffPlex.Chunkers;
using DiffPlex.Wpf.Controls;

namespace DatasetParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void loadFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Load the XML file into an XmlDocument object
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(openFileDialog.FileName);

                    // Deserialize the XML document into a Response object using an XmlSerializer
                    XmlSerializer serializer = new XmlSerializer(typeof(MESSAGE));
                    MESSAGE? response;
                    using (var reader = new StringReader(xmlDoc.InnerXml))
                    {
                        response = serializer.Deserialize(reader) as MESSAGE;
                    }

                    // Display the binary data in the TextBox
                    binaryDataTextBox.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.Text.Replace("0x", "").Replace(","," ");
                    sw_name.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.ZDC_NAME;
                    sw_version.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.ZDC_VERSION;

                    string[] hexValuesSplit = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.Text.Split(',');
                    byte[] datasetBytes = new byte[hexValuesSplit.Length];

    

                    for (int i = 0; i < hexValuesSplit.Length; i++)
                    {
                        datasetBytes[i] = Convert.ToByte(hexValuesSplit[i], 16);
                    }

                    int newLength = datasetBytes.Length - 4;
                    byte[] dataForCRC = new byte[newLength];
                    Array.Copy(datasetBytes, 0, dataForCRC, 0, newLength);

                    crc32calculation.Text = CRC32Reversed.Compute(dataForCRC).ToString("X");

                    // Set the file path text
                    filePathTextBlock.Text = openFileDialog.SafeFileName;
                    this.DataContext = this;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void loadFileButton2_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Load the XML file into an XmlDocument object
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(openFileDialog.FileName);

                    // Deserialize the XML document into a Response object using an XmlSerializer
                    XmlSerializer serializer = new XmlSerializer(typeof(MESSAGE));
                    MESSAGE? response;
                    using (var reader = new StringReader(xmlDoc.InnerXml))
                    {
                        response = serializer.Deserialize(reader) as MESSAGE;
                    }
                    binaryDataTextBox2.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.Text.Replace("0x", "").Replace(",", " ");
                    string[] hexValuesSplit = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.Text.Split(',');
                    byte[] datasetBytes = new byte[hexValuesSplit.Length];

 

                    for (int i = 0; i < hexValuesSplit.Length; i++)
                    {
                        datasetBytes[i] = Convert.ToByte(hexValuesSplit[i], 16);
                    }

                    // Set the file path text
                    filePathTextBlock2.Text = openFileDialog.SafeFileName;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private string AddLinesToResult(string input)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                sb.Append(input[i]);

                if ((i + 1) % 100 == 0)
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private void compareFilesButton_Click(object sender, RoutedEventArgs e)
        {
            DiffView.OldText = AddLinesToResult(binaryDataTextBox.Text);
            DiffView.NewText = AddLinesToResult(binaryDataTextBox2.Text);
            DiffView.IgnoreUnchanged = true;
        }

        private void saveFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a SaveFileDialog object
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All files (*.*)|*.*"; // Set the file type filter
            if (saveFileDialog.ShowDialog() == true) // Show the dialog and check if a file is selected
            {
                // Write the content of the binaryDataTextBox to the selected file
                File.WriteAllText(saveFileDialog.FileName, binaryDataTextBox.Text);
            }
        }
        private void binaryDataTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int selectedByteIndex = binaryDataTextBox.SelectionStart / 3;
            selectedByteTextBlock.Text = $"Selected byte index: {selectedByteIndex}";
        }
        private void binaryDataTextBox2_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int selectedByteIndex = binaryDataTextBox2.SelectionStart / 3;
            selectedByteTextBlock2.Text = $"Selected byte index: {selectedByteIndex}";
        }
    }
}
