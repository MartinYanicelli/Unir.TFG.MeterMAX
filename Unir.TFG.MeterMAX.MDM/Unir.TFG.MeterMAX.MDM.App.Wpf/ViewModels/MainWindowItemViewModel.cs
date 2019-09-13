using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels
{
    public class MainWindowItemViewModel : Noanet.XamArch.Mvvm.ViewModelBase
    {
        private string name;
        private object content;
        private string iconKind;

        private ScrollBarVisibility horizontalScrollBarVisibilityRequirement;
        private ScrollBarVisibility verticalScrollBarVisibilityRequirement;
        private Thickness marginRequirement = new Thickness(16);

        public MainWindowItemViewModel(string iconKind, string name, object content)
            : base()
        {
            Name = name;
            Content = content;
            IconKind = iconKind;
        }

        public string Name
        {
            get => name;
            set => SetValue(ref name, value);
        }

        public object Content
        {
            get => content;
            set => SetValue(ref content, value);
        }

        public string IconKind
        {
            get => iconKind;
            set => SetValue(ref iconKind, value);
        }

        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get => horizontalScrollBarVisibilityRequirement;
            set => SetValue(ref horizontalScrollBarVisibilityRequirement, value);
        }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get => verticalScrollBarVisibilityRequirement;
            set => SetValue(ref verticalScrollBarVisibilityRequirement, value);
        }

        public Thickness MarginRequirement
        {
            get => marginRequirement;
            set => SetValue(ref marginRequirement, value);
        }
    }
}
