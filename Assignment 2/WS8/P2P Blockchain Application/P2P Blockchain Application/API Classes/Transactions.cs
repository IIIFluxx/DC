using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class Transactions
    {
        private static Queue<Transaction> transactions = new Queue<Transaction>();

        public static Queue<Transaction> getTransactions()
        {
            return transactions;
        }


        public static void addTransaction(Transaction t)
        {
            /*if (notRunning)
            {
                ProcTrans procDel = ProcessTransactions;
                procDel.BeginInvoke(null, null);
                notRunning = false;
            }*/

            transactions.Enqueue(t);
        }

        public static void setProcessed(Transaction inTransaction)
        {
            foreach (Transaction t in transactions)
            {
                if(inTransaction == t)
                {
                    t.processed = true;
                }
                else
                {
                    Debug.WriteLine("Transaction not located in transactions queue. Please try again with a valid tranasction.");
                }
            }
        }

    }
}
