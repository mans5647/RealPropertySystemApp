using RealPropertySystemApp.codes;
using RealPropertySystemApp.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace RealPropertySystemApp.ui
{
    public class FloatNotification : Popup
    {
        private Window? window;

        public FloatNotification()
        {
            window = null;
        }

        public FloatNotification(double sW, double sH, Window window)
        {
            PlacementRectangle = new Rect(window.Left, window.Top, 0, 0);
            Width = sW;
            Height = sH;
            Placement = PlacementMode.Bottom;
            this.window = window;
            HorizontalOffset = 110;
            VerticalOffset = 50;
            Opacity = 0.0;
            AllowsTransparency = true;
            this.window.Closing += OnRootClose;
            this.window.LocationChanged += OnRootLocationChanged;
            this.window.SizeChanged += OnSizeChanged;
        }

        protected void OnRootClose(object sender, EventArgs e)
        {
            Hide();
        }

        protected void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var X = window.Left + (e.NewSize.Width - Constants.NotificationSizeW);
            var Y = window.Top + (e.NewSize.Height - Constants.NotificationSizeH);

            var nLoc = new Rect
            {
                X = X,
                Y = Y,
            };

            SetLocation(nLoc);
        }

        protected void OnRootLocationChanged(object sender, EventArgs e)
        {
            SetBottomRightPlaceLocation();
            UpdateLayout();
        }

        public void Show()
        {
            IsOpen = true;
        }


        public UIElement GetChild()
        {
            return Child;
        }


        public async Task<bool> ShowAnimated()
        {
            Show();

            const int MillisPerSec = 1000;
            var child = GetChild();


            // run timeout animation
            if (child is ISecondable settableSeconds)
            {
                await Task.Run(() => {


                    int Secs = Constants.StdNotificationTimelifeMillis / MillisPerSec;

                    while (Secs > 0)
                    {
                        window.Dispatcher.Invoke(() =>
                        {
                            settableSeconds.SetTimeoutLeftSeconds(Secs);
                        });

                        Thread.Sleep(MillisPerSec);
                        Secs--;
                    }

                });
            }


            // 2) run waninshing animation
            // 2.1 get opacity from child element
            // 2.2 run animation with decreasing Opacity factor


            var opCopy = GetChild().Opacity;

            const int MicroPerMilli = MillisPerSec / 10;

            var wanishAnimation = () =>
            {


                while (opCopy > 0.0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        GetChild().Opacity = opCopy;
                    });

                    opCopy -= 0.25;
                    Thread.Sleep(MicroPerMilli);
                }

            };

            await Task.Run(wanishAnimation);
            
            Hide();
            Opacity = 1.0;

            return true;
        }

        public void SetLocation(Rect value)
        {
            PlacementRectangle = value;
        }

        public void SetBottomRightPlaceLocation()
        {
            double X = window.Left + (window.Width - Constants.NotificationSizeW);
            double Y = window.Top + (window.Height - Constants.NotificationSizeH);

            Rect location = new Rect
            {
                X = X,
                Y = Y,
            };

            double d = location.Right;

            if (d < (window.Width - Constants.NotificationSizeW))
            {

                var diff = ((window.Width - Constants.NotificationSizeW) - d);

                location.X -= diff;
                
            }
            
            var conDiff = (location.Left - window.Left);
            var popDistanceToLeft = location.Left + (conDiff);
            var conSum = location.Left + window.Left;

            

            SetLocation(location);
            
        }

        public void SetChild(UIElement child)
        {
            Child = child;
        }

        public void Hide()
        {
            IsOpen = false;
        }

        public static FloatNotification withClassicChildIncluded(string title, string description, Window parent)
        {
            FloatNotification notification = new FloatNotification(Constants.NotificationSizeW, Constants.NotificationSizeH, parent);
            notification.SetBottomRightPlaceLocation();
            notification.SetChild(new ClassicNotificationContent(title, description));
            return notification;
        }

        
    }
}
