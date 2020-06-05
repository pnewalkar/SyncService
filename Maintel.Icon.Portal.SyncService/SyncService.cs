using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Maintel.Icon.Portal.SyncService
{
    partial class SyncService : ServiceBase
    {
        private Timer _timer;
        private int _interval = 30;
        private readonly ILogger _logger;
        private readonly IConfigurationRoot _configuration;
        private HighlightSyncService _highlightSyncService;
        private AutotaskSyncService _autotaskSyncService;

        public SyncService()
        {
            InitializeComponent();
            //Build a configurationBuilder object which can be understood by .net standard
            _configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", optional: true)
                 .Build();
            //Set up Serilog Settings 
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(_configuration["Log:Location"],
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 10)
                .CreateLogger();

            if (!int.TryParse(_configuration["Sync:Interval"], out _interval))
            {
                _interval = 30;
                _logger.Information($"Started Sync with a default interval of 30 seconds");
            }
            else
            {
                _logger.Information($"Started Sync with an interval of {_interval} seconds");
            }


        }

        protected override void OnStart(string[] args)
        {
            _timer = new Timer();
            _timer.Interval = _interval * 1000;
            _timer.Elapsed += new ElapsedEventHandler(this.InstanciateSync);
            _logger.Information("Starting Sync Service");
            _timer.Start();
        }
        protected void InstanciateSync(object sender, ElapsedEventArgs args)
        {

            _timer.Stop();
            startSync();
            _timer.Start();
        }

        private void startSync()
        {
            _highlightSyncService = new HighlightSyncService(_logger, _configuration);
            _logger.Information("Attempting Sync");

            try
            {
                _highlightSyncService.RefreshAssociations();
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Error occured running Highlight Associations Service");
            }

            try
            {
                _highlightSyncService.Sync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex,"Error occured running Highlight Sync Service");
            }

            _highlightSyncService = null;
            _autotaskSyncService = new AutotaskSyncService(_logger, _configuration);
            try
            {
                _autotaskSyncService.Sync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occured running Autotask Sync Service");
            }

            _autotaskSyncService = null;

            _logger.Information("Compeleted Sync");
        }
        protected override void OnStop()
        {
            _timer.Stop();
        }
    }
}
