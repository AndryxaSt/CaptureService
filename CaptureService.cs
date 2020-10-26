using CaptureRadio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CaptureService
{
    public partial class CaptureService : ServiceBase
    {
        RecordRadio radio;
        
        public CaptureService()
        {
            InitializeComponent();
            
        }

        protected override void OnStart(string[] args)
        {
            radio = new RecordRadio();
            Task.Run(()=>radio.Record());
        }

        protected override void OnStop()
        {
            radio.StopRecord();
            radio = null;
        }
    }
}
