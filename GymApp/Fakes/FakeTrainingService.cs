using System;
using System.Collections.Generic;
using GymApp.Models;
using GymApp.Services;

namespace GymApp.Fakes
{
    public class FakeTrainingService : ITrainingService
    {
        public List<Training> Trainings { get; set; }

        public List<Training> GetTrainingsInTheLastMonth(Guid trainerId)
        {
            return Trainings;
        }
    }
}