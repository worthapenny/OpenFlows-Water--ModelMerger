/**
 * @ Author: Akshaya Niraula
 * @ Create Time: 2021-10-22 19:19:33
 * @ Modified by: Akshaya Niraula
 * @ Modified time: 2021-10-26 17:33:18
 * @ Copyright: Copyright (c) 2021 Akshaya Niraula See LICENSE for details
 */


using OFW.ModelMerger.Forms;
using OpenFlows.Application;
using OpenFlows.Water;
using OpenFlows.Water.Application;
using Serilog;
using Serilog.Formatting.Display;
using Serilog.Sinks.WinForms;
using System;

namespace OFW.ModelMerger
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static int Main()
        {
            ApplicationManagerBase.SetApplicationManager(new WaterApplicationManager());
            WaterApplicationManager.GetInstance().SetParentFormSurrogateDelegate(
                new ParentFormSurrogateDelegate((fm) =>
                {
                    return new ModelMergerForm(fm);
                }));

            OpenFlowsWater.StartSession(WaterProductLicenseType.WaterGEMS);


            // Set up the logging mechanism            
            Log.Logger = new LoggerConfiguration()
                .WriteToSimpleAndRichTextBox(new MessageTemplateTextFormatter("{Timestamp:MM-dd HH:mm:ss.fff} [{Level}] {Message} {NewLine}{Exception}"))
                .CreateLogger();
            Log.Information("Logger initialized");

            WaterApplicationManager.GetInstance().Start();
            WaterApplicationManager.GetInstance().Stop();

            OpenFlowsWater.EndSession();
            return 0;
        }
    }
}
