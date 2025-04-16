using ElevatorApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorApp.Core.Models
{
    public class Elevator
    {
        public int Id { get; }
        public int CurrentFloor { get; set; } = 1;
        public Direction CurrentDirection { get; set; } = Direction.Idle;
        public Queue<int> Destinations { get; } = new();

        private Queue<int> _passengerDestinations = new();
        public int Passengers => _passengerDestinations.Count;

        public bool IsBusy => _passengerDestinations.Count > 0;

        public Elevator(int id)
        {
            Id = id;
        }

        public void AddPassenger(int destination)
        {
            _passengerDestinations.Enqueue(destination);
            if (!Destinations.Contains(destination))
            {
                Destinations.Enqueue(destination);
            }
        }

        public void RemoveArrivedPassengers(int currentFloor)
        {
            _passengerDestinations = new Queue<int>(_passengerDestinations.Where(dest => dest != currentFloor));
        }

        public int GetPassengerCountForFloor(int floor)
        {
            return _passengerDestinations.Count(d => d == floor);
        }
    }
}