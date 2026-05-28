using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapB1Connector.Core
{
    public class Startup
    {
        public static void Initialize(string[] args)
        {
            SapApplication.Connect(args);
            //Null Guard
            if (ConnectionManager.oApp == null) return;
            SapCompany.Connect();
        }
    }
}
