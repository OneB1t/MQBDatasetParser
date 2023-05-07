/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace DatasetXmlGerman
{
    [XmlRoot(ElementName = "IDENT")]
    public class IDENT
    {
        [XmlElement(ElementName = "CNT-DATEI")]
        public string CNTDATEI { get; set; }
        [XmlElement(ElementName = "CNT-VERSION-INHALT")]
        public string CNTVERSIONINHALT { get; set; }
        [XmlElement(ElementName = "CNT-VERSION-DATUM")]
        public string CNTVERSIONDATUM { get; set; }
    }

    [XmlRoot(ElementName = "DATENBEREICH")]
    public class DATENBEREICH
    {
        [XmlElement(ElementName = "DATEN-NAME")]
        public string DATENNAME { get; set; }
        [XmlElement(ElementName = "DATEN-FORMAT-NAME")]
        public string DATENFORMATNAME { get; set; }
        [XmlElement(ElementName = "START-ADR")]
        public string STARTADR { get; set; }
        [XmlElement(ElementName = "GROESSE-DEKOMPRIMIERT")]
        public string GROESSEDEKOMPRIMIERT { get; set; }
        [XmlElement(ElementName = "DATEN")]
        public string DATEN { get; set; }
    }

    [XmlRoot(ElementName = "DATENBEREICHE")]
    public class DATENBEREICHE
    {
        [XmlElement(ElementName = "DATENBEREICH")]
        public DATENBEREICH DATENBEREICH { get; set; }
    }

    [XmlRoot(ElementName = "SW-CNT")]
    public class SWCNT
    {
        [XmlElement(ElementName = "IDENT")]
        public IDENT IDENT { get; set; }
        [XmlElement(ElementName = "DATENBEREICHE")]
        public DATENBEREICHE DATENBEREICHE { get; set; }
    }

}
