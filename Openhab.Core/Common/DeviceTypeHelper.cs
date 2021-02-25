using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace OpenHAB.Core.Common
{
    /// <summary>Helper class to get the target platform the app is running on.</summary>
    public class DeviceTypeHelper
    {
        /// <summary>
        /// Initializes static members of the <see cref="DeviceTypeHelper"/> class.
        /// </summary>
        static DeviceTypeHelper()
        {
            DeviceFamily = RecognizeDeviceFamily(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily);
        }

        /// <summary>Gets the device family.</summary>
        /// <value>The device family.</value>
        public static DeviceFamily DeviceFamily
        {
            get;
        }

        private static DeviceFamily RecognizeDeviceFamily(string deviceFamily)
        {
            switch (deviceFamily)
            {
                case "Windows.Mobile":
                    return DeviceFamily.Mobile;
                case "Windows.Desktop":
                    return DeviceFamily.Desktop;
                case "Windows.Xbox":
                    return DeviceFamily.Xbox;
                case "Windows.Holographic":
                    return DeviceFamily.Holographic;
                case "Windows.IoT":
                    return DeviceFamily.IoT;
                case "Windows.Team":
                    return DeviceFamily.Team;
                default:
                    return DeviceFamily.Unidentified;
            }
        }
    }
}
