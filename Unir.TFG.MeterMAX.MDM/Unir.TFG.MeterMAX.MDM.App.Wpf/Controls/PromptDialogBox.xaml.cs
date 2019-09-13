using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for PromptDialogBox.xaml
    /// </summary>
    public partial class PromptDialogBox : UserControl
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public string OkText
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public string CancelText
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }


        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(PromptDialogBox), new PropertyMetadata("¡ATENCIÓN!"));

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(nameof(Caption), typeof(string), typeof(PromptDialogBox), new PropertyMetadata("Escriba aquí el texto que desea mostrar al usuario"));

        public static readonly DependencyProperty OkTextProperty =
            DependencyProperty.Register(nameof(OkText), typeof(string), typeof(PromptDialogBox), new PropertyMetadata("ACEPTAR"));

        public static readonly DependencyProperty CancelTextProperty =
            DependencyProperty.Register(nameof(CancelText), typeof(string), typeof(PromptDialogBox), new PropertyMetadata("CANCELAR"));


        public PromptDialogBox()
        {
            InitializeComponent();
        }
    }
}
