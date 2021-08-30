﻿
using System;

namespace SharedTrip.Models.Trip
{
    public class TripDetailsViewModel
    {
        public string Id { get; set; }

        public string Image { get; set; }

        public string StartPoint { get; set; }

        public string EndPoint { get; set; }

        public string DepartureTime { get; set; }

        public int Seats { get; set; }

        public string Description { get; set; }
    }
}
