using Microsoft.AspNetCore.SignalR;

namespace WaveMaster_Backend.HubConfig
{
    public class PlotDataHub : Hub
    {
        //public async Task askServer(decimal plotData)
        //{
        //    DateTime time = DateTime.UtcNow;

        //    await Clients.Clients(this.Context.ConnectionId).SendAsync("askPlotData", plotData);
        //}
    }
}
