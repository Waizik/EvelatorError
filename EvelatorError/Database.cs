using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http.Results;
using Microsoft.Owin.BuilderProperties;

#pragma warning disable 1587
namespace EvelatorError
{
   
    /// <summary>
    /// Class for saving error to database 
    /// </summary>
    /// <remarks>Seve error and save and update all information related to error</remarks>
    static class Database
    {
        /// <summary>
        /// connect string property
        /// </summary>
        /// <value> String used to connect to database</value>
        public static string ConnectString { get; private set; }

        /// <summary>
        /// Starting database
        /// </summary>
        public static void Start()
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder
            {
                DataSource = @"localhost\WAIZI",
                InitialCatalog = "ElevatorDB",
                IntegratedSecurity = true
            };
            ConnectString = csb.ConnectionString;
            Test();

        }

        /// <summary>
        /// Test if connection is OK
        /// </summary>
        /// <returns>Bool true, if it is ok</returns>
        public static void Test()
        {
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(ConnectString);
                connection.Open();
            }
            catch
            {
                throw;
            }
            finally
            {
                connection?.Close();
            }
        }

     


        /// <summary>
        /// Method to insert error into database
        /// </summary>
        /// <remarks>If Evelator with this id dont exists, evelator is automatic create with this ID</remarks>
        /// <param name="error"> Error which is saving. See:<see cref="Error"/></param>
        public static void InsertError(ErrorMessage error)
        {
            string sqlQuery =
                "BEGIN TRANSACTION [Tran] " +
                "BEGIN TRY "+
                "DECLARE @NEW_ID INT " +
                "INSERT INTO EvelatorMessage (EvelatorID, Floor, TimeStamp) VALUES(@EvelatorID, @Floor, @TimeStamp) " +
                "IF @@ROWCOUNT>0 " + //rowcount is number of affected row
                "BEGIN " +
                "SET @NEW_ID = SCOPE_IDENTITY() " +
                "END " +
                "IF @NEW_ID IS NOT NULL " +
                "BEGIN ";  
                    foreach (var err in error.Errors) //foreach pres celej list erroru
                    {
                        sqlQuery += "INSERT INTO Error (MessageID, ErrorCode) VALUES (@NEW_ID, @ErrorCode"+err+"); ";
                    }
                sqlQuery += "END " +
                "IF @NEW_ID IS NOT NULL " +
                "BEGIN ";
                    foreach (var state in error.States)
                    {
                        sqlQuery += "INSERT INTO State (MessageID, StateCode) VALUES (@NEW_ID, @StateCode" + state + "); ";
                    }//foreach pres celej list state
               sqlQuery += "END " +
                "COMMIT TRANSACTION [Tran] " +
                "END TRY " +
                "BEGIN CATCH " +
                ";THROW " +
                "ROLLBACK TRANSACTION [Tran] " +
                "END CATCH ";
                
                //"GO"; 
            
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@EvelatorID", error.EvelatorID);
                        sqlCommand.Parameters.AddWithValue("@Floor", error.Floor);
                        sqlCommand.Parameters.AddWithValue("@TimeStamp", error.TimeStamp);
                        foreach (var err in error.Errors)
                        {
                            sqlCommand.Parameters.AddWithValue("@ErrorCode" + err, err);
                        }
                        foreach (var state in error.States)
                        {
                            sqlCommand.Parameters.AddWithValue("@StateCode" + state, state);
                        }    
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            } ///Evelator with this ID dont exists 
            
            catch (SqlException e) when (e.Number == 547)
            {
                ///Create it
                sqlQuery =
                    "BEGIN TRANSACTION [Tran] " +
                    "BEGIN TRY " +
                    "DECLARE @NEW_ID INT " +
                    "INSERT INTO Evelator (EvelatorID) VALUES(@EvelatorID);INSERT INTO EvelatorMessage (EvelatorID, Floor, TimeStamp) VALUES (@EvelatorID, @Floor, @TimeStamp); " +
                    "IF @@ROWCOUNT>0 " + //rowcount is number of affected row
                    "BEGIN " +
                    "SET @NEW_ID = SCOPE_IDENTITY() " +
                    "END " +
                    "IF @NEW_ID IS NOT NULL " +
                    "BEGIN ";
                        foreach (var err in error.Errors) //foreach pres celej list erroru
                        {
                            sqlQuery += "INSERT INTO Error (MessageID, ErrorCode) VALUES (@NEW_ID, @ErrorCode" + err + "); ";
                        }
                    sqlQuery += "END " +
                    "IF @NEW_ID IS NOT NULL " +
                    "BEGIN ";
                        foreach (var state in error.States)
                        {
                            sqlQuery += "INSERT INTO State (MessageID, StateCode) VALUES (@NEW_ID, @StateCode" + state + "); ";
                        }//foreach pres celej list state
                    sqlQuery += "END " +
                   "COMMIT TRANSACTION [Tran] " +
                   "END TRY " +
                   "BEGIN CATCH " +
                    ";THROW " +
                   "ROLLBACK TRANSACTION [Tran] " +
                   "END CATCH ";
                using (SqlConnection connection = new SqlConnection(ConnectString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@EvelatorID", error.EvelatorID);
                        sqlCommand.Parameters.AddWithValue("@Floor", error.Floor);
                        sqlCommand.Parameters.AddWithValue("@TimeStamp", error.TimeStamp);
                        foreach (var err in error.Errors)
                        {
                            sqlCommand.Parameters.AddWithValue("@ErrorCode" + err, err);
                        }
                        foreach (var state in error.States)
                        {
                            sqlCommand.Parameters.AddWithValue("@StateCode" + state, state);
                        }
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }

        }

        

         /// <summary>
         /// Method to Update existing evelator according to evelatorID
         /// </summary>
         /// <remarks> Control if evelatorID exists. If it is not, throw exception. If newEvelatorID parameter is not null, then control if newEvelatorID in database is null
         ///  or dont exists  NewEvelatorID record here. In other case throw exeption. 
         /// All parameter except evelatorID should be null</remarks>
         /// <param name="evelatorID"> New evelator id record. Shouldnt be null</param>
         /// <param name="postcode">New postcode record</param>
         /// <param name="street">New street record</param>
         /// <param name="number">New number record</param>
         /// <param name="locality">New locality record</param>
         /// <param name="newEvelatorID">New newEvelator record</param>
         public static void UpdateEvelator(filterModel filter)
             //Finish update SerialID. If exits error with this SerialID, I have to update serialID of ther.                                                                                                         
         {

             using (SqlConnection connection = new SqlConnection(ConnectString))
             {

                 try
                 {
                     connection.Open();
                     ///Control existing record (evelatorID) existuje zaznam, ktery chceme aktualizovat? Existuje ID?
                     if (TestRecordIdExists(filter.EvelatorID, connection) == 0)
                     {
                         throw new UpdateRecordDontExistException();                
                     }
                     if (filter.NewEvelatorID.HasValue)
                     {
                         if (filter.EvelatorID == filter.NewEvelatorID.Value)
                         {
                             throw new UpdateIdIsSame();
                         }
                         if (TestRecordIdExists(filter.NewEvelatorID.Value, connection) == 0)//existuje new evelator id?
                         {
                             throw new UpdateRecordDontExistException();
                         }
                         ///Control if Evelator according to EvelatorID have NewEvelatorID record and where points. (jeslti uz tam neni zaznam)
                         if (TestRecordIdPoints(filter.EvelatorID, connection) != null)
                         {
                             throw new UpdateRecordPoint();
                         }
                         ///Control duplicate newEvelatorID
                         if (TestDuplicateNewRecordId(filter.NewEvelatorID.Value, connection) != null)
                         {
                             throw new UpdateDuplicateKeyException();
                         }
                     }
                     ///Update record
                     string sqlQuery = "UPDATE Evelator SET Postcode = @PostCode, Street = @Street, Number = @Number, Locality = @Locality, NewEvelatorID = @NewEvelatorID WHERE EvelatorID = @EvelatorID";
                     using (
                         SqlCommand sqlCommand =
                             new SqlCommand(sqlQuery, connection))
                     {
                         sqlCommand.Parameters.AddWithValue("@EvelatorID", filter.EvelatorID);
                         sqlCommand.Parameters.AddWithValue("@PostCode",
                             filter.PostCode.HasValue ? (object) filter.PostCode.Value : DBNull.Value);
                         sqlCommand.Parameters.AddWithValue("@Street",filter.Street != null ? (object) filter.Street : DBNull.Value);
                         sqlCommand.Parameters.AddWithValue("@Number",
                             filter.Number.HasValue ? (object) filter.Number.Value : DBNull.Value);
                         sqlCommand.Parameters.AddWithValue("@Locality",
                             filter.Locality != null ? (object) filter.Locality : DBNull.Value);
                         sqlCommand.Parameters.AddWithValue("@NewEvelatorID",
                             filter.NewEvelatorID.HasValue ? (object) filter.NewEvelatorID.Value : DBNull.Value);
                         sqlCommand.ExecuteNonQuery();
                     }
                 }
                 catch (Exception)
                 {
                     throw ;
                 }
                 return;
             }

         }

      
         /// <summary>
         /// Control if record exists
         /// </summary>
         /// <remarks>Return count of Evelator with RecordID existuje zaznam s timto ID</remarks>
         /// <param name="recordID">recordID to control</param>
         /// <param name="connection">Opening connection</param>
         /// <returns>Count of evelator with RecordID</returns>
         public static int TestRecordIdExists(int recordID, SqlConnection connection)
         {
             string testQuery;
             testQuery = "SELECT COUNT(*) FROM Evelator WHERE EvelatorID = @EvelatorID";
             using (SqlCommand sqlCommand = new SqlCommand(testQuery, connection))
             {
                 sqlCommand.Parameters.AddWithValue("@EvelatorID", recordID);
                 var count = (int) sqlCommand.ExecuteScalar();
                 return count;
             }
         }

         /// <summary>
         /// Control if newEvelatorID with this record is null or pointing to another Evelator (jestli uz nema zaznam)
         /// </summary>
         /// <remarks>Return newEvelatorID of Evelator with recordID</remarks>
         /// <param name="recordID">recordID to control</param>
         /// <param name="connection">Opening connection</param>
         /// <returns>newEvelatorID of Evelator with recordID</returns>
         public static int? TestRecordIdPoints(int recordID, SqlConnection connection)
         {
             int? newEvelatorID = null;
             string testQuery = "SELECT NewEvelatorID FROM Evelator WHERE EvelatorID = @EvelatorID";
             using (SqlCommand sqlCommand = new SqlCommand(testQuery, connection))
             {
                 sqlCommand.Parameters.AddWithValue("@EvelatorID", recordID);
                 SqlDataReader dataReader = sqlCommand.ExecuteReader();

                 while (dataReader.Read())
                 {
                     if (dataReader["NewEvelatorID"] == DBNull.Value)
                     {
                         dataReader.Close();
                         return null;
                     }
                     newEvelatorID = (int?) dataReader["NewEvelatorID"];
                 }
                 dataReader.Close();
                 return newEvelatorID;
             }
         }

         /// <summary>
         /// Control if newEvelatorID is not duplicate (jestli uz tento zaznam neni v databazi)
         /// </summary>
         /// <param name="NewrecordID">Controling duplicate record</param>
         /// <param name="connection">Opening connection</param>
         /// <returns>null if NewRecordID is not duplicate, in other case duplicate ID</returns>
         public static int? TestDuplicateNewRecordId(int NewrecordID, SqlConnection connection)
         {
             int? newEvelatorID = null;
             string testQuery = "SELECT NewEvelatorID FROM Evelator WHERE NewEvelatorID = @NewEvelatorID";
             using (SqlCommand sqlCommand = new SqlCommand(testQuery, connection))
             {
                 sqlCommand.Parameters.AddWithValue("@NewEvelatorID", NewrecordID);
                 SqlDataReader dataReader = sqlCommand.ExecuteReader();

                 while (dataReader.Read())
                 {
                     if (dataReader["NewEvelatorID"] == DBNull.Value)
                     {
                         dataReader.Close();
                         return null;
                     }
                     newEvelatorID = (int?) dataReader["NewEvelatorID"];
                 }
                 dataReader.Close();
                 return newEvelatorID;
             }
         }


         public static List<ErrorMessageAndEvelator> GetAllErrors()
         {
             List<ErrorMessageAndEvelator> errors = new List<ErrorMessageAndEvelator>();

             using (SqlConnection connection = new SqlConnection(ConnectString))
             {
                 connection.Open();
                 string sqlQuery = @"select f.OrderID, f.EvelatorID, f.Floor, f.TimeStamp, Evelator.Postcode, Evelator.Street, Evelator.Number, Evelator.Locality, Evelator.NewEvelatorID 
                                    from
	                                    (select f.OrderID, f.EvelatorID, f.Floor, f.TimeStamp
	                                    from 
		                                    (select EvelatorID, max(TimeStamp) as maxtime
		                                    from
		                                    EvelatorMessage
		                                    group by EvelatorID
	                                    ) as x inner join EvelatorMessage as f on f.EvelatorID = x.EvelatorID and f.TimeStamp = x.maxtime
                                    )as f, Evelator
                                    where Evelator.EvelatorID = f.EvelatorID;";

                 using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection))
                 {
                     SqlDataReader dataReader = sqlCommand.ExecuteReader();

                     while (dataReader.Read())
                     {
                        ErrorMessageAndEvelator error = new ErrorMessageAndEvelator();
                        error.EvelatorID = (int) dataReader["EvelatorID"];
                        error.Floor = (int) dataReader["Floor"];
                        error.TimeStamp = (System.DateTime) dataReader["TimeStamp"];
                        error.Number = (dataReader["Number"] == DBNull.Value
                              ? (int?) null
                              : (int) dataReader["Number"]);
                        error.Postcode = (dataReader["Postcode"] == DBNull.Value
                              ? (int?) null
                              : (int) dataReader["Postcode"]);
                        error.NewEvelatorID = (dataReader["NewEvelatorID"] == DBNull.Value
                              ? (int?) null
                              : (int) dataReader["NewEvelatorID"]);
                        error.Street = (dataReader["Street"] == DBNull.Value ? null : (string) dataReader["Street"]);
                        error.Locality = (dataReader["Locality"] == DBNull.Value
                              ? null
                              : (string) dataReader["Locality"]);
                        error.IsNull = (dataReader["Number"] == DBNull.Value ||
                                        dataReader["Postcode"] == DBNull.Value ||
                                        dataReader["Street"] == DBNull.Value ||
                                        dataReader["Locality"] == DBNull.Value
                                    ? 1
                                    : 0);
                        error.OrderId = (int) dataReader["OrderID"];
                        errors.Add(error);
                     }
                     dataReader.Close();
                        

                     foreach (var error in errors)
                     {
                        ErrorMessageAndEvelator referenceMessage = errors.Find(x => x.OrderId == error.OrderId);
                        using (SqlCommand sqlCommandErr = new SqlCommand("SELECT ErrorCode FROM Error WHERE MessageID = "+error.OrderId, connection))
                         {
                            SqlDataReader dataReaderError = sqlCommandErr.ExecuteReader();
                            referenceMessage.Errors = new List<int>();
                            while (dataReaderError.Read())
                             {
                                referenceMessage.Errors.Add((int)dataReaderError["ErrorCode"]);
                             }
                            dataReaderError.Close();
                         }

                         using (SqlCommand sqlCommandState = new SqlCommand("SELECT StateCode FROM State WHERE MessageID = "+error.OrderId, connection))
                         {
                             SqlDataReader dataReaderState = sqlCommandState.ExecuteReader();
                            referenceMessage.States = new List<int>();
                             while (dataReaderState.Read())
                             {
                                 referenceMessage.States.Add((int)dataReaderState["StateCode"]);
                             }
                             dataReaderState.Close();
                         }
                     }
                  }
             }
             return errors;
         }
        //todo predelat dotaz
        //+nacitani erroru a statu tak jako u getAllErrors
         public static List<ErrorMessageAndEvelator> GetAllFilterErrors(filterModel filter)
         {

             List<ErrorMessageAndEvelator> errors = new List<ErrorMessageAndEvelator>();

             using (SqlConnection connection = new SqlConnection(ConnectString))
             {
                 connection.Open();
                 string sqlQuery =
                     @"select f.OrderID, f.EvelatorID, f.Floor, f.TimeStamp, Evelator.Postcode, Evelator.Street, Evelator.Number, Evelator.Locality, Evelator.NewEvelatorID 
                                    from
	                                    (select f.OrderID, f.EvelatorID, f.Floor, f.TimeStamp
	                                    from 
		                                    (select EvelatorID, max(TimeStamp) as maxtime
		                                    from
		                                    EvelatorMessage
		                                    group by EvelatorID
	                                    ) as x inner join EvelatorMessage as f on f.EvelatorID = x.EvelatorID and f.TimeStamp = x.maxtime
                                    )as f, Evelator
                                    where Evelator.EvelatorID = f.EvelatorID";
                                    if (filter.Number.HasValue)
                                    {
                                        sqlQuery += " AND Evelator.Number = " + filter.Number.ToString() ;
                                    }
                                    if (filter.PostCode.HasValue)
                                    {
                                        sqlQuery += " AND Evelator.Postcode = " + filter.PostCode.ToString();
                                    }
                                    if (!String.IsNullOrEmpty(filter.Locality))
                                    {
                                        sqlQuery += " AND Evelator.Locality = '" + filter.Locality + "'";
                                    }
                                    if (!String.IsNullOrEmpty(filter.Street))
                                    {
                                         sqlQuery += " AND Evelator.Street = '" + filter.Street + "'";
                                    }
                                    sqlQuery += ";";

                 using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection))
                 {
                     SqlDataReader dataReader = sqlCommand.ExecuteReader();
                     while (dataReader.Read())
                     {
                         ErrorMessageAndEvelator error = new ErrorMessageAndEvelator();
                         error.EvelatorID = (int) dataReader["EvelatorID"];
                         error.Floor = (int) dataReader["Floor"];
                         error.TimeStamp = (System.DateTime) dataReader["TimeStamp"];
                         error.Number = (dataReader["Number"] == DBNull.Value ? (int?) null : (int) dataReader["Number"]);
                         error.Postcode = (dataReader["Postcode"] == DBNull.Value
                             ? (int?) null
                             : (int) dataReader["Postcode"]);
                         error.NewEvelatorID = (dataReader["NewEvelatorID"] == DBNull.Value
                             ? (int?) null
                             : (int) dataReader["NewEvelatorID"]);
                         error.Street = (dataReader["Street"] == DBNull.Value ? null : (string) dataReader["Street"]);
                         error.Locality = (dataReader["Locality"] == DBNull.Value
                             ? null
                             : (string) dataReader["Locality"]);
                         error.IsNull = (dataReader["Number"] == DBNull.Value || dataReader["Postcode"] == DBNull.Value ||
                                         dataReader["Street"] == DBNull.Value || dataReader["Locality"] == DBNull.Value
                             ? 1
                             : 0);
                        error.OrderId = (int)dataReader["OrderID"];
                        errors.Add(error);
                     }
                    dataReader.Close();
                    foreach (var error in errors)
                    {
                        ErrorMessageAndEvelator referenceMessage = errors.Find(x => x.OrderId == error.OrderId);
                        using (SqlCommand sqlCommandErr = new SqlCommand("SELECT ErrorCode FROM Error WHERE MessageID = " + error.OrderId, connection))
                        {
                            SqlDataReader dataReaderError = sqlCommandErr.ExecuteReader();
                            referenceMessage.Errors = new List<int>();
                            while (dataReaderError.Read())
                            {
                                referenceMessage.Errors.Add((int)dataReaderError["ErrorCode"]);
                            }
                            dataReaderError.Close();
                        }

                        using (SqlCommand sqlCommandState = new SqlCommand("SELECT StateCode FROM State WHERE MessageID = " + error.OrderId, connection))
                        {
                            SqlDataReader dataReaderState = sqlCommandState.ExecuteReader();
                            referenceMessage.States = new List<int>();
                            while (dataReaderState.Read())
                            {
                                referenceMessage.States.Add((int)dataReaderState["StateCode"]);
                            }
                            dataReaderState.Close();
                        }
                    }
                }
             }
             return errors;
         }
        
         public static ErrorMessageAndEvelator GetIdError(int id)
         {
             ErrorMessageAndEvelator error = new ErrorMessageAndEvelator();
             using (SqlConnection connection = new SqlConnection(ConnectString))
             {
                 connection.Open();
                 string sqlQuery = "SELECT Number, Postcode, Street, Locality FROM Evelator WHERE EvelatorID = @EvelatorID";
                 using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection))
                 {
                     sqlCommand.Parameters.AddWithValue("@EvelatorID", id);
                     SqlDataReader dataReader = sqlCommand.ExecuteReader();

                     while (dataReader.Read())
                     {
                         error.Number = (dataReader["Number"] == DBNull.Value ? (int?)null : (int)dataReader["Number"]);
                         error.Postcode = (dataReader["Postcode"] == DBNull.Value
                             ? (int?)null
                             : (int)dataReader["Postcode"]);
                         error.Street = (dataReader["Street"] == DBNull.Value ? null : (string)dataReader["Street"]);
                         error.Locality = (dataReader["Locality"] == DBNull.Value
                             ? null
                             : (string)dataReader["Locality"]);
                     }
                 }
             }
             return error;
         } 
    }
}
