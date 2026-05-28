using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapB1Connector.Core
{
    public class SapApplication
    {
        public static void Connect(string[] args)
        {
            try
            {
                if(args.Length == 0)
                {
                    throw new Exception("No SAP connection string found!");
                }
                string connStr = args[0];
                SAPbouiCOM.SboGuiApi sboGuiApi = new SAPbouiCOM.SboGuiApi();
                sboGuiApi.Connect(connStr);
                ConnectionManager.oApp = sboGuiApi.GetApplication();
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
    }
}
