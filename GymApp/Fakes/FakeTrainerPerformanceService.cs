using System;
using GymApp.Models;
using GymApp.Services;

namespace GymApp.Fakes
{
    public class FakeTrainerPerformanceService : ITrainerPerformanceService
    {
        public PerformanceReport Report { get; set; }

        public PerformanceReport GetTrainerPerformanceReport(Guid trainerId)
        {
            return Report;
        }
    }
}