using DBInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Business_Tier
{

    //Make this a service contract as it is a service interface
    [ServiceContract]
    public interface BusinessServerInterface
    {
        //Each of these are service function contracts. They need to be tagged as OperationContracts.
        [OperationContract]
        int GetNumEntries();

        // === Fault Contract addition below:
        [OperationContract]
        [FaultContract(typeof(IndexOutOfRangeFault))]
        //And so on for each type of Exception --- [FaultContract(typeof(ExceptionTypeHere))]
        void GetValuesForEntry(int index, out uint acctNo, out uint pin, out int bal, out string fName,
            out string lName, out Bitmap icon);

        [OperationContract]
        int searchLastName(string lastName);
        //        int searchLastName(string lastName,out uint acctNo, out uint pin, out int bal, out string fName, out string lName, out Bitmap icon);
    }

}
