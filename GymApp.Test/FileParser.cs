using GymApp.Models;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GymApp.Test
{
    internal class FileParser
    {
        public static IEnumerable GetData(string filename)
        {
            string filepath = $@"{AppDomain.CurrentDomain.BaseDirectory}..\..\{filename}";
            string[] lines = File.ReadAllLines(filepath);
            List<TestCaseData> testCases = new List<TestCaseData>();
            foreach (string line in lines)
            {
                if (line.Trim().Length == 0) continue;

                string[] values = line.Split(null);

                TrainingTime? trainingTime = null;
                if (values[0].ToLower() == "wholeday")
                    trainingTime = TrainingTime.WholeDay;
                else if (values[0].ToLower() == "onlymorning")
                    trainingTime = TrainingTime.OnlyMorning;
                else if (values[0].ToLower() == "onlynight")
                    trainingTime = TrainingTime.OnlyNight;

                bool groupTrainings = values[1] == "true";

                MembershipType? membershipType = null;
                if (values[2].ToLower() == "typea")
                    membershipType = MembershipType.TypeA;
                else if (values[2].ToLower() == "typeb")
                    membershipType = MembershipType.TypeB;
                else if (values[2].ToLower() == "typec")
                    membershipType = MembershipType.TypeC;
                else if (values[2].ToLower() == "typed")
                    membershipType = MembershipType.TypeD;

                int numberOfMonths = Convert.ToInt32(values[3]);
                double monthlyPriceBudget = Convert.ToDouble(values[4]);

                testCases.Add(new TestCaseData(numberOfMonths, groupTrainings, monthlyPriceBudget, trainingTime, membershipType));
            }
            return testCases;
        }
    }
}