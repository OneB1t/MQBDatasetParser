using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.Xml;
using Xml2CSharp;
using System.Text;
using System.Linq;

namespace DatasetParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XmlDocument xmlDoc;
        MESSAGE? response = new MESSAGE();
        byte[] datasetBytes;
        XmlSerializer serializer = new XmlSerializer(typeof(MESSAGE));
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
                    xmlDoc = new XmlDocument();
                    xmlDoc.Load(openFileDialog.FileName);                   
                    
                    using (var reader = new StringReader(xmlDoc.InnerXml))
                    {
                        response = serializer.Deserialize(reader) as MESSAGE;
                    }

                    // Display the binary data in the TextBox
                    binaryDataTextBox.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.Text.Replace("0x", "").Replace(",", " ");
                    start_address.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.START_ADDRESS;
                    diagnostics_address.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.DIAGNOSTIC_ADDRESS;
                    sw_name.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.ZDC_NAME;
                    sw_version.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.ZDC_VERSION;

                    string[] hexValuesSplit = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.Text.Split(',');
                    datasetBytes = new byte[hexValuesSplit.Length];



                    for (int i = 0; i < hexValuesSplit.Length; i++)
                    {
                        datasetBytes[i] = Convert.ToByte(hexValuesSplit[i], 16);
                    }

                    CalculateCRC();

                    // Set the file path text
                    filePathTextBlock.Text = openFileDialog.SafeFileName;

                    // here parse data from dataset into fields
                    GetVehicleType();
                    GetTrafficJamAssistEnabled();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private byte [] CalculateCRC()
        {
            byte[] result = { };
            if (datasetBytes != null)
            {
                int newLength = datasetBytes.Length - 4;
                byte[] dataForCRC = new byte[newLength];
                Array.Copy(datasetBytes, 0, dataForCRC, 0, newLength);

                CRC32 crc = new CRC32();
                byte[] bytes = crc.ComputeHash(dataForCRC);
                Array.Reverse(bytes, 0, bytes.Length);
                crc32calculation.Text = BitConverter.ToString(bytes).Replace("-", " ");
                result = bytes;
            }
            return result;
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

                    // Set the file path text
                    filePathTextBlock2.Text = openFileDialog.SafeFileName;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                compareFilesButton_Click(sender, e);
            }
        }

        private string GetVehicleType()
        {
            short shortValue = BitConverter.ToInt16(datasetBytes, 0);
            switch(shortValue)
            {
                case 12593:
                    vehicle_type.Text = "Passat Limo GTE";
                    break;
                case 14384:
                    vehicle_type.Text = "Passat Limo";
                    break;
                case 12337:
                    vehicle_type.Text = "Passat Combi XC/HC / Allroad";
                    break;
                case 12849:
                    vehicle_type.Text = "Passat Combi GTE / Variant";
                    break;
                case 12851:
                    vehicle_type.Text = "A3 hatchback";
                    break;
                case 14640:
                    vehicle_type.Text = "Passat Combi GTE";
                    break;
                case 12850:
                    vehicle_type.Text = "Atlas";
                    break;
                case 13363:
                    vehicle_type.Text = "A3 limo";
                    break;
                case 13875:
                    vehicle_type.Text = "A3 sportsback";
                    break;
                case 14386:
                    vehicle_type.Text = "VW Arteon";
                    break;
                case 14387:
                    vehicle_type.Text = "Seat Ateca";
                    break;
                case 16689:
                    vehicle_type.Text = "VW Golf Hatchback";
                    break;
                case 20529:
                    vehicle_type.Text = "VW Golf Hatchback A8T / R";
                    break;
                case 17969:
                    vehicle_type.Text = "VW Golf Variant";
                    break;
                case 18225:
                    vehicle_type.Text = "VW Golf Variant A8T / R";
                    break;
                case 14646:
                    vehicle_type.Text = "Škoda Kodiaq";
                    break;
                case 13108:
                    vehicle_type.Text = "Seat Leon 5-door";
                    break;
                case 12854:
                    vehicle_type.Text = "Škoda Octavia hatchback";
                    break;
                case 13366:
                    vehicle_type.Text = "Škoda Octavia hatchback RS";
                    break;
                case 13878:
                    vehicle_type.Text = "Škoda Octavia Scout";
                    break;
                case 13110:
                    vehicle_type.Text = "Škoda Octavia Variant (combi)";
                    break;
                case 13622:
                    vehicle_type.Text = "Škoda Octavia Variant RS (combi)";
                    break;
                case 12598:
                    vehicle_type.Text = "Škoda Superb Combi";
                    break;
                case 12342:
                    vehicle_type.Text = "Škoda Superb Limo";
                    break;
                case 13361:
                    vehicle_type.Text = "VW Tiguan";
                    break;
                case 21553:
                    vehicle_type.Text = "VW Tiguan XL";
                    break;
                case 13617:
                    vehicle_type.Text = "VW Touran";
                    break;
                case 13106:
                    vehicle_type.Text = "VW T-Roc";
                    break;
                case 14649:
                    vehicle_type.Text = "AUDI A3 / Q2";
                    break;
                case 25922:
                    vehicle_type.Text = "VW Caddy";
                    break;
                case 12599:
                    vehicle_type.Text = "Škoda Karoq";
                    break;
                case 12852:
                    vehicle_type.Text = "Seat Leon 3-door";
                    break;
                case 13364:
                    vehicle_type.Text = "Seat Leon ST";
                    break;
                case 16945:
                    vehicle_type.Text = "VW Golf Sportsvan";
                    break;
                default:
                    vehicle_type.Text = "NO IDEA";
                    break;
            }

            return "NO IDEA";
        }
        private void GetTrafficJamAssistEnabled()
        {
            // we need to decide where to look for it based on unit type as on older type offset is 13868
            traffic_jam_assist_enabled.Text = "Traffic Jam assist (WIP) only new unit: " + BitConverter.ToBoolean(datasetBytes, 13748);
        }
        private string AddLinesToResult(string input)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                sb.Append(input[i]);
                if(wordlenghtselector1.IsChecked == true)
                {
                    if ((i + 1) % 3 == 0) // single byte
                    {
                        sb.AppendLine();
                    }
                }
                else if(wordlenghtselector2.IsChecked == true)
                { 
                    if ((i + 1) % 6 == 0) // dual byte
                    {
                        sb.AppendLine();
                    }
                }
                else if(wordlenghtselector4.IsChecked == true)
                {
                    if ((i + 1) % 12 == 0) // quadro byte
                    {
                        sb.AppendLine();
                    }
                }
                else if(wordlenghtselector8.IsChecked == true)
                {
                    if ((i + 1) % 24 == 0) // octa byte
                    {
                        sb.AppendLine();
                    }
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
            // replace new CRC in result
            Array.Copy(CalculateCRC(), 0, datasetBytes, datasetBytes.Length - 4, 4);
            response.RESULT.RESPONSE.DATA.PARAMETER_DATA.Text = string.Join(",", datasetBytes.Select(b => $"0x{b:X2}"));
            binaryDataTextBox.Text = response.RESULT.RESPONSE.DATA.PARAMETER_DATA.Text.Replace("0x", "").Replace(",", " "); ;

            // Create a SaveFileDialog object
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML Files (*.xml)|*.xml"; // Set the file type filter
            if (saveFileDialog.ShowDialog() == true) // Show the dialog and check if a file is selected
            {
                WriteResponseToXml(response, saveFileDialog.FileName);
            }
        }
        private void WriteResponseToXml(MESSAGE? response, string filename)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };

            using (var writer = XmlWriter.Create(filename, settings))
            {
                serializer.Serialize(writer, response);
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

        private void binaryDataTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string[] byteStrings = binaryDataTextBox.Text.Split(' ');

            byte[] bytes = new byte[byteStrings.Length];
            try
            {
                for (int i = 0; i < byteStrings.Length; i++)
                {
                    bytes[i] = byte.Parse(byteStrings[i], System.Globalization.NumberStyles.HexNumber);
                }
                datasetBytes = bytes;
                CalculateCRC();
            }
            catch(Exception ex) { 
            // do nothing for invalid data
            }
        }
    }
}
