/*
Copyright © 2006, Drollic
All rights reserved.
http://www.drollic.com

Redistribution of this software in source or binary forms, with or 
without modification, is expressly prohibited. You may not reverse-assemble, 
reverse-compile, or otherwise reverse-engineer this software in any way.

THIS SOFTWARE ("SOFTWARE") IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;

namespace com.drollic.net
{
    public sealed class NetworkUtils
    {
        private static String physicalAddress = null;
        
        public static String GetPhysicalAddress()
        {
            if (physicalAddress == null)
            {
                IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

                if (nics == null || nics.Length < 1)
                {
                    return "unknown";
                }

                String macAddress = "";
                OperationalStatus[] statusValuesToCheck = { OperationalStatus.Up, OperationalStatus.Unknown, OperationalStatus.Testing, OperationalStatus.Dormant, OperationalStatus.LowerLayerDown };
                foreach (OperationalStatus status in statusValuesToCheck)
                {
                    if (macAddress != "")
                        break;

                    foreach (NetworkInterface adapter in nics)
                    {
                        if (macAddress != "")
                            break;

                        IPInterfaceProperties properties = adapter.GetIPProperties(); //  .GetIPInterfaceProperties();
                        PhysicalAddress address = adapter.GetPhysicalAddress();

                        if (adapter.OperationalStatus == status)
                        {
                            byte[] bytes = address.GetAddressBytes();
                            for (int i = 0; i < bytes.Length; i++)
                            {
                                // Display the physical address in hexadecimal.
                                macAddress += bytes[i].ToString("X2");

                                // Insert a hyphen after each byte, unless we are at the end of the 
                                // address.
                                if (i != bytes.Length - 1)
                                {
                                    macAddress += "-";
                                }
                            }
                        }
                    }
                }

                physicalAddress = macAddress;
            }
            
            return physicalAddress;
        }       
    }
}
