using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DlibDotNet;
using Dlib = DlibDotNet.Dlib;
using OpenCvSharp;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;

//using WinFormCharpWebCam;

namespace FaceDetection
{

    public partial class MainForm : Form
    {

        #region Fields

        VideoCapture cap; //= new VideoCapture(0);

        private  BackgroundWorker _BackgroundWorker;
        private  ShapePredictor _ShapePredictor;

        #endregion


        #region Constructors

        public MainForm()
        {
            this.InitializeComponent();

            ListCameras listcameras = new ListCameras();

            foreach (CameraDetails cam in listcameras.Enumerate())
            {
                if (cam.Name.ToUpper().Trim() == "SURFACE CAMERA FRONT")
                {
                    cameraIndex = cam.index+1; // 0 = Default
                    tsStatus.Text = $"Found cam.index: {cam.index} - Name: {cam.Name}";
                }
            }

            if(cameraIndex == 0)
            {
                tsStatus.Text = $"Using Default Camera - Surface Front not found";
            }

           
        }
        #endregion

        bool CaptureVideo = false;


        void ControlVideo()
        { 
            if (CaptureVideo)
            {
                CaptureVideo = false;
                // Cancel the asynchronous operation.
                _BackgroundWorker.CancelAsync();
            }
            else
            {
                CaptureVideo = true;
                if (_BackgroundWorker.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    _BackgroundWorker.RunWorkerAsync();
                }
            }
        }



        private FrontalFaceDetector? _frontalFaceDetector = null;
        /// <summary>
        /// Function so that we can keep the face detector in memory, should speed up consecutive face detects by ~300ms
        /// </summary>
        /// <returns></returns>
        private FrontalFaceDetector GetFrontalFaceDetector()
        {
            if (_frontalFaceDetector == null)
                _frontalFaceDetector = Dlib.GetFrontalFaceDetector();
            return _frontalFaceDetector;
        }


        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            DetectFaces();
        }

        void DetectFaces()
        {

            var detector = GetFrontalFaceDetector();
            using (_ShapePredictor)
            {
                // Grab and process frames until the main window is closed by the user.
                while (CaptureVideo)
                {
                    // Grab a frame
                    var VideoFrame = new Mat();
                    if (!cap.Read(VideoFrame))
                    {
                        // LblStatus.Text = "Error Reading Video frame...";
                        break;
                    }

                    // Turn OpenCV's Mat into something dlib can deal with.  Note that this just
                    // wraps the Mat object, it doesn't copy anything.  So cimg is only valid as
                    // long as temp is valid.  Also don't do anything to temp that would cause it
                    // to reallocate the memory which stores the image as that will make cimg
                    // contain dangling pointers.  This basically means you shouldn't modify temp
                    // while using cimg.
                    var array = new byte[VideoFrame.Width * VideoFrame.Height * VideoFrame.ElemSize()];
                    Marshal.Copy(VideoFrame.Data, array, 0, array.Length);
                    using (var cimg = Dlib.LoadImageData<RgbPixel>(array, (uint)VideoFrame.Height, (uint)VideoFrame.Width, (uint)(VideoFrame.Width * VideoFrame.ElemSize())))
                    {

                        //this.LblStatus.Invoke(new Action(() =>
                        //{
                        //    LblStatus.Text = "Detecting Faces....";
                        //}));

                        // Detect faces 
                        DlibDotNet.Rectangle[] faces = detector.Operator(cimg);
                        // Find the pose of each face.

                        List<FullObjectDetection> shapes = new List<FullObjectDetection>();

                        for (var i = 0; i < faces.Length; ++i)
                        {
                            var det = _ShapePredictor.Detect(cimg, faces[i]);
                            shapes.Add(det);
                        }

                        ImageWindow.OverlayLine[] lines = Dlib.RenderFaceDetections(shapes);
                        foreach (var s in shapes)
                            s.Dispose();

                        this.pictureBox.Invoke(new Action(() =>
                        {
                            //this.pictureBox.Image?.Dispose();
                            var image = Image.FromStream(VideoFrame.ToMemoryStream());
                            pictureBox.Image = (Image)image.Clone();

                            if (lines.Length > 0)
                            {
                                Graphics gfx = Graphics.FromImage(pictureBox.Image);
                                //ScaleTransform 
                                //float stretchX = (float)pictureBox1.Image.Size.Width / (float)pictureBox1.Size.Width;
                                //float stretchY = (float)pictureBox1.Image.Size.Height / (float)pictureBox1.Size.Height;

                                foreach (ImageWindow.OverlayLine line in lines)
                                {
                                    gfx.DrawEllipse(Pens.DarkBlue, new System.Drawing.Rectangle(line.Point1.X, line.Point1.Y, 4, 4));

                                    gfx.DrawLine(new Pen(Color.Green, 1), line.Point1.X, line.Point1.Y, line.Point2.X, line.Point2.Y);
                                }

                                //  LblStatus.Text = $"Rendering {lines.Length} lines....";
                                pictureBox.Refresh();
                            }
                        }));

                        foreach (var line in lines)
                            line.Dispose();
                    }
                }
            }
        }

        int cameraIndex = 0; //Default

        private void MainForm_Load(object sender, EventArgs e)
        {
            
            
        }

       

       
        private void MainForm_Shown(object sender, EventArgs e)
        {
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            //https://github.com/takuya-takeuchi/FaceRecognitionDotNet/wiki/Quickstart#1-initialize

            string exePath = Path.GetDirectoryName(Application.ExecutablePath);

            exePath = AppDomain.CurrentDomain.BaseDirectory;

            string neuralNet = Path.Combine(exePath, "shape_predictor_68_face_landmarks.dat");

            if (!File.Exists(neuralNet))
            {
                MessageBox.Show(string.Format("{0} Neural Net does not exist!", neuralNet));
                
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = exePath,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
                return;
            }


            button1.Visible = false;


            // dotnet add package DlibDotNet.ARM --version 19.17.0.20190623
            // System.BadImageFormatException: 'An attempt was made to load a program with an incorrect format. (0x8007000B)'

            _ShapePredictor = ShapePredictor.Deserialize(neuralNet); // shape_predictor_5_face_landmarks.dat");

            _BackgroundWorker = new BackgroundWorker();
            this._BackgroundWorker.DoWork += this.BackgroundWorkerOnDoWork;
            _BackgroundWorker.WorkerSupportsCancellation = true;

            //camera id '0' is for default camera
            cap = new VideoCapture(cameraIndex);

            Application.DoEvents();
            if (!cap.IsOpened())
            {

                Debug.WriteLine("Unable to connect to camera");
                return;
            }

            ControlVideo();
        }
    }

}
