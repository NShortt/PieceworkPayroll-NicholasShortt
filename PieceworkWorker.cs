// PieceworkWorker.cs
//         Title: IncInc Payroll (Piecework)
// Last Modified: October 19, 2021
//    Written By: Nicholas Shortt
// Adapted from PieceworkWorker by Kyle Chapman, September 2019
// 
// This is a class representing individual worker objects. Each stores
// their own name and number of messages and the class methods allow for
// calculation of the worker's pay and for updating of shared summary
// values. Name and messages are received as strings.
// This is being used as part of a piecework payroll application.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows;

namespace PayrollDemo // Ensure this namespace matches your own
{
    class PieceworkWorker
    {

        #region "Variable declarations"

        // Instance variables
        private string employeeFirstName;
        private string employeeLastName;
        private int employeeMessages;
        private decimal employeeRate;
        private decimal employeePay;
        private int employeeID;
        private DateTime employeeCreatedTime;
        
        // Class Constants
        private readonly int[] MinimumMessages = { 1, 1250, 2500, 3750, 5000 };
        private readonly double[] PayRates = { 0.02, 0.024, 0.028, 0.034, 0.04};
        private readonly int MinimumNameLength = 2;
        private readonly int MaxMessages = 15000;

        public const string NameParameter = "name";
        public const string MessagesParameter = "messages";
        public const string IDParameter = "id";

        #endregion

        #region "Constructors"

        /// <summary>
        /// PieceworkWorker constructor: accepts a worker's name and number of
        /// messages, sets and calculates values as appropriate.
        /// </summary>
        /// <param name="nameValue">the worker's name</param>
        /// <param name="messageValue">a worker's number of messages sent</param>
        public PieceworkWorker(string[] nameValue, string messagesValue, string idValue)
        {
            // Validate and set worker's id
            ID = idValue;

            // Validate and set the worker's name
            SetFirstAndLastName(nameValue);

            // Validate and set the worker's number of messages
            Messages = messagesValue;          

            // Calculate  the worker's pay and update all summary values
            FindPay();

            // Set Created time to now
            employeeCreatedTime = DateTime.Now;

            // Once valid worker is created add it to the database
            DataAccess.InsertNewRecord(this);
        }

        /// <summary>
        /// PieceworkWorker constructor: empty constructor used strictly for inheritance and instantiation
        /// </summary>
        public PieceworkWorker()
        {
        }

        #endregion

        #region "Class methods"

        /// <summary>
        /// Currently called in the constructor, the findPay() method is
        /// used to calculate a worker's pay using threshold values to
        /// change how much a worker is paid per message. This also updates
        /// all summary values.
        /// </summary>
        private void FindPay()
        {
            // Compare each range set for payrate to messages sent, setting rate according to the set in range of.
            for (int index = 0; index < MinimumMessages.Length - 1; index++)
            {
                if (employeeMessages >= MinimumMessages[index] && employeeMessages < MinimumMessages[index + 1])
                {
                    employeeRate = (decimal)PayRates[index];
                }
            }

            // If rate is still not set then set to highest pay rate.
            if (employeeRate == 0)
            {
                employeeRate = (decimal)PayRates.Last();
            }

            // Calculate the pay to nearest cent
            employeePay = Math.Round((employeeMessages * employeeRate), 2);
        }

        /// <summary>
        /// Validates that there is entry and that only 2 names were given.
        /// Then sets the first and last name
        /// </summary>
        private void SetFirstAndLastName(string[] names)
        {
            if (names.First() == "")
            {
                throw new ArgumentNullException(NameParameter, "Name must be entered.");
            }
            else if (names.Length != 2)
            {
                throw new ArgumentException("Enter only first and last name.", NameParameter);
            }
            FirstName = names[0];
            LastName = names[1];
        }

        /// <summary>
        /// Validates name given to be having atleast 2 alphabetic characters
        /// </summary>
        /// <returns>Returns the name if valid</returns>
        private string ValidateName(string name)
        {
            // Check if empty
            if (name == String.Empty)
            {
                throw new ArgumentNullException(NameParameter, "Names cannot be blank");
            }
            // Check if value meets minimum requirement for length
            else if (name.Length >= MinimumNameLength)
            {
                // Go through each character and see if it is alphabetic
                int letterCount = 0;
                foreach (char val in name)
                {
                    if ((val >= 'a' && val <= 'z') || (val >= 'A' && val <= 'Z'))
                    {
                        letterCount++;
                    }
                }
                // If the number of alphabetic is equal to or greater than the minimum, set the value
                if (letterCount < MinimumNameLength)
                {
                    // Else throw argument exception
                    throw new ArgumentException("Names must have at least " + MinimumNameLength + " letters", NameParameter);
                }
            }
            else
            {
                // Else throw an argument exception
                throw new ArgumentOutOfRangeException(NameParameter, "Names must have at least " + MinimumNameLength + " characters");
            }

            return name;
        }

        /// <summary>
        /// Get the entries of a given worker id
        /// </summary>
        /// <returns>returns table it found</returns>
        internal static DataTable GetWorkerEntries(int id)
        {
            return DataAccess.GetEmployeeEntries(id);
        }



        #endregion

        #region "Property Procedures"

        /// <summary>
        /// Gets and Sets the workers first name
        /// </summary>
        public string FirstName
        {
            get
            {
                return employeeFirstName;
            }
            set
            {
                // Set the name after validating the value given
                employeeFirstName = ValidateName(value.Trim()); 
            }
        }

        /// <summary>
        /// Gets and Sets the workers last name
        /// </summary>
        public string LastName
        {
            get
            {
                return employeeLastName;
            }
            set
            {
                // Set the name after validating the value given
                employeeLastName = ValidateName(value.Trim());
            }
        }
        
        public string Name { get { return FirstName + " " + LastName; } }

        /// <summary>
        /// Gets and sets the number of messages sent by a worker
        /// </summary>
        public string Messages
        {
            get
            {
                return employeeMessages.ToString();
            }
            set
            {
                // Check if empty
                if (value.Trim() == String.Empty)
                {
                    throw new ArgumentNullException(MessagesParameter, "Messages sent cannot be blank");
                }
                // Try to parse the value given as an int
                else if (int.TryParse(value, out employeeMessages))
                {
                    // Check if the value is out of range , throwing an out of range exception if it is
                    if (employeeMessages < MinimumMessages.First() || employeeMessages > MaxMessages)
                    {
                        throw new ArgumentOutOfRangeException(MessagesParameter, "Messages sent must be between " + MinimumMessages.First() + 
                                                                " and " + MaxMessages);
                    }
                }
                else
                {
                    // Else throw an argument exception
                    throw new ArgumentException("Messages sent must be a numeric interger", MessagesParameter);
                }
            }
        }

        /// <summary>
        /// Get's and Set's the employee's ID number
        /// </summary>
        public string ID
        {
            get
            {
                return employeeID.ToString();
            }
            set
            {
                // Check if empty
                if (value.Trim() == String.Empty)
                {
                    throw new ArgumentNullException(IDParameter, "Employee ID cannot be blank");
                }
                // Try to parse the value given as an int
                else if (int.TryParse(value, out employeeID))
                {
                    // Check if the value is out of range , throwing an out of range exception if it is
                    if (employeeID < 0)
                    {
                        throw new ArgumentOutOfRangeException(IDParameter, "Employee ID must be be greater than 0");
                    }
                }
                else
                {
                    // Else throw an argument exception
                    throw new ArgumentException("Messages sent must be a numeric interger", IDParameter);
                }
            }
        }

        /// <summary>
        /// Get's the time the worker was created
        /// </summary>
        public DateTime TimeCreated
        {
            get
            {
                return employeeCreatedTime;
            }
        }

        /// <summary>
        /// Gets the worker's pay
        /// </summary>
        /// <returns>a worker's pay</returns>
        internal decimal Pay { get { return employeePay; } }

        /// <summary>
        /// Gets the overall total pay among all workers
        /// </summary>
        /// <returns>the overall total pay among all workers</returns>
        internal static decimal TotalPay { get { return Convert.ToDecimal(DataAccess.GetTotalPay()); } }

        /// <summary>
        /// Gets the overall number of workers
        /// </summary>
        /// <returns>the overall number of workers</returns>
        internal static int TotalWorkers { get { return Convert.ToInt32(DataAccess.GetTotalEmployees()); } }

        /// <summary>
        /// Calculates and returns an average pay among all workers
        /// </summary>
        /// <returns>the average pay among all workers</returns>
        internal static decimal AveragePay
        {
            get
            {
                // No workers means there should be no average
                if (TotalWorkers == 0)
                {
                    return 0;
                }

                return TotalPay / TotalWorkers;
            }
        }

        /// <summary>
        /// Gets the overall messages sent from all workers
        /// </summary>
        internal static int TotalMessages { get { return Convert.ToInt32(DataAccess.GetTotalMessages()); } }

        /// <summary>
        /// Returns a list of all workers as a DataTable
        /// </summary>
        internal static DataTable AllWorkers { get { return DataAccess.GetEmployeeList(); } }

        

        #endregion

    }
}
/*
 *  Question? 
 *  Why are some property procedures in this class read/write while others are read-only?
 *  
 *  Answer.
 *  The reason we have this is their are some properties we don't want the user to be 
 *  able to change values of.  These values are usually based on data calulated or handled by 
 *  the class and it's methods.  For example in this class we have the "Pay" property is based 
 *  on user entered data for messages sent and a rate determined by that data.  At no point do
 *  we want the user manually change that value.  But we are fine with the values we want them to give
 *  like messages sent and their name.
 */