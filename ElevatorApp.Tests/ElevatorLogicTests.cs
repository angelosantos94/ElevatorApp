using Xunit;
using ElevatorApp.Core;
using ElevatorApp.Core.Models;
using ElevatorApp.Core.Enums;
using System.Collections.Generic;

namespace ElevatorApp.Tests
{
    public class ElevatorLogicTests
    {
        [Fact]
        public void AddRequest_AddsDestination()
        {
            var elevator = new Elevator(1);
            var logic = new ElevatorLogic(elevator);

            logic.AddRequest(5);

            Assert.Contains(5, elevator.Destinations);
        }
    }
}