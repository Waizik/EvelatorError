using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Owin.BuilderProperties;

#pragma warning disable 1587
namespace EvelatorError
{
    class ErrorAndEvelator
    {
        /// <summary>
        /// EvelatorID property
        /// </summary>
        /// <value> ID of evelator </value>
        public int? EvelatorID { get; set; }
        /// <summary>
        /// ErrorCode property
        /// </summary>
        /// <value>Specific error</value>
        public int? ErrorCode { get; set; }
        /// <summary>
        /// Floor property
        /// </summary>
        /// <value> Floor where Evelator is</value>
        public int? Floor { get; set; }
        /// <summary>
        /// Timestamp property
        /// </summary>
        /// <value> When the error comes to server</value>
        public DateTime TimeStamp { get; set; }

        public int? Postcode { get; set; }

        public string Street { get; set; }

        public int? Number { get; set; }

        public string Locality { get; set; }

        public int? NewEvelatorID { get; set; }

        public int IsNull { get; set; }
    }
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
                // ReSharper disable once UseNullPropagation
                if (connection != null)
                    connection.Close();
            }
        }

        /// <summary>
        /// Method to insert error into database
        /// </summary>
        /// <remarks>If Evelator with this id dont exists, evelator is automatic create with this ID</remarks>
        /// <param name="error"> Error which is saving. See:<see cref="Error"/></param>
        public static void InsertError(Error error)
        {
            string sqlQuery =
                "INSERT INTO EvelatorError (EvelatorID, ErrorCode, Floor, TimeStamp) VALUES(@EvelatorID, @ErrorCode, @Floor, @TimeStamp)";
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@EvelatorID", error.EvelatorID);
                        sqlCommand.Parameters.AddWithValue("@ErrorCode", error.ErrorCode);
                        sqlCommand.Parameters.AddWithValue("@Floor", error.Floor);
                        sqlCommand.Parameters.AddWithValue("@TimeStamp", error.TimeStamp);
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            } ///Evelator with this ID dont exists
            catch (SqlException e) when (e.Number == 547)
            {
                ///Create it
                sqlQuery =
                    "INSERT INTO Evelator (EvelatorID) VALUES(@EvelatorID);INSERT INTO EvelatorError (EvelatorID, ErrorCode, Floor, TimeStamp) VALUES (@EvelatorID, @ErrorCode, @Floor, @TimeStamp);";
                using (SqlConnection connection = new SqlConnection(ConnectString))
                {
                    connection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@EvelatorID", error.EvelatorID);
                        sqlCommand.Parameters.AddWithValue("@ErrorCode", error.ErrorCode);
                        sqlCommand.Parameters.AddWithValue("@Floor", error.Floor);
                        sqlCommand.Parameters.AddWithValue("@TimeStamp", error.TimeStamp);
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
        public static void UpdateEvelator(int evelatorID, int? postcode, string street, int? number, string locality,
            int? newEvelatorID)
            //Finish update SerialID. If exits error with this SerialID, I have to update serialID of ther.                                                                                                         
        {
            using (SqlConnection connection = new SqlConnection(ConnectString))
            {
                connection.Open();
                ///Control existing record (evelatorID)
                if (TestRecordIdExists(evelatorID, connection) == 0)
                    throw new UpdateRecordDontExistException();
                if (newEvelatorID.HasValue)
                {
                    ///Control existing record (EvelatorID in database with NewEvelatorID in parameters)
                    if (TestRecordIdExists(newEvelatorID.Value, connection) == 0)
                        throw new UpdateRecordDontExistException();
                    ///Control if Evelator according to EvelatorID have NewEvelatorID record and where points.
                    if (TestRecordIdPoints(evelatorID, connection) != null)
                    {
                        throw new UpdateRecordPoint((int) TestRecordIdPoints(evelatorID, connection));
                    }
                    ///Control duplicate newEvelatorID
                    if (TestDuplicateNewRecordId((int) newEvelatorID, connection) != null)
                    {
                        throw new UpdateDuplicateKeyException();
                    }
                }
                ///Update record
                using (
                    SqlCommand sqlCommand =
                        new SqlCommand(
                            "UPDATE Evelator SET Postcode = @PostCode, Street = @Street, Number = @Number, Locality = @Locality, NewEvelatorID = @NewEvelatorID WHERE EvelatorID = @EvelatorID",
                            connection))
                {
                    sqlCommand.Parameters.AddWithValue("@EvelatorID", evelatorID);
                    sqlCommand.Parameters.AddWithValue("@PostCode",
                        postcode.HasValue ? (object) postcode.Value : DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@Street", street != null ? (object) street : DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@Number", number.HasValue ? (object) number.Value : DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@Locality", locality != null ? (object) locality : DBNull.Value);
                    sqlCommand.Parameters.AddWithValue("@NewEvelatorID",
                        newEvelatorID.HasValue ? (object) newEvelatorID.Value : DBNull.Value);
                    sqlCommand.ExecuteNonQuery();
                }
            }

        }

        /// <summary>
        /// Update existing evelator according to OldEvelatorID to evelator with newEvelatorID
        /// </summary>
        /// <remarks>Control if Update record with OldEvelator exist, NewEvelatorID is not duplicate and if NewEvelatorID of OldEvelatorID is null.
        ///  In other case throw exception. All parameters except OldevelatorID and NewEvelatorID should be null. 
        /// Create a new record with EvelatorID accord to NewEvelatorID and in Older Record set NewEvelatorID to OldEvelatorID</remarks>
        /// <param name="newEvelatorID">NewEvelatorID record. Shouldnt be null</param>
        /// <param name="oldEvelatorID">OldEvelatorID record. Shouldnt be null</param>
        /// <param name="postcode">Postcode record</param>
        /// <param name="street">Street record</param>
        /// <param name="number">Number of evelator house record</param>
        /// <param name="locality">Locality of evelator house record (town)</param>
        public static void UpdateEvelatorToNew(int newEvelatorID, int oldEvelatorID, int? postcode, string street,
            int? number, string locality)
        {
            using (SqlConnection connection = new SqlConnection(ConnectString))
            {
                connection.Open();

                ///Test if record exists
                if (TestRecordIdExists(oldEvelatorID, connection) == 0)
                    throw new UpdateRecordDontExistException();
                ///Control if Evelator according to EvelatorID have NewEvelatorID record and where points. --> It is old record. 
                if (TestRecordIdPoints(oldEvelatorID, connection) != null)
                {
                    throw new UpdateRecordPoint((int) TestRecordIdPoints(oldEvelatorID, connection));
                }
                ///Control duplicate newEvelatorID. Record with this id already exist. 
                if (TestDuplicateNewRecordId(newEvelatorID, connection) != null)
                {
                    throw new UpdateDuplicateKeyException();
                }
                ///Create new evelator record
                string sqlQuery =
                    "INSERT INTO Evelator (EvelatorID, Postcode, Street, Number, Locality) VALUES(@EvelatorID, @Postcode, @Street, @Number, @Locality)";
                try
                {
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@EvelatorID", newEvelatorID);
                        sqlCommand.Parameters.AddWithValue("@Postcode", postcode.HasValue ? (object) postcode.Value : DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Street", street != null ? (object) street : DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Number", number.HasValue ? (object) number.Value : DBNull.Value);
                        sqlCommand.Parameters.AddWithValue("@Locality", locality != null ? (object) locality : DBNull.Value);
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                catch (SqlException e) when (e.Number == 2627) //Duplicate primary key (EvelatorID)
                {
                    throw new UpdateDuplicateKeyException();
                }
                sqlQuery = "UPDATE Evelator SET NewEvelatorID = @NewEvelatorID WHERE EvelatorID = @EvelatorID";
                using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection))
                {
                    sqlCommand.Parameters.AddWithValue("@NewEvelatorID", newEvelatorID);
                    sqlCommand.Parameters.AddWithValue("@EvelatorID", oldEvelatorID);
                    sqlCommand.ExecuteNonQuery();
                }

            }
        }

        /// <summary>
        /// Control if record exists
        /// </summary>
        /// <remarks>Return count of Evelator with RecordID</remarks>
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
        /// Control if newEvelatorID with this record is null or pointing to another Evelator
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
        /// Control if newEvelatorID is not duplicate
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

        public static List<ErrorAndEvelator> GetAllErrors()
        {
            List<ErrorAndEvelator> errors = new List<ErrorAndEvelator>();
           
            using (SqlConnection connection = new SqlConnection(ConnectString))
            {
                connection.Open();
                string sqlQuery = @"select f.OrderID, f.EvelatorID, f.ErrorCode, f.Floor, f.TimeStamp, Evelator.Postcode, Evelator.Street, Evelator.Number, Evelator.Locality, Evelator.NewEvelatorID
                                        from(
	                                        select f.OrderID, f.EvelatorID, f.ErrorCode, f.Floor, f.TimeStamp
	                                        from ( select EvelatorID, max(TimeStamp) as maxtime
	                                        from EvelatorError
	                                        group by EvelatorID
	                                        ) as x inner join EvelatorError as f on f.EvelatorID = x.EvelatorID and f.TimeStamp = x.maxtime
                                        )as f, Evelator
                                        where Evelator.EvelatorID = f.EvelatorID;";

                using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, connection))
                {
                    SqlDataReader dataReader = sqlCommand.ExecuteReader();


                    while (dataReader.Read())
                    {
                        ErrorAndEvelator error = new ErrorAndEvelator();
                        error.EvelatorID = (int) dataReader["EvelatorID"];
                        error.ErrorCode = (int) dataReader["ErrorCode"];
                        error.Floor = (int) dataReader["Floor"];
                        error.TimeStamp = (System.DateTime) dataReader["TimeStamp"];
                        error.Number = (dataReader["Number"] == DBNull.Value ?  (int?) null :  (int) dataReader["Number"]);
                        error.Postcode = (dataReader["Postcode"] == DBNull.Value ? (int?)null : (int)dataReader["Postcode"]);
                        error.NewEvelatorID = (dataReader["NewEvelatorID"] == DBNull.Value ? (int?)null : (int)dataReader["NewEvelatorID"]);
                        error.Street = (dataReader["Street"] == DBNull.Value ? null : (string) dataReader["Street"]);
                        error.Locality = (dataReader["Locality"] == DBNull.Value ? null : (string)dataReader["Locality"]);
                        error.IsNull = (dataReader["Number"] == DBNull.Value || dataReader["Postcode"] == DBNull.Value ||
                                        dataReader["Street"] == DBNull.Value || dataReader["Locality"] == DBNull.Value
                            ? 1
                            : 0);
                        errors.Add(error);

                    }
                }
            }
            
            return errors;
        }

        public static List<ErrorAndEvelator> GetAllFilterErrors(filterModel filter)
        {
           
            List<ErrorAndEvelator> errors = new List<ErrorAndEvelator>();

            using (SqlConnection connection = new SqlConnection(ConnectString))
            {
                connection.Open();
                string sqlQuery =
                    @"select f.OrderID, f.EvelatorID, f.ErrorCode, f.Floor, f.TimeStamp, Evelator.Postcode, Evelator.Street, Evelator.Number, Evelator.Locality, Evelator.NewEvelatorID
                                        from(
	                                        select f.OrderID, f.EvelatorID, f.ErrorCode, f.Floor, f.TimeStamp
	                                        from ( select EvelatorID, max(TimeStamp) as maxtime
	                                        from EvelatorError
	                                        group by EvelatorID
	                                        ) as x inner join EvelatorError as f on f.EvelatorID = x.EvelatorID and f.TimeStamp = x.maxtime
                                        )as f, Evelator
                                        where Evelator.EvelatorID = f.EvelatorID";
                                        if (filter.Number.HasValue)
                                        {
                                            sqlQuery += " AND Evelator.Number = " + filter.Number.ToString() ;
                                        }
                                        Console.WriteLine(filter.PostCode.HasValue);
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
                    Console.WriteLine(sqlQuery);
                    SqlDataReader dataReader = sqlCommand.ExecuteReader();

                   

                    while (dataReader.Read())
                    {
                        ErrorAndEvelator error = new ErrorAndEvelator();
                        error.EvelatorID = (int) dataReader["EvelatorID"];
                        error.ErrorCode = (int) dataReader["ErrorCode"];
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
                        errors.Add(error);

                    }
                }
                

            }
            return errors;
        }
    }
}
