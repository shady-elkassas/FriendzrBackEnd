using NUnit.Framework;
using Social.Services.Implementation;
using System;

namespace UnitTestProject
{
    public class Tests
    {
        [Test]
        public void CheckUserAngle()
        {
            //Assert.IsTrue(UserService.Cardinal(31.035639053407607, 31.358891210870365, 31.037480440560625, 31.361128324925254 , 43));
            //Assert.IsTrue(UserService.Cardinal(31.037480440560625, 31.361128324925254, 31.035639053407607, 31.358891210870365,  266));
            //Assert.IsTrue(UserService.Cardinal(31.037480440560625, 31.361128324925254, 31.035639053407607, 31.358891210870365, 200));
            //Assert.IsTrue(UserService.Cardinal(31.037480440560625, 31.361128324925254, 31.035639053407607, 31.358891210870365, 180));
            //Assert.IsTrue(UserService.Cardinal(31.0346967, 31.358598, 31.035405, 20));

            //Assert.IsFalse(UserService.Cardinal(31.0346967, 31.358598, 31.035405, 90));
            //Assert.IsFalse(UserService.Cardinal(31.0346967, 31.358598, 31.035405, 180));
            //Assert.IsFalse(UserService.Cardinal(31.0346967, 31.358598, 31.035405, 270));
        }

        [Test]
        public void CheckCalculateDistance()
        {
            var dist1 = Math.Round(UserService.CalculateDistance(31.035377319058554, 31.35859403305195, 31.046151635241678, 31.366734726763223)/1000,1);
            Assert.AreEqual(dist1, 1.4);

            var dist2 = Math.Round(UserService.CalculateDistance(31.03605783825886, 31.35889186330968, 31.051963588912297, 31.39499550899666) / 1000, 1);
            Assert.AreEqual(dist2, 3.9);

            var dist3 = Math.Round(UserService.CalculateDistance(31.03605783825886, 31.35889186330968, 31.03400702200246, 31.363773677614553) / 1000, 1);
            Assert.AreEqual(dist3, 0.5);

            var dist4 = Math.Round(UserService.CalculateDistance(31.03605783825886, 31.35889186330968, 31.035184494956436, 31.361858703409386) / 1000, 1);
            Assert.AreEqual(dist4, 0.3);
        }
    }
}