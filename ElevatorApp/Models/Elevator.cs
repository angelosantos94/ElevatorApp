using ElevatorApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorApp.Models
{
    public class Elevator
    {
        public int Id { get; }
        public int CurrentFloor { get; set; } = 1;
        public Direction CurrentDirection { get; set; } = Direction.Idle;
        public Queue<int> Destinations { get; } = new();
        public int Passengers { get; set; } = 0;

        public bool IsBusy => Destinations.Count > 0;

        public Elevator(int id)
        {
            Id = id;
        }
    }
}
