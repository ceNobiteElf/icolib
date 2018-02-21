using System.Drawing;
using System.IO;

namespace Icolib
{
    public class IconExporter
    {
        #region Properties
        public ExportTemplate ExportTemplate { get; set; }
        #endregion


        #region Constructors
        public IconExporter(ExportTemplate exportTemplate)
        {
            ExportTemplate = exportTemplate;
        }
        #endregion


        #region Public API
        public void ExportIcons(string imagePath, string exportDirectory)
        {
            string extension = Path.GetExtension(imagePath);

            using (var image = Image.FromFile(imagePath))
            {
                foreach (var item in ExportTemplate)
                {
                    string outputPath = Path.ChangeExtension(Path.Combine(exportDirectory, ExportTemplate.OuputDirectory, ExportTemplate.GetItemName(item)), extension);
                    ImageProcessor.Process(image, outputPath, source => ImageProcessor.Resize(source, item.Width, item.Height, false));
                }
            }
        }
        #endregion
    }
}
