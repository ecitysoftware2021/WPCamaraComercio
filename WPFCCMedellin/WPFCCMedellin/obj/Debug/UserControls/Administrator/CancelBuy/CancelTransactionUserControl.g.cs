﻿#pragma checksum "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "330FB4958F103386B9700ACCE5990EB02CADE29F911FF4BA4B7D76859B713CF6"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WPFCCMedellin.UserControls.Administrator.CancelBuy;


namespace WPFCCMedellin.UserControls.Administrator.CancelBuy {
    
    
    /// <summary>
    /// CancelTransactionUserControl
    /// </summary>
    public partial class CancelTransactionUserControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 48 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtIdCompra;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtValorCompra;
        
        #line default
        #line hidden
        
        
        #line 128 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtReferenciaPago;
        
        #line default
        #line hidden
        
        
        #line 168 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtObservaciones;
        
        #line default
        #line hidden
        
        
        #line 187 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image btn_consult;
        
        #line default
        #line hidden
        
        
        #line 198 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image btn_Back;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WPFCCMedellin;component/usercontrols/administrator/cancelbuy/canceltransactionus" +
                    "ercontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.txtIdCompra = ((System.Windows.Controls.TextBox)(target));
            
            #line 46 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
            this.txtIdCompra.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.txtform_TouchDown);
            
            #line default
            #line hidden
            
            #line 47 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
            this.txtIdCompra.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtIdCompra_TextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txtValorCompra = ((System.Windows.Controls.TextBox)(target));
            
            #line 86 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
            this.txtValorCompra.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.txtform_TouchDown);
            
            #line default
            #line hidden
            
            #line 87 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
            this.txtValorCompra.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtValorCompra_TextChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.txtReferenciaPago = ((System.Windows.Controls.TextBox)(target));
            
            #line 126 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
            this.txtReferenciaPago.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.txtform_TouchDown);
            
            #line default
            #line hidden
            
            #line 127 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
            this.txtReferenciaPago.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtReferenciaPago_TextChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.txtObservaciones = ((System.Windows.Controls.TextBox)(target));
            
            #line 166 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
            this.txtObservaciones.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.txtObservaciones_TouchDown);
            
            #line default
            #line hidden
            
            #line 167 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
            this.txtObservaciones.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.txtObservaciones_TextChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btn_consult = ((System.Windows.Controls.Image)(target));
            
            #line 196 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
            this.btn_consult.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnConsult_TouchDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btn_Back = ((System.Windows.Controls.Image)(target));
            
            #line 205 "..\..\..\..\..\UserControls\Administrator\CancelBuy\CancelTransactionUserControl.xaml"
            this.btn_Back.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnBack_TouchDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
