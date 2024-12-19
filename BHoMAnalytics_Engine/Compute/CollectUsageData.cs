/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.BHoMAnalytics;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base.Attributes;

namespace BH.Engine.BHoMAnalytics
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Input("deleteProcessedFiles", "Whether the method should delete processed log files or not.")]
        [Output("entries", "The list of processed usage entries.")]
        public static List<UsageEntry> CollectUsageData(bool deleteProcessedFiles = false)
        {
            string logFolder = @"C:\ProgramData\BHoM\Logs";
            if (!Directory.Exists(logFolder))
            {
                Engine.Base.Compute.RecordWarning("The folder C:\\ProgramData\\BHoM\\Logs doesn't exist so nothing was collected");
                return new List<UsageEntry>();
            }
            
            string[] usageFiles = Directory.GetFiles(logFolder, "Usage_*.log");

            //TODO: this is a crowbar solution to disable pushing Revit logs to the database when not in Revit thread to avoid serialisation issues - to be improved
            if (AppDomain.CurrentDomain.GetAssemblies().All(x => x.GetName().Name != "RevitAPI"))
                usageFiles = usageFiles.Where(x => !x.Contains("Revit")).ToArray();

            List<UsageEntry> usageEntries = new List<UsageEntry>();
            foreach (string file in usageFiles)
            {
                // Get the file content
                string[] json = new string[0];
                try
                {
                    json = File.ReadAllLines(file);
                }
                catch
                {
                    // The file might be currently writen to
                    continue;
                } 

                // Extract the usage entries and summarise them
                try
                {
                    List<UsageLogEntry> logEntries = json.Select(x => Engine.Serialiser.Convert.FromJson(x)).OfType<UsageLogEntry>().ToList();
                    List<UsageEntry> summary = logEntries.SummariseUsageData();
                    usageEntries.AddRange(summary);
                }
                catch (Exception e)
                {
                    Engine.Base.Compute.RecordError("Failed to process usage log file " + file + ". Error: \n" + e.Message);
                }

                // Delete the file if requested
                if (deleteProcessedFiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        Engine.Base.Compute.RecordError("Failed to delete usage log file " + file + ".");
                    }
                } 
            }

            return usageEntries;
        }

        /***************************************************/
    }
}





