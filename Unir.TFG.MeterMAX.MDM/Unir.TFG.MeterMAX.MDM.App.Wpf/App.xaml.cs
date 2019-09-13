using Noanet.XamArch.Mvvm.IoC.TinyIoC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PubSub.Extension;
using System.Windows;

namespace Unir.TFG.MeterMAX.MDM.App.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly TinyIoCContainer container;
        static string AppName => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        string AppDbFolderPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases"); // Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Databases");
        string AppLogFolderPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"); // Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Logs");

        string DbName => Noanet.XamArch.Configuration.ConfigurationManager.Current.GetConnectionStringSettings()?.First()?.DbName ?? throw new InvalidOperationException("No se puede determinar el nombre de la base de datos");

        string DbResourceName => "db-template.db";

        public App() 
            : base()
        {
            InitializeComponent();

            container = new TinyIoCContainer();
            var serviceLocator = new Noanet.XamArch.Mvvm.Services.TinyIoCServiceLocator(container);
            Noanet.XamArch.Mvvm.Services.ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!Directory.Exists(AppDbFolderPath))
            {
                Directory.CreateDirectory(AppDbFolderPath);
            }

            if (!Directory.Exists(AppLogFolderPath))
            {
                Directory.CreateDirectory(AppLogFolderPath);
            }

            var dbFullPath = Path.Combine(AppDbFolderPath, DbName);
            if (!File.Exists(dbFullPath))
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(DbResourceName)) ?? throw new InvalidOperationException($"La Aplicación no encuentra la plantilla de base de datos necesaria para su inicialización {DbResourceName}");
                using (Stream buffer = assembly.GetManifestResourceStream(resourceName))
                {
                    using (FileStream fs = new FileStream(dbFullPath, FileMode.Create))
                    {
                        buffer.CopyTo(fs);
                    }
                }
            }

            RegisterServicesAndProviders();

            this.Subscribe<Logic.Validations.AuthenticationResult>(authenticationResult => {
                var mainWindow = authenticationResult.UserSession != null ? new Views.MainWindow() : (Window)new Views.LoginWindow();
                mainWindow.Show();
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            this.Unsubscribe<Logic.Validations.AuthenticationResult>();
            base.OnExit(e);

        }

        private void RegisterServicesAndProviders()
        {
            container.Register<Noanet.XamArch.Mvvm.Services.IDialogService, Services.DialogService>();

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var repositoryAssembly = loadedAssemblies.SingleOrDefault(x => x.GetName().Name == "Unir.TFG.MeterMAX.MDM.Repository") ?? System.Reflection.Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "Unir.TFG.MeterMAX.MDM.Repository.dll"));
            var logicAssembly = loadedAssemblies.SingleOrDefault(x => x.GetName().Name == "Unir.TFG.MeterMAX.MDM.Logic") ?? System.Reflection.Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "Unir.TFG.MeterMAX.MDM.Logic.dll"));
            
            container.AutoRegister(new System.Reflection.Assembly[] { repositoryAssembly, logicAssembly });
            container.AutoRegister(new System.Reflection.Assembly[] { System.Reflection.Assembly.GetExecutingAssembly() }, x => x.Namespace == "Unir.TFG.MeterMAX.MDM.App.Wpf.ViewModels" || x.Namespace == "Unir.TFG.MeterMAX.MDM.App.Wpf.Services");
            InitializeLogginService();
        }

        private void InitializeLogginService()
        {
            var logConfig = new NLog.Config.LoggingConfiguration();
            var logFilePath = Path.Combine(AppLogFolderPath, $"noanet.{AppName.ToLower()}.log");
            var logArchiveFileName = $"noanet.{AppName.ToLower()}.{{#}}.log";
            var logFileTarget = new NLog.Targets.FileTarget()
            {
                Name = "logfile",
                Layout = "${longdate} ${logger} ${message}${exception:format=ToString}",
                FileName = logFilePath,
                ArchiveFileName = Path.Combine(AppLogFolderPath, logArchiveFileName),
                MaxArchiveFiles = 3,
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Rolling,
                ConcurrentWrites = false // If only single process (and single AppDomain) application is logging, then it is faster to set to concurrentWrites = False
            };
#if DEBUG
            var minLogLevel = NLog.LogLevel.Debug;
#else
            var minLogLevel = NLog.LogLevel.Error;
#endif
            logConfig.LoggingRules.Add(new NLog.Config.LoggingRule("*", minLogLevel, logFileTarget));
            NLog.LogManager.Configuration = logConfig;
        }

    }
}
