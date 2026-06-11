

using GymApp.Exceptions;
using GymApp.Fakes;
using GymApp.Models;
using GymApp.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace GymApp.Test
{
    //Guid example: "00000000-0000-0000-0000-000000000001"

    [TestFixture]
    public class GymServiceTest
    {
        FakeTrainingService trainingService;
        FakeTrainerPerformanceService trainerPerformanceService;
        FakePaymentService paymentService;
        GymService service;

        [SetUp]
        public void SetUp()
        {
            trainingService = new FakeTrainingService();
            trainerPerformanceService = new FakeTrainerPerformanceService();
            paymentService = new FakePaymentService();
            service = new GymService(paymentService, trainingService, trainerPerformanceService);
        }

        /*
            if (rank == Second && notHeld < 10)    
                if (freeDays > 13) -> 150           
                else -> 120
            else if (rank == First)                   
                if (groups >= 7 || notHeld < 5) -> 200 
                else -> 150
            else -> 0

            T1: Second, notHeld=9,  freeDays=14 -> 150
            T2: Second, notHeld=9,  freeDays=13 -> 120
            T3: Second, notHeld=10 -> 0
            T4: First, groups=7 -> 200
            T5: First, notHeld=4 -> 200
            T6: First, groups=6, notHeld=5 -> 150
            T7: Third -> 0
         */
        [TestCase(PerformanceRank.Second, 9, 14, 0, 150.0)]
        [TestCase(PerformanceRank.Second, 9, 13, 0, 120.0)]
        [TestCase(PerformanceRank.Second, 10, 0, 0, 0.0)]
        [TestCase(PerformanceRank.First, 10, 0, 7, 200.0)]
        [TestCase(PerformanceRank.First, 4, 0, 6, 200.0)]
        [TestCase(PerformanceRank.First, 5, 0, 6, 150.0)]
        [TestCase(PerformanceRank.Third, 0, 0, 0, 0.0)]
        public void DoStaffBonusPaymentCalculation_CorrectBonus(PerformanceRank rank, int percentNotHeld, int freeDaysLeft, int numOfGroupTrainings, double expected)
        {
            Guid trainerId = new Guid("00000000-0000-0000-0000-000000000001");

            List<Training> trainings = new List<Training>();
            for (int i = 0; i < numOfGroupTrainings; i++)
                trainings.Add(new Training { Type = TrainingType.Group });
            trainings.Add(new Training { Type = TrainingType.Personal });
            trainingService.Trainings = trainings;

            trainerPerformanceService.Report = new PerformanceReport
            {
                PerformanceRank = rank,
                PercentOfTrainingsNotHeld = percentNotHeld,
                NumberOfFreeDaysLeft = freeDaysLeft
            };

            service.DoStaffBonusPaymentCalculation(new Trainer { Id = trainerId });

            Assert.That(paymentService.Payment.Amount, Is.EqualTo(expected));
        }

        [Test]
        public void DoStaffBonusPaymentCalculation_ThrowsException()
        {
            Guid trainerId = new Guid("00000000-0000-0000-0000-000000000001");
            string error = "Bonus payment cannot be calculated";

            var trainingServiceSub = Substitute.For<ITrainingService>();
            var performanceServiceSub = Substitute.For<ITrainerPerformanceService>();
            var paymentServiceSub = Substitute.For<IPaymentService>();

            trainingServiceSub.GetTrainingsInTheLastMonth(trainerId).Returns(new List<Training>());

            GymService serviceNSub = new GymService(paymentServiceSub, trainingServiceSub, performanceServiceSub);

            var ex = Assert.Throws<NoTrainingsInTheLastMonthException>((TestDelegate)(() => serviceNSub.DoStaffBonusPaymentCalculation(new Trainer { Id = trainerId })));
            Assert.That(ex.Message, Is.EqualTo(error));
        }

        [TestCaseSource(typeof(FileParser), "GetData", new object[] { "data.txt" })]
        public void GetMemberhipType_CorrectType(int numberOfMonths, bool groupTrainings, double monthlyPriceBudget, TrainingTime trainingTime, MembershipType? expected)
        {
            MembershipType? actual = service.GetMemberhipType(numberOfMonths, groupTrainings, monthlyPriceBudget, trainingTime);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
