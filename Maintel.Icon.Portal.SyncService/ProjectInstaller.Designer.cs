namespace Maintel.Icon.Portal.SyncService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.spInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // spInstaller
            // 
            this.spInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.spInstaller.Password = null;
            this.spInstaller.Username = null;
            this.spInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller1_AfterInstall);
            // 
            // serviceInstaller
            // 
            this.serviceInstaller.Description = "Maintel Sync Service";
            this.serviceInstaller.DisplayName = "Maintel.SyncService";
            this.serviceInstaller.ServiceName = "Maintel.SyncService";
            this.serviceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.spInstaller,
            this.serviceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller spInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;
    }
}