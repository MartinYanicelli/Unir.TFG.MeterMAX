using MaterialDesignExtensions.Controls;
using MaterialDesignExtensions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels
{
    public abstract class StepperViewModelBase : Noanet.XamArch.Mvvm.ViewModelBase
    {
        protected StepperLayout stepperLayout;
        protected bool isLinear;
        protected bool contentAnimationsEnabled;
        protected bool blockNavigationOnValidationErrors;

        public IEnumerable<IStep> Steps {
            get; protected set;
        }

        public StepperLayout StepperLayout
        {
            get => stepperLayout;
            set => SetValue(ref stepperLayout, value);
        }

        public bool IsLinear
        {
            get => isLinear;
            set => SetValue(ref isLinear, value);
        }

        public bool ContentAnimationsEnaled
        {
            get => contentAnimationsEnabled;
            set => SetValue(ref contentAnimationsEnabled, value);
        }

        public bool BlockNavigationOnValidationErros
        {
            get => blockNavigationOnValidationErrors;
            set => SetValue(ref blockNavigationOnValidationErrors, value);
        }

        protected StepperViewModelBase()
        {
            isLinear = true;
            contentAnimationsEnabled = true;
            stepperLayout = StepperLayout.Horizontal;
            blockNavigationOnValidationErrors = true;
        }
    }
}
