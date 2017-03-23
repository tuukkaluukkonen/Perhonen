using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MySuperGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Butterfly
        private Butterfly butterfly;

        // Flowers, Flower
        private List<Flower> flowers;

        // Wich keys are pressed
        private bool UpPressed;
        private bool LeftPressed;
        private bool RightPressed;

        // Game loop timer
        private DispatcherTimer timer;

        // audio 
        private MediaElement mediaElement;


        public MainPage()
        {
            this.InitializeComponent();

            //add butterfly
            butterfly = new Butterfly
            {
                LocationX = MyCanvas.Width / 2, 
                LocationY = MyCanvas.Height / 2
            };
            // add butterfly to canvas
            MyCanvas.Children.Add(butterfly);

            // init list of flowers
            flowers = new List<Flower>();

            // key listeners
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;

            // Mouse listener
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

            // load audio
            LoadAudio();

            // start game loop
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // load audio use when colliding happens
        private async void LoadAudio()
        {
            StorageFolder folder =
                await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile file =
                await folder.GetFileAsync("ding.wav");
            var stream = await file.OpenAsync(FileAccessMode.Read);

            mediaElement = new MediaElement();
            mediaElement.AutoPlay = false;
            mediaElement.SetSource(stream, file.ContentType);
        }

        private void CoreWindow_PointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            Flower flower = new Flower();
            flower.LocationX = args.CurrentPoint.Position.X - flower.Width / 2;
            flower.LocationY = args.CurrentPoint.Position.Y - flower.Width / 2;
            // add to canvas
            MyCanvas.Children.Add(flower);
            flower.SetLocation();
            // add to flowers list
            flowers.Add(flower);

        }

        private void Timer_Tick(object sender, object e)
        {
            // Move butterfly if UpPressed
            if (UpPressed) butterfly.Move();


            // Rotate if left/right pressed
            // -1 == left 1 == right
            if (LeftPressed) butterfly.Rotate(-1);
            if (RightPressed) butterfly.Rotate(1);

            // update butterfly
            butterfly.SetLocation();

            // collission...
            CheckCollision();
        }

        private void CheckCollision()
        {
            // loop flowers list
            foreach(Flower flower in flowers)
            {
                // get rects
                Rect BRect = new Rect(
                    butterfly.LocationX,
                    butterfly.LocationY,
                    butterfly.ActualWidth,
                    butterfly.ActualHeight
                    );
                Rect FRect = new Rect(
                    flower.LocationX,
                    flower.LocationY,
                    flower.ActualWidth,
                    flower.ActualHeight
                    );
                // Does objects intersect
                BRect.Intersect(FRect);
                if (!BRect.IsEmpty)
                {
                    // collision! area isnt empty
                    // remove flower from canvas
                    MyCanvas.Children.Remove(flower);
                    // remove from list
                    flowers.Remove(flower);
                    // play audio
                    mediaElement.Play();
                    break;
                }
            }
        }

        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = false;
                    break;
                case VirtualKey.Left:
                    LeftPressed = false;
                    break;
                case VirtualKey.Right:
                    RightPressed = false;
                    break;

            }
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = true;
                    break;
                case VirtualKey.Left:
                    LeftPressed = true;
                    break;
                case VirtualKey.Right:
                    RightPressed = true;
                    break;

            }
        }
    }
}
