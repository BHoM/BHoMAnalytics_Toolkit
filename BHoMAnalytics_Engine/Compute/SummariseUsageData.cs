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

        [Input("logEntries", "The list of logged usage entries to summarise.")]
        [Output("usageEntries", "A list of summarised usage entries.")]
        public static List<UsageEntry> SummariseUsageData(this List<UsageLogEntry> logEntries)
        {
            string computer = Environment.MachineName;

            List<UsageEntry> dbEntries = new List<UsageEntry>();

            foreach (var fileGroup in logEntries.GroupBy(x => x.FileId))
            {
                string fileId = fileGroup.Key;
                string fileName = fileGroup.Select(x => x.FileName).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();
                string projectID = fileGroup.Select(x => x.ProjectID).Where(x => !string.IsNullOrEmpty(x)).FirstOrDefault();

                foreach (var itemGroup in fileGroup.GroupBy(x => x.CallerName + x.SelectedItem))
                {
                    UsageLogEntry firstEntry = itemGroup.First();

                    dbEntries.Add(new UsageEntry
                    {
                        StartTime = itemGroup.Min(x => x.Time),
                        EndTime = itemGroup.Max(x => x.Time),
                        UI = firstEntry.UI,
                        UiVersion = firstEntry.UiVersion,
                        CallerName = firstEntry.CallerName,
                        SelectedItem = firstEntry.SelectedItem,
                        Computer = computer,
                        BHoMVersion = firstEntry.BHoMVersion,
                        FileId = fileId,
                        FileName = fileName,
                        ProjectID = projectID,
                        NbCallingComponents = itemGroup.Select(x => x.ComponentId).Distinct().Count(),
                        TotalNbCalls = itemGroup.Count(),
                        Errors = itemGroup.SelectMany(x => x.Errors).GroupBy(x => x.Message).Select(g => g.First()).ToList()
                    });
                }
            }

            return dbEntries;
        }

        /***************************************************/
    }
}




