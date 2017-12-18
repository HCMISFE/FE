
/*
'===============================================================================
'  Generated From - CSharp_dOOdads_BusinessEntity.vbgen
' 
'  ** IMPORTANT  ** 
'  How to Generate your stored procedures:
' 
'  SQL        = SQL_StoredProcs.vbgen
'  ACCESS     = Access_StoredProcs.vbgen
'  ORACLE     = Oracle_StoredProcs.vbgen
'  FIREBIRD   = FirebirdStoredProcs.vbgen
'  POSTGRESQL = PostgreSQL_StoredProcs.vbgen
'
'  The supporting base class SqlClientEntity is in the Architecture directory in "dOOdads".
'  
'  This object is 'abstract' which means you need to inherit from it to be able
'  to instantiate it.  This is very easilly done. You can override properties and
'  methods in your derived class, this allows you to regenerate this class at any
'  time and not worry about overwriting custom code. 
'
'  NEVER EDIT THIS FILE.
'
'  public class YourObject :  _YourObject
'  {
'
'  }
'
'===============================================================================
*/

// Generated by MyGeneration Version # (1.3.0.3)

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Specialized;

using MyGeneration.dOOdads;

namespace DAL
{
	public abstract class _HalfPalletLocation : SqlClientEntity
	{
		public _HalfPalletLocation()
		{
			this.QuerySource = "HalfPalletLocation";
			this.MappingName = "HalfPalletLocation";

		}	

		//=================================================================
		//  public Overrides void AddNew()
		//=================================================================
		//
		//=================================================================
		public override void AddNew()
		{
			base.AddNew();
			
		}
		
		
		public override void FlushData()
		{
			this._whereClause = null;
			this._aggregateClause = null;
			base.FlushData();
		}
		
		//=================================================================
		//  	public Function LoadAll() As Boolean
		//=================================================================
		//  Loads all of the records in the database, and sets the currentRow to the first row
		//=================================================================
		public bool LoadAll() 
		{
			ListDictionary parameters = null;
			
			return base.LoadFromSql("[" + this.SchemaStoredProcedure + "proc_HalfPalletLocationLoadAll]", parameters);
		}
	
		//=================================================================
		// public Overridable Function LoadByPrimaryKey()  As Boolean
		//=================================================================
		//  Loads a single row of via the primary key
		//=================================================================
		public virtual bool LoadByPrimaryKey(int ID)
		{
			ListDictionary parameters = new ListDictionary();
			parameters.Add(Parameters.ID, ID);

		
			return base.LoadFromSql("[" + this.SchemaStoredProcedure + "proc_HalfPalletLocationLoadByPrimaryKey]", parameters);
		}
		
		#region Parameters
		protected class Parameters
		{
			
			public static SqlParameter ID
			{
				get
				{
					return new SqlParameter("@ID", SqlDbType.Int, 0);
				}
			}
			
			public static SqlParameter PalletLocationID
			{
				get
				{
					return new SqlParameter("@PalletLocationID", SqlDbType.Int, 0);
				}
			}
			
			public static SqlParameter Label
			{
				get
				{
					return new SqlParameter("@Label", SqlDbType.VarChar, 50);
				}
			}
			
			public static SqlParameter PalleteID
			{
				get
				{
					return new SqlParameter("@PalleteID", SqlDbType.Int, 0);
				}
			}
			
			public static SqlParameter Confirmed
			{
				get
				{
					return new SqlParameter("@Confirmed", SqlDbType.Bit, 0);
				}
			}
			
		}
		#endregion		
	
		#region ColumnNames
		public class ColumnNames
		{  
            public const string ID = "ID";
            public const string PalletLocationID = "PalletLocationID";
            public const string Label = "Label";
            public const string PalleteID = "PalleteID";
            public const string Confirmed = "Confirmed";

			static public string ToPropertyName(string columnName)
			{
				if(ht == null)
				{
					ht = new Hashtable();
					
					ht[ID] = _HalfPalletLocation.PropertyNames.ID;
					ht[PalletLocationID] = _HalfPalletLocation.PropertyNames.PalletLocationID;
					ht[Label] = _HalfPalletLocation.PropertyNames.Label;
					ht[PalleteID] = _HalfPalletLocation.PropertyNames.PalleteID;
					ht[Confirmed] = _HalfPalletLocation.PropertyNames.Confirmed;

				}
				return (string)ht[columnName];
			}

			static private Hashtable ht = null;			 
		}
		#endregion
		
		#region PropertyNames
		public class PropertyNames
		{  
            public const string ID = "ID";
            public const string PalletLocationID = "PalletLocationID";
            public const string Label = "Label";
            public const string PalleteID = "PalleteID";
            public const string Confirmed = "Confirmed";

			static public string ToColumnName(string propertyName)
			{
				if(ht == null)
				{
					ht = new Hashtable();
					
					ht[ID] = _HalfPalletLocation.ColumnNames.ID;
					ht[PalletLocationID] = _HalfPalletLocation.ColumnNames.PalletLocationID;
					ht[Label] = _HalfPalletLocation.ColumnNames.Label;
					ht[PalleteID] = _HalfPalletLocation.ColumnNames.PalleteID;
					ht[Confirmed] = _HalfPalletLocation.ColumnNames.Confirmed;

				}
				return (string)ht[propertyName];
			}

			static private Hashtable ht = null;			 
		}			 
		#endregion	

		#region StringPropertyNames
		public class StringPropertyNames
		{  
            public const string ID = "s_ID";
            public const string PalletLocationID = "s_PalletLocationID";
            public const string Label = "s_Label";
            public const string PalleteID = "s_PalleteID";
            public const string Confirmed = "s_Confirmed";

		}
		#endregion		
		
		#region Properties
	
		public virtual int ID
	    {
			get
	        {
				return base.Getint(ColumnNames.ID);
			}
			set
	        {
				base.Setint(ColumnNames.ID, value);
			}
		}

		public virtual int PalletLocationID
	    {
			get
	        {
				return base.Getint(ColumnNames.PalletLocationID);
			}
			set
	        {
				base.Setint(ColumnNames.PalletLocationID, value);
			}
		}

		public virtual string Label
	    {
			get
	        {
				return base.Getstring(ColumnNames.Label);
			}
			set
	        {
				base.Setstring(ColumnNames.Label, value);
			}
		}

		public virtual int PalleteID
	    {
			get
	        {
				return base.Getint(ColumnNames.PalleteID);
			}
			set
	        {
				base.Setint(ColumnNames.PalleteID, value);
			}
		}

		public virtual bool Confirmed
	    {
			get
	        {
				return base.Getbool(ColumnNames.Confirmed);
			}
			set
	        {
				base.Setbool(ColumnNames.Confirmed, value);
			}
		}


		#endregion
		
		#region String Properties
	
		public virtual string s_ID
	    {
			get
	        {
				return this.IsColumnNull(ColumnNames.ID) ? string.Empty : base.GetintAsString(ColumnNames.ID);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(ColumnNames.ID);
				else
					this.ID = base.SetintAsString(ColumnNames.ID, value);
			}
		}

		public virtual string s_PalletLocationID
	    {
			get
	        {
				return this.IsColumnNull(ColumnNames.PalletLocationID) ? string.Empty : base.GetintAsString(ColumnNames.PalletLocationID);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(ColumnNames.PalletLocationID);
				else
					this.PalletLocationID = base.SetintAsString(ColumnNames.PalletLocationID, value);
			}
		}

		public virtual string s_Label
	    {
			get
	        {
				return this.IsColumnNull(ColumnNames.Label) ? string.Empty : base.GetstringAsString(ColumnNames.Label);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(ColumnNames.Label);
				else
					this.Label = base.SetstringAsString(ColumnNames.Label, value);
			}
		}

		public virtual string s_PalleteID
	    {
			get
	        {
				return this.IsColumnNull(ColumnNames.PalleteID) ? string.Empty : base.GetintAsString(ColumnNames.PalleteID);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(ColumnNames.PalleteID);
				else
					this.PalleteID = base.SetintAsString(ColumnNames.PalleteID, value);
			}
		}

		public virtual string s_Confirmed
	    {
			get
	        {
				return this.IsColumnNull(ColumnNames.Confirmed) ? string.Empty : base.GetboolAsString(ColumnNames.Confirmed);
			}
			set
	        {
				if(string.Empty == value)
					this.SetColumnNull(ColumnNames.Confirmed);
				else
					this.Confirmed = base.SetboolAsString(ColumnNames.Confirmed, value);
			}
		}


		#endregion		
	
		#region Where Clause
		public class WhereClause
		{
			public WhereClause(BusinessEntity entity)
			{
				this._entity = entity;
			}
			
			public TearOffWhereParameter TearOff
			{
				get
				{
					if(_tearOff == null)
					{
						_tearOff = new TearOffWhereParameter(this);
					}

					return _tearOff;
				}
			}

			#region WhereParameter TearOff's
			public class TearOffWhereParameter
			{
				public TearOffWhereParameter(WhereClause clause)
				{
					this._clause = clause;
				}
				
				
				public WhereParameter ID
				{
					get
					{
							WhereParameter where = new WhereParameter(ColumnNames.ID, Parameters.ID);
							this._clause._entity.Query.AddWhereParameter(where);
							return where;
					}
				}

				public WhereParameter PalletLocationID
				{
					get
					{
							WhereParameter where = new WhereParameter(ColumnNames.PalletLocationID, Parameters.PalletLocationID);
							this._clause._entity.Query.AddWhereParameter(where);
							return where;
					}
				}

				public WhereParameter Label
				{
					get
					{
							WhereParameter where = new WhereParameter(ColumnNames.Label, Parameters.Label);
							this._clause._entity.Query.AddWhereParameter(where);
							return where;
					}
				}

				public WhereParameter PalleteID
				{
					get
					{
							WhereParameter where = new WhereParameter(ColumnNames.PalleteID, Parameters.PalleteID);
							this._clause._entity.Query.AddWhereParameter(where);
							return where;
					}
				}

				public WhereParameter Confirmed
				{
					get
					{
							WhereParameter where = new WhereParameter(ColumnNames.Confirmed, Parameters.Confirmed);
							this._clause._entity.Query.AddWhereParameter(where);
							return where;
					}
				}


				private WhereClause _clause;
			}
			#endregion
		
			public WhereParameter ID
		    {
				get
		        {
					if(_ID_W == null)
	        	    {
						_ID_W = TearOff.ID;
					}
					return _ID_W;
				}
			}

			public WhereParameter PalletLocationID
		    {
				get
		        {
					if(_PalletLocationID_W == null)
	        	    {
						_PalletLocationID_W = TearOff.PalletLocationID;
					}
					return _PalletLocationID_W;
				}
			}

			public WhereParameter Label
		    {
				get
		        {
					if(_Label_W == null)
	        	    {
						_Label_W = TearOff.Label;
					}
					return _Label_W;
				}
			}

			public WhereParameter PalleteID
		    {
				get
		        {
					if(_PalleteID_W == null)
	        	    {
						_PalleteID_W = TearOff.PalleteID;
					}
					return _PalleteID_W;
				}
			}

			public WhereParameter Confirmed
		    {
				get
		        {
					if(_Confirmed_W == null)
	        	    {
						_Confirmed_W = TearOff.Confirmed;
					}
					return _Confirmed_W;
				}
			}

			private WhereParameter _ID_W = null;
			private WhereParameter _PalletLocationID_W = null;
			private WhereParameter _Label_W = null;
			private WhereParameter _PalleteID_W = null;
			private WhereParameter _Confirmed_W = null;

			public void WhereClauseReset()
			{
				_ID_W = null;
				_PalletLocationID_W = null;
				_Label_W = null;
				_PalleteID_W = null;
				_Confirmed_W = null;

				this._entity.Query.FlushWhereParameters();

			}
	
			private BusinessEntity _entity;
			private TearOffWhereParameter _tearOff;
			
		}
	
		public WhereClause Where
		{
			get
			{
				if(_whereClause == null)
				{
					_whereClause = new WhereClause(this);
				}
		
				return _whereClause;
			}
		}
		
		private WhereClause _whereClause = null;	
		#endregion
	
		#region Aggregate Clause
		public class AggregateClause
		{
			public AggregateClause(BusinessEntity entity)
			{
				this._entity = entity;
			}
			
			public TearOffAggregateParameter TearOff
			{
				get
				{
					if(_tearOff == null)
					{
						_tearOff = new TearOffAggregateParameter(this);
					}

					return _tearOff;
				}
			}

			#region AggregateParameter TearOff's
			public class TearOffAggregateParameter
			{
				public TearOffAggregateParameter(AggregateClause clause)
				{
					this._clause = clause;
				}
				
				
				public AggregateParameter ID
				{
					get
					{
							AggregateParameter aggregate = new AggregateParameter(ColumnNames.ID, Parameters.ID);
							this._clause._entity.Query.AddAggregateParameter(aggregate);
							return aggregate;
					}
				}

				public AggregateParameter PalletLocationID
				{
					get
					{
							AggregateParameter aggregate = new AggregateParameter(ColumnNames.PalletLocationID, Parameters.PalletLocationID);
							this._clause._entity.Query.AddAggregateParameter(aggregate);
							return aggregate;
					}
				}

				public AggregateParameter Label
				{
					get
					{
							AggregateParameter aggregate = new AggregateParameter(ColumnNames.Label, Parameters.Label);
							this._clause._entity.Query.AddAggregateParameter(aggregate);
							return aggregate;
					}
				}

				public AggregateParameter PalleteID
				{
					get
					{
							AggregateParameter aggregate = new AggregateParameter(ColumnNames.PalleteID, Parameters.PalleteID);
							this._clause._entity.Query.AddAggregateParameter(aggregate);
							return aggregate;
					}
				}

				public AggregateParameter Confirmed
				{
					get
					{
							AggregateParameter aggregate = new AggregateParameter(ColumnNames.Confirmed, Parameters.Confirmed);
							this._clause._entity.Query.AddAggregateParameter(aggregate);
							return aggregate;
					}
				}


				private AggregateClause _clause;
			}
			#endregion
		
			public AggregateParameter ID
		    {
				get
		        {
					if(_ID_W == null)
	        	    {
						_ID_W = TearOff.ID;
					}
					return _ID_W;
				}
			}

			public AggregateParameter PalletLocationID
		    {
				get
		        {
					if(_PalletLocationID_W == null)
	        	    {
						_PalletLocationID_W = TearOff.PalletLocationID;
					}
					return _PalletLocationID_W;
				}
			}

			public AggregateParameter Label
		    {
				get
		        {
					if(_Label_W == null)
	        	    {
						_Label_W = TearOff.Label;
					}
					return _Label_W;
				}
			}

			public AggregateParameter PalleteID
		    {
				get
		        {
					if(_PalleteID_W == null)
	        	    {
						_PalleteID_W = TearOff.PalleteID;
					}
					return _PalleteID_W;
				}
			}

			public AggregateParameter Confirmed
		    {
				get
		        {
					if(_Confirmed_W == null)
	        	    {
						_Confirmed_W = TearOff.Confirmed;
					}
					return _Confirmed_W;
				}
			}

			private AggregateParameter _ID_W = null;
			private AggregateParameter _PalletLocationID_W = null;
			private AggregateParameter _Label_W = null;
			private AggregateParameter _PalleteID_W = null;
			private AggregateParameter _Confirmed_W = null;

			public void AggregateClauseReset()
			{
				_ID_W = null;
				_PalletLocationID_W = null;
				_Label_W = null;
				_PalleteID_W = null;
				_Confirmed_W = null;

				this._entity.Query.FlushAggregateParameters();

			}
	
			private BusinessEntity _entity;
			private TearOffAggregateParameter _tearOff;
			
		}
	
		public AggregateClause Aggregate
		{
			get
			{
				if(_aggregateClause == null)
				{
					_aggregateClause = new AggregateClause(this);
				}
		
				return _aggregateClause;
			}
		}
		
		private AggregateClause _aggregateClause = null;	
		#endregion
	
		protected override IDbCommand GetInsertCommand() 
		{
		
			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "[" + this.SchemaStoredProcedure + "proc_HalfPalletLocationInsert]";
	
			CreateParameters(cmd);
			    
			return cmd;
		}
	
		protected override IDbCommand GetUpdateCommand()
		{
		
			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "[" + this.SchemaStoredProcedure + "proc_HalfPalletLocationUpdate]";
	
			CreateParameters(cmd);
			      
			return cmd;
		}
	
		protected override IDbCommand GetDeleteCommand()
		{
		
			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "[" + this.SchemaStoredProcedure + "proc_HalfPalletLocationDelete]";
	
			SqlParameter p;
			p = cmd.Parameters.Add(Parameters.ID);
			p.SourceColumn = ColumnNames.ID;
			p.SourceVersion = DataRowVersion.Current;

  
			return cmd;
		}
		
		private IDbCommand CreateParameters(SqlCommand cmd)
		{
			SqlParameter p;
		
			p = cmd.Parameters.Add(Parameters.ID);
			p.SourceColumn = ColumnNames.ID;
			p.SourceVersion = DataRowVersion.Current;

			p = cmd.Parameters.Add(Parameters.PalletLocationID);
			p.SourceColumn = ColumnNames.PalletLocationID;
			p.SourceVersion = DataRowVersion.Current;

			p = cmd.Parameters.Add(Parameters.Label);
			p.SourceColumn = ColumnNames.Label;
			p.SourceVersion = DataRowVersion.Current;

			p = cmd.Parameters.Add(Parameters.PalleteID);
			p.SourceColumn = ColumnNames.PalleteID;
			p.SourceVersion = DataRowVersion.Current;

			p = cmd.Parameters.Add(Parameters.Confirmed);
			p.SourceColumn = ColumnNames.Confirmed;
			p.SourceVersion = DataRowVersion.Current;


			return cmd;
		}
	}
}
