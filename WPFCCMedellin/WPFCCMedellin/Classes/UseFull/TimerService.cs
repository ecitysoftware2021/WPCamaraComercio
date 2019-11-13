using System;
using System.Reflection;
using System.Timers;
using WPFCCMedellin.Resources;

namespace WPFCCMedellin.Classes
{
    public static class TimerService
    {
        public static Action<string> CallBackTimerOut;

        private static Timer timer;

        private static void ConfigureTimer()
        {
            try
            {
                if (timer == null)
                {
                    timer = new Timer();
                    timer.AutoReset = false;
                    timer.Elapsed += new ElapsedEventHandler(TimerTick);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "TimerService", ex, MessageResource.StandarError);
            }
        }

        public static void Start(int interval)
        {
            try
            {
                if (timer != null)
                {
                    timer.Stop();
                }
                else
                {
                    ConfigureTimer();
                }

                timer.Interval = interval;
                timer.Start();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "TimerService", ex, MessageResource.StandarError);
            }
        }

        public static void Stop(bool close = false)
        {
            try
            {
                if (timer != null)
                {
                    timer.Stop();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "TimerService", ex, MessageResource.StandarError);
            }
        }

        public static void Close()
        {
            try
            {
                if (timer != null)
                {
                    timer.Stop();
                    CallBackTimerOut = null;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "TimerService", ex, MessageResource.StandarError);
            }
        }


        public static void Reset()
        {
            try
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Start();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "TimerService", ex, MessageResource.StandarError);
            }
        }
        private static void TimerTick(object sender, ElapsedEventArgs e)
        {
            try
            {
                timer.Stop();

                CallBackTimerOut?.Invoke("Termino tiempo");
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "TimerService", ex, MessageResource.StandarError);
            }
        }
    }
}
