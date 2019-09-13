using Noanet.XamArch.Mvvm.Services;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels
{
    public class ToolsAndServicesViewModel : Noanet.XamArch.Mvvm.ObservableObject
    {
        public IEnumerable<TabItemViewModel> TabItems { get; private set; }

        public ToolsAndServicesViewModel()
        {
            TabItems = new TabItemViewModel[] {
                new TabItemViewModel() { HeaderIconKind = "Powershell", HeaderTitle="TAREA BAJO DEMANDA", Content = new Views.ToolsAndServices.OnDemandMeterSessionHomeView() },
                new TabItemViewModel() { HeaderIconKind = "CalendarMultiselect", HeaderTitle="TAREAS PROGRAMADAS", Content = new Views.ToolsAndServices.ScheduleTasksView() },
                new TabItemViewModel() { HeaderIconKind = "ClockFast", HeaderTitle="TAREAS PROGRAMAS ACTIVAS", Content = new Views.ToolsAndServices.ScheduleTasksRunView() },
                

            };
        }
    }
}
