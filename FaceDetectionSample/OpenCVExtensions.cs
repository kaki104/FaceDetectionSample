﻿using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace FaceDetectionSample
{
    public static class OpenCVExtensions
    {
        public static async Task<Mat> ToMatAsync(this StorageFile file)
        {
            if (file == null) return null;
            Mat returnValue = null;
            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                byte[] buff = new byte[stream.Length];
                stream.Read(buff, 0, buff.Length);
                returnValue = Cv2.ImDecode(buff, ImreadModes.Color);
            }
            return returnValue;
        }

        public static async Task<BitmapImage> ToBitmapImageAsync(this Mat source)
        {
            if (source == null)
            {
                return null;
            }

            using (MemoryStream ms = source.ToMemoryStream())
            {
                BitmapImage bi = new BitmapImage();
                await bi.SetSourceAsync(ms.AsRandomAccessStream());
                return bi;
            }
        }
    }
}
