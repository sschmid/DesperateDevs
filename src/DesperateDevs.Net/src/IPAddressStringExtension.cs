using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace DesperateDevs.Net
{
    public static class IPAddressStringExtension
    {
        public static IPAddress ResolveHost(this string host) => Dns.GetHostEntry(host)
            .AddressList
            .FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork);
    }
}
