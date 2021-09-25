// PieceworkWorker.cs
//         Title: IncInc Payroll (Piecework)
// Last Modified: September 21, 2021
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

        private bool isValid = true;

        // Shared class variables
        private static int overallNumberOfEmployees;
        private static decimal overallPayroll;

        // Class Constants
        private readonly int[] MinimumMessages = { 1, 1250, 2500, 3750, 5000 };
        private readonly double[] PayRates = { 0.02, 0.024, 0.028, 0.034, 0.04};
        private readonly int MinimumNameLength = 2;

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

            // Validate Validate and set the worker's number of messages
            Messages = messagesValue;
            // Calculcate the worker's pay and update all summary values if entry is valid
            if (isValid)
            {
                FindPay();
            }
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
            // TO DO
            // Fill in this entire method by following the instructions provided
            // in the NETD 3202 Lab 1 handout
            // It is suggested that you use the requirements as a checklist in
            // order to ensure you don't miss any requirements.
            /*for (int index = 0; index < MinimumMessages.Length; index++)
            {

            }*/

            // Check each range set for payroll setting according if in range
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
                        // Else set to invalid and display error message
                        isValid = false;
                        MessageBox.Show("The name entered must have at least " + MinimumNameLength + " alphabetic characters.", "Entry Error");
                    }            
                }
                else
                {
                    // Else set to invalid and display error message
                    isValid = false;
                    MessageBox.Show("The name entered must be at least " + MinimumNameLength + " characters long.", "Entry Error");
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
                // TO DO
                // Add validation for the number of messages based on the
                // requirements document
                // Try to parse the value given as an int
                if (int.TryParse(value, out employeeMessages))
                {
                    // Check if the value is less than the minimum, setting to invalid and display message if less.
                    if (employeeMessages < MinimumMessages.First())
                    {
                        isValid = false;
                        MessageBox.Show("The messages sent must be at least" + MinimumMessages[0] + ".", "Entry Error");
                    }
                }
                else
                {
                    // Else set to invalid and display error message.
                    isValid = false;
                    MessageBox.Show("Please enter the number of messages sent as a number.", "Entry Error");
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
                return TotalPay / TotalWorkers;
            }
        }

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