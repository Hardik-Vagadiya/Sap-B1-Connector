using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapB1Connector.Core
{
    public class SapCompany
    {
        public static void Connect()
        {
            try
            {
                SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
                string Cookie = oCompany.GetContextCookie();
                string conContext = ConnectionManager.oApp.Company.GetConnectionContext(Cookie);
                int ret = oCompany.SetSboLoginContext(conContext);

                if(ret != 0)
                {
                    throw new Exception("SetSboLoginContext Failed!");
                }

                ret = oCompany.Connect();

                if(ret != 0)
                {
                    oCompany.GetLastError(out int errCode, out string errMsg);
                    throw new Exception($"DI-API Connection Failed! {errMsg} - {errCode}");
                }

                ConnectionManager.oCom = oCompany;
                ConnectionManager.oApp.StatusBar.SetText("DI-API Connected!", SAPbouiCOM.BoMessageTime.bmt_Short,
                    SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "DI-API Connection Error!");
            }
        }
    }
}
