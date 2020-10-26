using System;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Threading;

namespace CaptureRadio
{
    class RecordRadio
    {
        static DateTime timeNow = default;
        static Task Time;
        static CancellationTokenSource source = new CancellationTokenSource();
        private bool IsStoped { get; set; }

        CancellationToken token = source.Token;

        Task TaskCapture;

        readonly string timeToStartRecord, path, url;
        readonly int hoursToRecord;


        static RecordRadio()
        {
            Time = new Task(() =>
            {
                while (true)
                {
                    timeNow = DateTime.Now;
                    Console.Title = timeNow.ToString();
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            });

            Time.Start();

        }
        public RecordRadio() : this("08:00")
        {

        }
        public RecordRadio(string timeToStartRecord) : this(timeToStartRecord, 5)
        {

        }
        public RecordRadio(string timeToStartRecord, int hoursToRecord) : this(timeToStartRecord, hoursToRecord, $@"D:\NewRockStream\StreamDate{DateTime.Now.Day}-{DateTime.Now.Month}\")
        {

        }
        public RecordRadio(string timeToStartRecord, int hoursToRecord, string path) : this(timeToStartRecord, hoursToRecord, path, @"http://online.radioroks.ua/RadioROKS_NewRock")
        {

        }
        public RecordRadio(string timeToStartRecord, int hoursToRecord, string path, string url)
        {
            this.timeToStartRecord = timeToStartRecord;
            this.hoursToRecord = hoursToRecord;
            this.path = path;
            this.url = url;
            IsStoped = false;

        }

        void GetStream()
        {
            Directory.CreateDirectory(path);

            var fs = new FileStream(path + $@"NewRock{timeNow.Hour}-{timeNow.Minute}.mp3", FileMode.Create);

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            WebResponse myResponse = myRequest.GetResponse();

            token.Register(() => { myRequest.Abort(); fs.Close(); });

            try
            {
                myResponse.GetResponseStream().CopyTo(fs);
            }
            catch (WebException)
            {
                Console.WriteLine($"Record stoped at {timeNow}");
            }
            finally
            {
                fs.Close();
                fs.Dispose();
                myResponse.Close();
                myResponse.Dispose();
            }
        }

        public void Record()
        {
            while (!IsStoped)
            {
                while (!IsStoped)
                {
                    if (timeNow.ToShortTimeString() == timeToStartRecord)
                    {
                        Console.WriteLine($"Record started at {timeNow} and be stoped at {timeNow + TimeSpan.FromHours(hoursToRecord)}");
                        TaskCapture = new Task(GetStream, token);
                        source.CancelAfter(TimeSpan.FromHours(hoursToRecord));
                        TaskCapture.Start();
                        TaskCapture.Wait();
                        TaskCapture.Dispose();
                        break;
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(1));
                    }
                }

            }
        }

        public void StopRecord()
        {
            IsStoped = true;
            source.Cancel();
            source.Dispose();
            TaskCapture.Dispose();
        }

    }
}



