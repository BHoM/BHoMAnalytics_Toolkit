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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using BH.oM.UI;

namespace BH.UI.Analytics
{
    /// <summary>
    /// Interaction logic for CaptureProjectData.xaml
    /// </summary>
    public partial class CaptureProjectData : Window
    {
        public CaptureProjectData()
        {
            InitializeComponent();
            this.ShowDialog();
            ProjectBtn.Focus();
        }

        private void Click_ProjectBtn(object sender, EventArgs e)
        {
            ProjectInputPanel.Visibility = Visibility.Visible;
            this.Height = 250;
            ProjectIDInput.Focus();
        }

        private void Click_NonProjectBtn(object sender, EventArgs e)
        {
            string projectID = "Non-Project work";

            BH.Engine.Base.Compute.RecordEvent(new ProjectIDEvent
            {
                Message = "The project ID for this file is now set to " + projectID,
                ProjectID = projectID
            });

            this.Close();
        }

        private void Click_ConfirmProjectBtn(object sender, EventArgs e)
        {
            ConfirmProject();
        }

        private void KeyDown_ProjectIDEntry(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                ConfirmProject();
        }

        private void Deactivate_Window(object sender, EventArgs e)
        {
            this.Topmost = true;
        }

        private void ConfirmProject()
        {
            var projectID = ProjectIDInput.Text;
            if (string.IsNullOrEmpty(projectID))
            {
                MessageBox.Show("Project ID cannot be empty", "Error", MessageBoxButton.OK);
                return;
            }

            BH.Engine.Base.Compute.RecordEvent(new ProjectIDEvent
            {
                Message = "The project ID for this file is now set to " + projectID,
                ProjectID = projectID
            });

            this.Close();
        }

        private void ProjectBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(Colors.Black);
        }
    }
}
