﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Icolib
{
    [XmlRoot("template")]
    public class ExportTemplate : IEnumerable<ExportTemplate.Item>
    {
        #region Internal Types
        [XmlType("item")]
        public class Item
        {
            [XmlAttribute("namingPattern")]
            public string NamingPattern { get; set; }

            [XmlElement("Width")]
            public int Width { get; set; }

            [XmlElement("height")]
            public int Height { get; set; }


            public Item(int sideLength = 1, string namingPattern = null)
                : this(sideLength, sideLength, namingPattern) { }

            public Item(int width, int height, string namingPattern = null)
            {
                Width = width;
                Height = height;

                NamingPattern = namingPattern;
            }
        }
        #endregion


        #region Constants
        public const string SchemaNamespace = "http://www.cenobiteelf.com/ns/icolib";
        #endregion


        #region Properties
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("outputDirectory")]
        public string OuputDirectory { get; set; }

        [XmlAttribute("fallbackNamingPattern")]
        public string FallbackNamingPattern { get; set; }

        [XmlArray("items")]
        public IList<Item> Items { get; set; } = new List<Item>();

        [XmlIgnore]
        public string FilePath { get; set; }
        #endregion


        #region Constructors
        public ExportTemplate(string filePath = null)
        {
            FilePath = filePath;
        }
        #endregion


        #region Public API
        public static ExportTemplate LoadFromFile(string templatePath)
        {
            XmlSerializer serialiser = new XmlSerializer(typeof(ExportTemplate), SchemaNamespace);

            using (XmlReader reader = XmlReader.Create(templatePath))
            {
                if (serialiser.CanDeserialize(reader))
                {
                    var template = serialiser.Deserialize(reader, Encoding.UTF8.WebName) as ExportTemplate;
                    template.FilePath = templatePath;
                    return template;
                }
                else
                {
                    throw new SerializationException();
                }
            }
        }

        public static void SaveToFile(string templatePath, ExportTemplate template)
        {
            XmlSerializer serialiser = new XmlSerializer(typeof(ExportTemplate), SchemaNamespace);

            using (XmlWriter writer = XmlWriter.Create(templatePath))
            {
                serialiser.Serialize(writer, template, null, Encoding.UTF8.WebName);
            }
        }

        public void Save()
        {
            if (!string.IsNullOrWhiteSpace(FilePath))
            {
                SaveToFile(FilePath, this);
            }

            throw new Exception("Cannot save template as no valid file path was specified.");
        }

        public string GetItemName(Item item)
        {
            if (item != null)
            {
                string pattern = !string.IsNullOrWhiteSpace(item.NamingPattern) ? item.NamingPattern : FallbackNamingPattern;

                return pattern.Replace("%h", item.Height.ToString()).Replace("%w", item.Width.ToString());
            }

            throw new ArgumentNullException(nameof(item));
        }
        #endregion


        #region Interface Implementation
        public IEnumerator<Item> GetEnumerator()
            => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Items.GetEnumerator();
        #endregion
    }
}
