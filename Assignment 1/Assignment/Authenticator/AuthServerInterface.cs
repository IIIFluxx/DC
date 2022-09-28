using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Authenticator
{
    [ServiceContract]
    public interface AuthServerInterface
    {
        // Each of these are service function contracts. They need to be tagged as OperationContracts.
        //[OperationContract]
        // Method
        [OperationContract]
        string Register(string name, string password);

        [OperationContract]
        int Login(string name, string password); // SPECIFY IF OUT OR NOT.

        [OperationContract]
        string Validate(int token);

    }
}
