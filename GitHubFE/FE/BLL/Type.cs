
// Generated by MyGeneration Version # (1.3.0.3)

using System;
using DAL;
using System.Data;

namespace BLL
{
	public class Type : _Type
	{

        public class Constants
        {
            public const int Pharmacuticals = 7;

        }

		public Type()
		{
		
		}

        public static DataTable GetAllTypes()
        {
            BLL.Type type = new Type();
            type.LoadAll();
            return type.DataTable;
        }

        public DataTable GetAllCategory()
        {
            BLL.Type type = new Type();
            type.LoadAll();
            return type.DataTable;
        }
	}
}