#region Copyright
//
// Copyright (C) 2015 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//
// Written by M.Harada
// 
#endregion // Copyright

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// Added for RestSharp. 
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers; 

namespace FieldAPIIntro
{
    public partial class Form1 : Form
    {
        // Member variables. 
        private static string m_ticket = "";
        private static string m_project_id = ""; // current project id 
        private static int m_proj_index = 0;     // index in the list of projects 
        private static string m_issue_id = "";   // current issue id  
        private static int m_issue_index = 0;    // index in the list of issues
        private static List<Issue> m_issue_list; 

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            // Get the user name and password from the user. 
            string userName = textBoxUserName.Text;
            string password = textBoxPassword.Text;

            textBoxRequest.Text = "Request comes here";
            textBoxResponse.Text = "Response comes here. This may take secones. Please wait...";
            this.Update();

            // Here is the main part that we call Glue login 
            m_ticket = Field.Login(userName, password);

            // For our learning, 
            // show the request and response in the form. 
            ShowRequestResponse(); 
        }

        // show the request and response in the form. 
        private void ShowRequestResponse()
        {
            IRestResponse response = Field.m_lastResponse;
            textBoxRequest.Text = response.ResponseUri.AbsoluteUri;
            textBoxResponse.Text = 
                "Status Code: " + response.StatusCode.ToString() 
                + Environment.NewLine + response.Content;
        }

        private void buttonProject_Click(object sender, EventArgs e)
        {
            textBoxRequest.Text = "Request comes here";
            textBoxResponse.Text = "Response comes here. This may take secones. Please wait...";
            this.Update();

            List<Project> proj_list = Field.ProjectNames(m_ticket);

            ShowRequestResponse(); 

            // Post process to get hold of one project. 
            // For simplicity, just pick up arbitrary one for our exercise. 

            m_proj_index %= proj_list.Count;
            Project proj = proj_list[m_proj_index++];
            m_project_id = proj.id;
            string project_name = proj.name;
            labelProject.Text = "Project (" + m_proj_index.ToString() + "/" + proj_list.Count.ToString() + ")";
            textBoxProject.Text = project_name; 

        }

        private void buttonIssue_Click(object sender, EventArgs e)
        {
            textBoxRequest.Text = "Request comes here";
            textBoxResponse.Text = "Response comes here. This may take secones. Please wait...";
            this.Update();

            string area_ids = "No Area"; 
            List<Issue> issue_list = Field.IssueList(m_ticket, m_project_id, area_ids);
            m_issue_list = issue_list; 

            ShowRequestResponse();

            // Post process to get hold of one project. 
            // For simplicity, just pick up arbitrary one for our exercise. 

            m_issue_index %= issue_list.Count;
            Issue issue= issue_list[m_issue_index++];
            m_issue_id = issue.id;
            IssueFieldItem item = issue.fields.Find(x => x.name.Equals("Identifier"));
            labelIssue.Text = "Issue (" + m_issue_index.ToString() + "/" + issue_list.Count.ToString() + ")";

            // Show the issue string 
            JsonSerializer serial = new JsonSerializer(); 
            textBoxIssue.Text = serial.Serialize(issue); 
            
            // Make one issue for creation. this is for later use.  
            textBoxNewIssue.Text = IssueToString(issue); 
        }

        /// Given an issue data, compose a string like below.
        /// This is for creating an issue. 
        /// 
        /// ex. 
        /// [{
        ///     "temporary_id":"Q45", 
        ///     "fields": [
        ///        {"id":"f--description","value":"Test"}, 
        ///        {"id":"f--issue_type_id", "value":"f498d0f5-0be0-11e2-9694-14f6960d7e4f"} 
        ///     ]
        /// }]
        /// 
        private string IssueToString(Issue issue)
        {
            string issueString = null;

            // Compose the JSON data with fields values 
            foreach (IssueFieldItem item in issue.fields)
            {
                if (item.value == null) continue;
                if (item.id.Equals("f--identifier")) continue; 
                
                string s = "{\"id\":\"" + item.id + "\","
                    + "\"value\":\"" + item.value.ToString() + "\"},";
                issueString += s;
                
            }
            int len = issueString.Length; 
            if (len > 0)
            {
                // removing the last extra ',' 
                issueString = issueString.Remove(len - 1); 
            }
            // This is the whole string 
            issueString = "[{\"temporary_id\":\"Tmp001\",\"fields\":["
                + issueString + "]}]"; 

            return issueString;  
        }

        private void buttonIssueCreate_Click(object sender, EventArgs e)
        {
            textBoxRequest.Text = "Request comes here";
            textBoxResponse.Text = "Response comes here. This may take secones. Please wait...";
            this.Update();

            // ex.  
            // [{
            //     "temporary_id":"Q45", 
            //     "fields": [
            //        {"id":"f--description","value":"Test"}, 
            //        {"id":"f--issue_type_id", "value":"f498d0f5-0be0-11e2-9694-14f6960d7e4f"} 
            //     ]
            // }]

            string issues = textBoxNewIssue.Text; 
            string issue_id = Field.IssueCreate(m_ticket, m_project_id, issues);

            ShowRequestResponse();
        }

        //============================================================
        // Make a a chart showing the numbers of issues by status.
        // This is not a part of Field API. 
        // Just to show what you can do with the retrieved data.
        //============================================================
        private void buttonReport_Click(object sender, EventArgs e)
        {
            // Collect data from the issue list.
            // Count the number of issues for each status. 
            Dictionary<string, int> data = new Dictionary<string,int>();  

            foreach(Issue issue in m_issue_list) {
                string status = issue.fields.Find(x => x.name.Equals("Status")).value; 
                if(data.ContainsKey(status)) {
                    data[status]++;
                }
                else {
                    data.Add(status, 1); 
                }

            }

            // Clear the chart data 
            chart1.Series[0].Points.Clear(); 

            // Fill the chart data  
            foreach (var item in data)
            {
                chart1.Series[0].Points.AddXY(item.Key, item.Value); 
            }

            // Add a title text 
            chart1.Titles[0].Text = "Issues by Status"; 
        }

    }
}
