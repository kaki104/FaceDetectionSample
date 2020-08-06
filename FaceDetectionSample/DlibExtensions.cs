using DlibDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace FaceDetectionSample
{
    public static class DlibExtensions
    {
        public static async Task<Matrix<RgbPixel>> ToMatrixAsync(this StorageFile file)
        {
            Matrix<RgbPixel> matrix = null;
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var sb = await decoder.GetSoftwareBitmapAsync();
                var array2d = sb.ToArray2D();
                matrix = new Matrix<RgbPixel>(array2d);
            }
            return matrix;
        }

        //https://www.bountysource.com/issues/73661016-add-new-method-to-dlib-which-used-to-load-data-from-softwarebitmap
        public static Array2D<RgbPixel> ToArray2D(this SoftwareBitmap bitmap)
        {
            uint bufferSize = (uint)(bitmap.PixelHeight * bitmap.PixelWidth * 4);
            byte[] dlibImageArray = new byte[bufferSize];

            Windows.Storage.Streams.Buffer buffer =
                new Windows.Storage.Streams.Buffer(bufferSize);
            bitmap.CopyToBuffer(buffer);
            using (var Reader = DataReader.FromBuffer(buffer))
            {
                Reader.ReadBytes(dlibImageArray);
            }
            return Dlib.LoadImageData<RgbPixel>(ImagePixelFormat.Bgra, dlibImageArray,
                (uint)bitmap.PixelHeight, (uint)bitmap.PixelWidth, (uint)bitmap.PixelWidth * 4);
        }

        public static async Task<BitmapImage> ToBitmapImageAsync(
            this Matrix<RgbPixel> matrix)
        {
            int width = matrix.Columns;
            int height = matrix.Rows;

            WriteableBitmap bitmap = new WriteableBitmap(width, height);

            using (InMemoryRandomAccessStream imras = new InMemoryRandomAccessStream())
            {
                RgbPixel[] array = matrix.ToArray();
                byte[] data = new byte[array.Length * 4];
                for (int d = 0, s = 0; d < data.Length; d += 4, s++)
                {
                    data[d] = array[s].Blue;
                    data[d + 1] = array[s].Green;
                    data[d + 2] = array[s].Red;
                }

                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(
                    BitmapEncoder.BmpEncoderId, imras);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)bitmap.PixelWidth,
                    (uint)bitmap.PixelHeight,
                    96.0,
                    96.0,
                    data);

                await encoder.FlushAsync();

                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(imras);
                return bitmapImage;
            }
        }
    }
}
