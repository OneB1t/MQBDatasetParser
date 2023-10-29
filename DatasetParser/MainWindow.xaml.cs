using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using System.Xml;
using DatasetXml;
using System.Text;
using System.Linq;
using System.Data;
using DatasetXmlGerman;
using System.Text.RegularExpressions;
using System.Buffers.Binary;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Window = System.Windows.Window;

namespace DatasetParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XmlDocument xmlDoc;
        MESSAGE? response = new MESSAGE();
        SWCNT? germanResponse = new SWCNT();
        byte[] datasetBytes;
        string tempFilePath;
        XmlSerializer serializer = new XmlSerializer(typeof(MESSAGE));
        XmlSerializer germanXmlForSpecificUnitSerializer = new XmlSerializer(typeof(SWCNT));
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
                    LoadXmlFileToObject(openFileDialog);

                    if(response?.RESULT != null)
                    { 
                        // Display the binary data in the TextBox
                        binaryDataTextBox.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.Text.Replace("\n","").Replace("0x", "").Replace(",", " ").ToUpperInvariant();
                        binaryDataTextBox.Text = Regex.Replace(binaryDataTextBox.Text, @"\s+", " ");
                        if (binaryDataTextBox.Text.Substring(0, 1) == " ")
                            binaryDataTextBox.Text = binaryDataTextBox.Text.Remove(0, 1);

                        start_address.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.START_ADDRESS;
                        diagnostics_address.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.DIAGNOSTIC_ADDRESS;
                        sw_name.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.ZDC_NAME;
                        sw_version.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.ZDC_VERSION;

                    }
                    else if(germanResponse?.IDENT != null) // this is for 6D datasets
                    {
                        string text = Regex.Replace(germanResponse?.DATENBEREICHE.DATENBEREICH.DATEN, ".{2}", "$0 ");
                        binaryDataTextBox.Text = text.Substring(0, text.Length - 1); // cut last characters
                        start_address.Text = germanResponse?.DATENBEREICHE.DATENBEREICH.STARTADR;
                        diagnostics_address.Text = germanResponse?.IDENT.CNTDATEI;
                        sw_name.Text = germanResponse?.DATENBEREICHE.DATENBEREICH.DATENNAME;
                        sw_version.Text = germanResponse?.IDENT.CNTVERSIONINHALT;
                    }
                    string[] hexValuesSplit = binaryDataTextBox.Text
                        .Replace("\\n", "")
                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();



                    datasetBytes = new byte[hexValuesSplit.Length];



                    for (int i = 0; i < hexValuesSplit.Length; i++)
                    {
                        datasetBytes[i] = Convert.ToByte(hexValuesSplit[i], 16);
                    }

                    // Create a temporary file
                    tempFilePath = Path.GetTempFileName();

                    // Write the byte[] data to the temporary file
                    File.WriteAllBytes(tempFilePath, datasetBytes);

                    CalculateCRC();

                    // Set the file path text
                    filePathTextBlock.Text = openFileDialog.SafeFileName;
                    GetDatasetVersion();
                    GetVehicleType();
                    if (diagnostics_address.Equals("A5"))
                    { 
                        // here parse data from dataset into fields
                        GetTrafficJamAssistEnabled();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                if (!String.IsNullOrEmpty(binaryDataTextBox2.Text))
                    compareFilesButton_Click(sender, e);
            }
        }

        private void GetDatasetVersion()
        {
            byte majorVersion = datasetBytes[2];
            byte minorVersion = datasetBytes[3];
            dataset_version.Text = "Dataset version: " + "Major - " + Convert.ToByte(majorVersion).ToString("x2") + " Minor - " + Convert.ToByte(minorVersion).ToString("x2");
        }

        private void LoadXmlFileToObject(OpenFileDialog openFileDialog)
        {
            // Load the XML file into an XmlDocument object
            xmlDoc = new XmlDocument();
            xmlDoc.Load(openFileDialog.FileName);
            try
            {
                using (var reader = new StringReader(xmlDoc.InnerXml))
                {
                    response = serializer.Deserialize(reader) as MESSAGE;
                }
            }
            catch(Exception e) {

            }
            if(response?.RESULT == null) // if we failed to parse it try german version of xml
            { 
                using (var reader = new StringReader(xmlDoc.InnerXml))
                {
                    SWCNT? sWCNT = germanXmlForSpecificUnitSerializer.Deserialize(reader) as SWCNT;
                    germanResponse = sWCNT;
                }
            }
        }

        private byte [] CalculateCRC()
        {
            byte[] result = { };
            if (datasetBytes == null)
                return result;
            if(diagnostics_address.Text.Contains("6D"))
            {
                result = calculateCRC32(false);
            }
            else if (datasetBytes.Length > 10000 )
            {
                result = calculateCRC32(true);
            }
            else
            {
                result = calculateCRC16();
            }
            return result;
        }

        private byte[] calculateCRC16()
        {
            byte[] result;
            int newLength = datasetBytes.Length - 2;
            byte[] dataForCRC = new byte[newLength];
            Array.Copy(datasetBytes, 0, dataForCRC, 0, newLength);


            byte[] bytes = CRC16.ComputeHash(dataForCRC);
            Array.Reverse(bytes, 0, bytes.Length);
            crc32calculation.Text = "CRC16/ARC-REVERSED: " + BitConverter.ToString(bytes).Replace("-", " ");
            result = bytes;
            return result;
        }

        private byte[] calculateCRC32(Boolean reverse)
        {
            byte[] result;
            int newLength = datasetBytes.Length - 4;
            byte[] dataForCRC = new byte[newLength];
            Array.Copy(datasetBytes, 0, dataForCRC, 0, newLength);

            CRC32 crc = new CRC32();
            byte[] bytes = crc.ComputeHash(dataForCRC);
            if(reverse)
            { 
                Array.Reverse(bytes, 0, bytes.Length);
                crc32calculation.Text = "CRC32-REVERSED: " + BitConverter.ToString(bytes).Replace("-", " ");
            }
            else
            { 
                crc32calculation.Text = "CRC32: " + BitConverter.ToString(bytes).Replace("-", " ");
            }
            result = bytes;
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
                    LoadXmlFileToObject(openFileDialog);

                    if (response?.RESULT != null)
                    {
                        // Display the binary data in the TextBox
                        // Display the binary data in the TextBox
                        binaryDataTextBox2.Text = response?.RESULT.RESPONSE.DATA.PARAMETER_DATA.Text.Replace("\n", "").Replace("0x", "").Replace(",", " ").ToUpperInvariant();
                        binaryDataTextBox2.Text = Regex.Replace(binaryDataTextBox2.Text, @"\s+", " ");
                        if (binaryDataTextBox2.Text.Substring(0, 1) == " ")
                            binaryDataTextBox2.Text = binaryDataTextBox2.Text.Remove(0, 1);


                    }
                    else if (germanResponse?.IDENT != null) // this is for 6D datasets
                    {
                        string text = Regex.Replace(germanResponse?.DATENBEREICHE.DATENBEREICH.DATEN, ".{2}", "$0 ");
                        binaryDataTextBox2.Text = text.Substring(0, text.Length - 1); // cut last characters
                    }


                    string[] hexValuesSplit = binaryDataTextBox2.Text
                        .Replace("\\n", "")
                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();
                    datasetBytes = new byte[hexValuesSplit.Length];



                    for (int i = 0; i < hexValuesSplit.Length; i++)
                    {
                        datasetBytes[i] = Convert.ToByte(hexValuesSplit[i], 16);
                    }

                    filePathTextBlock2.Text = openFileDialog.SafeFileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                if(!String.IsNullOrEmpty(binaryDataTextBox.Text))
                    compareFilesButton_Click(sender, e);
            }
        }

        private string GetVehicleType()
        {
            string result = "";
            short shortValue = BitConverter.ToInt16(datasetBytes, 0);
            short reversedValue = BinaryPrimitives.ReverseEndianness(shortValue);
            result = GetResult(shortValue);
            if (result == "NO IDEA")
                return GetResult(reversedValue);
            else
                return result;
        }

        private string GetResult(short shortValue)
        {
            switch (shortValue)
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
                case 12338:
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
                case 12853:
                    vehicle_type.Text = "Audi TT";
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
            if(datasetBytes.Length > 13748) // old datasets are short...
            {
                traffic_jam_assist_enabled.Text = "Traffic Jam assist (WIP) only new unit: " + BitConverter.ToBoolean(datasetBytes, 13748);
            }
            else
            {
                traffic_jam_assist_enabled.Text = "Traffic Jam assist not supported for this unit";
            }
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
            //DiffView.IgnoreUnchanged = true;
            DiffView.SideBySideModeToggleTitle = true;
            DiffView.OldTextHeader = filePathTextBlock.Text;
            DiffView.NewTextHeader = filePathTextBlock2.Text;
            DiffView.OldText = AddLinesToResult(binaryDataTextBox.Text);
            DiffView.NewText = AddLinesToResult(binaryDataTextBox2.Text);
        }

        private void saveFileButton_Click(object sender, RoutedEventArgs e)
        {
            // replace new CRC in result
            if (datasetBytes != null && datasetBytes.Length > 10000)
                Array.Copy(CalculateCRC(), 0, datasetBytes, datasetBytes.Length - 4, 4);
            else
                Array.Copy(CalculateCRC(), 0, datasetBytes, datasetBytes.Length - 2, 2);
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

        public static string EndianFromString(string input)
        {
            ulong littleEndianResult = 0;
            string binaryValue = "";
            try
            {
                // Remove any leading/trailing whitespace from the input string
                input = input.Trim();

                // Split the input string into individual hexadecimal values
                string[] hexValues = input.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                byte[] bytes = new byte[hexValues.Length];
                for (int i = 0; i < hexValues.Length; i++)
                {
                    bytes[i] = Convert.ToByte(hexValues[i], 16);
                }



                for (int i = 0; i < bytes.Length; i++)
                {
                    littleEndianResult += ((ulong)bytes[i] << (i * 8));
                    string binaryString = Convert.ToString(bytes[i], 2).PadLeft(8, '0'); // convert to binary string and pad to 8 characters
                    binaryValue += binaryString;
                }

                byte[] bigEndianBytes = BitConverter.GetBytes(littleEndianResult);
                Array.Reverse(bigEndianBytes);
                ulong bigEndianResult = BitConverter.ToUInt16(bigEndianBytes, 0);
                return "LITTLE-ENDIAN DECIMAL: " + littleEndianResult.ToString() + "\nBINARY Value: " + binaryValue.ToString();
            }
            catch { }
            return "";
        }

        private void binaryDataTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int selectedByteIndex = binaryDataTextBox.SelectionStart / 3;
            selectedByteTextBlock.Text = $"Selected byte index: {selectedByteIndex}";
            littleEndianValue.Text = EndianFromString(binaryDataTextBox.Text.Substring(binaryDataTextBox.SelectionStart, binaryDataTextBox.SelectionLength));
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

        private void openHexEditor_Click(object sender, RoutedEventArgs e)
        {
            HexEditor win2 = new HexEditor(tempFilePath);
            
            win2.Show();
        }
    }
}
