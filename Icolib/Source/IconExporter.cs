using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
                    string savePath = Path.ChangeExtension(Path.Combine(exportDirectory, ExportTemplate.OuputDirectory, ExportTemplate.GetItemName(item)), extension);
                    
                    new FileInfo(savePath).Directory.Create();

                    Bitmap resizedImage = ResizeImage(image, item.Width, item.Height, false);
                    resizedImage.Save(savePath);
                }
            }
        }
        #endregion


        #region Helper Functions
        public Bitmap ResizeImage(Image image, int width, int height, bool maintainAspect = false)
        {
            if (maintainAspect)
            {
                double aspectRatio = image.Width / image.Height;
                double resizeRatio = width / height;

                double scaleFactor = (resizeRatio > aspectRatio) ? height / image.Height : width / image.Width;

                width = (int)(image.Width * scaleFactor);
                height = (int)(image.Height * scaleFactor);
            }

            var result = new Bitmap(width, height);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);

                    var rect = new Rectangle(0, 0, width, height);
                    graphics.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return result;
        }
        #endregion
    }
}
