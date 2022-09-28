using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseGenerator
{
    // Class 1: Database Generator
    internal class DbGen
    {

        private Random rng = new Random();

        //private List<Bitmap> icons;

        /*
         * Remember, they need to generate their outputs randomly!
            private string GetFirstname()
            private string GetLastname()
            private uint GetPIN()
            private uint GetAcctNo()
            private int GetBalance()
         */

        private string genName()
        {
            string name = "";

            String[] consonant =
                { "q","w","r","t","y", "p","s","d","f"
                    ,"g","h","j","k","l","z","x",
                    "c","v","b","n","m"};

            String[] vowel = { "a", "e", "i", "o", "u" };


            for (int i = 0; i < 3; i++)
            {
                int num1 = rng.Next(0, 20);
                int num2 = rng.Next(0, 4);
                name = String.Concat(name, consonant[num1], consonant[num2]);
                // Returns a name -- either first or last, not both.
            }
            return name;
        }


        //private Bitmap getIcon() => icons[rng.Next(icons.Count)];
        /*private Bitmap getIcon()
        {
            return icons[rng.Next(icons.Count)];
        }*/

        /*public DbGen()
        {
            /*icons = new List<Bitmap>();
            // Generate a few really basic icons
            // Probably not the best way to do it, but it works :)
            for (var i = 0; i < 10; i++)
            {
                var image = new Bitmap(64, 64);
                for (var x = 0; x < 64; x++)
                {
                    for (var y = 0; y < 64; y++)
                    {
                        image.SetPixel(x, y, Color.FromArgb(rng.Next(256), rng.Next(256), rng.Next(256)));
                    }
                }
                icons.Add(image);
            }
        }*/

        //private Bitmap genIcon() => icons[rng.Next(icons.Count)];


        // You’ll also need a public function that lets someone request a “record”. This should ideally use the private functions.
        //public void genNext(out uint pin, out uint acctNo, out string firstName, out string lastName, out int balance, out Bitmap icon)
        public void genNext(out uint pin, out uint acctNo, out string firstName, out string lastName, out int balance)
        {
            pin = (uint)rng.Next(0, 9999);
            acctNo = (uint)rng.Next(10000, 99999);
            firstName = genName();
            lastName = genName();
            balance = rng.Next(-999999, 999999);
            //icon = getIcon();
        }
        /*
         *  You’ll notice that this returns void, but has a whole bunch of “out” variables. These are “out mode”
            parameters, and are basically C#’s way of doing multiple returns. It’s kind of like passing pointers
            through in C, except you can’t pass things in to the function using out, values only go one way. If
            you really want to pass things through as if they were a pointer reference, you can do that with the
            similar “ref” keyword. We’ll look at this in a later week.
         */
    }

    // Class 2: Database storage class
    // This will store individual records in the “database”.
    // It will need a field for each variable that the generator can create

    internal class DataStruct
    {
        public uint acctNo;
        public uint pin;
        public int balance;
        public string firstName;
        public string lastName;
        //public Bitmap icon;

        public DataStruct()
        {
            acctNo = 0;
            pin = 0;
            balance = 0;
            firstName = "Roger";
            lastName = "Woger";
            //icon = null;
        }

        //public DataStruct(uint inA, uint inP, int inBal, string inFN, string inLN, Bitmap inI)
        public DataStruct(uint inA, uint inP, int inBal, string inFN, string inLN)
        {
            acctNo = inA;
            pin = inP;            
            balance = inBal;
            firstName = inFN;
            lastName = inLN;
            //icon = inI;
        }
    }

    // Class 3: Database Class
    // we’ll be building a publicly accessible object that will define the “database”.

    public class DatabaseClass
    {
        List<DataStruct> dataStruct; // From Worksheet
        private DbGen newDB = new DbGen();
        private int MAX_SIZE = 100000;
        //private int MAX_SIZE = 100;
        

        public DatabaseClass()
        {
            dataStruct = new List<DataStruct>(); // From Worksheet.

            uint acctNo, pin;
            string fName, lName;
            int balance;
            //Bitmap icon;


            // The constructor will also need to load the List up with a large number of entries
            
            // Fill up list with many entries
            for (int ii=0; ii< MAX_SIZE; ii++)
            {
                DataStruct ds = new DataStruct();
                //newDB.genNext(out ds.acctNo, out ds.pin, out ds.firstName, out ds.lastName, out ds.balance, out ds.icon);
                newDB.genNext(out ds.acctNo, out ds.pin, out ds.firstName, out ds.lastName, out ds.balance);
                dataStruct.Add(ds); 
            }
        }

        /*
         * In addition, you need to implement the following functions:
         *  public uint GetAcctNoByIndex(int index)
            public uint GetPINByIndex(int index)
            public string GetFirstNameByIndex(int index)
            public string GetLastNameByIndex(int index)
            public int GetBalanceByIndex(int index)
            public int GetNumRecords()
        */

        public uint GetAcctNo(int index)
        {
            return dataStruct.ElementAt(index).acctNo;
        }

        public uint GetPin(int index)
        {
            return dataStruct.ElementAt(index).pin;
        }


        public int GetBalance(int index)
        {
            return dataStruct.ElementAt(index).balance;
        }

        public string GetFName(int index)
        {
            return dataStruct.ElementAt(index).firstName;
        }

        public string GetLName(int index)
        {
            return dataStruct.ElementAt(index).lastName;
        }
        
        /*public Bitmap GetIconByIndex(int index)
        {
            return dataStruct[index].icon;
        }*/

        public int GetNumRecords()
        {
            return dataStruct.Count;
        }

    }

}