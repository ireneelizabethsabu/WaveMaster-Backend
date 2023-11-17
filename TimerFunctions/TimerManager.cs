﻿using System.Threading;

namespace WaveMaster_Backend.TimerFunctions
{
    public class TimerManager
    {
        private Timer? _timer;
        private AutoResetEvent? _autoResetEvent;
        private Action? _action;
        public DateTime TimerStarted { get; set; }
        public TimerManager(Action action)
        {
            _action = action;
            _autoResetEvent = new AutoResetEvent(false);
            _timer = new Timer(Execute, _autoResetEvent, 1, 1);
            TimerStarted = DateTime.Now;
        }
        public void Execute(object? stateInfo)
        {
            _action();
            //Console.WriteLine(DateTime.Now);
            if ((DateTime.Now - TimerStarted).TotalSeconds > 600)
            {
                _timer.Dispose();
            }
        }
    }
}
