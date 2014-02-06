using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Threading;

namespace Yuhan.Deployment
{
    /// <summary>
    /// Click Once Deployment의 Updater 모듈입니다.
    /// Update 확인 및 Update 기능을 제공 합니다.
    /// </summary>
    public static class Updater
    {
        /// <summary>
        /// Update Checking Interval입니다.
        /// </summary>
        public static TimeSpan Interval = new TimeSpan(0, 0, 0, 5);

        private static BackgroundWorker _worker;

        private static BackgroundWorker Worker 
        {
            get
            {
                if (_worker == null)
                {
                    _worker = new BackgroundWorker();
                    _worker.DoWork += _worker_DoWork;
                    _worker.WorkerSupportsCancellation = true;
                }
                return _worker;
            }
            set { _worker = value; }
        }

        static void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!Worker.CancellationPending)
            {
                try
                {
                    if (!IsDownloading)
                        InstallUpdateSyncWithInfo();
                }
                catch (Exception ex)
                {
                    OnUpdateChecked(ex);
                }
                Thread.Sleep(Interval);
            }
        }

        public static Boolean SkipToUpdate = false;

        public static Boolean IsDownloading = false;

        private static void InstallUpdateSyncWithInfo()
        {
            UpdateCheckInfo info = null;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                
                try
                {
                    info = ad.CheckForDetailedUpdate();

                }
                catch (DeploymentDownloadException dde)
                {
                    OnUpdateChecked(
                        new Exception(
                            "The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " +
                            dde.Message));
                    return;
                }
                catch (InvalidDeploymentException ide)
                {
                    OnUpdateChecked(
                        new Exception(
                            "Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " +
                            ide.Message));
                    return;
                }
                catch (InvalidOperationException ioe)
                {
                    OnUpdateChecked(
                        new Exception(
                            "This application cannot be updated. It is likely not a ClickOnce application. Error: " +
                            ioe.Message));
                    return;
                }

                if (info != null && info.UpdateAvailable)
                {
                    if (!info.IsUpdateRequired)
                    {
                        OnUpdateChecked(UpdateStateType.UpdateAvailable,
                            "An update is available. Would you like to update the application now?");
                    }
                    else
                    {
                        // Display a message that the app MUST reboot. Display the minimum required version.
                        OnUpdateChecked(UpdateStateType.IsUpdateRequired, "This application has detected a mandatory update from your current " +
                                          "version to version " + info.MinimumRequiredVersion.ToString() +
                                          ". The application will now install the update and restart.");
                    }
                }else OnUpdateChecked(UpdateStateType.ReleasedVersion, "Released Version, No need update yet.");
            }
            else
            {
                OnUpdateChecked(new Exception("Cannot found UpdateInfo"));
            }
        }

        public static Boolean IsAsync = false;

        public static void CancelUpdate()
        {
            ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
            ad.UpdateAsyncCancel();
        }

        private static void OnUpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (UpdateCompleted != null)
                UpdateCompleted(sender, e);
        }

        private static void OnUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            if (UpdateProgressChanged != null)
                UpdateProgressChanged(sender, e);
        }
        //
        // 요약:
        //     System.Deployment.Application.ApplicationDeployment.UpdateAsync() 호출의 결과로서
        //     ClickOnce에서 응용 프로그램의 업그레이드를 완료했을 때 발생합니다.
        public static event AsyncCompletedEventHandler UpdateCompleted;
        //
        // 요약:
        //     System.Deployment.Application.ApplicationDeployment.UpdateAsync() 메서드를 호출하여
        //     시작된 업데이트 작업에 대한 새 상태 정보가 ClickOnce에 있을 때 발생합니다.
        public static event DeploymentProgressChangedEventHandler UpdateProgressChanged;

        public static void Update()
        {
            try
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                
                if (IsAsync)
                {
                    ad.UpdateCompleted -= CurrentDeployment_UpdateCompleted;
                    ad.UpdateProgressChanged -= CurrentDeployment_UpdateProgressChanged;

                    ad.UpdateCompleted += CurrentDeployment_UpdateCompleted;
                    ad.UpdateProgressChanged += CurrentDeployment_UpdateProgressChanged;
                    ad.UpdateAsync();
                }
                else
                {
                    IsDownloading = true;
                    ad.Update();
                    IsDownloading = false;
                    OnUpdateChecked(UpdateStateType.Updated);
                }
            }
            catch (DeploymentDownloadException dde)
            {
                OnUpdateChecked(
                    new Exception(
                        "Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " +
                        dde));
                IsDownloading = false;
                return;
            }
        }

        private static void CurrentDeployment_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            IsDownloading = true;
            OnUpdateProgressChanged(sender, e);
        }

        private static void CurrentDeployment_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            IsDownloading = false;
            OnUpdateCompleted(sender, e);   
        }

        public static void StartUpdateAsync()
        {
            if (!Worker.IsBusy)
                Worker.RunWorkerAsync();
        }

        public static void StopUpdateAsync()
        {
            if (Worker.IsBusy)
            {
                Worker.CancelAsync();
                SkipToUpdate = true;
            }
        }

        private static void OnUpdateChecked(UpdateStateType type, String message = null)
        {
            if (!SkipToUpdate)
                if (UpdateChecked != null)
                    UpdateChecked(typeof (Updater), new UpdateStateEventArgs() {StateType = type, Message = message});
        }
        private static void OnUpdateChecked(Exception ex)
        {
            if (!SkipToUpdate)
                if (UpdateChecked != null)
                    UpdateChecked(typeof (Updater), new UpdateStateEventArgs(ex));
        }
        public static event UpdateStateEventHandler UpdateChecked;
    }
}
