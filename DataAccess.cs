// DataAccess.cs
//         Title: DataAccess - Data Access Layer for Piecework Payroll
// Last Modified: October 19, 2021
//    Written By: Nicholas Shortt
// Based on code samples provided by Kyle Chapman
// 
// This is a module with a set of classes allowing for interaction between
// Piecework Worker data objects and a database.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;
using System.Data.SqlClient; // This may work or not depending on local versions.
                             // See this StackOverflow answer: https://stackoverflow.com/a/54472192
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;

namespace PayrollDemo
{
    class DataAccess
    {

        #region "Connection String"

        /// <summary>
        /// Return connection string
        /// </summary>
        /// <returns>Connection string</returns>
        private static string GetConnectionString()
        {
            /* Somehow, we need to have a working connection string. 
             * This is best done by adding an App.config file to your project by
             * using Add -> New Item... -> Application Configuration file.
             * For further details, refer to the Week 5 slides and/or the
             * class recording on the subject of connection strings (Week 6/7),
             * as well as https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/connection-strings-and-configuration-files .
             * Other options may be viable. */

            string returnValue = null;

            // Look for myConnectionString in the connectionStrings section.
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[1];
            
            // If found, return the connection string.
            if (settings != null)
                returnValue = settings.ConnectionString;

            return returnValue;
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Function that returns all workers in the database as a DataTable for display
        /// </summary>
        /// <returns>a DataTable containing all workers in the database</returns>
        internal static DataTable GetEmployeeList()
        {
            // Declare the SQL connection, SQL command, and SQL adapter
            SqlConnection dbConnection = new SqlConnection(GetConnectionString());
            SqlCommand command = new SqlCommand("SELECT [EmployeeId] AS ID, CONCAT([FirstName], ' ', [LastName]) AS Name, [StartDate] AS 'Start Date' FROM [Employee]", dbConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            // Declare a DataTable object that will hold the return value
            DataTable employeeTable = new DataTable();

            // Try to connect to the database, and use the adapter to fill the table
            try
            {
                dbConnection.Open();
                adapter.Fill(employeeTable);
            }
            catch (Exception ex)
            {
                // If there is an error, re-throw the exception to be handled by the presentation tier.
                // (You could also just do error messaging here but that's not as nice.)
                throw ex;
            }
            finally
            {
                adapter.Dispose();
                dbConnection.Close();
            }

            // Return the populated DataTable
            return employeeTable;
        }

        /// <summary>
        /// Function to add a new worker to the worker database
        /// </summary>
        /// <param name="insertWorker">a worker object to be inserted</param>
        /// <returns>true if successful</returns>
        internal static bool InsertNewRecord(PieceworkWorker insertWorker)
        {
            // Create return value
            bool returnValue = false;

            // Declare the SQL connection
            SqlConnection dbConnection = new SqlConnection(GetConnectionString());

            SqlCommand command;

            // Check if that worker exists
            if (!WorkerExists(insertWorker.ID))
            {
                // If not create a new employee on the database
                command = new SqlCommand("INSERT INTO Employee VALUES(@employeeID, @firstName, @lastName, @entryDate)", dbConnection);

                //Assign values
                command.Parameters.AddWithValue("@employeeID", insertWorker.ID);
                command.Parameters.AddWithValue("@firstName", insertWorker.FirstName);
                command.Parameters.AddWithValue("@lastName", insertWorker.LastName);
                command.Parameters.AddWithValue("@entryDate", insertWorker.TimeCreated);

                // Try to insert the new record, return result
                try
                {
                    dbConnection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // If there is an error, re-throw the exception to be handled by the presentation tier.
                    // (You could also just do error messaging here but that's not as nice.)
                    throw ex;
                }
                finally
                {
                    dbConnection.Close();
                }
            }

            // Check if the employee name matched employee name with that ID
            if(GetEmployeeName(insertWorker.ID) != insertWorker.Name)
            {
                // throw exception if not
                throw new ArgumentException("Name does not match ID's name", PieceworkWorker.NameParameter);
            }

            // Create new SQL command and assign it paramaters for a new entry
            command = new SqlCommand("INSERT INTO Entries VALUES(@employeeID, @messages, @pay, @entryDate)", dbConnection);

            // TO DO The next two lines assume workers only have 1 name. Read your requirements carefully!
            command.Parameters.AddWithValue("@employeeID", insertWorker.ID);
            command.Parameters.AddWithValue("@messages", insertWorker.Messages);
            command.Parameters.AddWithValue("@pay", insertWorker.Pay);
            command.Parameters.AddWithValue("@entryDate", insertWorker.TimeCreated);

            // Try to insert the new record, return result
            try
            {
                dbConnection.Open();
                returnValue = (command.ExecuteNonQuery() == 1);
            }
            catch (Exception ex)
            {
                // If there is an error, re-throw the exception to be handled by the presentation tier.
                // (You could also just do error messaging here but that's not as nice.)
                throw ex;
            }
            finally
            {
                dbConnection.Close();
            }

            // Return the true if this worked, false if it failed
            return returnValue;
        }

        /// <summary>
        /// Returns a total number of employees from the database.
        /// </summary>
        /// <returns>total employees, as a string</returns>
        internal static string GetTotalEmployees()
        {
            // Declare the SQL connection and the SQL command
            SqlConnection dbConnection = new SqlConnection(GetConnectionString());
            SqlCommand command = new SqlCommand("SELECT COUNT(EmployeeId) FROM Employee", dbConnection);

            // Try to open a connection to the database and read the total. Return result.
            try
            {
                dbConnection.Open();
                return command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                // If there is an error, re-throw the exception to be handled by the presentation tier.
                // (You could also just do error messaging here but that's not as nice.)
                throw ex;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        /// <summary>
        /// Returns a total number of messages from the database.
        /// </summary>
        /// <returns>total messages, as a string</returns>
        internal static string GetTotalMessages()
        {
            // Declare the SQL connection and the SQL command
            SqlConnection dbConnection = new SqlConnection(GetConnectionString());
            SqlCommand command = new SqlCommand("SELECT SUM(Messages) FROM Entries", dbConnection);

            // Try to open a connection to the database and read the total. Return result.
            try
            {
                dbConnection.Open();
                return command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                // If there is an error, re-throw the exception to be handled by the presentation tier.
                // (You could also just do error messaging here but that's not as nice.)
                throw ex;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        /// <summary>
        /// Returns a total pay among all employees from the database.
        /// </summary>
        /// <returns>total pay, as a string</returns>
        internal static string GetTotalPay()
        {
            // Declare the SQL connection and the SQL command
            SqlConnection dbConnection = new SqlConnection(GetConnectionString());
            SqlCommand command = new SqlCommand("SELECT SUM(Pay) FROM Entries", dbConnection);

            // Try to open a connection to the database and read the total. Return result.
            try
            {
                dbConnection.Open();
                return command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                // If there is an error, re-throw the exception to be handled by the presentation tier.
                // (You could also just do error messaging here but that's not as nice.)
                throw ex;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        /// <summary>
        /// Checks if given worker ID already exits
        /// </summary>
        /// <param name="id">The ID you are checking
        /// <returns>True if it does, False if not</returns>
        internal static bool WorkerExists(string id)
        {
            // Declare the SQL connection and the SQL command
            SqlConnection dbConnection = new SqlConnection(GetConnectionString());
            SqlCommand command = new SqlCommand("SELECT EmployeeId FROM Employee WHERE EmployeeId = " + id, dbConnection);

            try
            {
                dbConnection.Open();
                // Check if anything is returned
                if (command.ExecuteScalar() != null)
                {
                    // If so it exists
                    return true;
                }
                else
                {
                    // Else it does not
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        /// <summary>
        /// Function that returns a specific workers entry history
        /// </summary>
        /// <param name="id">ID of employee you want the entries for
        /// <returns>a DataTable containing all workers in the database</returns>
        internal static DataTable GetEmployeeEntries(int id)
        {
            // Declare the SQL connection, SQL command, and SQL adapter
            SqlConnection dbConnection = new SqlConnection(GetConnectionString());
            SqlCommand command = new SqlCommand("SELECT [EntryDate], [Messages], [Pay] FROM [Entries] WHERE [EmployeeId] = " + id, dbConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            // Declare a DataTable object that will hold the return value
            DataTable employeeTable = new DataTable();

            // Try to connect to the database, and use the adapter to fill the table
            try
            {
                dbConnection.Open();
                adapter.Fill(employeeTable);
            }
            catch (Exception ex)
            {
                // If there is an error, re-throw the exception to be handled by the presentation tier.
                // (You could also just do error messaging here but that's not as nice.)
                throw ex;
            }
            finally
            {
                adapter.Dispose();
                dbConnection.Close();
            }

            // Return the populated DataTable
            return employeeTable;
        }

        /// <summary>
        /// Get a specific employees name from there id
        /// </summary>
        /// <param name="id">Employee Id number you wish to find</param>
        /// <returns>The Name of the employee</returns>
        internal static string GetEmployeeName(string id)
        {
            // Declare the SQL connection and the SQL command
            SqlConnection dbConnection = new SqlConnection(GetConnectionString());
            SqlCommand command = new SqlCommand("SELECT CONCAT([FirstName], ' ', [LastName]) FROM [Employee] WHERE [EmployeeId] = " + id, dbConnection);

            // Try to open a connection to the database and read the total. Return result.
            try
            {
                dbConnection.Open();
                return command.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                // If there is an error, re-throw the exception to be handled by the presentation tier.
                // (You could also just do error messaging here but that's not as nice.)
                throw ex;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        #endregion

    }
}
