using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace WPCamaraComercio.Keyboard
{
    public class TouchScreenKeyboard : Window
    {
        static Window window;

        #region Property & Variable & Constructor

        private static int _position;

        public static int Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private static double _WidthTouchKeyboard = 630;

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

        public static RoutedUICommand CmdQ = new RoutedUICommand();
        public static RoutedUICommand Cmdw = new RoutedUICommand();
        public static RoutedUICommand CmdE = new RoutedUICommand();
        public static RoutedUICommand CmdR = new RoutedUICommand();
        public static RoutedUICommand CmdT = new RoutedUICommand();
        public static RoutedUICommand CmdY = new RoutedUICommand();
        public static RoutedUICommand CmdU = new RoutedUICommand();
        public static RoutedUICommand CmdI = new RoutedUICommand();
        public static RoutedUICommand CmdO = new RoutedUICommand();
        public static RoutedUICommand CmdP = new RoutedUICommand();

        public static RoutedUICommand CmdA = new RoutedUICommand();
        public static RoutedUICommand CmdS = new RoutedUICommand();
        public static RoutedUICommand CmdD = new RoutedUICommand();
        public static RoutedUICommand CmdF = new RoutedUICommand();
        public static RoutedUICommand CmdG = new RoutedUICommand();
        public static RoutedUICommand CmdH = new RoutedUICommand();
        public static RoutedUICommand CmdJ = new RoutedUICommand();
        public static RoutedUICommand CmdK = new RoutedUICommand();
        public static RoutedUICommand CmdL = new RoutedUICommand();
        public static RoutedUICommand CmdEnter = new RoutedUICommand();

        public static RoutedUICommand CmdShift = new RoutedUICommand();
        public static RoutedUICommand CmdZ = new RoutedUICommand();
        public static RoutedUICommand CmdX = new RoutedUICommand();
        public static RoutedUICommand CmdC = new RoutedUICommand();
        public static RoutedUICommand CmdV = new RoutedUICommand();
        public static RoutedUICommand CmdB = new RoutedUICommand();
        public static RoutedUICommand CmdN = new RoutedUICommand();
        public static RoutedUICommand CmdNI = new RoutedUICommand();
        public static RoutedUICommand CmdM = new RoutedUICommand();

        public static RoutedUICommand CmdSpaceBar = new RoutedUICommand();
        public static RoutedUICommand CmdClear = new RoutedUICommand();

        public TouchScreenKeyboard()
        {
            this.Width = WidthTouchKeyboard;
            this.Height = 320;
        }

        static TouchScreenKeyboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TouchScreenKeyboard), new FrameworkPropertyMetadata(typeof(TouchScreenKeyboard)));

            SetCommandBinding();
        }
        #endregion
        #region CommandRelatedCode
        private static void SetCommandBinding()
        {
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
            CommandBinding CbBackspace = new CommandBinding(CmdBackspace, RunCommand);

            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb1);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb2);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb3);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb4);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb5);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb6);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb7);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb8);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb9);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cb0);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbBackspace);

            CommandBinding CbQ = new CommandBinding(CmdQ, RunCommand);
            CommandBinding Cbw = new CommandBinding(Cmdw, RunCommand);
            CommandBinding CbE = new CommandBinding(CmdE, RunCommand);
            CommandBinding CbR = new CommandBinding(CmdR, RunCommand);
            CommandBinding CbT = new CommandBinding(CmdT, RunCommand);
            CommandBinding CbY = new CommandBinding(CmdY, RunCommand);
            CommandBinding CbU = new CommandBinding(CmdU, RunCommand);
            CommandBinding CbI = new CommandBinding(CmdI, RunCommand);
            CommandBinding Cbo = new CommandBinding(CmdO, RunCommand);
            CommandBinding CbP = new CommandBinding(CmdP, RunCommand);

            CommandBinding CbA = new CommandBinding(CmdA, RunCommand);
            CommandBinding CbS = new CommandBinding(CmdS, RunCommand);
            CommandBinding CbD = new CommandBinding(CmdD, RunCommand);
            CommandBinding CbF = new CommandBinding(CmdF, RunCommand);
            CommandBinding CbG = new CommandBinding(CmdG, RunCommand);
            CommandBinding CbH = new CommandBinding(CmdH, RunCommand);
            CommandBinding CbJ = new CommandBinding(CmdJ, RunCommand);
            CommandBinding CbK = new CommandBinding(CmdK, RunCommand);
            CommandBinding CbL = new CommandBinding(CmdL, RunCommand);
            CommandBinding CbEnter = new CommandBinding(CmdEnter, RunCommand);

            CommandBinding CbShift = new CommandBinding(CmdShift, RunCommand);
            CommandBinding CbZ = new CommandBinding(CmdZ, RunCommand);
            CommandBinding CbX = new CommandBinding(CmdX, RunCommand);
            CommandBinding CbC = new CommandBinding(CmdC, RunCommand);
            CommandBinding CbV = new CommandBinding(CmdV, RunCommand);
            CommandBinding CbB = new CommandBinding(CmdB, RunCommand);
            CommandBinding CbN = new CommandBinding(CmdN, RunCommand);
            CommandBinding CbNI = new CommandBinding(CmdNI, RunCommand);
            CommandBinding CbM = new CommandBinding(CmdM, RunCommand);

            CommandBinding CbSpaceBar = new CommandBinding(CmdSpaceBar, RunCommand);
            CommandBinding CbClear = new CommandBinding(CmdClear, RunCommand);

            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbQ);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cbw);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbE);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbR);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbT);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbY);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbU);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbI);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), Cbo);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbP);

            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbA);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbS);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbD);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbF);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbG);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbH);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbJ);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbK);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbL);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbEnter);

            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbShift);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbZ);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbX);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbC);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbV);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbB);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbN);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbNI);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbM);

            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbSpaceBar);
            CommandManager.RegisterClassCommandBinding(typeof(TouchScreenKeyboard), CbClear);

        }
        static void RunCommand(object sender, ExecutedRoutedEventArgs e)
        {

            if (e.Command == Cmd1)
            {
                    TouchScreenKeyboard.TouchScreenText += "1";
            }
            else if (e.Command == Cmd2)
            {
                    TouchScreenKeyboard.TouchScreenText += "2";
            }
            else if (e.Command == Cmd3)
            {
                    TouchScreenKeyboard.TouchScreenText += "3";
            }
            else if (e.Command == Cmd4)
            {
                    TouchScreenKeyboard.TouchScreenText += "4";
            }
            else if (e.Command == Cmd5)
            {
                    TouchScreenKeyboard.TouchScreenText += "5";
            }
            else if (e.Command == Cmd6)
            {
                    TouchScreenKeyboard.TouchScreenText += "6";
            }
            else if (e.Command == Cmd7)
            {
                    TouchScreenKeyboard.TouchScreenText += "7";
            }
            else if (e.Command == Cmd8)
            {
                    TouchScreenKeyboard.TouchScreenText += "8";
            }
            else if (e.Command == Cmd9)
            {
                    TouchScreenKeyboard.TouchScreenText += "9";
            }
            else if (e.Command == Cmd0)
            {
                    TouchScreenKeyboard.TouchScreenText += "0";
            }
            else if (e.Command == CmdBackspace)
            {
                if (!string.IsNullOrEmpty(TouchScreenKeyboard.TouchScreenText))
                {
                    TouchScreenKeyboard.TouchScreenText = TouchScreenKeyboard.TouchScreenText.Substring(0, TouchScreenKeyboard.TouchScreenText.Length - 1);
                }
            }
            else if (e.Command == CmdQ)
            {
                AddKeyBoardINput('Q');
            }
            else if (e.Command == Cmdw)
            {
                AddKeyBoardINput('W');
            }
            else if (e.Command == CmdE)
            {
                AddKeyBoardINput('E');
            }
            else if (e.Command == CmdR)
            {
                AddKeyBoardINput('R');
            }
            else if (e.Command == CmdT)
            {
                AddKeyBoardINput('T');
            }
            else if (e.Command == CmdY)
            {
                AddKeyBoardINput('Y');
            }
            else if (e.Command == CmdU)
            {
                AddKeyBoardINput('U');

            }
            else if (e.Command == CmdI)
            {
                AddKeyBoardINput('I');
            }
            else if (e.Command == CmdO)
            {
                AddKeyBoardINput('O');
            }
            else if (e.Command == CmdP)
            {
                AddKeyBoardINput('P');
            }
            else if (e.Command == CmdA)
            {
                AddKeyBoardINput('A');
            }
            else if (e.Command == CmdS)
            {
                AddKeyBoardINput('S');
            }
            else if (e.Command == CmdD)
            {
                AddKeyBoardINput('D');
            }
            else if (e.Command == CmdF)
            {
                AddKeyBoardINput('F');
            }
            else if (e.Command == CmdG)
            {
                AddKeyBoardINput('G');
            }
            else if (e.Command == CmdH)
            {
                AddKeyBoardINput('H');
            }
            else if (e.Command == CmdJ)
            {
                AddKeyBoardINput('J');
            }
            else if (e.Command == CmdK)
            {
                AddKeyBoardINput('K');
            }
            else if (e.Command == CmdL)
            {
                AddKeyBoardINput('L');
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
            else if (e.Command == CmdShift) //Fourth Row
            {
                ShiftFlag = true; ;
            }
            else if (e.Command == CmdZ)
            {
                AddKeyBoardINput('Z');
            }
            else if (e.Command == CmdX)
            {
                AddKeyBoardINput('X');
            }
            else if (e.Command == CmdC)
            {
                AddKeyBoardINput('C');
            }
            else if (e.Command == CmdV)
            {
                AddKeyBoardINput('V');
            }
            else if (e.Command == CmdB)
            {
                AddKeyBoardINput('B');
            }
            else if (e.Command == CmdN)
            {
                AddKeyBoardINput('N');
            }
            else if (e.Command == CmdNI)
            {
                AddKeyBoardINput('Ñ');
            }
            else if (e.Command == CmdM)
            {
                AddKeyBoardINput('M');
            }
            else if (e.Command == CmdSpaceBar)//Last row
            {
                TouchScreenKeyboard.TouchScreenText += " ";
            }
            else if (e.Command == CmdClear)//Last row
            {
                TouchScreenKeyboard.TouchScreenText = "";
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

        //public static readonly DependencyProperty TouchScreenKeyboardProperty =
        //    DependencyProperty.RegisterAttached("TouchScreenKeyboard", typeof(bool), typeof(TouchScreenKeyboard), new UIPropertyMetadata(default(bool), TouchScreenKeyboardPropertyChanged));


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
                        ((Window)ct).LocationChanged += new EventHandler(TouchScreenKeyboard_LocationChanged);
                        ((Window)ct).Activated += new EventHandler(TouchScreenKeyboard_Activated);
                        ((Window)ct).Deactivated += new EventHandler(TouchScreenKeyboard_Deactivated);
                        break;
                    }
                    ct = (FrameworkElement)ct.Parent;
                }
                window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                _InstanceObject = new TouchScreenKeyboard();
                _InstanceObject.AllowsTransparency = true;
                _InstanceObject.WindowStyle = WindowStyle.None;
                _InstanceObject.ShowInTaskbar = false;
                _InstanceObject.ShowInTaskbar = false;
                _InstanceObject.Topmost = true;
                window.IsEnabled = false;
                host.LayoutUpdated += new EventHandler(tb_LayoutUpdated);
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
