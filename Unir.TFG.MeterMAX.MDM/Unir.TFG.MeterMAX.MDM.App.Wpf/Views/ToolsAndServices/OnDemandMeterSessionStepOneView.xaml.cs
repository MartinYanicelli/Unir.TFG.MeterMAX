using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.Views.ToolsAndServices
{
    /// <summary>
    /// Interaction logic for OnDemandMeterSessionStepOneView.xaml
    /// </summary>
    public partial class OnDemandMeterSessionStepOneView : UserControl
    {
        public OnDemandMeterSessionStepOneView()
        {
            InitializeComponent();
            Loaded += OnDemandMeterSessionStepOneView_Loaded;
        }

        private void OnDemandMeterSessionStepOneView_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ViewModels.OnDemandMeterSessionViewModel;
        }
    }
}
