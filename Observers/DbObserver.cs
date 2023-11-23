﻿using EFCore.BulkExtensions;
using Serilog;
using WaveMaster_Backend.Models;
using WaveMaster_Backend.Services;

namespace WaveMaster_Backend.Observers
{
    public class DbObserver : IObserver<List<PlotData>>
    {
        private IDisposable unsubscriber;
        private readonly WaveMasterDbContext _context;
        private Mutex _mutex = new Mutex(false);
        
        public DbObserver(WaveMasterDbContext context)
        {
            _context = context;
        }

        public virtual void Subscribe(IReadService provider)
        {
            if (provider != null)
            {
                unsubscriber = provider.Subscribe(this);
            }
        }
        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }

        public virtual void OnCompleted()
        {
            Console.WriteLine("Additional temperature data will not be transmitted.");
        }

        public virtual void OnError(Exception error)
        {
            // Do nothing.
        }

        public async virtual void OnNext(List<PlotData> dataStore)
        {
            //Console.WriteLine("Going to wait");
            //Console.WriteLine("writing dataStore of count " + dataStore.Count());

            //_context.plotDatas.AddRange(dataStore);
            await Task.Run(() => {
                //_context.plotDatas.AddRange(dataStore);
                //_context.SaveChanges();
                _mutex.WaitOne();             
                _context.BulkInsert(dataStore);
                _mutex.ReleaseMutex();
                //Console.WriteLine("Finished writing!!!!");
                //_context.BulkSaveChanges();
            });
            Log.Information("Writing to DB done");
        }        
    }
}