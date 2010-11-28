/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * Pako Jabber-bot.                                                *
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
using System.Data;
using System.Data.SqlClient;

namespace Core.DataBase
{
    public class UserStats : IDisposable
    {
        public static readonly string DBTableName = "UserStats";

        //---------------------------------------------------------------

        DataRow _innerDataRow;
        private bool _isDisposed = false; // Track whether Dispose has been called.


        public UserStats()
        {
            this._innerDataRow = UserStats.GetEmptyTable().NewRow();

        }

        public UserStats(DataRow dataRow)
        {
            this._innerDataRow = dataRow;
        }

        #region Properties

        public DataRow InnerDataRow
        {
            get { return _innerDataRow; }
            set { _innerDataRow = value; }
        }

        public String Room
        {
            get
            {
                if (this._innerDataRow["Room"] != DBNull.Value)
                    return (String)this._innerDataRow["Room"];
                else
                    return "";
            }
            set
            {
                if (value != "")
                    this._innerDataRow["Room"] = value;
                else
                    this._innerDataRow["Room"] = DBNull.Value;
            }
        }

        #endregion


        #region Instance methods

        #endregion

        #region Collection methods

        public static UserStatsCollection Load(String room)
        {
            UserStatsCollection retValue = null;

            return retValue;
        }

        public static void Save(UserStatsCollection userStatsCollection)
        {
            
        }

		public static void Save(UserStats userStats)
        {
            
        }
		
        public static void Delete(String room, String jid)
        {
            
        }

        #endregion


        #region Internal-Private methods

        internal static DataTable GetEmptyTable()
        {
            DataTable retValue = new DataTable(UserStats.DBTableName);

            retValue.Columns.Add("Room", typeof(String));

            return retValue;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._isDisposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    this._innerDataRow.Table.Dispose();
                }

            }
            _isDisposed = true;
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method 
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~UserStats()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion

    }
}
