using DlibDotNet;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Rect = OpenCvSharp.Rect;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FaceDetectionSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private CascadeClassifier _haarCascade;

        public MainPage()
        {
            this.InitializeComponent();

            _haarCascade = InitializeFaceClassifier();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file == null)
            {
                return;
            }

            var mat = await file.ToMatAsync();
            if (mat == null) return;

            var faces = DetectFaces(_haarCascade, mat);
            if (faces.Any() == false)
            {
                OpenCVImage.Source = null;
                return;
            }

            using (var renderedFaces = RenderFaces(faces, mat))
            {
                OpenCVImage.Source = null;
                OpenCVImage.Source = await renderedFaces.ToBitmapImageAsync();
            }
        }

        /// <summary>
        /// Render detected faces via OpenCV.
        /// </summary>
        /// <param name="state">Current frame state.</param>
        /// <param name="image">Web cam or video frame.</param>
        /// <returns>Returns new image frame.</returns>
        private static Mat RenderFaces(Rect[] faces, Mat image)
        {
            Mat result = image.Clone();
            //Cv2.CvtColor(image, image, ColorConversionCodes.BGR2GRAY);

            // Render all detected faces
            foreach (Rect face in faces)
            {
                Cv2.Rectangle(result,
                    face.TopLeft, face.BottomRight,
                    _faceColorBrush
                    , 4);
            }
            return result;
        }

        private static readonly Scalar _faceColorBrush = new Scalar(0, 0, 255);

        /// <summary>
        /// Use OpenCV Cascade classifier to do offline face detection.
        /// </summary>
        /// <param name="cascadeClassifier">OpenCV cascade classifier.</param>
        /// <param name="image">Web cam or video frame.</param>
        /// <returns>Return list of faces as rectangles.</returns>
        public static Rect[] DetectFaces(
            CascadeClassifier cascadeClassifier, Mat image)
        {
            //https://stackoverflow.com/questions/20801015/recommended-values-for-opencv-detectmultiscale-parameters
            //1.05
            //3-6
            return cascadeClassifier
                .DetectMultiScale(
                    image,
                    1.1,
                    3,
                    HaarDetectionType.ScaleImage,
                    new OpenCvSharp.Size(60, 60));
        }

        /// <summary>
        /// Initialize classifier used for offline face detection.
        /// </summary>
        private static CascadeClassifier InitializeFaceClassifier()
        {
            return new CascadeClassifier("Assets/haarcascade_frontalface_alt.xml");
            //return new CascadeClassifier("Assets/haarcascade_frontalface_default.xml");
            //return new CascadeClassifier("Assets/lbpcascade_frontalface.xml");
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file == null)
            {
                return;
            }

            using (FrontalFaceDetector faceDetector = Dlib.GetFrontalFaceDetector())
            {
                using (var sp = ShapePredictor.Deserialize("shape_predictor_5_face_landmarks.dat"))
                {
                    var matrix = await file.ToMatrixAsync();
                    var faces = faceDetector.Operator(matrix);
                    foreach (var face in faces)
                    {
                        FullObjectDetection location = sp.Detect(matrix, face);
                        if (location == null) continue;
                        //var faceChipDetail = Dlib.GetFaceChipDetails(location);
                        //var faceChip = Dlib.ExtractImageChip<RgbPixel>(matrix, faceChipDetail);

                        Dlib.DrawRectangle(matrix, location.Rect, 
                            new RgbPixel(255,0,0), 4);
                    }

                    DlibImage.Source = null;
                    DlibImage.Source = await matrix.ToBitmapImageAsync();
                }
            }
        }
    }
}
