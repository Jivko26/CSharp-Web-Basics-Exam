using MyWebServer.Controllers;
using MyWebServer.Http;
using SharedTrip.Data;
using SharedTrip.Data.Models;
using SharedTrip.Models.Trip;
using SharedTrip.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SharedTrip.Controllers
{
    public class TripsController : Controller
    {
        private readonly ApplicationDbContext data;
        private readonly IValidator validator;

        public TripsController(ApplicationDbContext data, IValidator validator)
        {
            this.data = data;
            this.validator = validator;
        }

        [Authorize]
        public HttpResponse AddUserToTrip(string tripId)
        {
            try
            {
                var errors = new List<string>();

                var trip = this.data.Trips
                    .Where(t => t.Id == tripId)
                    .FirstOrDefault();

                if (trip.Seats == 2)
                {
                    return Redirect($"/Trips/Details?tripId={tripId}");
                }
                if (trip.UserTrips.Any(ut => ut.UserId == this.User.Id))
                {
                    return Redirect($"/Trips/Details?tripId={tripId}");
                }

                else
                {
                    trip.UserTrips.Add(new UserTrip
                    {
                        UserId = this.User.Id
                    });

                    trip.Seats--;

                    this.data.SaveChanges();
                }

                return Redirect("/Trips/All");
            }
            catch (Exception)
            {

                return Redirect($"/Trips/Details?tripId={tripId}");
            }
        }

        [Authorize]
        public HttpResponse All()
        {
            var trips = this.data.Trips.ToList();

            return this.View(trips);
        }

        [Authorize]
        public HttpResponse Details(string tripId)
        {
            var trip = this.data.Trips
                .Where(t => t.Id == tripId)
                .Select(t => new TripDetailsViewModel
                {
                    Id = t.Id,
                    Image = t.ImagePath,
                    StartPoint = t.StartPoint,
                    EndPoint = t.EndPoint,
                    Description = t.Description,
                    Seats = t.Seats,
                    DepartureTime = t.DepartureTime.ToString("dd.MM.yyyy HH:mm"),
                })
                .FirstOrDefault();

            return View(trip);
        }

        [Authorize]
        public HttpResponse Add() => View();

        [Authorize]
        [HttpPost]
        public HttpResponse Add(AddTripFormModel model)
        {
            var errors = this.validator.ValidateTrip(model);

            if (errors.Any())
            {
                return this.Redirect("/Trips/Add");
            }

            var isValidDate = DateTime.TryParseExact(model.DepartureTime, "dd.MM.yyyy HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime departureTime);

            if (!isValidDate)
            {
                return this.Redirect("Trips/Add");
            }

            var trip = new Trip
            {
                StartPoint = model.StartPoint,
                EndPoint = model.EndPoint,
                DepartureTime = departureTime,
                Description = model.Description,
                Seats = model.Seats,
                ImagePath = model.ImagePath
            };

            trip.UserTrips.Add(new UserTrip
            {
                UserId = this.User.Id,
            });

            this.data.Trips.Add(trip);

            this.data.SaveChanges();

            return this.Redirect("/Trips/All");
        }
    }
}
