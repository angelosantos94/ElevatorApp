﻿using ElevatorApp.Enums;
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

        public Func<int, int>? PickupCallback { get; set; }
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
            if (!_elevator.IsBusy)
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
            LogMessage?.Invoke($"[Elevator {_elevator.Id}] Arrived at floor {_elevator.CurrentFloor}. Picking up passengers...");

            Thread.Sleep(1000); // simulate stop delay

            int pickedUp = PickupCallback?.Invoke(_elevator.CurrentFloor) ?? _random.Next(1, 6);
            _elevator.Passengers += pickedUp;
            LogMessage?.Invoke($"[Elevator {_elevator.Id}] Picked up {pickedUp} passenger(s).");

            _elevator.Destinations.Dequeue();

            if (_elevator.Destinations.Count == 0)
            {
                LogMessage?.Invoke($"[Elevator {_elevator.Id}] Final stop. All passengers disembark.");
                _elevator.Passengers = 0;
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
            if (!_elevator.IsBusy)
            {
                _elevator.CurrentDirection = Direction.Idle;
                return;
            }

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
