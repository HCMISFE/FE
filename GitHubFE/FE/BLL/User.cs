
// Generated by MyGeneration Version # (1.3.0.3)

using System;
using DAL;
using System.Data;

namespace BLL
{
	public class User : _User
	{
		public User()
		{
		
		}
        public DataTable GetUsers()
        {
            this.FlushData();
            this.LoadFromRawSql(String.Format("SELECT * FROM vwGetUsers Order By FullName"));
            return this.DataTable;
        }

        public DataTable GetUserByAccountInfo(string userName, string password)
        {           
            this.FlushData();
            this.Where.WhereClauseReset();
            this.Where.UserName.Value = userName;
            this.Where.Password.Conjuction = MyGeneration.dOOdads.WhereParameter.Conj.And;
            this.Where.Password.Value = password;
            this.Where.Active.Conjuction = MyGeneration.dOOdads.WhereParameter.Conj.And;
            this.Where.Active.Value = true;
            this.Query.Load();
            return this.DataTable;
        }
	}
}