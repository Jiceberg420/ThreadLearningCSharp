using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace BasicThread
{
    public partial class fMain : Form
    {
        delegate void UpdateUI(string msg);
        private int threadCount = 0;
        public fMain()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //https://www.pluralsight.com/guides/how-to-write-your-first-multi-threaded-application-with-c

            var a = new Thread(CountDownThread);
            a.Name = $"Thread{this.threadCount++,2}";
            a.Start();
            AppendOutput($"{Thread.CurrentThread.Name}: {a.Name} started. status is {a.ThreadState.ToString()}");

        }

        private void CountDownThread()
        {
            for (int i = 10; i >=0; i--)
            {

                int interval_ms = 333;
                //Thread.Sleep(interval_ms);
                Thread.Sleep(TimeSpan.FromMilliseconds(interval_ms));

                //To update the UI created at another thread,
                //we have to Invoke it.
                string msg = $"{Thread.CurrentThread.Name}: {i}";
                txtOutput.Invoke(new UpdateUI(AppendOutput), msg);
            }

        }



        private void AppendOutput(string msg)
        {
            txtOutput.Text += $"{msg}\r\n";
            txtOutput.Select(txtOutput.Text.Length, 0);
            txtOutput.ScrollToCaret();
        }
    }
}
