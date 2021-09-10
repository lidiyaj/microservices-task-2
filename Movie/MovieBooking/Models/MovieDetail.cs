using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBooking.Models
{
    public class MovieDetail
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Venue { get; set; }
        public int NoOfTickets { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
