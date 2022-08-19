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
            this.Show();
        }

        private void Click_ProjectBtn(object sender, EventArgs e)
        {
            ProjectInputPanel.Visibility = Visibility.Visible;
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
            var projectID = ProjectIDInput.Text;
            if(string.IsNullOrEmpty(projectID))
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

        private void Deactivate_Window(object sender, EventArgs e)
        {
            this.Topmost = true;
        }
    }
}
