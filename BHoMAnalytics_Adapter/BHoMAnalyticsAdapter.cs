/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
    }
}





