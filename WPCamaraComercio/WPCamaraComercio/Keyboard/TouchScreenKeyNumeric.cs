using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace WPCamaraComercio.Keyboard
{
    public class TouchScreenKeyNumeric : Window
    {
        static Window window;

        #region Property & Variable & Constructor

        private static int _position;

        public static int Position
        {
            get { return _position; }
            set { _position = value; }
        }


        private static double _WidthTouchKeyboard = 320;

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
            get { return TouchScreenKeyNumeric._CapsLockFlag; }
            set { TouchScreenKeyNumeric._CapsLockFlag = value; }
        }

        private static Window _InstanceObject;

        private static Brush _PreviousTextBoxBackgroundBrush = null;
        private static Brush _PreviousTextBoxBorderBrush = null;
        private static Thickness _PreviousTextBoxBorderThickness;

        private static bool isShow = false;

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

        public static RoutedUICommand Cmd1 = new RoutedUICommand();
        public static RoutedUICommand Cmd2 = new RoutedUICommand();
        public static RoutedUICommand Cmd3 = new RoutedUICommand();
        public static RoutedUICommand Cmd4 = new RoutedUICommand();
        public static RoutedUICommand Cmd5 = new RoutedUICommand();
        public static RoutedUICommand Cmd6 = new RoutedUICommand();
        public static RoutedUICommand Cmd7 = new RoutedUICommand();
        public static RoutedUICommand Cmd8 = new RoutedUICommand();
        public static RoutedUICommand Cmd9 = new RoutedUICommand();
        public static RoutedUICommand Cmd0 = new RoutedUICommand();

        public static RoutedUICommand CmdBackspace = new RoutedUICommand();
        public static RoutedUICommand CmdClear = new RoutedUICommand();
        public static RoutedUICommand CmdEnter = new RoutedUICommand();

        public TouchScreenKeyNumeric()
        {
            isShow = true;
            Topmost = true;
            this.Width = WidthTouchKeyboard;
            this.Height = 420;
        }

        static TouchScreenKeyNumeric()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchScreenKeyNumeric), new FrameworkPropertyMetadata(typeof(TouchScreenKeyNumeric)));
            SetCommandBinding();
        }
        #endregion
        #region CommandRelatedCode
        private static void SetCommandBinding()
        {
            CommandBinding CbBackspace = new CommandBinding(CmdBackspace, RunCommand);

            CommandBinding Cb1 = new CommandBinding(Cmd1, RunCommand);
            CommandBinding Cb2 = new CommandBinding(Cmd2, RunCommand);
            CommandBinding Cb3 = new CommandBinding(Cmd3, RunCommand);

            CommandBinding Cb4 = new CommandBinding(Cmd4, RunCommand);
            CommandBinding Cb5 = new CommandBinding(Cmd5, RunCommand);
            CommandBinding Cb6 = new CommandBinding(Cmd6, RunCommand);

            CommandBinding Cb7 = new CommandBinding(Cmd7, RunCommand);
            CommandBinding Cb8 = new CommandBinding(Cmd8, RunCommand);
            CommandBinding Cb9 = new CommandBinding(Cmd9, RunCommand);

            CommandBinding Cb0 = new CommandBinding(Cmd0, RunCommand);
            CommandBinding CbEnter = new CommandBinding(CmdEnter, RunCommand);
            CommandBinding CbClear = new CommandBinding(CmdClear, RunCommand);


            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), CbBackspace);

            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), Cb1);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), Cb2);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), Cb3);

            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), Cb4);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), Cb5);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), Cb6);

            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), Cb7);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), Cb8);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), Cb9);

            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), Cb0);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), CbEnter);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyNumeric), CbClear);

        }
        static void RunCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == Cmd1)
            {
                TouchScreenKeyNumeric.TouchScreenText += "1";
            }
            else if (e.Command == Cmd2)
            {
                TouchScreenKeyNumeric.TouchScreenText += "2";
            }
            else if (e.Command == Cmd3)
            {
                TouchScreenKeyNumeric.TouchScreenText += "3";
            }
            else if (e.Command == Cmd4)
            {
                TouchScreenKeyNumeric.TouchScreenText += "4";
            }
            else if (e.Command == Cmd5)
            {
                TouchScreenKeyNumeric.TouchScreenText += "5";
            }
            else if (e.Command == Cmd6)
            {
                TouchScreenKeyNumeric.TouchScreenText += "6";
            }
            else if (e.Command == Cmd7)
            {
                TouchScreenKeyNumeric.TouchScreenText += "7";
            }
            else if (e.Command == Cmd8)
            {
                TouchScreenKeyNumeric.TouchScreenText += "8";
            }
            else if (e.Command == Cmd9)
            {
                TouchScreenKeyNumeric.TouchScreenText += "9";
            }
            else if (e.Command == Cmd0)
            {
                TouchScreenKeyNumeric.TouchScreenText += "0";
            }
            else if (e.Command == CmdBackspace)
            {
                if (!string.IsNullOrEmpty(TouchScreenKeyNumeric.TouchScreenText))
                {
                    TouchScreenKeyNumeric.TouchScreenText = TouchScreenKeyNumeric.TouchScreenText.Substring(0, TouchScreenKeyNumeric.TouchScreenText.Length - 1);
                }

            }
            else if (e.Command == CmdClear)//Last row
            {
                TouchScreenKeyNumeric.TouchScreenText = "";
            }
            else if (e.Command == CmdEnter)
            {

                if (_InstanceObject != null)
                {
                    _InstanceObject.Close();
                    window.IsEnabled = true;
                    _InstanceObject = null;
                }
                _CurrentControl.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
        #endregion
        #region Main Functionality
        private static void AddKeyBoardINput(char input)
        {
            if (CapsLockFlag)
            {
                if (ShiftFlag)
                {
                    TouchScreenKeyNumeric.TouchScreenText += char.ToLower(input).ToString();
                    ShiftFlag = false;

                }
                else
                {
                    TouchScreenKeyNumeric.TouchScreenText += char.ToUpper(input).ToString();
                }
            }
            else
            {
                if (!ShiftFlag)
                {
                    TouchScreenKeyNumeric.TouchScreenText += char.ToLower(input).ToString();
                }
                else
                {
                    TouchScreenKeyNumeric.TouchScreenText += char.ToUpper(input).ToString();
                    ShiftFlag = false;
                }
            }
        }


        private static void syncchild()
        {
            if (_CurrentControl != null && _InstanceObject != null)
            {

                System.Windows.Point virtualpoint = new Point(0, _CurrentControl.ActualHeight + 3);
                Point Actualpoint = _CurrentControl.PointToScreen(virtualpoint);

                if (WidthTouchKeyboard + Actualpoint.X > SystemParameters.VirtualScreenWidth)
                {
                    double difference = WidthTouchKeyboard + Actualpoint.X - SystemParameters.VirtualScreenWidth;
                    _InstanceObject.Left = difference;
                }
                else if (!(Actualpoint.X > 1))
                {
                    _InstanceObject.Left = 1;
                }
                else
                {
                    _InstanceObject.Left = Actualpoint.X;
                }

                _InstanceObject.Top = Actualpoint.Y - Position;
                _InstanceObject.Show();
            }
        }

        public static bool GetTouchScreenKeyNumeric(DependencyObject obj)
        {
            return (bool)obj.GetValue(TouchScreenKeyNumericProperty);
        }

        public static void SetTouchScreenKeyNumeric(DependencyObject obj, bool value)
        {
            obj.SetValue(TouchScreenKeyNumericProperty, value);
        }

        /*public static readonly DependencyProperty TouchScreenKeyNumericProperty =
                  DependencyProperty.RegisterAttached("TouchScreenKeyNumeric", typeof(bool),
                  typeof(TouchScreenKeyNumeric), new UIPropertyMetadata(default(bool),
                  TouchScreenKeyNumericPropertyChanged));*/

        public static readonly DependencyProperty TouchScreenKeyNumericProperty =
           DependencyProperty.RegisterAttached("TouchScreenKeyNumeric", typeof(bool),
               typeof(TouchScreenKeyNumeric), new UIPropertyMetadata(default(bool),
                   TouchScreenKeyNumericPropertyChanged));



        static void TouchScreenKeyNumericPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
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
            Control host = sender as Control;

            _PreviousTextBoxBackgroundBrush = host.Background;
            _PreviousTextBoxBorderBrush = host.BorderBrush;
            _PreviousTextBoxBorderThickness = host.BorderThickness;

            host.Background = Brushes.Transparent;
            host.BorderBrush = Brushes.Transparent;
            host.BorderThickness = new Thickness(4);


            _CurrentControl = host;

            if (_InstanceObject == null)
            {
                FrameworkElement ct = host;
                while (true)
                {
                    if (ct is Window)
                    {
                        ((Window)ct).LocationChanged += new EventHandler(TouchScreenKeyNumeric_LocationChanged);
                        ((Window)ct).Activated += new EventHandler(TouchScreenKeyNumeric_Activated);
                        ((Window)ct).Deactivated += new EventHandler(TouchScreenKeyNumeric_Deactivated);
                        break;
                    }
                    ct = (FrameworkElement)ct.Parent;
                }

                window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                _InstanceObject = new TouchScreenKeyNumeric();
                _InstanceObject.AllowsTransparency = true;
                _InstanceObject.WindowStyle = WindowStyle.None;
                _InstanceObject.ShowInTaskbar = false;
                _InstanceObject.ShowInTaskbar = false;
                _InstanceObject.Topmost = true;
                window.IsEnabled = false;
                host.LayoutUpdated += new EventHandler(tb_LayoutUpdated);
            }
        }

        static void TouchScreenKeyNumeric_Deactivated(object sender, EventArgs e)
        {
            if (_InstanceObject != null)
            {
                _InstanceObject.Topmost = false;
            }
        }

        static void TouchScreenKeyNumeric_Activated(object sender, EventArgs e)
        {
            if (_InstanceObject != null)
            {
                _InstanceObject.Topmost = true;
            }
        }



        static void TouchScreenKeyNumeric_LocationChanged(object sender, EventArgs e)
        {
            syncchild();
        }

        static void tb_LayoutUpdated(object sender, EventArgs e)
        {
            syncchild();
        }

        static void OnLostFocus(object sender, RoutedEventArgs e)
        {

            Control host = sender as Control;
            host.Background = _PreviousTextBoxBackgroundBrush;
            host.BorderBrush = _PreviousTextBoxBorderBrush;
            host.BorderThickness = _PreviousTextBoxBorderThickness;

            if (_InstanceObject != null)
            {
                _InstanceObject.Close();
                _InstanceObject = null;
            }

        }

        #endregion
    }

}
