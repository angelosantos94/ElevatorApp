using ElevatorApp.Enums;
using ElevatorApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ElevatorApp
{
    public class ElevatorSystem
    {
        private readonly List<(Elevator elevator, ElevatorLogic logic)> _elevators;
        private readonly Random _random = new();
        private readonly ConsoleColor[] _elevatorColors = new[]
        {
            ConsoleColor.Cyan,
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.Magenta,
            ConsoleColor.Blue
        };

        private readonly Dictionary<int, int> _waitingPassengers = new();
        private readonly List<string> _logMessages = new();
        private const int MaxLogs = 5;

        public ElevatorSystem(int elevatorCount)
        {
            for (int floor = 1; floor <= 10; floor++)
            {
                _waitingPassengers[floor] = 0;
            }

            _elevators = Enumerable.Range(1, elevatorCount).Select(id =>
            {
                var elevator = new Elevator(id);
                var logic = new ElevatorLogic(elevator)
                {
                    PickupCallback = (floor) =>
                    {
                        int count = _waitingPassengers[floor];
                        _waitingPassengers[floor] = 0;

                        var destinations = new List<int>();
                        for (int i = 0; i < count; i++)
                        {
                            int dest;
                            do
                            {
                                dest = _random.Next(1, 11);
                            } while (dest == floor);
                            destinations.Add(dest);
                        }

                        return destinations;
                    },
                    LogMessage = Log
                };
                return (elevator, logic);
            }).ToList();
        }

        private void Log(string message)
        {
            _logMessages.Add(message);
            if (_logMessages.Count > MaxLogs)
                _logMessages.RemoveAt(0);
        }

        public void Run()
        {
            int consoleLines = Console.WindowHeight;

            // Pre-buffer: draw once to reserve all screen space
            Console.Clear();
            for (int i = 0; i < consoleLines; i++)
                Console.WriteLine();

            while (true)
            {
                Console.SetCursorPosition(0, 0);

                GenerateRandomCall();

                foreach (var (elevator, logic) in _elevators)
                {
                    logic.Step();
                }

                DrawUI();

                Thread.Sleep(5000);
            }
        }

        private void GenerateRandomCall()
        {
            if (_random.Next(0, 4) == 0)
            {
                int floor = _random.Next(1, 11);
                int newWaiters = _random.Next(1, 6);
                _waitingPassengers[floor] += newWaiters;
                Log($"[Call] {newWaiters} passenger(s) waiting on floor {floor}.");

                var best = FindClosestElevator(floor);
                best?.logic.AddRequest(floor);
            }
        }

        private (Elevator elevator, ElevatorLogic logic)? FindClosestElevator(int floor)
        {
            return _elevators
                .Where(e => !e.elevator.IsBusy || e.elevator.CurrentDirection == Direction.Idle)
                .OrderBy(e => Math.Abs(e.elevator.CurrentFloor - floor))
                .FirstOrDefault();
        }

        private void DrawUI()
        {
            Console.WriteLine("ELEVATOR SIMULATION\n");
            const int boxWidth = 7;

            for (int floor = 10; floor >= 1; floor--)
            {
                Console.Write("    ");
                for (int i = 0; i < _elevators.Count; i++)
                {
                    var elevator = _elevators[i].elevator;
                    if (elevator.CurrentFloor == floor)
                    {
                        Console.ForegroundColor = _elevatorColors[i % _elevatorColors.Length];
                        Console.Write("┌─────┐");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("       ");
                    }
                }
                Console.WriteLine();

                Console.Write($"{floor,2}| ");
                for (int i = 0; i < _elevators.Count; i++)
                {
                    var elevator = _elevators[i].elevator;
                    if (elevator.CurrentFloor == floor)
                    {
                        string content = elevator.Passengers.ToString();
                        int pad = 5 - content.Length;
                        int padLeft = pad / 2;
                        int padRight = pad - padLeft;
                        Console.ForegroundColor = _elevatorColors[i % _elevatorColors.Length];
                        Console.Write("│" + new string(' ', padLeft) + content + new string(' ', padRight) + "│");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("       ");
                    }
                }
                Console.Write($"Waiting: {_waitingPassengers[floor]}");
                Console.WriteLine();

                Console.Write("    ");
                for (int i = 0; i < _elevators.Count; i++)
                {
                    var elevator = _elevators[i].elevator;
                    if (elevator.CurrentFloor == floor)
                    {
                        Console.ForegroundColor = _elevatorColors[i % _elevatorColors.Length];
                        Console.Write("└─────┘");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("       ");
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine("\nLegend: Each box represents an elevator. Number = onboard passengers.\n");

            Console.WriteLine("Elevator Status:");
            for (int i = 0; i < _elevators.Count; i++)
            {
                var e = _elevators[i].elevator;
                Console.ForegroundColor = _elevatorColors[i % _elevatorColors.Length];
                Console.Write($"Elevator {e.Id}: ");
                Console.ResetColor();
                Console.WriteLine(
                    $"Floor {e.CurrentFloor}, Direction: {e.CurrentDirection}, " +
                    $"Passengers: {e.Passengers}, Destinations: {(e.Destinations.Any() ? string.Join(",", e.Destinations) : "None")}"
                );
            }

            Console.WriteLine("\nRecent Activity:");
            foreach (var msg in _logMessages)
            {
                Console.WriteLine(msg);
            }
        }
    }
}