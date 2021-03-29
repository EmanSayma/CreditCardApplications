using System;
using Xunit;
using Moq;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AccpetHighIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };
            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void RefereYoungApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.DefaultValue = DefaultValue.Mock;

            mockValidator.Setup(x => x.IsValid(It.IsAny<String>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication { Age = 19 };
            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.Setup(x => x.IsValid("x")).Returns(true);
            //mockValidator.Setup(x => x.IsValid(It.IsAny<String>())).Returns(true);
            //mockValidator.Setup(
            //    x => x.IsValid(It.Is<String>(number => number.StartsWith("y"))))
            //    .Returns(true);

            //mockValidator.Setup(
            //    x => x.IsValid(It.IsInRange("a", "z", Moq.Range.Inclusive)))
            //    .Returns(true);

            //mockValidator.Setup(
            //    x => x.IsValid(It.IsIn("x", "a", "z")))
            //    .Returns(true);

            mockValidator.Setup(
                x => x.IsValid(It.IsRegex("[a-z]")))
                .Returns(true);

            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "y"
            };
            
            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);

        }


        [Fact]
        public void RefereInvalidFrequentFlyerApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator =
                new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.IsValid(It.IsAny<String>())).Returns(false);

            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        //[Fact]
        //public void DeclineLowIncomeApplicationsOutDemo()
        //{
        //    Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

        //    var isValid = true;

        //    mockValidator.Setup(x => x.IsValid(It.IsAny<String>(), out isValid));

        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

        //    var application = new CreditCardApplication
        //    {
        //        GrossAnnualIncome = 19_999,
        //        Age = 42
        //    };

        //    CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

        //    Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        //}

        [Fact]
        public void RefereWhenLicenseKeyExpired()
        {
            //var mockLicenceData = new Mock<ILicenceData>();
            //mockLicenceData.Setup(x => x.LicenseKey).Returns("EXPIRED");
            //var mockServiceInfo = new Mock<IServiceInformation>();
            //mockServiceInfo.Setup(x => x.Licence).Returns(mockLicenceData.Object);
            //Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object);

            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenseKey).Returns("EXPIRED");

            mockValidator.Setup(x => x.IsValid(It.IsAny<String>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                Age = 42
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        string GetLicesnceKeyExpiryString()
        {
            // E.g. read from vendor-spplied constants file
            return "EXPIRED";
        }

        [Fact]
        public void UseDetailedLookupForOlderApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.SetupProperty(x => x.ValidationMode);
            mockValidator.SetupAllProperties();
            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenseKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);

            Assert.Equal(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
        }

    }
}
