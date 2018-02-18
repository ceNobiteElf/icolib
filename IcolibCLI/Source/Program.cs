using System;
using System.Collections.Generic;
using System.IO;

namespace Icolib.CLI
{
    class Program
    {
        #region Constants
        private const string GenerateTemplates = "gen_templates";
        private const string Quit = "quit";
        #endregion


        #region Entry Point
        static void Main(string[] args)
        {
            string[] input;
            string command;

            while (true)
            {
                Menu();
                input = Console.ReadLine().Split(' ');

                command = input[0].ToLowerInvariant();

                if (command == Quit)
                {
                    break;
                }

                switch (command)
                {
                    case GenerateTemplates:
                        GenerateDefaultTemplates();
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
                Items = new List<ExportTemplate.Item>() {
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

            var androidTemplate = new ExportTemplate(Path.Combine("Templates", "android.xml")) {
                Name = "Android",
                OuputDirectory = "Android",
                FallbackNamingPattern = Path.Combine("drawable", "icon"),
                Items = new List<ExportTemplate.Item>() {
                    new ExportTemplate.Item(72),
                    new ExportTemplate.Item(72, Path.Combine("drawable-hdpi", "icon")),
                    new ExportTemplate.Item(48, Path.Combine("drawable-mdpi", "icon")),
                    new ExportTemplate.Item(96, Path.Combine("drawable-xhdpi", "icon")),
                    new ExportTemplate.Item(144, Path.Combine("drawable-xxhdpi", "icon")),
                    new ExportTemplate.Item(192, Path.Combine("drawable-xxxhdpi", "icon"))
                }
            };
        }
        #endregion


        #region Helper Functions - Console Outputs
        static void Menu()
        {
            Console.WriteLine("Please enter a command");
            Console.WriteLine($" {GenerateTemplates}");
            Console.WriteLine($" {Quit}");
            Console.WriteLine();
        }
        #endregion
    }
}
