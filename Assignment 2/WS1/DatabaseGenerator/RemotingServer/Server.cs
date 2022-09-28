using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using DatabaseGenerator;
using DBInterface;
using System.Drawing;
using System.Diagnostics;

/*
 *  The server will need to encapsulate the Database Class that
    we made in the DLL. To do this, first add a reference to the
    library project you made earlier (to start, right click on the
    References submenu in the Solution Explorer for your
    console app). Once that is done, add a reference to it in code
    like the following:

 */


namespace DatabaseGenerator
{
    class ServerRun
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to my Server.");
            //This is the actual host service system
            ServiceHost host;
            //This represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();
            //Bind server to the implementation of DataServer
            host = new ServiceHost(typeof(DataServer));
            //Present the publicly accessible interface to the client. 0.0.0.0 tells .net to accept on any interface.
            //:8100 means this will use port 8100. DataService is a name for the actual service, this can be any string.

            host.AddServiceEndpoint(typeof(DataServerInterface), tcp,
            "net.tcp://0.0.0.0:8100/DataService");
            //And open the host for business!
            host.Open();
            Console.WriteLine("System Online");
            Console.ReadLine();
            //Don't forget to close the host after you're done!
            host.Close();
        }
    }
    
    // To actually do anything, we need an implementation of the interface.
    // We don’t actually want clients to use the implementation(as we want it to use the remote interface)
    // so this will be classified as an internal class

    //Time to describe our service behavior

    /** AKA -- These are what the clients get to see. We call methods from our DatabaseGenerator class (i.e. DatabaseClass object)
     and give the same functionalities to the user, except they don't get to directly use the source code, they get to use this -- remote interface to that. 
     */ 

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class DataServer : DataServerInterface
    {
        private DatabaseClass db;

        public DataServer()
        {
            db = new DatabaseClass(); // Make a new database (from DatabaseGenerator project).
        }
        public int GetNumEntries()
        {
            return db.GetNumRecords(); // again calls the method from DatabaseClass; not calling the source code method itself.
        }
                                       // ^ 
        public void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap icon)
        {
            try
            {
                // Check if index is out-of-range, if it is return an error
                if (index < 0 || index >= db.GetNumRecords())
                {
                    Console.WriteLine("Client tried to get a record that was out of range...");
                    throw new FaultException<IndexOutOfRangeFault>(new IndexOutOfRangeFault()
                    { Issue = "Index was not in range..." });
                }

                pin = db.GetPin(index);
                acctNo = db.GetAcctNo(index);
                fName = db.GetFName(index);
                lName = db.GetLName(index);
                bal = db.GetBalance(index);
                icon = new Bitmap(db.GetIconByIndex(index));
            }
            catch(FaultException)
            {
                Debug.WriteLine("Please enter in range index value.");
                pin = db.GetPin(0);
                acctNo = db.GetAcctNo(0);
                fName = db.GetFName(0);
                lName = db.GetLName(0);
                bal = db.GetBalance(0);
                icon = new Bitmap(db.GetIconByIndex(0));
            }
        }
    }
}
