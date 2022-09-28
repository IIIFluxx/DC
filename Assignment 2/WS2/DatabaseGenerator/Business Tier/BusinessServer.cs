using DBInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Business_Tier
{
        //Then, you’ll want to create an implementation called BusinessServer
        //that for each function calls ONE ChannelFactory<DataServerInterface>.
        // <Make ONE ChannelFactory for all functions> and calls the required functions on the Data tier.

        // ==================================================
        // ==================================================

        [System.ServiceModel.ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
        //internal class DataServer : DataServerInterface
        internal class BusinessServer : BusinessServerInterface
        {
            //private DatabaseClass db;
            private ChannelFactory<DataServerInterface> foobFactory;
            private DataServerInterface foob;
            private uint LogNumber;

            public BusinessServer()
            {
                NetTcpBinding tcp = new NetTcpBinding();
                string URL = "net.tcp://localhost:8100/DataService";
                foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
                foob = foobFactory.CreateChannel();
                // Use foob to create channel and access data tier
                Log("Business-Tier Server has been created.");
        }

        public int GetNumEntries()
            {
                Log("Method GetNumEntries() has been called from elsewhere");
                return foob.GetNumEntries(); // again calls the method from DatabaseClass; not calling the source code method itself.
            }
        
            public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap icon)
            {
                Log($"Method GetValuesForEntry() has been called for the provided index: {index}");
                foob.GetValuesForEntry(index, out acctNo, out pin, out bal, out fName, out lName, out icon);
            }


        //The business tier needs to implement a function that will take in a string and return the contents of the
        // first record that has a last name matching the string.

        public int searchLastName(string lastName)
        {
            int recordIdx = -1;
            uint inAcctNo, inPIN;   
            int inBal;
            string inFName, inLName;
            Bitmap inIcon;

            Log($"Search query initiated for a user with last name: {lastName}");
            for (int ii = 0; ii < foob.GetNumEntries(); ii++)
            {
                foob.GetValuesForEntry(ii, out inAcctNo, out inPIN, out inBal, out inFName, out inLName, out inIcon);
                if(lastName.Contains(inLName))
                {
                    recordIdx = ii;
                    Log("Found entry " + inLName + " at index: " + ii);
                    break;
                }
            }
            return recordIdx;
        }

        // Keeps track of how many log-able tasks have been performed.
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Log(string logString)
        {
            LogNumber++;
            Console.WriteLine(LogNumber.ToString() + ". " + logString + "\n");
        }


    }
}

