/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot. Bbodio's Lab.                                                *
 * Copyright. All rights reserved © 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
 * Contact information is here: http://code.google.com/p/pako                    *
 *                                                                               *
 * Pako is under GNU GPL v3 license:                                             *
 * YOU CAN SHARE THIS SOFTWARE WITH YOUR FRIEND, MAKE CHANGES, REDISTRIBUTE,     *
 * CHANGE THE SOFTWARE TO SUIT YOUR NEEDS, THE GNU GENERAL PUBLIC LICENSE IS     *
 * FREE, COPYLEFT LICENSE FOR SOFTWARE AND OTHER KINDS OF WORKS.                 *
 *                                                                               *
 * Visit http://www.gnu.org/licenses/gpl.html for more information about         *
 * GNU General Public License v3 license                                         *
 *                                                                               *
 * Download source code: http://pako.googlecode.com/svn/trunk                    *
 * See the general information here:                                             *
 * http://code.google.com/p/pako.                                                *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

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
