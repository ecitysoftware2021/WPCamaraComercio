using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;

namespace WPFCCMedellin.Keyboard
{
    class TouchScreenKeyboard : Window
    {
        #region Property & Variable & Constructor

        private static bool isRun = false;

        static UserControl window;

        private static int _position;

        public static int Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private static int _positionY = 0;

        public static int PositionY
        {
            get { return _positionY; }
            set { _positionY = value; }
        }

        private static int _positionX = 40;

        public static int PositionX
        {
            get { return _positionX; }
            set { _positionX = value; }
        }

        private static double _WidthTouchKeyboard = 615;

        public static double WidthTouchKeyboard
        {
            get { return _WidthTouchKeyboard; }
            set { _WidthTouchKeyboard = value; }

        }
        private static bool _ShiftFlag;

        protected static bool ShiftFlag
        {
            get { return _ShiftFlag; }
            set { _ShiftFlag = value; }
        }

        private static bool _CapsLockFlag;

        protected static bool CapsLockFlag
        {
            get { return TouchScreenKeyboard._CapsLockFlag; }
            set { TouchScreenKeyboard._CapsLockFlag = value; }
        }

        private static Window _InstanceObject;

        private static Brush _PreviousTextBoxBackgroundBrush = null;
        private static Brush _PreviousTextBoxBorderBrush = null;
        private static Thickness _PreviousTextBoxBorderThickness;

        private static Control _CurrentControl;
        public static string TouchScreenText
        {
            get
            {
                if (_CurrentControl is TextBox)
                {
                    return ((TextBox)_CurrentControl).Text;
                }
                else if (_CurrentControl is ComboBox)
                {
                    return ((ComboBox)_CurrentControl).Text;
                }
                else if (_CurrentControl is PasswordBox)
                {
                    return ((PasswordBox)_CurrentControl).Password;
                }
                else return "";

            }
            set
            {
                if (_CurrentControl is TextBox)
                {
                    ((TextBox)_CurrentControl).Text = value;
                }
                else if (_CurrentControl is ComboBox)
                {
                    ((ComboBox)_CurrentControl).Text = value;
                }
                else if (_CurrentControl is PasswordBox)
                {
                    ((PasswordBox)_CurrentControl).Password = value;
                }
            }
        }

        public TouchScreenKeyboard()
        {
            this.Width = WidthTouchKeyboard;
            this.Height = 320;
        }

        static TouchScreenKeyboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchScreenKeyboard), new FrameworkPropertyMetadata(typeof(TouchScreenKeyboard)));
        }
        #endregion
        #region Main Functionality
        private static void AddKeyBoardINput(char input)
        {
            if (CapsLockFlag)
            {
                if (ShiftFlag)
                {
                    TouchScreenKeyboard.TouchScreenText += char.ToLower(input).ToString();
                    ShiftFlag = false;
                }
                else
                {
                    TouchScreenKeyboard.TouchScreenText += char.ToUpper(input).ToString();
                }
            }
            else
            {
                if (!ShiftFlag)
                {
                    TouchScreenKeyboard.TouchScreenText += char.ToLower(input).ToString();
                }
                else
                {
                    TouchScreenKeyboard.TouchScreenText += char.ToUpper(input).ToString();
                    ShiftFlag = false;
                }
            }
        }

        public static void BtnSearch_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                Button item;
                Image itemI;
                if (sender is Button)
                {
                    item = (Button)sender;
                    if (item.Tag.ToString() == "SpaceBar")
                    {
                        TouchScreenKeyboard.TouchScreenText += " ";
                    }
                    else
                    {
                        char data = Convert.ToChar(item.Tag.ToString());
                        AddKeyBoardINput(data);
                    }
                }
                else if (sender is Image)
                {
                    itemI = (Image)sender;
                    if (itemI.Tag.ToString() == "Backspace")
                    {
                        if (!string.IsNullOrEmpty(TouchScreenKeyboard.TouchScreenText))
                        {
                            TouchScreenKeyboard.TouchScreenText = TouchScreenKeyboard.TouchScreenText.Substring(0, TouchScreenKeyboard.TouchScreenText.Length - 1);
                        }
                    }
                    else if (itemI.Tag.ToString() == "Clear")
                    {
                        TouchScreenKeyboard.TouchScreenText = string.Empty;
                    }
                    else if (itemI.Tag.ToString() == "Enter")
                    {
                        if (_InstanceObject != null)
                        {
                            _InstanceObject.Close();
                            _InstanceObject = null;
                        }

                        if (window != null)
                        {
                            window.IsEnabled = true;
                        }
                        _CurrentControl.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }
                    else
                    {
                        char data = Convert.ToChar(itemI.Tag.ToString());
                        AddKeyBoardINput(data);
                    }
                }
            }
            catch { }
        }

        private static void syncchild()
        {
            try
            {
                if (_CurrentControl != null && _InstanceObject != null)
                {
                    System.Windows.Point virtualpoint = new Point(0, _CurrentControl.ActualHeight + 3);
                    Point Actualpoint = _CurrentControl.PointToScreen(virtualpoint);

                    _InstanceObject.Left = Actualpoint.X - PositionX;
                    _InstanceObject.Top = Actualpoint.Y + PositionY;
                    if (!_InstanceObject.IsLoaded)
                    {
                        _InstanceObject.Show();
                        foreach (var item in FindVisualChildren<Button>(_InstanceObject))
                        {
                            item.TouchDown += new EventHandler<TouchEventArgs>(BtnSearch_TouchDown);
                        }
                        foreach (var item in FindVisualChildren<Image>(_InstanceObject))
                        {
                            item.TouchDown += new EventHandler<TouchEventArgs>(BtnSearch_TouchDown);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static bool GetTouchScreenKeyboard(DependencyObject obj)
        {
            return (bool)obj.GetValue(TouchScreenKeyboardProperty);
        }

        public static void SetTouchScreenKeyboard(DependencyObject obj, bool value)
        {
            obj.SetValue(TouchScreenKeyboardProperty, value);
        }

        public static readonly DependencyProperty TouchScreenKeyboardProperty =
                  DependencyProperty.RegisterAttached("TouchScreenKeyboard", typeof(bool),
                  typeof(TouchScreenKeyboard), new UIPropertyMetadata(default(bool),
                  TouchScreenKeyboardPropertyChanged));

        static void TouchScreenKeyboardPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement host = sender as FrameworkElement;
            if (host != null)
            {
                host.GotFocus += new RoutedEventHandler(OnGotFocus);
                host.LostFocus += new RoutedEventHandler(OnLostFocus);
            }
        }

        static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            isRun = true;
            Control host = sender as Control;

            _PreviousTextBoxBackgroundBrush = host.Background;
            _PreviousTextBoxBorderBrush = host.BorderBrush;
            _PreviousTextBoxBorderThickness = host.BorderThickness;

            //host.Background = Brushes.Transparent;
            host.BorderBrush = Brushes.Transparent;
            //host.BorderThickness = new Thickness(4);

            _CurrentControl = host;

            if (_InstanceObject == null)
            {
                //if (window == null)
                //{
                window = (UserControl)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive).Content;
                //}

                window.IsEnabled = false;

                _InstanceObject = new TouchScreenKeyboard();
                _InstanceObject.AllowsTransparency = true;
                _InstanceObject.WindowStyle = WindowStyle.None;
                _InstanceObject.ShowInTaskbar = false;
                _InstanceObject.ShowInTaskbar = false;
                _InstanceObject.Topmost = true;
                host.LayoutUpdated += new EventHandler(tb_LayoutUpdated);
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        static void TouchScreenKeyboard_Deactivated(object sender, EventArgs e)
        {
            if (_InstanceObject != null)
            {
                _InstanceObject.Topmost = false;
            }
        }

        static void TouchScreenKeyboard_Activated(object sender, EventArgs e)
        {
            if (_InstanceObject != null)
            {
                _InstanceObject.Topmost = true;
            }
        }

        static void TouchScreenKeyboard_LocationChanged(object sender, EventArgs e)
        {
            syncchild();
        }

        static void tb_LayoutUpdated(object sender, EventArgs e)
        {
            if (isRun)
            {
                syncchild();
            }
        }

        static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            isRun = false;
            window.IsEnabled = true;
            Control host = sender as Control;
            host.Background = _PreviousTextBoxBackgroundBrush;
            host.BorderBrush = _PreviousTextBoxBorderBrush;
            host.BorderThickness = _PreviousTextBoxBorderThickness;
            host.LayoutUpdated -= new EventHandler(tb_LayoutUpdated);

            if (_InstanceObject != null)
            {

                _InstanceObject.Close();
                _InstanceObject = null;
            }
        }

        public static void CloseKeyboard()
        {
            try
            {
                if (_InstanceObject != null)
                {
                    _InstanceObject.Close();
                    _InstanceObject = null;
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}