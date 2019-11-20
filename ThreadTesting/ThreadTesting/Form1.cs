using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreadTesting
{
    public partial class Form1 : Form
    {
        BackgroundWorker bw;
        delegate void UpdateStatus(string status);

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if(bw!=null)
            {
                if (bw.IsBusy) bw.CancelAsync();
            }

            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += Bw_DoWork;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;

            int loopTimes = 100;
            bool createError = chkTestError.Checked;

            List<object> args = new List<object>();
            args.Add(loopTimes);
            args.Add(new UpdateStatus(UpdateProgressStatus));
            args.Add(createError);

            bw.RunWorkerAsync(args);
        }

  

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var args = e.Argument as List<object>;
            var loopTimes = (int)args[0];
            var updateStatus = (UpdateStatus)args[1];
            var createError = (bool)args[2];
            var worker = (BackgroundWorker)sender;

            for (int i = 0; i < loopTimes; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                System.Threading.Thread.Sleep(100);
                string msg = $"Current progress is {i}";
                worker.ReportProgress(i*100/loopTimes, msg);
                updateStatus?.Invoke(msg);
            }

            if (createError) throw new Exception("An error test!");
            worker.ReportProgress(100);
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            barProgress.Value = e.ProgressPercentage * barProgress.Maximum / 100;
            lblStatus.Text = e.UserState as string;
        }
        private void UpdateProgressStatus(string status)
        {
            //lblStatus.Text = status; //This will trigger the error of UI update not in its created thread.
        }
        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                lblStatus.Text = "User Cancelled!";
            }
            else if (e.Error != null)
            {
                lblStatus.Text = $"Error! {e.Error.Message}";
            }
            else
            {
                lblStatus.Text = "Accomplished!";
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy)
                bw.CancelAsync();
        }

        private void chkTestError_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
