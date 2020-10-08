using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

//DB Package
using System.Data;
using System.Data.OleDb;


namespace IC_Project
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //Constants
            const string _BILLFILEPATH = "BillFile.xml";
            const string _DATEFORMAT = "MM/dd/yyyy";
            const string _CLIENT_GUID = "8203ACC7-2094-43CC-8F7A-B8F19AA9BDA2";
            const string _INVOICE_FORMAT = "8E2FEA69-5D77-4D0F-898E-DFA25677D19E";

            //Hashmap which contains kvp of the rpt file content.
            Dictionary<string, string> hash = new Dictionary<string, string>();

            static void connectDB()
            {
                string myConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                           "Data Source=Billing.mdb;";
                      
                try
                {
                    // Open OleDb Connection
                    OleDbConnection myConnection = new OleDbConnection
                    {
                        ConnectionString = myConnectionString
                    };
                    myConnection.Open();

                    // Execute Queries
                    OleDbCommand cmd = myConnection.CreateCommand();
                    cmd.CommandText = "SELECT * FROM `Bills`";
                    OleDbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection); // close conn after complete

                    // Load the result into a DataTable
                    DataTable myDataTable = new DataTable();
                    myDataTable.Load(reader);
                }
                catch (Exception ex)
                {
                    //Fails with this thrown exception message: FAILED: System.Data.OleDb is not supported on this platform.
                    Console.WriteLine("OLEDB Connection FAILED: " + ex.Message);
                }
            }




            //Date format
            string dateNow = DateTime.Now.ToString(_DATEFORMAT);

            //RPT File date
            string fileDate = DateTime.Now.ToString("MMddyyyy");


            //Check if file is reachable.
            if (File.Exists(_BILLFILEPATH))
            {
                int invoiceRecordCount = 0;
                double invoiceRecordTotalAmount = 0.0;

                DateTime dateNowPlusFive = DateTime.Now.AddDays(5);

                string firstNotification = dateNowPlusFive.ToString(_DATEFORMAT);
                //Init Output File
                using StreamWriter outputFile = new StreamWriter($"BillFile-{fileDate}.rpt");

                //Init hashmap
                hash.Add("1", "FR");
                hash.Add("2", _CLIENT_GUID);
                hash.Add("3", "Sample UT file");
                hash.Add("4", dateNow);
                hash.Add("5", invoiceRecordCount.ToString());
                hash.Add("6", invoiceRecordTotalAmount.ToString());

                hash.Add("AA", "CT");
                hash.Add("BB", "");
                hash.Add("VV", "");
                hash.Add("CC", "");
                hash.Add("DD", "");
                hash.Add("EE", "");
                hash.Add("FF", "");
                hash.Add("GG", "");
                hash.Add("HH", "IH");
                hash.Add("II", "R");
                hash.Add("JJ", _INVOICE_FORMAT);
                hash.Add("KK", "");
                hash.Add("LL", "");
                hash.Add("MM", "");
                hash.Add("NN", "");
                hash.Add("OO", ""); //OO 5+ Current Date.
                hash.Add("PP", ""); //Due date -3 Days.
                hash.Add("QQ", "");
                hash.Add("RR", dateNow);
                hash.Add("SS", "");


                //First Notification Date.
                hash["OO"] = firstNotification;

                XmlReader xmlOut = XmlReader.Create(_BILLFILEPATH);

                //Read until EOF.
                while (xmlOut.Read())
                {


                    switch (xmlOut.Name.ToString())
                    {
                        //BB
                        case "Account_No":
                            invoiceRecordCount++;
                            hash["BB"] = xmlOut.ReadString();
                            outputFile.WriteLine(String.Format("{0}~{1}|", "AA", hash["AA"]));
                            outputFile.WriteLine(String.Format("{0}~{1}|", "BB", hash["BB"]));
                            continue;
                        //VV
                        case "Customer_Name":

                            hash["VV"] = xmlOut.ReadString();
                            outputFile.WriteLine(String.Format("{0}~{1}|", "VV", hash["VV"]));
                            continue;
                        //CC
                        case "Mailing_Address_1":

                            hash["CC"] = xmlOut.ReadString();
                            outputFile.WriteLine(String.Format("{0}~{1}|", "CC", hash["CC"]));
                            continue;
                        //DD
                        case "Mailing_Address_2":

                            hash["DD"] = xmlOut.ReadString();
                            outputFile.WriteLine(String.Format("{0}~{1}|", "DD", hash["DD"]));
                            continue;
                        //EE
                        case "City":

                            hash["EE"] = xmlOut.ReadString();
                            outputFile.WriteLine(String.Format("{0}~{1}|", "EE", hash["EE"]));
                            continue;
                        //FF
                        case "State":

                            hash["FF"] = xmlOut.ReadString();
                            outputFile.WriteLine(String.Format("{0}~{1}|", "FF", hash["FF"]));
                            continue;
                        //GG
                        case "Zip":

                            hash["GG"] = xmlOut.ReadString();
                            outputFile.WriteLine(String.Format("{0}~{1}|", "GG", hash["GG"]));
                            outputFile.WriteLine(String.Format("{0}~{1}|", "HH", hash["HH"]));
                            outputFile.WriteLine(String.Format("{0}~{1}|", "II", hash["II"]));
                            outputFile.WriteLine(String.Format("{0}~{1}|", "JJ", hash["JJ"]));
                            continue;
                        //KK
                        case "Invoice_No":

                            hash["KK"] = xmlOut.ReadString();
                            outputFile.WriteLine(String.Format("{0}~{1}|", "KK", hash["KK"]));
                            continue;
                        //LL
                        case "Bill_Dt":
                            string billDtTmp = xmlOut.ReadString();
                            billDtTmp = Convert.ToDateTime(billDtTmp).ToString(_DATEFORMAT);
                            hash["LL"] = billDtTmp;
                            outputFile.WriteLine(String.Format("{0}~{1}|", "LL", hash["LL"]));
                            continue;
                        //MM
                        case "Due_Dt":
                            string dueDtTmp = xmlOut.ReadString();
                            dueDtTmp = Convert.ToDateTime(dueDtTmp).ToString(_DATEFORMAT);
                            hash["MM"] = dueDtTmp;

                            //PP
                            DateTime secondNotificationDateTime = Convert.ToDateTime(dueDtTmp).AddDays(-3);
                            string secondNotifcationString = secondNotificationDateTime.ToString(_DATEFORMAT);
                            hash["PP"] = secondNotifcationString;

                            outputFile.WriteLine(String.Format("{0}~{1}|", "MM", hash["MM"]));
                            outputFile.WriteLine(String.Format("{0}~{1}|", "PP", hash["PP"]));
                            continue;
                        //NN
                        case "Bill_Amount":

                            hash["NN"] = xmlOut.ReadString();
                            outputFile.WriteLine(String.Format("{0}~{1}|", "NN", hash["NN"]));
                            outputFile.WriteLine(String.Format("{0}~{1}|", "OO", hash["OO"]));
                            continue;
                        //QQ
                        case "Balance_Due":
                            string amountTmp = xmlOut.ReadString();
                            invoiceRecordTotalAmount += Convert.ToDouble(amountTmp);
                            hash["QQ"] = amountTmp;
                            outputFile.WriteLine(String.Format("{0}~{1}|", "QQ", hash["QQ"]));
                            outputFile.WriteLine(String.Format("{0}~{1}|", "RR", hash["RR"]));
                            continue;
                        //SS
                        case "Account_Class": 
                            hash["SS"] = xmlOut.ReadString();
                            outputFile.WriteLine(String.Format("{0}~{1}|", "SS", hash["SS"]));
                            continue;


                    }




                }

                hash["5"] = invoiceRecordCount.ToString();
                hash["6"] = invoiceRecordTotalAmount.ToString();


                //Print summary report 
                for (int i = 1; i <= 6; i++)
                {
                    outputFile.WriteLine(String.Format("{0}~{1}|", i.ToString(), hash[i.ToString()]));
                }

                //Close .rpt file.
                outputFile.Close();

                Console.WriteLine($"BillFile-{fileDate}.rpt was created successfully.");
            }
            else
            {
                Console.WriteLine("BillFile.xml was not found in project dir!");
            }

            //Run routine to DB after file is rpt is populated.
            if (File.Exists($"BillFile-{fileDate}.rpt"))
            {
                //Populate DB
                connectDB();


            } else
            {
                Console.WriteLine("RPT file not found in project dir!");
            }

        }

        
    }
}
