using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels
{
    public class SelectableViewModel<TModel> : Noanet.XamArch.Mvvm.ObservableObject
        where TModel : Noanet.XamArch.Domain.Entity
    {
        private bool isSelected;
        private TModel model;

        public bool IsSelected
        {
            get => isSelected;
            set {
                if (isSelected == value) return;
                SetValue(ref isSelected, value);
            } 
        }

        public TModel Model
        {
            get => model;
            set => SetValue(ref model, value);
        }
    }
}
