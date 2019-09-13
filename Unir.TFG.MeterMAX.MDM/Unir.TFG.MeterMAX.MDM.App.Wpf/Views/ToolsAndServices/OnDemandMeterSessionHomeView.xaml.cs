using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using Noanet.XamArch.Mvvm.Services;
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
using Unir.TFG.MeterMAX.MDM.App.Wpf.Extensions;
using Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.Views.ToolsAndServices
{
    /// <summary>
    /// Interaction logic for OnDemandMeterSessionHomeView.xaml
    /// </summary>
    public partial class OnDemandMeterSessionHomeView : UserControl
    {
        public OnDemandMeterSessionHomeView()
        {
            InitializeComponent();

            DataContext = ServiceLocator.Current.GetInstance<OnDemandMeterSessionViewModel>();
        }
    }
}
