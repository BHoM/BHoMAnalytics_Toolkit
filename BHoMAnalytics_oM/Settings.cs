/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using BH.oM.Reflection.Debugging;
using BH.oM.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.BHoMAnalytics
{
    public class Settings : BHoMObject, ISettings, IInitialisationSettings
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public virtual string ServerAddress { get; set; } = "";

        public virtual string DatabaseName { get; set; } = "";

        public virtual string CollectionName { get; set; } = "";

        public virtual string InitialisationMethod { get; } = "BH.Adapter.BHoMAnalytics.BHoMAnalyticsAdapter.SendUsageData";


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public Settings(string serverAddress, string databaseName, string collectionName)
        {
            ServerAddress = serverAddress;
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }

        /***************************************************/
    }
}

