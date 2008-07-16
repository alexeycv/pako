﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
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
using Core;
using Core.Plugins;
using Core.Other;
using my = agsXMPP.protocol.client;

namespace Core.Kernel
{
    public class Cmd
    {
        private string m_body = null;
        private Response m_r = null;
        private int? naccess = null;
        private int? m_first_access = null;
        private AccessType acc_type;
        private CmdAccessibilityType avail;
        private bool has_alias;
        private string volume;
        private string[] args;

        public int? CompleteAccess
        {
            get { return naccess == null ? 0 : naccess; }
        }

        public int? FistStageAccess
        {
            get { return m_first_access == null ? 0 : m_first_access; }
        }

        public Response Response
        {
            get { return m_r; }
        }

        public AccessType AccessType
        {
            get { return acc_type; }
        }


        public CmdAccessibilityType Accessibility
        {
            get { return avail; }
        }

        public string Body
        {
            get { return m_body; }
        }

        public string Volume
        {
            get { return volume; }
        }

        public string[] vArgs
        {
            get { return args; }
            set { args = value; }
        }

        public bool IsAlias
        {
            get { return has_alias; }
        }

        public string[] Args()
        {
            return args;
        }

        public string[] Args(int Fixed)
        {
            return Utils.SplitEx(Body, Fixed);
        }


        public AccessType CompleteAccessType
        {
            get { return acc_type; }
        }

        static public Cmd CreateInstance(string body, Response r, bool? roster)
        {
            Cmd cmd = new Cmd(body, r, roster);
            return cmd;
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="Body"></param>
        /// <param name="r"></param>
        Cmd(string cmd, Response r, bool? roster)
        {
            m_r = r;
            bool alias_exists = false;

            this.m_body = cmd;
            string pure_cmd = null;
            if (r.MUC != null && r.MUser != null && (roster == null || (roster != null && roster == false)))
            {
                r.Access = r.Access ?? 0;
                @out.exe("cmd_muc_access_setting_started");
                naccess = r.MUC.OptionsHandler.GetOption("cmdaccess") == "+" ? r.MUC.AccessManager.GetAccess(m_body, ref pure_cmd) : null;
                this.acc_type = AccessType.SetByMuc;
                alias_exists = r.MUC.HasAlias(m_body);
                @out.exe("pure_cmd: '" + pure_cmd + "'");
                this.has_alias = alias_exists;
                @out.exe("cmd_muc_access_setting_finished");
                if (r.MUC.chkal_rec(m_body, 0, r.Sh.S.Config.RecursionLevel, r.MUC, r.MUser))
                {
                    @out.exe("recursion_excided");
                    //r.Reply(r.f("commands_recursion", r.Sh.S.Config.RecursionLevel.ToString()));
                    this.avail = CmdAccessibilityType.AliasRecursion;
                    return;
                }
                int steps = 0;
                string al_name = null,
                       al_body = null,
                       al_formatted = null,
                       al_temp_f = null;
                while (r.MUC.HasAlias(m_body))
                {
                    @out.exe("alias_launching");
                    m_body = r.MUC.GetAlias(m_body, ref al_name, ref al_body, ref al_temp_f, r.MUC, r.MUser);
                    if (steps == 0)
                        al_formatted = al_temp_f;
                    @out.exe("alias_launched");
                    steps++;
                }
                r.Msg.Body = m_body;

                if (alias_exists)
                {
                    int? _nalias = 0;
                    int? _naccess = 0;
                    r.Format = false;
                    string a_purec_cmd = null;
                    string _purec_cmd = null;
                    _nalias = r.MUC.OptionsHandler.GetOption("cmdaccess") == "+" ? r.MUC.AccessManager.GetAccess(al_body, ref a_purec_cmd) : null;
                    _naccess = r.MUC.OptionsHandler.GetOption("cmdaccess") == "+" ? r.MUC.AccessManager.GetAccess(m_body, ref _purec_cmd) : null;
                    _nalias = r.Sh.S.AccessManager.GetAccess(al_body, a_purec_cmd, _nalias);
                    _naccess = r.Sh.S.AccessManager.GetAccess(m_body, _purec_cmd, _naccess);
                    this.m_first_access = _nalias;
                    _nalias = _nalias ?? 0;
                    _naccess = _naccess ?? 0;
                    @out.exe("test_cmd: " + al_formatted);
                    @out.exe("test_access: " + _naccess.ToString());
                    @out.exe("test_access_finish: " + _nalias.ToString());
                    if (_nalias != _naccess && _naccess > r.Access)
                    {
                        @out.exe("access_not_enough");
                        this.naccess = _naccess;
                        //r.Reply(r.f("access_not_enough", naccess.ToString()));
                        this.avail = CmdAccessibilityType.NotAccessible;
                        return;
                    }
                }
                else
                    @out.exe("alias_not_found: stage2");



            }

            args = Utils.SplitEx(m_body, 2);
            volume = args[0];
            @out.exe("cmd_global_access_setting_started: " + m_body);
            naccess = r.Sh.S.AccessManager.GetAccess(m_body, pure_cmd, naccess, ref this.acc_type);
            if (naccess == null)
            {
                this.acc_type = AccessType.None;
                @out.exe("cmd_no_access_notifies_found_access=0");
            }
            naccess = naccess ?? 0;

            if ((r.Sh.S.PluginHandler.Handles(volume)) || (alias_exists))
            {
                if (r.Access < naccess)
                {
                    @out.exe("access_not_enough");
                    this.avail = CmdAccessibilityType.NotAccessible;
                    return;
                }
            }

        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Cmd()
        {
            @out.exe("cmd_destroyed");
        }


        public bool Execute()
        {
            @out.exe("cmd_body: " + m_body);
            if (m_r.Sh.S.PluginHandler.Handles(volume))
            {

                object obj = m_r.Sh.S.PluginHandler.Execute(volume);
                if (obj != null)
                {
                    @out.exe("cmd_body: handles");
                    IPlugin plugin = (IPlugin)obj;
                    PluginTransfer pt = new PluginTransfer(m_r);
                    @out.exe("EXE");
                    plugin.PerformAction(pt);
                    return true;

                }
                return false;
            }
            return false;
        }


        /// <summary>
        /// Overriding the ToString method of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Body;
        }

    }
}
