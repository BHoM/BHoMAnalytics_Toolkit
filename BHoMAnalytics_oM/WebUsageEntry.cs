/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.BHoMAnalytics
{
    public class WebUsageEntry : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public virtual string Toolkit { get; set; } = "";

        public virtual string ItemName { get; set; } = "";

        public virtual ItemType ItemType { get; set; } = ItemType.Undefined;

        public virtual string FullName { get; set; } = "";

        public virtual string CallerName { get; set; } = "";

        public virtual DateTime TimeStamp { get; set; }

        public virtual string Website { get; set; } = "";

        public virtual string Database { get; set; } = "";

        public virtual string UserName { get; set; } = "";

        public virtual string ProjectID { get; set; } = "";

        public virtual string BHoMVersion { get; set; } = "";

        public virtual int ErrorCount { get; set; } = 0;

        /***************************************************/
    }
}


