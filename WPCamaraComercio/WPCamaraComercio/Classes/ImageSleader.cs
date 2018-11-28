using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WPCamaraComercio.Classes
{
    class ImageSleader : Window
    {
        #region "Referencias"
        private List<string> images;

        private string strImagePath = string.Empty;

        private int CurrentSourceIndex;

        private int CurrentCtrlIndex;

        public int time;

        public bool isRotate = false;

        public ImageStory imageModel;

        private DispatcherTimer dispatcherTimer;

        public Action<string> callbackError;
        #endregion

        #region "Métodos"
        public ImageSleader(List<string> images)
        {
            this.images = images;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            init();
        }

        public void star()
        {
            try
            {
                PlaySlideShow(0);
                if (isRotate)
                {
                    starTime();
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.ToString());
            }
        }

        public void stop()
        {
            stopTime();
        }

        private void init()
        {
            try
            {
                imageModel = new ImageStory
                {
                    sourse = images[0],
                    buttonBack = Visibility.Hidden,
                    buttonNext = Visibility.Visible,
                    buttonFinish = Visibility.Hidden
                };

            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.ToString());
            }
        }

        private ImageSource CreateImageSource(string file, bool forcePreLoad)
        {
            try
            {
                if (forcePreLoad)
                {
                    var src = new BitmapImage();
                    src.BeginInit();
                    src.UriSource = new Uri(file, UriKind.Absolute);
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.EndInit();
                    src.Freeze();
                    return src;
                }
                else
                {
                    var src = new BitmapImage(new Uri(file, UriKind.Absolute));
                    src.Freeze();
                    return src;
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.ToString());
                return null;
            }
        }

        private void PlaySlideShow(int direction)
        {
            try
            {
                if (images.Count == 0 || CurrentSourceIndex >= images.Count || CurrentSourceIndex < 0)
                {
                    return;
                }
                var oldCtrlIndex = CurrentCtrlIndex;

                if (direction == 1)
                {
                    CurrentSourceIndex = (CurrentSourceIndex + 1);
                }
                else if (direction == 2)
                {
                    CurrentSourceIndex = (CurrentSourceIndex - 1);
                }

                Dispatcher.Invoke(() =>
                {
                    imageModel.sourse = images[CurrentSourceIndex];

                    if (!isRotate)
                    {
                        if (CurrentSourceIndex < (images.Count - 1) && CurrentSourceIndex > 0)
                        {
                            imageModel.buttonBack = Visibility.Visible;
                            imageModel.buttonNext = Visibility.Visible;
                            imageModel.buttonFinish = Visibility.Hidden;
                        }
                        else if (CurrentSourceIndex == 0)
                        {
                            imageModel.buttonBack = Visibility.Hidden;
                            imageModel.buttonNext = Visibility.Visible;
                            imageModel.buttonFinish = Visibility.Hidden;
                        }
                        else if (CurrentSourceIndex == (images.Count - 1))
                        {
                            imageModel.buttonBack = Visibility.Visible;
                            imageModel.buttonNext = Visibility.Hidden;
                            imageModel.buttonFinish = Visibility.Visible;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.ToString());
            }
        }

        public void starTime()
        {
            dispatcherTimer.Interval = new TimeSpan(0, 0, this.time);
            if (dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Stop();
            }
            dispatcherTimer.Start();
        }

        public void stopTime()
        {
            dispatcherTimer.Stop();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (CurrentSourceIndex == images.Count - 1)
                {
                    CurrentSourceIndex = 0;
                    PlaySlideShow(0);
                }
                else
                {
                    PlaySlideShow(1);
                }
            }
            catch (Exception ex)
            {


            }
        }

        public void moveBack()
        {
            try
            {
                PlaySlideShow(2);
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.ToString());
            }
        }

        public void moveNext()
        {
            try
            {
                PlaySlideShow(1);
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(ex.ToString());
            }
        }
        #endregion
    }

    class ImageStory : INotifyPropertyChanged
    {
        private string _sourse;

        public string sourse
        {
            get
            {
                return _sourse;
            }
            set
            {
                _sourse = value;
                OnPropertyRaised("sourse");
            }
        }


        private Visibility _buttonBack;

        public Visibility buttonBack
        {
            get
            {
                return _buttonBack;
            }
            set
            {
                _buttonBack = value;
                OnPropertyRaised("buttonBack");
            }
        }

        private Visibility _buttonNext;

        public Visibility buttonNext
        {
            get
            {
                return _buttonNext;
            }
            set
            {
                _buttonNext = value;
                OnPropertyRaised("buttonNext");
            }
        }

        private Visibility _buttonFinish;

        public Visibility buttonFinish
        {
            get
            {
                return _buttonFinish;
            }
            set
            {
                _buttonFinish = value;
                OnPropertyRaised("buttonFinish");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));

        }
    }
}
