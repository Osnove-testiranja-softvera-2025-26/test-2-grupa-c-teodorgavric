using System;
using GymApp.Models;
using GymApp.Services;

namespace GymApp.Fakes
{
    public class FakePaymentService : IPaymentService
    {
        public BonusPayment Payment { get; set; }

        public void UpdateTrainerBonusPayment(Guid trainerId, BonusPayment payment)
        {
            Payment = payment;
        }
    }
}