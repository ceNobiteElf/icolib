using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

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
					resizedImage.Dispose();
                }
            }
        }

		public void InvertAndExportIcons(string imagePath, string exportDirectory)
		{
			string inverseOutputPath = Path.ChangeExtension(Path.Combine(new FileInfo(imagePath).DirectoryName, "Inverse"), Path.GetExtension(imagePath));
			TransformImage(imagePath, inverseOutputPath, c => Color.FromArgb(0xFF - c.A, 0xFF - c.R, 0xFF - c.G, 0xFF - c.B));

			//ExportIcons(inverseOutputPath, exportDirectory);
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

		public void TransformImage(string imagePath, string outputPath, Func<Color, Color> transform)
		{
			var bitmap = (Bitmap)Image.FromFile(imagePath);

			Size s = bitmap.Size;
			PixelFormat format = bitmap.PixelFormat;

			byte bytesPerPixel = (byte)(format == PixelFormat.Format32bppArgb ? 4 : 3);

			Rectangle rect = new Rectangle(Point.Empty, s);
			BitmapData imageData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, format);

			int dataSize = imageData.Stride * imageData.Height;
			byte[] data = new byte[dataSize];

			Marshal.Copy(imageData.Scan0, data, 0, dataSize);

			for (int y = 0; y < s.Height; y++)
			{
				for (int x = 0; x < s.Width; x++)
				{
					int index = y * imageData.Stride + x * bytesPerPixel;

					Color c = Color.FromArgb(bytesPerPixel == 4 ? data[index + 3] : 255,
										  		data[index + 2], data[index + 1],
					                         	data[index]);

					c = transform(c);

					data[index + 0] = c.B;
					data[index + 1] = c.G;
					data[index + 2] = c.R;

					if (bytesPerPixel == 4)
					{
						data[index + 3] = c.A;
					}
				}
			}

			Marshal.Copy(data, 0, imageData.Scan0, dataSize);
			bitmap.UnlockBits(imageData);

			bitmap.Save(outputPath);
			bitmap.Dispose();
		}
        #endregion
    }
}
