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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
// Reuse the Field web services calls
using FieldAPIIntro;
// Added for RestSharp. 
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers; 

namespace FieldAPIWebIntro
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonLogin_Click(object sender, EventArgs e)
        {
            // Field Login call here. 
            string ticket = Field.Login(TextBoxUserName.Text, TextBoxPassword.Text);

            bool authenticated = !string.IsNullOrEmpty(ticket);
            if (authenticated)
            {
                // Save ticket for this session 
                Session["ticket"] = ticket;
                TextBoxResponse.Text = ticket;
                ButtonLogin.Enabled = false;

                // Initialize indices of project and model index
                Session["projectIndex"] = 0;
                Session["projectId"] = "";
                Session["issueIndex"] = 0;
                Session["issueId"] = "";
            }

            // Show the request and response in the form. 
            // This is for learning purpose. 
            ShowRequestResponse(); 
        }
        
        // Show the request and response in the form.
        // This is for learning purpose.
        protected void ShowRequestResponse()
        {            
            IRestResponse response = Field.m_lastResponse;
            TextBoxRequest.Text = response.ResponseUri.AbsoluteUri;
            string responseText = RemoveAngleBracket(response.Content); 
            TextBoxResponse.Text =
                 "Status Code: " + response.StatusCode.ToString()
                + Environment.NewLine + responseText;
        }

        // Remove angle brackets ("<" and ">") from the given string.
        // .NET framework does not like "<...>"  
        // e.g., Field response might contain like "<no description>"
        // http://forums.asp.net/t/1235144.aspx?A+potentially+dangerous+Request+Form+value+was+detected+from+the+client
        //
        protected string RemoveAngleBracket(string str)
        {
            string s = str.Replace("<", "");
            s = s.Replace(">", "");
            return s; 
        }

        protected void ButtonProject_Click(object sender, EventArgs e)
        {
            
            string ticket = Session["ticket"] as string;

            // Retrieve ticket for this session 
            List<Project> proj_list = Field.ProjectNames(ticket);

            // Show the request and response in the form. 
            // This is for learning purpose. 
            ShowRequestResponse();

            // Post process to get hold of one project. 
            // For simplicity, just pick up arbitrary one for our exercise. 

            int proj_index = Convert.ToInt32(HttpContext.Current.Session["projectIndex"]);
            proj_index %= proj_list.Count;
            Project proj = proj_list[proj_index++];
            string project_id = proj.id;
            string project_name = proj.name;
            Session["projectIndex"] = proj_index;
            Session["projectId"] = project_id;

            LabelProject.Text = "Project (" + proj_index.ToString() + "/" + proj_list.Count.ToString() + ")";
            TextBoxProject.Text = project_name; 
        }

        protected void ButtonIssue_Click(object sender, EventArgs e)
        {
            string ticket = HttpContext.Current.Session["ticket"] as string;
            string project_id = HttpContext.Current.Session["projectId"] as string;

            // Here is the main call to the Field web services API.  
            List<Issue> issue_list = Field.IssueList(ticket, project_id);
            Session["issueList"] = issue_list; 

            // Show the request and response in the form. 
            // This is for learning purpose. 
            ShowRequestResponse(); 

            // Post process to get hold of one issue. 
            // For simplicity, just pick up arbitrary one.

            int issue_index = Convert.ToInt32(HttpContext.Current.Session["issueIndex"]);
            issue_index %= issue_list.Count;
            Issue issue = issue_list[issue_index++];
            string issue_id = issue.id;
            Session["issueIndex"] = issue_index;
            Session["issueId"] = issue_id;
            // Show issue count on the label.
            LabelIssue.Text = "Issue (" + issue_index.ToString() + "/" + issue_list.Count.ToString() + ")";

            // Show the issue string 
            JsonSerializer serial = new JsonSerializer();
            TextBoxIssue.Text = serial.Serialize(issue); 

            // Make a string for an issue creation based on the current issue. 
            // This is for another button to create new issue. 
            TextBoxNewIssue.Text = IssueToString(issue); 
        }

        /// Helper function to compose a string to create an issue. 
        /// Given an issue data, compose a string like below.
        /// This is for creating an issue. 
        /// 
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

            // Compose the JSON data for fields values 
            foreach (IssueFieldItem item in issue.fields)
            {
                if (item.value == null) continue; // Skip when no value. 
                if (item.id.Equals("f--identifier")) continue; // Avoid duplicating the ID

                string s = "{\"id\":\"" + item.id + "\","
                    + "\"value\":\"" + item.value.ToString() + "\"},";

                issueString += s;
            }
            int len = issueString.Length;
            if (len > 0)
            {
                // Removing the last extra ',' 
                issueString = issueString.Remove(len - 1);
            }
            // This is the whole string 
            issueString = "[{\"temporary_id\":\"Tmp001\",\"fields\":["
                + issueString + "]}]";

            return issueString;
        }

        protected void ButtonCreate_Click(object sender, EventArgs e)
        {
            string ticket = HttpContext.Current.Session["ticket"] as string;
            string project_id = HttpContext.Current.Session["projectId"] as string;

            // Sample JSON string:  
            // [{
            //     "temporary_id":"Q45", 
            //     "fields": [
            //        {"id":"f--description","value":"Test"}, 
            //        {"id":"f--issue_type_id", "value":"f498d0f5-0be0-11e2-9694-14f6960d7e4f"} 
            //     ]
            // }]

            string issues = TextBoxNewIssue.Text;
            string issue_id = Field.IssueCreate(ticket, project_id, issues);

            ShowRequestResponse();
        }


    }
}