using System;
using System.Collections.Generic;
using System.Text;

namespace FloraEjemplo.Interfaces
{
    /// <summary>
    /// Interface to implement a function to open wifi settings on each platform
    /// </summary>
    public interface IOpenWifiSettings
    {
        /// <summary>
        /// Opens wifi settings on the device
        /// </summary>
        void OpenWifiSettings();
    }
}
