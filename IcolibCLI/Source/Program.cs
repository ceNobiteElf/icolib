using System;
using System.Collections.Generic;
using System.IO;

namespace Icolib.CLI
{
    class Program
    {
        #region Constants
        private const string GENERATE_TEMPLATES = "gen_templates";
        private const string GENERATE_ICONS = "gen_icons";
		private const string INVERT = "invert";
		private const string INVERT2 = "invert2";
		private const string ADJUST = "adjust";
        private const string ADJUST2 = "adjust2";
        private const string QUIT = "quit";
        #endregion


        #region Entry Point
        static void Main(string[] args)
        {
            string[] input;
            string command;

            while (true)
            {
                Menu();
                input = Console.ReadLine()?.Split(' ');

                command = input[0].ToLowerInvariant();

                if (command == QUIT)
                {
                    break;
                }

                switch (command)
                {
                    case GENERATE_TEMPLATES:
                        GenerateDefaultTemplates();
                        break;

                    case GENERATE_ICONS:
                        GenerateIconSet(input[1], input[2]);
                        break;

                    case INVERT:
                        Invert(input[1], input[2]);
                        break;

					case INVERT2:
						Invert2(input[1], input[2]);
						break;

					case ADJUST:
						Adjust(input[1], input[2]);
						break;

					case ADJUST2:
						Adjust2(input[1], input[2]);
						break;

                    default:
                        break;
                }
            }
        }
        #endregion


        #region Helper Functions - General
        static void GenerateDefaultTemplates()
        {
            var iosTemplate = new ExportTemplate(Path.Combine("Templates", "ios.xml")) {
                Name = "iOS",
                OuputDirectory = "iOS",
                FallbackNamingPattern = "Icon-%w",
                Items = new List<ExportTemplate.Item> {
                    new ExportTemplate.Item(20, "Icon-20"),
                    new ExportTemplate.Item(40, "Icon-20@2x"),
                    new ExportTemplate.Item(60, "Icon-20@3x"),
                    new ExportTemplate.Item(29, "Icon-29"),
                    new ExportTemplate.Item(58, "Icon-29@2x"),
                    new ExportTemplate.Item(87, "Icon-29@3x"),
                    new ExportTemplate.Item(40, "Icon-40"),
                    new ExportTemplate.Item(80, "Icon-40@2x"),
                    new ExportTemplate.Item(120, "Icon-40@3x"),
                    new ExportTemplate.Item(50, "Icon-50"),
                    new ExportTemplate.Item(100, "Icon-50@2x"),
                    new ExportTemplate.Item(57, "Icon-57"),
                    new ExportTemplate.Item(114, "Icon-57@2x"),
                    new ExportTemplate.Item(120, "Icon-60@2x"),
                    new ExportTemplate.Item(180, "Icon-60@3x"),
                    new ExportTemplate.Item(72, "Icon-72"),
                    new ExportTemplate.Item(144, "Icon-72@2x"),
                    new ExportTemplate.Item(76, "Icon-76"),
                    new ExportTemplate.Item(152, "Icon-76@2x"),
                    new ExportTemplate.Item(167, "Icon-167"),
                    new ExportTemplate.Item(512, "iTunesArtwork"),
                    new ExportTemplate.Item(1024, "iTunesArtwork@2x")
                }
            };

            iosTemplate.Save();

            var androidTemplate = new ExportTemplate(Path.Combine("Templates", "android.xml")) {
                Name = "Android",
                OuputDirectory = "Android",
                FallbackNamingPattern = "drawable/icon",
                Items = new List<ExportTemplate.Item> {
                    new ExportTemplate.Item(72),
                    new ExportTemplate.Item(72, "drawable-hdpi/icon"),
                    new ExportTemplate.Item(48, "drawable-mdpi/icon"),
                    new ExportTemplate.Item(96, "drawable-xhdpi/icon"),
                    new ExportTemplate.Item(144, "drawable-xxhdpi/icon"),
                    new ExportTemplate.Item(192, "drawable-xxxhdpi/icon"),

                }
            };

            androidTemplate.Save();
        }

        static void GenerateIconSet(string sourcePath, string templateName)
        {
            var template = ExportTemplate.LoadFromFile(Path.Combine("Templates", $"{templateName}.xml"));
            var exporter = new IconExporter(template);

            exporter.ExportIcons(sourcePath, "Output");
        }

        static void Invert(string sourcePath, string outputPath)
        {
            ImageProcessor.Process(sourcePath, outputPath, ImageProcessor.Invert);
        }

		static void Invert2(string sourcePath, string outputPath)
		{
			ImageProcessor.Process(sourcePath, outputPath, src => ImageProcessor.Invert(src, ImageProcessor.Channels.R | ImageProcessor.Channels.B));
		}

		static void Adjust(string sourcePath, string outputPath)
		{
			ImageProcessor.Process(sourcePath, outputPath, src => ImageProcessor.Adjust(src, 2));
		}

		static void Adjust2(string sourcePath, string outputPath)
		{
			ImageProcessor.Process(sourcePath, outputPath, src => ImageProcessor.Adjust(src, 0.5));
		}
        #endregion


        #region Helper Functions - Console Outputs
        static void Menu()
        {
            Console.WriteLine("Please enter a command");
            Console.WriteLine($" {GENERATE_TEMPLATES}");
            Console.WriteLine($" {GENERATE_ICONS}");
            Console.WriteLine($" {INVERT}");
            Console.WriteLine($" {QUIT}");
            Console.WriteLine();
        }
        #endregion
    }
}
