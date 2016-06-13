using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace FolderManager
{
    public partial class Form1 : Form
    {
        BindingList<FolderInfo> info = new BindingList<FolderInfo>();
        XmlSerializer serializer = new XmlSerializer(typeof(List<FolderInfo>));
        string fileName = "folders.xml";

        public Form1()
        {
            InitializeComponent();

            populateData();
            
            foldersGrid.DataSource = info;

            SetColumnSettings();
        }

        private void SetColumnSettings()
        {
            if (info.Count > 0)
            {
                foldersGrid.Columns[0].MinimumWidth = 350;
                foldersGrid.Columns[0].ReadOnly = true;
                foldersGrid.Columns[1].MinimumWidth = 350;
            }
        }

        private void populateData()
        {
            if (File.Exists(fileName))
            {
                var xmlReader = System.Xml.XmlReader.Create(fileName);
                List<FolderInfo> retrievedList = (List<FolderInfo>)serializer.Deserialize(xmlReader);
                info = new BindingList<FolderInfo>(retrievedList);
                xmlReader.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {                
                var xmlwriter = System.Xml.XmlWriter.Create(fileName);                
                serializer.Serialize(xmlwriter, info.ToList<FolderInfo>());
                xmlwriter.Close();
            }
            catch { }
        }

        private void addNewPathButton_Click(object sender, EventArgs e)
        {
            int count = info.Count(n => n.Name.ToLower() == nameTextBox.Text.ToLower());
            if (Directory.Exists(pathTextbox.Text) && count == 0 )
            {
                FolderInfo fInfo = new FolderInfo { Name = nameTextBox.Text, Link = pathTextbox.Text };
                info.Add(fInfo);
                RefreshGrid();
                nameTextBox.Text = string.Empty;
                pathTextbox.Text = string.Empty;
            }
            else if (count > 0)
            {
                MessageBox.Show("Name already exists");
            }
            else
            {
                MessageBox.Show("Path is not valid");
            }
        }

        private void RefreshGrid()
        {
            filterTextBox.Text = "";
            foldersGrid.DataSource = info;
            SetColumnSettings();
        }

        private void openFoldersButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in foldersGrid.SelectedRows)
            {
                Process.Start(row.Cells["Link"].Value.ToString());
            }
        }

        private void deleteSelectedButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in foldersGrid.SelectedRows)
                {
                    info.Remove(info.Single(n => n.Name == row.Cells["Name"].Value.ToString()));                    
                }

                RefreshGrid();
            }
        }

        private void filterButton_Click(object sender, EventArgs e)
        {
            foldersGrid.DataSource = info.Where(n => n.Name.ToLower().Contains(filterTextBox.Text.ToLower())).ToList<FolderInfo>();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            foldersGrid.DataSource = info;
            filterTextBox.Text = "";
        }

    }

    [Serializable]
    public class FolderInfo
    {
        public string Name { get; set; }
        public string Link { get; set; }
    }

}
