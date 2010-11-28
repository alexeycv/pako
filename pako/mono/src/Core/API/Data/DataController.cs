/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot. Bbodio's Lab.                                                *
 * Copyright. All rights reserved © 2007-2010 by Pako bot developers team        *
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
using System.Collections;
using System.Text;
using Mono.Data.SqliteClient;
using System.Threading;
using System.IO;
using System.Data;
using Core.Other;

namespace Core.API.Data
{
    /// <summary>
    /// Class for data driving
    /// </summary>
    public class DataController
    {
		string _dbName;
		string _version;
		SqliteConnection _connection;
		object[] _lockers = new object[10];
		bool _dbJustCreated;
		
		#region Properties
		
		public SqliteConnection SQLiteConn
        {
            get { lock (_lockers[1]) { return _connection; } }
            set { lock (_lockers[1]) { _connection = value; } }
        }
		
		public bool JustCreated
        {
            get { lock (_lockers[1]) { return _dbJustCreated; } }
            set { lock (_lockers[1]) { _dbJustCreated = value; } }
        }
		
		#endregion 
		
		#region Constructors
		
		DataController(String dbFile, String version)
		{
            _dbName = dbFile;
			_dbJustCreated = false;
			
			this.Load();
		}
		
		#endregion
		
		#region Methods
		
		public void Load()
		{
			JustCreated = !File.Exists(_dbName);
            SQLiteConn = new SqliteConnection("URI=file:" + _dbName.Replace("\\", "/") + ",version=" + _version);
            SQLiteConn.Open();
		}
		
		public void Close()
		{
			SQLiteConn.Close();
		}
		
		public int ExecuteNonQuery(String command)
		{
			try
			{
				SqliteCommand cmd = new SqliteCommand(@"" + command, SQLiteConn);

            	return cmd.ExecuteNonQuery();
			}
			catch (Exception exx)
			{
				return -1;
			}
		}
		
		public SqliteDataReader ExecuteReader(String command)
		{
			try
			{
				SqliteCommand cmd = new SqliteCommand(@"" + command, SQLiteConn);

            	return cmd.ExecuteReader();
			}
			catch (Exception exx)
			{
				return null;
			}
		}
		
		public int ExecuteDANonQuery(String command, DataTable table)
		{
			try
			{
				SqliteDataAdapter _da = new SqliteDataAdapter(@""+command, SQLiteConn);
				
				return 0;
			}
			catch (Exception exx)
			{
				return -1;
			}
		}
		
		#endregion
	}
}