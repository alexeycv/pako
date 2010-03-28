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
using System.Collections.Generic;
using System.Text;
using agsXMPP;
using agsXMPP.protocol.iq.version;
using agsXMPP.protocol.client;
using Core.Kernel;
using Core.Conference;

namespace Core.Kernel
{
    public class VersionCB
    {
        Jid m_jid;
        MUser _mUser;
        MUC _muc;
        SessionHandler _sh;
        string lng;

        public VersionCB(MUser user, MUC muc, SessionHandler sh, string language, XmppClientConnection conn)
        {
            _mUser = user;
            _muc = muc;
            _sh = sh;
            lng = language;
            m_jid = _mUser.Jid;
            VersionIq vi = new VersionIq();
            vi.Type = IqType.get;
            vi.To = m_jid;
            vi.GenerateId();
            conn.IqGrabber.SendIq(vi, new IqCB(VersionExtractor), null); 
            
        }




        private void VersionExtractor(object obj, IQ iq, object arg)
        {
           

          
                @out.exe(" before translate  =>  ");
                agsXMPP.protocol.iq.version.Version vi = iq.Query as agsXMPP.protocol.iq.version.Version;
                @out.exe(" after translate  =>  ");

                string answer;
                string jid = m_jid.ToString();

		//Return a version to mucuser object
		_mUser.Version = vi.Name + " " + vi.Ver + " " + vi.Os;

                // Implementing version censor
                this.VersionCensor();
        }

        private void VersionCensor()
        {
            Response r = new Response(_sh.S.Rg[lng]);
            r.MUC = _muc;
            r.MUser = _mUser;
            r.Sh = _sh;
            // Checking uuser version for a censor
                    string found_censored = _muc.IsVRCensored(_mUser.Version, _muc.OptionsHandler.GetOption("global_censor") == "+", "ver");
                    @out.exe("versioncensor_next_stage");
                    if (found_censored != null)
                    {
                        switch (_muc.OptionsHandler.GetOption("censor_result"))
                        {
                            case "kick":
                                {
                                    @out.exe("versioncensor_next_kick");
                                    if (_muc.KickableForCensored(_mUser))
                                    { _muc.Kick(null, _mUser, found_censored); return; }
                                    else
                                    {
                                        MessageType original_type = r.Msg.Type;
                                        r.Msg.Type = MessageType.groupchat;
                                        r.Reply(found_censored);
                                        r.Msg.Type = original_type;
                                        @out.exe("versioncensor_next_sleeping");
                                        _sh.S.Sleep();

                                        @out.exe("versioncensor_next_slept");
                                    }
                                    break;
                                } 

                            case "devoice":
                                {
                                    @out.exe("versioncensor_next_devoice");
                                    if (_muc.KickableForCensored(_mUser))
                                    { _muc.Devoice(null, _mUser, found_censored); return; }
                                    else
                                    {
                                        MessageType original_type = r.Msg.Type;
                                        r.Msg.Type = MessageType.groupchat;
                                        r.Reply(found_censored);
                                        r.Msg.Type = original_type;
                                        @out.exe("versioncensor_next_sleeping");
                                        _sh.S.Sleep();
                                        @out.exe("versioncensor_next_slept");
                                    }
                                    break;
                                }
                            case "ban":
                                {
                                    @out.exe("versioncensor_next_ban");
                                    if (_muc.KickableForCensored(_mUser))
                                    { _muc.Ban(null, _mUser, found_censored); return; }
                                    else
                                    {
                                        MessageType original_type = r.Msg.Type;
                                        r.Msg.Type = MessageType.groupchat;
                                        r.Reply(found_censored);
                                        r.Msg.Type = original_type;
                                        @out.exe("versioncensor_next_sleeping");
                                        _sh.S.Sleep();
                                        @out.exe("versioncensor_next_slept");
                                    }
                                    break;
                                }
                             
                            case "warn":
                                {
                                    @out.exe("versioncensor_next_warn");
                                    MessageType original_type = r.Msg.Type;
                                    r.Msg.Type = MessageType.groupchat;
                                    r.Reply(found_censored);
                                    r.Msg.Type = original_type;
                                    @out.exe("versioncensor_next_sleeping");
                                    _sh.S.Sleep();
                                    @out.exe("versioncensor_next_slept");

                                }
                                break;
                            default:
                                break;

                        }
                    }
                    else
                        @out.exe("versioncensored_not_found");
                    @out.exe("versioncensored_check_finished");
        }

    }
}
