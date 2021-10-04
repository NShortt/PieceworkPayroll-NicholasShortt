// PieceworkWorker.cs
//         Title: IncInc Payroll (Piecework)
// Last Modified: October 04, 2021
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
using System.Threading.Tasks;
using System.Windows;

namespace PayrollDemo // Ensure this namespace matches your own
{
    class PieceworkWorker
    {

        #region "Variable declarations"

        // Instance variables
        private string employeeName;
        private int employeeMessages;
        private decimal employeeRate;
        private decimal employeePay;
        
        // Shared class variables
        private static int overallNumberOfEmployees = 0;
        private static decimal overallPayroll = 0;
        private static int overallMessages = 0;

        // Class Constants
        private readonly int[] MinimumMessages = { 1, 1250, 2500, 3750, 5000 };
        private readonly double[] PayRates = { 0.02, 0.024, 0.028, 0.034, 0.04};
        private readonly int MinimumNameLength = 2;
        private readonly int MaxMessages = 15000;

        #endregion

        #region "Constructors"

        /// <summary>
        /// PieceworkWorker constructor: accepts a worker's name and number of
        /// messages, sets and calculates values as appropriate.
        /// </summary>
        /// <param name="nameValue">the worker's name</param>
        /// <param name="messageValue">a worker's number of messages sent</param>
        public PieceworkWorker(string nameValue, string messagesValue)
        {
            // Validate and set the worker's name
            Name = nameValue;

            // Validate and set the worker's number of messages
            Messages = messagesValue;

            // Calculate  the worker's pay and update all summary values
            FindPay();

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
            // Increment number of employees
            overallNumberOfEmployees++;
            // Add pay to running total
            overallPayroll += employeePay;
            // Add messages to running total
            overallMessages += employeeMessages;
        }

        #endregion

        #region "Property Procedures"

        /// <summary>
        /// Gets and sets a worker's name
        /// </summary>
        /// <returns>an employee's name</returns>
        public string Name
        {
            get
            {
                return employeeName;
            }
            set
            {
                // Check if value meets minimum requirement for length
                if (value.Length >= MinimumNameLength)
                {
                    // Go through each character and see if it is alphabetic
                    int numOfLetters = 0;
                    foreach (char val in value)
                    {
                        if ((val >= 'a' && val <= 'z') || (val >= 'A' && val <= 'Z'))
                        {
                            numOfLetters++;
                        }
                    }
                    // If the number of alphabetic is equal to or greater than the minimum, set the value
                    if (numOfLetters >= MinimumNameLength)
                    {
                        employeeName = value;
                    }
                    else
                    {
                        // Else throw argument exception
                        throw new ArgumentException("The name entered must have at least " + MinimumNameLength + " alphabetic characters.", "name");
                    }            
                }
                else
                {
                    // Else throw an argument exception
                    throw new ArgumentException("The name entered must be at least " + MinimumNameLength + " characters long.", "name");
                }
            }
        }

        /// <summary>
        /// Gets and sets the number of messages sent by a worker
        /// </summary>
        /// <returns>an employee's number of messages</returns>
        public string Messages
        {
            get
            {
                return employeeMessages.ToString();
            }
            set
            {
                // Try to parse the value given as an int
                if (int.TryParse(value, out employeeMessages))
                {
                    // Check if the value is out of range , throwing an out of range exception if it is
                    if (employeeMessages < MinimumMessages.First() || employeeMessages > MaxMessages)
                    {
                        throw new ArgumentOutOfRangeException("messages", "The messages sent must be at least" + MinimumMessages[0] +
                                                                " and at most " + MaxMessages + ".");
                    }
                }
                else
                {
                    // Else throw an argument exception
                    throw new ArgumentOutOfRangeException("messages", "Please enter the number of messages sent as a number.");
                }
            }
        }

        /// <summary>
        /// Gets the worker's pay
        /// </summary>
        /// <returns>a worker's pay</returns>
        internal decimal Pay
        {
            get
            {
                return employeePay;
            }
        }

        /// <summary>
        /// Gets the overall total pay among all workers
        /// </summary>
        /// <returns>the overall total pay among all workers</returns>
        internal static decimal TotalPay { get { return overallPayroll; } }

        /// <summary>
        /// Gets the overall number of workers
        /// </summary>
        /// <returns>the overall number of workers</returns>
        internal static int TotalWorkers { get { return overallNumberOfEmployees; } }

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
        internal static int TotalMessages { get { return overallMessages; } }

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