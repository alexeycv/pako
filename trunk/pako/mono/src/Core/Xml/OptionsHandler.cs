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

using Core.Other;
using Core.Kernel;
using Core.Xml;
using System;
using System.Collections.Generic;
using agsXMPP;
using agsXMPP.Xml.Dom;

namespace Core.Xml
{
	public class OptionsHandler : XMLContainer
	{
		
		public OptionsHandler(string file)
        {
            if (!System.IO.File.Exists(file))
            {
                Document doc = new Document();
                doc.LoadXml("<Options></Options>");
                doc.Save(file);
            }
            Open(file, 10);
            
           @out.exe("option_hnd_created_or_opened");
           string[] opts = new string[] { "global_censor", "censor_result", "amoderator", "akick", "avisitor", "aliases", "mode", "cmdaccess", "cleanup_unit", "enable_logging", "nick_limit", "nick_limit_result", "length_limit", "length_limit_overflow_result" };
           foreach (string opt in opts)
               AddOption(opt); 
            
            
        }
        
        

        public string GetOption(string Name)
        {
        	lock (Document)
		    {
        	foreach (Element el in Document.RootElement.SelectElements("option"))
        	{
        		string name = el.GetAttribute("name");
        		if (name == Name)
        		{
					return el.GetAttribute("value");
				}
			}
			return null;
		    }
						
		}
		
		public bool SetOption(string Name, string Value)
		{
			lock (Document)
			{
				foreach (Element el in Document.RootElement.SelectElements("option"))
        	    {
        		  string name = el.GetAttribute("name");
        		  if (name == Name)
        		  {
				     Value = Value.ToLower();
				     string[] opts = GetPossibleValues(Name);
				     foreach (string opt in opts)
				     {
					    if (opt == Value || opt == "*")
					    { el.SetAttribute("value", Value); Save(); return true; }
				     }
				     return false;
			      }
			   }
			   return false;
				
			}
		}
		
		public List<string> GetOptions()
		{
			lock (Document)
			{
				List<string> data = new List<string>();
				foreach (Element el in Document.RootElement.SelectElements("option"))
				data.Add(el.GetAttribute("name"));
				return data;
			}
		}
		
		public string[] GetPossibleValues(string Name)
		{
			lock (Document)
			{
		    foreach (Element el in Document.RootElement.SelectElements("option"))
        	{
        		string name = el.GetAttribute("name");
        		if (name == Name)
        		{
        			return el.GetAttribute("possible").Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
				}
			}
			return null;
		    }
			
		}
        
        
        public bool AddOption(string Name)
        {
           
        	lock (Document)
        	{
        		@out.exe("option_start");
        	 if (GetOption(Name) != null)
        	 return false;
        	 @out.exe("option_adding");
        	 switch (Name)
        	 {
                 case "amoderator":
                 case "akick":
                 case "cmdaccess":
                 case "avisitor":
                 case "aliases":
        	     case "global_censor":
        	         Document.RootElement.AddTag("option");
                    foreach (Element el in Document.RootElement.SelectElements("option"))
                    {
                        if (!el.HasAttribute("name"))
                        {
                            el.SetAttribute("name", Name);
                            el.SetAttribute("value", "+");
                            el.SetAttribute("possible","+|-");
                            Save();
                            return true;
                        }
                    }
                    break;
                     
                 case "enable_logging":
        	         Document.RootElement.AddTag("option");
                    foreach (Element el in Document.RootElement.SelectElements("option"))
                    {
                        if (!el.HasAttribute("name"))
                        {
                            el.SetAttribute("name", Name);
                            el.SetAttribute("value", "+");
                            el.SetAttribute("possible","+|-");
                            Save();
                            return true;
                        }
                    }
                    break;                

                case "censor_result":
                   Document.RootElement.AddTag("option");
                    foreach (Element el in Document.RootElement.SelectElements("option"))
                    {
                        if (!el.HasAttribute("name"))
                        {
                            el.SetAttribute("name", Name);
                            el.SetAttribute("value", "kick");
                            el.SetAttribute("possible","kick|devoice|nothing|warn|ban");
                            Save();
                            return true;
                        }
                    }
                    break;

                case "nick_limit_result":
                   Document.RootElement.AddTag("option");
                    foreach (Element el in Document.RootElement.SelectElements("option"))
                    {
                        if (!el.HasAttribute("name"))
                        {
                            el.SetAttribute("name", Name);
                            el.SetAttribute("value", "warn");
                            el.SetAttribute("possible","kick|devoice|nothing|warn|ban");
                            Save();
                            return true;
                        }
                    }
                    break;

                case "length_limit_overflow_result":
                   Document.RootElement.AddTag("option");
                    foreach (Element el in Document.RootElement.SelectElements("option"))
                    {
                        if (!el.HasAttribute("name"))
                        {
                            el.SetAttribute("name", Name);
                            el.SetAttribute("value", "warn");
                            el.SetAttribute("possible","kick|devoice|nothing|warn|ban");
                            Save();
                            return true;
                        }
                    }
                    break;

                case "cleanup_unit":
                    Document.RootElement.AddTag("option");
                    foreach (Element el in Document.RootElement.SelectElements("option"))
                    {
                        if (!el.HasAttribute("name"))
                        {
                            el.SetAttribute("name", Name);
                            el.SetAttribute("value", "~");
                            el.SetAttribute("possible", "*|null|empty");
                            Save();
                            return true;
                        }
                    }
                    break;

                case "nick_limit":
                    Document.RootElement.AddTag("option");
                    foreach (Element el in Document.RootElement.SelectElements("option"))
                    {
                        if (!el.HasAttribute("name"))
                        {
                            el.SetAttribute("name", Name);
                            el.SetAttribute("value", "20");
                            el.SetAttribute("possible", "*");
                            Save();
                            return true;
                        }
                    }
                    break;

                case "length_limit":
                    Document.RootElement.AddTag("option");
                    foreach (Element el in Document.RootElement.SelectElements("option"))
                    {
                        if (!el.HasAttribute("name"))
                        {
                            el.SetAttribute("name", Name);
                            el.SetAttribute("value", "1000");
                            el.SetAttribute("possible", "*");
                            Save();
                            return true;
                        }
                    }
                    break;

                case "mode":
                   Document.RootElement.AddTag("option");
                    foreach (Element el in Document.RootElement.SelectElements("option"))
                    {
                        if (!el.HasAttribute("name"))
                        {
                            el.SetAttribute("name", Name);
                            el.SetAttribute("value", "full");
                            el.SetAttribute("possible","full|private|groupchat");
                            Save();
                            return true;
                        }
                    }
                    break;
                default: return false;
                    
			 }
			 return true;
		    }
		    
		 }
		 
		 	 
		
        
        
	}
	
}

