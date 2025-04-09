using ElevatorApp.Enums;
using ElevatorApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorApp
{
    public class ElevatorLogic
    {
        private readonly Elevator _elevator;
        private readonly Random _random = new();

        public Func<int, List<int>>? PickupCallback { get; set; }
        public Action<string>? LogMessage { get; set; }

        public ElevatorLogic(Elevator elevator)
        {
            _elevator = elevator;
        }

        public void AddRequest(int floor)
        {
            if (!_elevator.Destinations.Contains(floor))
            {
                _elevator.Destinations.Enqueue(floor);
                UpdateDirection();
                LogMessage?.Invoke($"[Elevator {_elevator.Id}] Added request to floor {floor}.");
            }
        }

        public void Step()
        {
            if (!_elevator.IsBusy && !_elevator.Destinations.Any())
            {
                _elevator.CurrentDirection = Direction.Idle;
                return;
            }

            int target = _elevator.Destinations.Peek();

            if (_elevator.CurrentFloor == target)
            {
                HandleStop();
            }
            else
            {
                MoveOneFloor();
            }

            UpdateDirection();
        }

        private void HandleStop()
        {
            LogMessage?.Invoke($"[Elevator {_elevator.Id}] Arrived at floor {_elevator.CurrentFloor}.");

            // Drop off passengers
            int dropOffCount = _elevator.GetPassengerCountForFloor(_elevator.CurrentFloor);
            if (dropOffCount > 0)
            {
                LogMessage?.Invoke($"[Elevator {_elevator.Id}] {dropOffCount} passenger(s) disembark.");
                _elevator.RemoveArrivedPassengers(_elevator.CurrentFloor);
            }

            Thread.Sleep(1000); // simulate stop delay

            // Pick up new passengers
            var newDestinations = PickupCallback?.Invoke(_elevator.CurrentFloor);
            if (newDestinations != null && newDestinations.Count > 0)
            {
                foreach (var dest in newDestinations)
                {
                    _elevator.AddPassenger(dest);
                }

                LogMessage?.Invoke($"[Elevator {_elevator.Id}] Picked up {newDestinations.Count} passenger(s).");
            }

            // Remove current floor from destination queue
            if (_elevator.Destinations.Contains(_elevator.CurrentFloor))
            {
                var remaining = new Queue<int>(_elevator.Destinations.Where(d => d != _elevator.CurrentFloor));
                while (_elevator.Destinations.Count > 0) _elevator.Destinations.Dequeue();
                foreach (var d in remaining) _elevator.Destinations.Enqueue(d);
            }

            Thread.Sleep(1000); // simulate door close delay
        }

        private void MoveOneFloor()
        {
            if (_elevator.CurrentDirection == Direction.Up)
                _elevator.CurrentFloor++;
            else if (_elevator.CurrentDirection == Direction.Down)
                _elevator.CurrentFloor--;

            LogMessage?.Invoke($"[Elevator {_elevator.Id}] Moving {_elevator.CurrentDirection} to floor {_elevator.CurrentFloor}...");
            Thread.Sleep(1000); // simulate movement delay
        }

        private void UpdateDirection()
        {
            if (!_elevator.IsBusy && !_elevator.Destinations.Any())
            {
                _elevator.CurrentDirection = Direction.Idle;
                return;
            }

            if (_elevator.Destinations.Any())
            {
                int target = _elevator.Destinations.Peek();
                if (_elevator.CurrentFloor < target)
                    _elevator.CurrentDirection = Direction.Up;
                else if (_elevator.CurrentFloor > target)
                    _elevator.CurrentDirection = Direction.Down;
                else
                    _elevator.CurrentDirection = Direction.Idle;
            }
        }
    }
}