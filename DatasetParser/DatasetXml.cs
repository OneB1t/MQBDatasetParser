/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace DatasetXml
{
    [XmlRoot(ElementName = "PARAMETER_DATA")]
    public class PARAMETER_DATA
    {
        [XmlAttribute(AttributeName = "DIAGNOSTIC_ADDRESS")]
        public string DIAGNOSTIC_ADDRESS { get; set; }
        [XmlAttribute(AttributeName = "START_ADDRESS")]
        public string START_ADDRESS { get; set; }
        [XmlAttribute(AttributeName = "PR_IDX")]
        public string PR_IDX { get; set; }
        [XmlAttribute(AttributeName = "ZDC_NAME")]
        public string ZDC_NAME { get; set; }
        [XmlAttribute(AttributeName = "ZDC_VERSION")]
        public string ZDC_VERSION { get; set; }
        [XmlAttribute(AttributeName = "LOGIN")]
        public string LOGIN { get; set; }
        [XmlAttribute(AttributeName = "LOGIN_IND")]
        public string LOGIN_IND { get; set; }
        [XmlAttribute(AttributeName = "DSD_TYPE")]
        public string DSD_TYPE { get; set; }
        [XmlAttribute(AttributeName = "SESSIONNAME")]
        public string SESSIONNAME { get; set; }
        [XmlAttribute(AttributeName = "FILENAME")]
        public string FILENAME { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "COMPOUND")]
    public class COMPOUND
    {
        [XmlElement(ElementName = "SW_NAME")]
        public string SW_NAME { get; set; }
        [XmlElement(ElementName = "SW_VERSION")]
        public string SW_VERSION { get; set; }
        [XmlElement(ElementName = "SW_PART_NO")]
        public string SW_PART_NO { get; set; }
        [XmlAttribute(AttributeName = "COMPOUND_ID")]
        public string COMPOUND_ID { get; set; }
    }

    [XmlRoot(ElementName = "COMPOUNDS")]
    public class COMPOUNDS
    {
        [XmlElement(ElementName = "COMPOUND")]
        public List<COMPOUND> COMPOUND { get; set; }
    }

    [XmlRoot(ElementName = "INFORMATION")]
    public class INFORMATION
    {
        [XmlElement(ElementName = "CODE")]
        public string CODE { get; set; }
    }

    [XmlRoot(ElementName = "COMPRESSED_DATA")]
    public class COMPRESSED_DATA
    {
        [XmlAttribute(AttributeName = "CONTENT")]
        public string CONTENT { get; set; }
        [XmlAttribute(AttributeName = "CONTENT_TYPE")]
        public string CONTENT_TYPE { get; set; }
        [XmlAttribute(AttributeName = "CONTENT_TRANSFER_ENCODING")]
        public string CONTENT_TRANSFER_ENCODING { get; set; }
        [XmlAttribute(AttributeName = "BYTES_UNCOMPRESSED")]
        public string BYTES_UNCOMPRESSED { get; set; }
        [XmlAttribute(AttributeName = "BYTES_COMPRESSED")]
        public string BYTES_COMPRESSED { get; set; }
    }

    [XmlRoot(ElementName = "DSD_DATA")]
    public class DSD_DATA
    {
        [XmlElement(ElementName = "COMPRESSED_DATA")]
        public COMPRESSED_DATA COMPRESSED_DATA { get; set; }
    }

    [XmlRoot(ElementName = "DATA")]
    public class DATA
    {
        [XmlElement(ElementName = "REQUEST_ID")]
        public string REQUEST_ID { get; set; }
        [XmlElement(ElementName = "PARAMETER_DATA")]
        public PARAMETER_DATA PARAMETER_DATA { get; set; }
        [XmlElement(ElementName = "COMPOUNDS")]
        public COMPOUNDS COMPOUNDS { get; set; }
        [XmlElement(ElementName = "INFORMATION")]
        public INFORMATION INFORMATION { get; set; }
        [XmlElement(ElementName = "DSD_DATA")]
        public DSD_DATA DSD_DATA { get; set; }
    }

    [XmlRoot(ElementName = "RESPONSE")]
    public class RESPONSE
    {
        [XmlElement(ElementName = "DATA")]
        public DATA DATA { get; set; }
        [XmlAttribute(AttributeName = "NAME")]
        public string NAME { get; set; }
        [XmlAttribute(AttributeName = "DTD")]
        public string DTD { get; set; }
        [XmlAttribute(AttributeName = "VERSION")]
        public string VERSION { get; set; }
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "RESULT")]
    public class RESULT
    {
        [XmlElement(ElementName = "RESPONSE")]
        public RESPONSE RESPONSE { get; set; }
    }

    [XmlRoot(ElementName = "MESSAGE")]
    public class MESSAGE
    {
        [XmlElement(ElementName = "RESULT")]
        public RESULT RESULT { get; set; }
        [XmlAttribute(AttributeName = "DTD")]
        public string DTD { get; set; }
        [XmlAttribute(AttributeName = "VERSION")]
        public string VERSION { get; set; }
    }

}
