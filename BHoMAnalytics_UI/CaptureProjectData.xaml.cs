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
using System.Collections.ObjectModel;
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
using BH.oM.BHoMAnalytics;
using BH.oM.UI;

using BH.Adapter.SQL;
using BH.oM.Adapters.SQL;

namespace BH.UI.Analytics
{
    /// <summary>
    /// Interaction logic for CaptureProjectData.xaml
    /// </summary>
    public partial class CaptureProjectData : Window
    {
        #region Properties
        private string _uiName;
        private int _initialHeight = 240;
        private List<string> _loadedProjectData;
        public ObservableCollection<string> AllProjectData { get; set; }
        public string SelectedProject { get; set; }
        #endregion

        #region Constructor
        public CaptureProjectData(string uiName)
        {
            InitializeComponent();
            DataContext = this;
            VersionTextBlock.Text = $"BHoM Version: {BH.Engine.Base.Query.BHoMVersion()}";
            NonProjectListBox.ItemsSource = Enum.GetValues(typeof(NonProjectOption));
            _uiName = uiName;
            
            if (_uiName == "Grasshopper")
            {
                UIProtipText.Visibility = Visibility.Visible;
                UIProtipText.Text = "Protip: Set use in ProjectNumber panel to avoid getting this popup.";
            }
            else if (_uiName == "Revit")
            {
                UIProtipText.Visibility = Visibility.Visible;
                BHoMUseTextBlock.Text = "Please indicate purpose of this use of BHoM Tools for Revit";
                UIProtipText.Text = "Protip: Set Project Number in Revit settings to avoid getting this popup.";
            }
            else if (_uiName == "Excel")
            {
                UIProtipText.Visibility = Visibility.Visible;
                UIProtipText.Text = "Protip: Set Project Number in Excel Info Title to avoid getting this popup.";
            }

            ReadProjectInformation();

            ProjectBtn.Focus();
            this.ShowDialog();
        }
        #endregion

        #region EventActions
        
        #region Project
        private void Click_ProjectBtn(object sender, EventArgs e)
        {
            ResetForms();
            ProjectInputPanel.Visibility = Visibility.Visible;
            this.Height = 380;
            ProjectComboBox.Focus();
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
        #endregion
        
        #region NonProject
        private void Click_NonProjectBtn(object sender, EventArgs e)
        {
            ResetForms();
            NonProjectBtn.Focus();
            NonProjectSelectionPanel.Visibility = Visibility.Visible;
            this.Height = 380;
            NonProjectListBox.SelectedIndex = 0;
            NonProjectListBox.Focus();
        }

        private void Click_ConfirmNonProjectBtn(object sender, RoutedEventArgs e)
        {
            ConfirmProject();
        }

        private void KeyDown_NonProjectListBox(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                ConfirmProject();
        }

        private void NonProjectListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NonProjectListBox.SelectedValue != null && NonProjectListBox.SelectedValue.Equals(NonProjectOption.Other))
            {
                this.Height = 445;
                OtherSpecifyInput.Visibility = Visibility.Visible;
                OtherSpecifyText.Visibility = Visibility.Visible;
            }
            else
            {
                this.Height = 380;
                OtherSpecifyInput.Visibility = Visibility.Collapsed;
                OtherSpecifyText.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region Window
        
        private void Deactivate_Window(object sender, EventArgs e)
        {
            this.Topmost = true;
        }
        
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        #endregion
        
        #endregion

        #region PrivateMethods
        
        private void ConfirmProject()
        {
            if ((NonProjectSelectionPanel.Visibility == Visibility.Visible && OtherSpecifyInput.Visibility == Visibility.Visible) && string.IsNullOrEmpty(OtherSpecifyInput.Text))
            {
                MessageBox.Show("Please specify a description for the non-project work", "Error", MessageBoxButton.OK);
                return;
            }

            string projectID = "";
            if (ProjectInputPanel.Visibility == Visibility.Visible)
            {
                projectID = SelectedProject;
                projectID = projectID.Split('-').First().Trim();

                if(!_loadedProjectData.Contains(SelectedProject))
                {
                    var answer = MessageBox.Show($"{projectID} is not currently set up as a project in the database. Are you sure you wish to continue with this?", "Confirm", MessageBoxButton.YesNo);
                    if (answer == MessageBoxResult.No)
                        return;
                }
                else
                {
                    if(SelectedProject.ToLower().Contains("inactive") || SelectedProject.ToLower().Contains("dormant"))
                    {
                        var answer = MessageBox.Show($"{projectID} is listed as inactive or dormant. Are you sure you wish to continue with this?", "Confirm", MessageBoxButton.YesNo);
                        if (answer == MessageBoxResult.No)
                            return;
                    }
                }
            }
            else if (NonProjectSelectionPanel.Visibility == Visibility.Visible && OtherSpecifyInput.Visibility == Visibility.Visible)
            {
                projectID = $"Non-Project Work - {(int)NonProjectListBox.SelectedValue} - {OtherSpecifyInput.Text}";
            }
            else if (NonProjectSelectionPanel.Visibility == Visibility.Visible)
            {
                projectID = $"Non-Project Work - {(int)NonProjectListBox.SelectedValue}";
            }

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
        
        private void ResetForms()
        {
            //Non Project
            NonProjectSelectionPanel.Visibility = Visibility.Hidden;
            NonProjectListBox.SelectedValue = null;

            //Other
            OtherSpecifyInput.Text = "";
            OtherSpecifyInput.Visibility = Visibility.Collapsed;
            OtherSpecifyText.Visibility = Visibility.Collapsed;

            //Project
            ProjectInputPanel.Visibility = Visibility.Hidden;
            ProjectComboBox.Text = "";
            ProjectComboBox.SelectedIndex = -1;
            ProjectBtn.Focus();

            //UI specific
            UIProtipText.Visibility = Visibility.Hidden;

            //Window
            this.Height = _initialHeight;
        }

        private void ReadProjectInformation()
        {
            _loadedProjectData = new List<string>();
            AllProjectData = new ObservableCollection<string>();

            try
            {
                var sqlAdapter = new SqlAdapter("SQL-BHoM01", "BuroHappoldData");
                TableRequest request = new TableRequest() { Table = "Project" };
                var obj = sqlAdapter.Pull(request).Cast<BH.oM.Base.CustomObject>().ToList();
                obj = obj.Where(x => x.CustomData["Status"].ToString().ToLower() == "active").ToList();

                obj = obj.OrderBy(x => x.CustomData["Status"].ToString()).ThenBy(x => x.CustomData["ProjectId"].ToString()).ToList();

                Parallel.ForEach(obj, item =>
                {
                    string projectInfo = $"{item.CustomData["ProjectId"].ToString()} - {item.Name}";

                    var projectStatus = item.CustomData["Status"].ToString().ToLower().Trim();
                    if (projectStatus == "dormant")
                        projectInfo += " - DORMANT";
                    else if (projectStatus == "inactive")
                        projectInfo += " - INACTIVE";

                    AllProjectData.Add(projectInfo);
                });

                _loadedProjectData = AllProjectData.ToList();
            }
            catch(Exception e)
            {
                BH.Engine.Base.Compute.RecordWarning("Could not connect to project database for project analytics information. Error: " + e.ToString());
            }
        }
        #endregion
    }
}
