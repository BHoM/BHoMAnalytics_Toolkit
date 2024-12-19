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

using BH.oM.Base;
using BH.oM.Base.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.BHoMAnalytics
{
    public class UsageEntry : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public virtual DateTime StartTime { get; set; } 

        public virtual DateTime EndTime { get; set; }

        public virtual string UI { get; set; } = "";

        public virtual string UiVersion { get; set; } = "";

        public virtual string CallerName { get; set; } = "";

        public virtual object SelectedItem { get; set; } = null;

        public virtual string Computer { get; set; } = "";

        public virtual string Username { get; set; } = "";

        public virtual string BHoMVersion { get; set; } = "";

        public virtual string FileId { get; set; } = "";

        public virtual string FileName { get; set; } = "";

        public virtual string ProjectID { get; set; } = "";

        public virtual int NbCallingComponents { get; set; } = 0;

        public virtual int TotalNbCalls { get; set; } = 0;

        public virtual List<Event> Errors { get; set; } = new List<Event>();


        /***************************************************/
    }
}





