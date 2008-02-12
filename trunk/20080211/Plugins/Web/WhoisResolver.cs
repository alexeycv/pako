using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace www
{
    /// <summary>
    /// Queries the appropriate whois server for a given domain name and returns the results.
    /// </summary>
    public class WhoisResolver
    {
        /// <summary>
        /// Do not allow any instances of this class.
        /// </summary>
        private WhoisResolver() { }
        /// <summary>
        /// Queries an appropriate whois server for the given domain name.
        /// </summary>
        /// <param name="domain">The domain name to retrieve the information of.</param>
        /// <returns>A string that contains the whois information of the specified domain name.</returns>
        /// <exception cref="ArgumentNullException"><c>domain</c> is null.</exception>
        /// <exception cref="ArgumentException"><c>domain</c> is invalid.</exception>
        /// <exception cref="SocketException">A network error occured.</exception>
        public static string Whois(string domain)
        {
            int ccStart = domain.LastIndexOf(".");
            if (ccStart < 0 || ccStart == domain.Length)
                return null;
            string ret = "";
            Socket s = null;
            try
            {
                string cc = domain.Substring(ccStart + 1);
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(new IPEndPoint(Dns.Resolve(cc + ".whois-servers.net").AddressList[0], 43));
                s.Send(Encoding.ASCII.GetBytes(domain + "\r\n"));
                byte[] buffer = new byte[1024];
                int recv = s.Receive(buffer);
                while (recv > 0)
                {
                    ret += Encoding.ASCII.GetString(buffer, 0, recv);
                    recv = s.Receive(buffer);
                }
                s.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (s != null)
                    s.Close();
            }
            return ret;
        }
    }
}