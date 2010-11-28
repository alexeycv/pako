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
using System.Collections;
using System.Data;

namespace Core.DataBase
{
    public class UserStatsCollection : IDisposable
    {
        DataTable _innerDataTable;

        public UserStatsCollection()
        {
            this._innerDataTable = UserStats.GetEmptyTable();
        }

        public UserStatsCollection(DataTable dataTable)
        {
            this._innerDataTable = dataTable;
        }


        public UserStats this[int index]
        {
            get
            {
                return new UserStats(this.InnerDataTable.Rows[index]);
            }
            set
            {
                this.InnerDataTable.Rows[index].ItemArray = value.InnerDataRow.ItemArray;
            }
        }


        #region Properties

        public DataTable InnerDataTable
        {
            get
            {
                return this._innerDataTable;
            }
            set
            {
                this._innerDataTable = value;
            }
        }

        public int Count
        {
            get
            {
                return this.InnerDataTable.Rows.Count;
            }
        }


        #endregion

        #region Methods

        public void Add(UserStats userStats)
        {
            DataRow _dr = this.InnerDataTable.NewRow();
            _dr.ItemArray = userStats.InnerDataRow.ItemArray;
            this.InnerDataTable.Rows.Add(_dr);
        }

        public void AddRange(UserStatsCollection userStatsCollection)
        {
            for (int i = 0; i < userStatsCollection.Count; i++)
            {
                this.Add(awardValues[i]);
            }
        }

        public void Remove(String room, String jid)
        {
            DataRow[] _drows = this._innerDataTable.Select("Room = '" + room + "' AND Jid = '" + jid + "'");

            if (_drows != null && _drows.Length > 0)
                this.InnerDataTable.Rows.Remove(_drows[0]);

        }

        public void Clear()
        {
            this.InnerDataTable.Rows.Clear();
        }

        public bool Contains(String room, String jid)
        {
            DataRow[] _drows = this._innerDataTable.Select("Room = '" + room + "' AND Jid = '" + jid + "'");

            if (_drows != null && _drows.Length > 0)
                return true;
            else
                return false;
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            this.InnerDataTable.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
