/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot.                                                              *
 * Copyright. All rights reserved © 2007-2008 by Klichuk Bogdan (Bbodio's Lab)   *
 * Copyright. All rights reserved © 2009-2012 by Alexey Bryohov                  *
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
using Core.Kernel;

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
		bool _canCreate = false;
		
		#region Properties
		
		public SqliteConnection SQLiteConn
        {
            get { lock (_lockers[1]) { return _connection; } }
            set { lock (_lockers[1]) { _connection = value; } }
        }
		
		public bool JustCreated
        {
            get { lock (_lockers[2]) { return _dbJustCreated; } }
            set { lock (_lockers[2]) { _dbJustCreated = value; } }
        }
		
		#endregion 
		
		#region Constructors
		
		public DataController(String dbFile, String version, bool canCreate)
		{
			// Iniy lockers
			for (int i = 0; i < _lockers.Length; i++)
				_lockers[i] = new object();
					
			@out.write ("Init DC");
            _dbName = dbFile;
			_dbJustCreated = false;
			_version = version;
			this._canCreate = canCreate;
			
			try{
				@out.write ("DC: Call Load()");
				this.Load();
			}
			catch (Exception ex)
			{
				@out.write ("DC: Throw exception. ");
				throw ex;
			}
		}
		
		#endregion
		
		#region Methods
		
		public void Load()
		{
			if (_dbName != null)
				@out.write ("DC: Load(): Start. Dbname: " + _dbName);
			else
				@out.write ("DC: Load(): Start. Dbname: NULL." );
			JustCreated = !File.Exists(_dbName);
			
			if (JustCreated && !this._canCreate)
			{
				@out.write ("DC: Load(): Database does not exists. Creation is not allowed");
				throw new Exception("Database not exists\n" + this._dbName);
				//return;
			}
			
			try{
				@out.write ("DC: Load(): Init database");
            	SQLiteConn = new SqliteConnection("URI=file:" + _dbName.Replace("\\", "/") + ",version=" + _version);
				@out.write ("DC: Load(): Open database");
            	SQLiteConn.Open();
			}
			catch (Exception exx)
			{
				@out.write ("DC: Load(): Exception: Error while loadiing database.");
				throw new Exception("Error while loading database " + this._dbName + " Message: " + exx.Message);
			}
			
			@out.write ("DC: Load(): End.");
		}
		
		public void Close()
		{
			SQLiteConn.Close();
		}
		
		/// <summary>
		/// Execute database query that's don't return any result. 
		/// Return value : number of rows affected.
		/// </summary>
		/// <param name="command">
		/// A <see cref="String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		public int ExecuteNonQuery(String command)
		{
			try
			{
				SqliteCommand cmd = new SqliteCommand(command, SQLiteConn);

            	return cmd.ExecuteNonQuery();
			}
			catch (Exception exx)
			{
				throw new Exception("Error while executing querry in " + this._dbName + " Message: " + exx.Message + "\n Command:\n\n" + command);
			}
		}
		
		/// <summary>
		/// Execute DataReader. 
		/// Returns DataReader.
		/// </summary>
		/// <param name="command">
		/// A <see cref="String"/>
		/// </param>
		/// <returns>
		/// A <see cref="SqliteDataReader"/>
		/// </returns>
		public SqliteDataReader ExecuteReader(String command)
		{
			try
			{
				SqliteCommand cmd = new SqliteCommand(@"" + command, SQLiteConn);

            	return cmd.ExecuteReader();
			}
			catch (Exception exx)
			{
				throw new Exception("Error while executing querry in " + this._dbName + " Message: " + exx.Message);
			}
		}
		
		/// <summary>
		/// Load query result to a DataTable object
		/// Return value : DataTable.
		/// </summary>
		/// <param name="command">
		/// A <see cref="String"/>
		/// </param>
		/// <returns>
		/// A <see cref="DataTable"/>
		/// </returns>
		public DataTable ExecuteDALoad(String command)
		{
			try
			{
				SqliteDataAdapter _da = new SqliteDataAdapter(@""+command, SQLiteConn);
				SqliteCommandBuilder _cb = new SqliteCommandBuilder();
				_cb.DataAdapter = _da;
				
				DataTable _tmpTable = new DataTable();
				_da.Fill(_tmpTable);
				
				if (_tmpTable.Rows.Count > 0)
				{
					return _tmpTable;
				}
				else
				{
					return null;
				}
			}
			catch (Exception exx)
			{
				throw new Exception("Error while executing querry in " + this._dbName + " Message: " + exx.Message);
			}
		}
		
		/// <summary>
		/// Execute saving data over DataAdapter
		/// </summary>
		/// <param name="command">
		/// A <see cref="String"/>
		/// </param>
		/// <param name="row">
		/// A <see cref="DataRow"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		public int ExecuteDASave(String command, DataRow row)
		{
			try
			{
				SqliteDataAdapter _da = new SqliteDataAdapter(command, SQLiteConn);
				SqliteCommandBuilder _cb = new SqliteCommandBuilder();
				_cb.DataAdapter = _da;
				
				DataTable _tmpTable = new DataTable();
				_da.Fill(_tmpTable);
				
				if (_tmpTable.Rows.Count == 0)
				{
					DataRow _dr = _tmpTable.NewRow();
					_tmpTable.Rows.Add(_dr);
				}
				
				_tmpTable.Rows[0].ItemArray = row.ItemArray;
				
				_da.Update(_tmpTable);
				
				return 0;
			}
			catch (Exception exx)
			{
				throw new Exception("Error while executing querry in " + this._dbName + " Message: " + exx.Message);
			}
		}
		
		#endregion
	}
}