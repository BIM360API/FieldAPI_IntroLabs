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
using System.Text;
using System.Threading.Tasks;
using System.Net; // for HttpStatusCode 
// Added for RestSharp 
using RestSharp;
using RestSharp.Deserializers;  

namespace HelloFieldWorld
{
    class Field
    {
        // Constants 
        private const string _baseUrl = @"https://bim360field.autodesk.com";

        // Member variables 
        // Save the last response. This is for learning purpose. 
        public static IRestResponse m_lastResponse = null; 

        ///===================================================
        /// Login
        /// URL:  
        /// https://bim360field.autodesk.com/api/login
        /// Method: POST
        /// Description: 
        /// Authenticate using BIM 360 Field credentials. 
        /// On success, returns a 36 byte GUID "ticket" which 
        /// needs to be passed in on subsequent calls.
        /// Doc:
        /// https://bim360field.autodesk.com/apidoc/index.html#mobile_api_method_1
        /// 
        /// Sample Response (JSON) 
        /// {"ticket":"0054444d-be79-1345-6657-45422efd9d80","message":"","title":""}
        /// 
        ///===================================================
        public static string Login(string userName, string password)
        {
            // (1) Build request 
            var client = new RestClient();
            client.BaseUrl = new System.Uri(_baseUrl);

            // Set resource/end point 
            var request = new RestRequest();
            request.Resource = "/api/login";
            request.Method = Method.POST;

            // Set required parameters 
            request.AddParameter("username", userName);
            request.AddParameter("password", password); 

            // (2) Execute request and get response
            IRestResponse response = client.Execute(request);

            // Save response. This is to see the response for our learning.
            m_lastResponse = response;

            // (3) Parse the response and get the ticket. 
            string ticket = "";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                JsonDeserializer deserial = new JsonDeserializer();
                LoginResponse loginResponse = 
                    deserial.Deserialize<LoginResponse>(response);
                ticket = loginResponse.ticket; 
            }

            return ticket; 
        }
    }
}
