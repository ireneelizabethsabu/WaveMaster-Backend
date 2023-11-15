using System.ComponentModel.DataAnnotations;

namespace WaveMaster_Backend.Models
{
    public class PlotData
    {
        public PlotData() { }

        [Key]
        public int? id { get; set; }
        public decimal voltage { get; set; }
        public DateTime time {  get; set; }
    }
}
