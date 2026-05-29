using System;
using System.Windows.Forms;

namespace SapB1Connector.Core
{
    public class SapEventHandler
    {
        public static void Register()
        {
            ConnectionManager.oApp.AppEvent += OnAppEvent;
        }

        private static void OnAppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            if (EventType != SAPbouiCOM.BoAppEventTypes.aet_ShutDown) return;

            try
            {
                if (ConnectionManager.oCom != null)
                {
                    ConnectionManager.oCom.Disconnect();
                    ConnectionManager.oCom = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[ShutDown] DI-API disconnect error: {ex.Message}");
            }
            finally
            {
                ConnectionManager.oApp = null;
                Application.Exit();
            }
        }
    }
}