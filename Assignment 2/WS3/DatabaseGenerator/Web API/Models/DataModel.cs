using DBInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace Web_API.Models
{
    
    /*
     *  You will want to set this up so that it connects to the Data Tier via .NET remoting
        and allows for the usual services (so, GetNumEntries and such).*/
    public class DataModel
    {
        private DataServerInterface foob;
        // Open Conn:
        public DataModel()
        {
            ChannelFactory<DataServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8100/DataService";

            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel(); // remote connection
        }


        // =============

        public int GetNumEntries()
        {
            return foob.GetNumEntries(); // again calls the method from DatabaseClass; not calling the source code method itself.
        }
        //public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap icon)
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName)
        {
            System.Diagnostics.Debug.WriteLine("Searching for Index: " + index + ".");
            foob.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName);
        }


        //The business tier needs to implement a function that will take in a string and return the contents of the
        // first record that has a last name matching the string.

        public void searchLastName(string lastName, out string fName, out uint acctNo, out uint pin, out int balance)
        {
            int recordIdx = 0;
            uint inAcctNo, inPIN;
            int inBal;
            string inFName, inLName;
            //Bitmap inIcon;

            // Default vals:
            acctNo = 0;
            pin = 0;
            fName = "Account Not found!";
            balance = 0;


            for (int ii = 0; ii < foob.GetNumEntries(); ii++)
            {
                //foob.GetValuesForEntry(ii, out inAcctNo, out inPIN, out inBal, out inFName, out inLName, out inIcon);
                foob.GetValuesForEntry(ii, out inAcctNo, out inPIN, out inBal, out inFName, out inLName);
                if (lastName.Contains(inLName))
                {
                    System.Diagnostics.Debug.WriteLine("Index found @: " + ii + ".");
                    acctNo = inAcctNo;
                    pin = inPIN;
                    fName = inFName;
                    balance = inBal;
                    break;
                }
            }
            //return recordIdx;
        }

    }
    // ============
}