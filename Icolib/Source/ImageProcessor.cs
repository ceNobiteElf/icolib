using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Icolib
{
	public static class ImageProcessor
	{
		#region Internal Types
		[Flags]
		public enum Channels
		{
			None = 0x0,
			A = 0x1,
			R = 0x2,
			G = 0x4,
			B = 0x8,
			RGB = R | G | B,
			ARGB = A | R | G | B
		}
		#endregion


		#region Public API
		public static void Process(string sourcePath, string outputPath, Func<Image, Bitmap> transform)
		{
			using (var source = Image.FromFile(sourcePath))
			{
				Process(source, Path.ChangeExtension(outputPath, Path.GetExtension(sourcePath)), transform);
			}
		}

		public static void Process(Image source, string outputPath, Func<Image, Bitmap> transform)
		{
			new FileInfo(outputPath).Directory.Create();

			Bitmap processedImage = transform(source);
			processedImage.Save(outputPath);
			processedImage.Dispose();
		}

		//public static Bitmap Brighten()
		public static Bitmap Adjust(Image image, double scalar)
			=> Transform(image, c => Color.FromArgb(c.A, Clamp(scalar * c.R), Clamp(scalar * c.G), Clamp(scalar * c.B)));

		public static Bitmap Adjust(Image image, double scalar, Channels channels)
		{
			int AdjustPixel(int p, Channels channel)
				=> channels.HasFlag(channel) ? Clamp(scalar * p) : p;

			return Transform(image, c => Color.FromArgb(c.A, AdjustPixel(c.R, Channels.R), AdjustPixel(c.R, Channels.G), AdjustPixel(c.R, Channels.B)));
		}

		public static Bitmap Invert(Image image)
			=> Transform(image, c => Color.FromArgb(c.A, 0xFF - c.R, 0xFF - c.G, 0xFF - c.B));

		public static Bitmap Invert(Image image, Channels channels)
		{
			int InvertPixel(int p, Channels channel)
				=> channels.HasFlag(channel) ? 0xFF - p : p;

			return Transform(image, c => Color.FromArgb(c.A, InvertPixel(c.R, Channels.R), InvertPixel(c.G, Channels.G), InvertPixel(c.B, Channels.B)));
		}

		public static Bitmap Resize(Image image, int width, int height, bool maintainAspect = false)
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

		public static Bitmap Transform(Image image, Func<Color, Color> transform)
		{
			var bitmap = (Bitmap)image;

			Size imageSize = bitmap.Size;
			PixelFormat format = bitmap.PixelFormat;

			byte bytesPerPixel = (byte)(format == PixelFormat.Format32bppArgb ? 4 : 3);

			Rectangle rect = new Rectangle(Point.Empty, imageSize);
			BitmapData imageData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, format);

			int dataSize = imageData.Stride * imageData.Height;
			var data = new byte[dataSize];

			Marshal.Copy(imageData.Scan0, data, 0, dataSize);

			for (int y = 0; y < imageSize.Height; ++y)
			{
				for (int x = 0; x < imageSize.Width; ++x)
				{
					int index = y * imageData.Stride + x * bytesPerPixel;

					Color c = Color.FromArgb(bytesPerPixel == 4 ? data[index + 3] : 255, data[index + 2], data[index + 1], data[index]);
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

			return bitmap;
		}

		public static int Clamp(double value, int min = 0, int max = 255)
			=> Clamp((int)value, min, max);

		public static int Clamp(int value, int min = 0, int max = 255)
		{
			if (value < min)
			{
				value = min;
			}

			if (value > max)
			{
				value = max;
			}

			return value;
		}
		#endregion
	}
}
