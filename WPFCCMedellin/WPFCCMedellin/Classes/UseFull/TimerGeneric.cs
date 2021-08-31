using System;
using System.Timers;

namespace WPFCCMedellin.Classes.UseFull
{

    class TimerGeneric
    {
        public Action<string> CallBackTimer;
        public Action<int> CallBackClose;
        public Action<int> CallBackStop;

        public int Minutos;
        public int Segundos;

        Timer timer;

        public TimerGeneric(string tiempo)
        {
            try
            {
                timer = new Timer();
                Minutos = int.Parse(tiempo.Split(':')[0]);
                Segundos = int.Parse(tiempo.Split(':')[1]);
                timer.Interval = 1000;
                timer.Elapsed += new ElapsedEventHandler(TimerTick);
                timer.Start();
            }
            catch
            {
            }
        }

        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            try
            {
                CallBackStop = response =>
                {
                    timer.Elapsed -= TimerTick;
                    timer.Stop();
                };

                if (Minutos >= 1)
                {
                    Segundos--;
                    if (Segundos == 0)
                    {
                        Minutos--;
                        Segundos = 59;
                    }
                }
                else
                {
                    Segundos--;
                    if (Segundos == 0)
                    {
                        timer.Stop();
                        CallBackClose?.Invoke(1);
                    }
                }

                CallBackTimer?.Invoke(string.Concat(Minutos.ToString().PadLeft(2, '0'), ":", Segundos.ToString().PadLeft(2, '0')));
            }
            catch
            {
            }
        }
    }
}
