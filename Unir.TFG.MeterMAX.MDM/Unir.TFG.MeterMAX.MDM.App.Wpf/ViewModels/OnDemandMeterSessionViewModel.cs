using MaterialDesignExtensions.Model;
using Noanet.XamArch.Domain;
using Noanet.XamArch.Logic;
using Noanet.XamArch.Mvvm.Command;
using Noanet.XamArch.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unir.TFG.MeterMAX.MDM.Domain;
using Unir.TFG.MeterMAX.MDM.Domain.Enumerations;
using Unir.TFG.MeterMAX.MDM.Logic;
using Unir.TFG.MeterMAX.MDM.Repository.Interfaces;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels
{
    public class OnDemandMeterSessionViewModel : Noanet.XamArch.Mvvm.ViewModelBase
    {
        private IEnumerable<MeterSesstionTypeViewModel> meterSessionTypes;
        private IEnumerable<Meter> meters;
        private Meter selectedMeter;
        private MeterSesstionTypeViewModel selectedMeterSessionType;
        private MeterSessionSetting meterSessionSetting;
        private IEnumerable<DataSet> dataSets;
        private DataSet selectedDataSet;
        private IEnumerable<DataSetComponentViewModel> dataSetComponents;
        private readonly MeterSessionManager meterSessionManager;

        public enum MeterSessionRunStates {
            Idle,
            Starting,
            Running,
            Ended
        }
        
        private readonly IMeterSessionTypeRepository meterSessionTypeRepository;
        private readonly IMeterRepository meterRepository;
        private readonly IInsituMeterSessionSettingRepository insituMeterSessionSettingRepository;
        private readonly IRemoteMeterSessionSettingRepository remoteMeterSessionSettingRepository;
        private readonly IDataSetRepository dataSetRepository;
        private readonly IDataSetComponentRepository dataSetComponentRepository;

        private int currentStep;

        public AsyncCommand LoadStepOneDataCommand { get; }
        public AsyncCommand LoadStepTwoDataCommand { get; }
        public AsyncCommand LoadStepThreeDataCommand { get; }
        public Command GoToStepFourCommand { get; }
        public AsyncCommand StartMeterSessionCommand { get; }

        public bool CanMoveToStepOne => !IsBusy;
        public bool CanMoveToStepTwo => !IsBusy && (selectedMeter != null) && (selectedMeterSessionType != null);
        public bool CanMoveToStepThree => !IsBusy;
        //public bool CanMoveToStepThree => !IsBusy && (meterSessionSetting != null) &&
        //    (meterSessionSetting.ProtocolSetting != null) &&
        //    !string.IsNullOrWhiteSpace(meterSessionSetting.ProtocolSetting.UserName) &&
        //    !string.IsNullOrWhiteSpace(meterSessionSetting.ProtocolSetting.Password);

        public bool CanMoveToStepFour => !IsBusy && (SelectedDataSetComponents.Count > 0);

        public int CurrentStep
        {
            get => currentStep;
            private set => SetValue(ref currentStep, value);
        }

        public IEnumerable<MeterSesstionTypeViewModel> MeterSessionTypes
        {
            get => meterSessionTypes;
            set => SetValue(ref meterSessionTypes, value);
        }

        public MeterSesstionTypeViewModel SelectedMeterSessionType
        {
            get => selectedMeterSessionType;
            set
            {
                SetValue(ref selectedMeterSessionType, value);
                LoadStepTwoDataCommand.ChangeCanExecute();
            }
        }

        public Meter SelectedMeter
        {
            get => selectedMeter;
            set
            {
                SetValue(ref selectedMeter, value);
                LoadStepTwoDataCommand.ChangeCanExecute();
            }
        }

        public IEnumerable<Meter> Meters
        {
            get => meters;

            set => SetValue(ref meters, value);
        }

        public MeterSessionSetting MeterSessionSetting
        {
            get => meterSessionSetting;
            set => SetValue(ref meterSessionSetting, value);
        }

        public IEnumerable<DataSet> DataSets
        {
            get => dataSets;
            set => SetValue(ref dataSets, value);
        }

        public DataSet SelectedDataSet
        {
            get => selectedDataSet;
            set => SetValue(ref selectedDataSet, value);
        }

        public IEnumerable<DataSetComponentViewModel> DataSetComponents
        {
            get => dataSetComponents;
            set => SetValue(ref dataSetComponents, value);
        }

        public MeterSessionRunStates MeterSessionRunState { get; private set; }

        public MeterSessionProgressViewModel MeterSessionProgressViewModel { get; }
        public MeterSessionSummaryViewModel MeterSessionSummaryViewModel { get; }

        //public ObservableCollection<DataSetComponent> SelectedDataSetComponents { get; }
        public ObservableCollection<DataSetComponent> SelectedDataSetComponents { get; }

        public IEnumerable<MeterSessionTask> MeterSessionTasks => meterSessionManager.CurrentMeterSession?.SessionTasks.Values; 
        public string MeterSessionTrace { get; private set; }
        public string MeterSessionStatus { get; private set; }
        
        public OnDemandMeterSessionViewModel(IMeterSessionTypeRepository meterSessionTypeRepository, IMeterRepository meterRepository,
            IInsituMeterSessionSettingRepository insituMeterSessionSettingRepository,
            IRemoteMeterSessionSettingRepository remoteMeterSessionSettingRepository,
            IDataSetRepository dataSetRepository, IDataSetComponentRepository dataSetComponentRepository,
            MeterSessionManager meterSessionManager, MeterSessionProgressViewModel meterSessionProgressViewModel, MeterSessionSummaryViewModel meterSessionSummaryViewModel)
            : base()
        {
            this.meterSessionTypeRepository = meterSessionTypeRepository ?? throw new ArgumentNullException(nameof(meterSessionTypeRepository));
            this.meterRepository = meterRepository ?? throw new ArgumentNullException(nameof(meterRepository));
            this.insituMeterSessionSettingRepository = insituMeterSessionSettingRepository ?? throw new ArgumentNullException(nameof(insituMeterSessionSettingRepository));
            this.remoteMeterSessionSettingRepository = remoteMeterSessionSettingRepository ?? throw new ArgumentNullException(nameof(remoteMeterSessionSettingRepository));
            this.dataSetRepository = dataSetRepository ?? throw new ArgumentNullException(nameof(dataSetRepository));
            this.dataSetComponentRepository = dataSetComponentRepository ?? throw new ArgumentNullException(nameof(dataSetComponentRepository));
            this.meterSessionManager = meterSessionManager ?? throw new ArgumentNullException(nameof(meterSessionManager));
            MeterSessionProgressViewModel = meterSessionProgressViewModel ?? throw new ArgumentNullException(nameof(meterSessionProgressViewModel)); ;
            MeterSessionSummaryViewModel = meterSessionSummaryViewModel ?? throw new ArgumentNullException(nameof(meterSessionSummaryViewModel)); ;

            LoadStepOneDataCommand = new AsyncCommand(async () => await LoadStepOneDataAsync(), () => CanMoveToStepOne);
            LoadStepTwoDataCommand = new AsyncCommand(async () => await LoadDefaultSettingAsync(), () => CanMoveToStepTwo);
            LoadStepThreeDataCommand = new AsyncCommand(async () => await LoadStepThreeDataAsync(), () => CanMoveToStepThree);
            GoToStepFourCommand = new Command(GoToStepFour, () => CanMoveToStepFour);
            StartMeterSessionCommand = new AsyncCommand(async () => await StartMeterSessionAsync(), () => !IsBusy);

            SelectedDataSetComponents = new ObservableCollection<DataSetComponent>();

            CurrentStep = 0;
            //Steps = new IStep[] {
            //    new WizardStep<OnDemandMeterSessionWizardOneViewModel>("Medidor", "Seleccione Medidor y Tipo de Sesión", ServiceLocator.Current.GetInstance<OnDemandMeterSessionWizardOneViewModel>()),
            //    new WizardStep<OnDemandMeterSessionWizardTwoViewModel>("Configuración", "Seleccione y ajuste los Parámetros de Sesión", ServiceLocator.Current.GetInstance<OnDemandMeterSessionWizarViewModel>()),
            //    new WizardStep<OnDemandMeterSessionWizardThreeViewModel>("Creación de Tarea", "Confeccione la nueva Tarea", ServiceLocator.Current.GetInstance<OnDemandMeterSessionWizardThreeViewModel>()),
            //    new WizardStep<OnDemandMeterSessionWizardFourViewModel>("Confirmación", "Confirme y ejecute la nueva Tarea", ServiceLocator.Current.GetInstance<OnDemandMeterSessionWizardFourViewModel>()),
            //};
        }

        private async Task LoadStepOneDataAsync()
        {
            var operationResult = await TryExecuteAsync(Task.Run(() =>
            {
                try
                {
                    ValidationResult validationResult = ValidationResult.Success;
                    meterRepository.DbContext.OpenConnection();
                    Meters = meterRepository.GetAll();
                    MeterSessionTypes = meterSessionTypeRepository.GetAll().Select(x => new MeterSesstionTypeViewModel() { MeterSessionType = x, IconKind = x.Id == (int)MeterSessionTypeCode.Insitu ? "Teacher" : "Wireless" });
                    if (Meters.Count() == 0 || MeterSessionTypes.Count() == 0)
                    {
                        validationResult = new ValidationResult("El Sistema no registra información básica para poder iniciar el proceso de inicio de sesión bajo demanda.");
                        if (Meters.Count() == 0)
                        {
                            validationResult.MemberNames.Add("Medidores", "El Sistema no tiene cargado ningún Medidor.");
                        }
                        if (MeterSessionTypes.Count() == 0)
                        {
                            validationResult.MemberNames.Add("Tipos de Sesión", "El Sistema no tiene cargado ningún Tipo de Sesión de Comunicación");
                        }
                    }
                    return validationResult;
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    meterRepository.DbContext.CloseConnection();
                }

            }));
            if (operationResult.IsSuccess)
            {

                if (operationResult.Value == ValidationResult.Success)
                {
                    CurrentStep = 1;
                }
                else
                {
                    await dialogService.ShowValidationMessage(operationResult.Value.ErrorMessage, "¡ATENCIÓN!");
                    foreach (var item in operationResult.Value.MemberNames)
                    {
                        await dialogService.ShowValidationMessage($"{item.Key}: {item.Value}", "¡ATENCIÓN!");
                    }
                }
            }
        }

        public async Task LoadDefaultSettingAsync()
        {
            var operationResult = await TryExecuteAsync(Task.Run(() => {
                try
                {
                    ValidationResult validation = ValidationResult.Success;
                    var meterSessionTypeCode = (MeterSessionTypeCode)SelectedMeterSessionType.MeterSessionType.Id;
                    if ((meterSessionTypeCode == MeterSessionTypeCode.Remote) && (selectedMeter.RemoteDevice == null))
                        throw new InvalidOperationException("El Medidor seleccionado no soporta este tipo de Sesión de Comunicación");

                    insituMeterSessionSettingRepository.DbContext.OpenConnection();
                    MeterSessionSetting = selectedMeterSessionType.MeterSessionType.Id == (int)MeterSessionTypeCode.Insitu ? insituMeterSessionSettingRepository.FindOne(new EntityPropertyInfo(nameof(MeterSessionSetting.Name), $"Default|{selectedMeterSessionType.MeterSessionType.Name}"))
                    : (MeterSessionSetting)remoteMeterSessionSettingRepository.FindOne(new EntityPropertyInfo(nameof(MeterSessionSetting.Name), $"Default|{selectedMeterSessionType.MeterSessionType.Name}"));
                    if (MeterSessionSetting == null)
                    {
                        validation = new ValidationResult("El Sistema no registra ninguna Configuración por defecto, se procederá a crear una.");
                        MeterSessionSetting = CreateDefaultMeterSessionSetting(SelectedMeterSessionType.MeterSessionType);
                    }
                    return validation;

                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    insituMeterSessionSettingRepository.DbContext.CloseConnection();
                }
            }));

            if (operationResult.IsSuccess)
            {
                if (operationResult.Value == ValidationResult.Success)
                {
                    CurrentStep = 2;
                }
                else
                {
                    await dialogService.ShowValidationMessage(operationResult.Value.ErrorMessage, "¡ATENCIÓN!");

                }
            }
        }

        public async Task LoadStepThreeDataAsync()
        {
            var operationResult = await TryExecuteAsync(Task.Run(() => {
                try
                {
                    var validationResult = ValidationResult.Success;
                    dataSetRepository.DbContext.OpenConnection();
                    DataSets = dataSetRepository.GetAll();
                    DataSetComponents = dataSetComponentRepository.GetAll().Select(delegate (DataSetComponent model) {
                        var dataSetComponentViewModel = new DataSetComponentViewModel() { Model = model };
                        dataSetComponentViewModel.PropertyChanged += DataSetComponentViewModel_PropertyChanged;
                        return dataSetComponentViewModel;
                    }); 

                    if (dataSetComponents.Count() == 0)
                        throw new InvalidOperationException("El Sistema no registra ningún Componente para poder crear una Tarea Programada.");

                    if (dataSets.Count() == 0)
                    {
                        validationResult = new ValidationResult("El Sistema no tiene registrado ningún Conjunto de Datos predefinidos");
                    }

                    SelectedDataSet = new DataSet() { Name = "DataSetOnDemand", Description = "DataSet creado por petición Bajo Demanda" };

                    return validationResult;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    dataSetRepository.DbContext.CloseConnection();
                }
            }));

            if (operationResult.IsSuccess)
            {
                if (operationResult.Value != ValidationResult.Success)
                {
                    await dialogService.ShowValidationMessage(operationResult.Value.ErrorMessage, "¡ATENCIÓN!");
                }
                CurrentStep = 3;
            }
        }

        public void GoToStepFour()
        {
            SelectedDataSet.DataSetComponents = SelectedDataSetComponents.ToList();
            CurrentStep = 4;
            MeterSessionRunState = MeterSessionRunStates.Idle;
            OnPropertyChanged(nameof(MeterSessionRunState));
        }

        private async Task StartMeterSessionAsync()
        {
            if (IsBusy) return;

            meterSessionManager.SessionStarted += (sender, e) => {
                MeterSessionRunState = MeterSessionRunStates.Running;
                OnPropertyChanged(nameof(MeterSessionRunState));
            };

            meterSessionManager.SessionEnded += (sender, e) => {
                Application.Current.Dispatcher.InvokeAsync(() => {
                    MeterSessionSummaryViewModel.MeterSession = e.MeterSession;
                    MeterSessionSummaryViewModel.HasResume = e.DataSetExecutionQuality != DataSetExecutionQuality.Critical;
                    MeterSessionSummaryViewModel.ProgressPercent = e.DataSetExecutionPercent;
                    MeterSessionSummaryViewModel.Quality = (int)e.DataSetExecutionQuality;
                    MeterSessionSummaryViewModel.BytesSentAndReceived = $"{e.TotalBytesSent}/{e.TotalBytesReceived}";
                    MeterSessionSummaryViewModel.ResultMessage = e.DataSetExecutionSuccess ? "¡ENHORABUENA! Cronograma ejecutado Satisfactoriamente"
                    : (e.DataSetExecutionQuality != DataSetExecutionQuality.Critical) ? $"¡ATENCIÓN! El Cronograma solo pudo ejecutarse el {e.DataSetExecutionPercent} %"
                    : "¡ERROR! No se pudo ejecutar el Cronograma de Tareas";
                    OnPropertyChanged(nameof(MeterSessionSummaryViewModel));
                    MeterSessionRunState = MeterSessionRunStates.Ended;
                    OnPropertyChanged(nameof(MeterSessionRunState));
                    IsBusy = false;
                    StartMeterSessionCommand.ChangeCanExecute();
                });
            };

            meterSessionManager.SessionProgressChanged += (sender, e) =>
            {
                Application.Current.Dispatcher.InvokeAsync(() => {
                    MeterSessionProgressViewModel.MeterSessionTask = e.SessionTask;
                    MeterSessionProgressViewModel.ProgressPercent = e.PercentAdvance;
                    MeterSessionProgressViewModel.ElapsedTime = e.ElapsedTime;

                    OnPropertyChanged(nameof(MeterSessionProgressViewModel));
                });
            };

            meterSessionManager.SessionStatusChanged += (sender, e) => {
                if (e.SessionStatus == Domain.MeterSessionStatus.StartingSession)
                {
                    OnPropertyChanged(nameof(MeterSessionTasks));
                }
                MeterSessionStatus = e.SessionStatus.ToString();
                MeterSessionTrace = e.CurrentSessionTrace;
                OnPropertyChanged(nameof(MeterSessionTrace));
                OnPropertyChanged(nameof(MeterSessionStatus));
            };

            meterSessionManager.ReconnectionCountdownChanged += (sender, e) =>
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MeterSessionStatus = $"#Esperando {e.RemainingTime.ToString(@"hh\:mm\:ss")} para el próximo reintento de conexión automática.";
                    OnPropertyChanged(nameof(MeterSessionStatus));
                });
            };

            MeterSessionRunState = MeterSessionRunStates.Starting;
            OnPropertyChanged(nameof(MeterSessionRunState));
            IsBusy = true;
            StartMeterSessionCommand.ChangeCanExecute();

            await meterSessionManager.StartAsync(selectedMeter, selectedDataSet, meterSessionSetting);
        }

        private void DataSetComponentViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var dataSetComponentViewModel = (DataSetComponentViewModel)sender;
            if (e.PropertyName == nameof(DataSetComponentViewModel.IsSelected))
            {
                if (dataSetComponentViewModel.IsSelected)
                {
                    SelectedDataSetComponents.Add(dataSetComponentViewModel.Model);
                }
                else
                {
                    SelectedDataSetComponents.Remove(dataSetComponentViewModel.Model);
                }

                GoToStepFourCommand.ChangeCanExecute();
            }
        }

        public MeterSessionSetting CreateDefaultMeterSessionSetting(MeterSessionType meterSessionType)
        {
            return new MeterSessionSetting() { Name = $"Default|{meterSessionType.Name}", InternalReconnectionAttempts = 1 };
        }
    }

    public class DataSetComponentViewModel : SelectableViewModel<DataSetComponent>
    {

    }

    public class WizardStep<TViewModel> : Step
            where TViewModel : Noanet.XamArch.Mvvm.ViewModelBase, IWizardViewModel
    {

        public WizardStep(string firstLevelTitle, string secondLevelTitle, TViewModel viewModel)
        {
            Header = new StepTitleHeader() { FirstLevelTitle = firstLevelTitle, SecondLevelTitle = secondLevelTitle };
            Content = viewModel;
        }

        public WizardStep()
            : base()
        {

        }

        public override void Validate()
        {
            base.HasValidationErrors = !(base.Content as TViewModel)?.CanMoveToNextStep ?? false;
        }
    }
}
