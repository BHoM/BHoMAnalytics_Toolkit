/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Linq;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Threading;
using System.IO;
using BH.Adapter.Mongo;
using BH.oM.BHoMAnalytics;
using System.Collections.Generic;
using System.Threading.Tasks;
using BH.oM.UI;

namespace BH.Adapter.BHoMAnalytics
{
    public partial class BHoMAnalyticsAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public BHoMAnalyticsAdapter()
        {

        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool InitialiseAnalytics()
        {
            try
            {
                BH.Engine.UI.Compute.m_UsageLogTriggered += UsageLogTriggered;
            }
            catch (Exception e)
            {
                BH.Engine.Base.Compute.RecordError($"Error occurred when initialising analytics trigger. Exception was: {e.ToString()}");
            }

            return SendUsageData();
        }

        /***************************************************/

        public static bool SendUsageData()
        {
            Task.Run(() =>
            {
                MongoAdapter mongo = GetTargetDatabase();
                if (mongo != null && mongo.IsConnected())
                {
                    List<UsageEntry> usageData = Engine.BHoMAnalytics.Compute.CollectUsageData(true);
                    string tag = System.DateTime.UtcNow.Ticks.ToString();
                    mongo.Push(usageData, tag);
                }
            });

            return true;
        }

        /***************************************************/

        /***************************************************/
        /**** Helper Methods                            ****/
        /***************************************************/

        private static MongoAdapter GetTargetDatabase()
        {
            string settingsFile = @"C:\ProgramData\BHoM\Settings\BHoMAnalytics.cfg";
            if (!File.Exists(settingsFile))
                return null;

            try
            {
                string json = File.ReadAllText(settingsFile);
                ToolkitSettings settings = Engine.Serialiser.Convert.FromJson(json) as ToolkitSettings;
                if (settings == null)
                    return null;

                return new MongoAdapter(settings.ServerAddress, settings.DatabaseName, settings.CollectionName);
            }
            catch
            {
                return null;
            }
        }

        /***************************************************/

        private static void UsageLogTriggered(object sender, EventArgs e)
        {
            var args = e as TriggerLogUsageArgs;
            if (args == null)
                return;

            List<string> ignoredSelectedItems = new List<string>()
            {
                "BH.oM.Base.Output`10[System.String,System.String,System.String,System.Collections.Generic.List`1[System.String],System.Collections.Generic.List`1[System.Type],System.Collections.Generic.List`1[System.Reflection.MethodInfo],System.Collections.Generic.List`1[System.Type],System.String,System.String,System.String] GetInfo()",
                "Boolean SetProjectID(System.String)",
            };

            if (args.SelectedItem == null || ignoredSelectedItems.Contains(args.SelectedItem.ToString()))
                return; //Don't handle any pop up when the SetProjectID component is the one being called - it means someone is setting the project ID already!

            var projectIDEvent = BH.Engine.Base.Query.AllEvents().OfType<ProjectIDEvent>().FirstOrDefault();
            if (projectIDEvent == null && !m_ProjectWindowDIsplayed)
            {
                Thread t = new Thread(() => ShowProjectCaptureWindow(args.UIName));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                
                m_ProjectWindowDIsplayed = true;
            }
        }

        /***************************************************/

        private static void ShowProjectCaptureWindow(string uiName)
        {
            BH.UI.Analytics.CaptureProjectData window = new UI.Analytics.CaptureProjectData(uiName);
        }

        private static bool m_ProjectWindowDIsplayed = false;
    }
}



