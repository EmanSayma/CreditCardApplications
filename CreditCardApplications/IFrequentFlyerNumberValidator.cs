using System;

namespace CreditCardApplications
{
    public interface ILicenceData
    {
        string LicenseKey { get; }
    }

    public interface IServiceInformation
    {
        ILicenceData Licence { get; }
    }

    public interface IFrequentFlyerNumberValidator
    {
        bool IsValid(string frequentFlyerNumber);
        void IsValid(string frequentFlyerNumber, out bool isValid);
       // string LicenseKey { get;  }

        IServiceInformation ServiceInformation { get; }

        ValidationMode ValidationMode { get; set; }
    }
}