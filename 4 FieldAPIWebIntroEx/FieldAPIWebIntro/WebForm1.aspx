<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="FieldAPIWebIntro.WebForm1" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Field API Web Intro</title>
        <style type="text/css">
            #form1
        {
            height: 171px;
            width: 600px;
        }
        body
        {
            background-color:#c6d7e2; 
        }
        h1
        {
            color:#2a2e74; /*orange*/  
            text-align: center;
        }
        #iframeGlue
        {
            height: 442px;
            width: 561px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>My First Field API</h1>
    </div>
        <asp:Label ID="LabelUserName" runat="server" Text="User Name" Width="100px"></asp:Label>
<asp:TextBox ID="TextBoxUserName" runat="server" Width="400px"></asp:TextBox>

        <p>
            <asp:Label ID="LabelPassword" runat="server" Text="Password" Width="100px"></asp:Label>
            <asp:TextBox ID="TextBoxPassword" runat="server" TextMode="Password" Width="400px"></asp:TextBox>
&nbsp;<asp:Button ID="ButtonLogin" runat="server" OnClick="ButtonLogin_Click" Text="Login" Width="80px" />
        </p>
        <p>
            <asp:Label ID="LabelProject" runat="server" Text="Project" Width="100px"></asp:Label>
<asp:TextBox ID="TextBoxProject" runat="server" ReadOnly="True" BackColor="#E4E4E4" Width="400px"></asp:TextBox>
&nbsp;<asp:Button ID="ButtonProject" runat="server" OnClick="ButtonProject_Click" Text="Project" Width="80px" />
        </p>
        <p>
            <asp:Label ID="LabelIssue" runat="server" Text="Issue" Width="100px"></asp:Label>
<asp:TextBox ID="TextBoxIssue" runat="server" ReadOnly="True" BackColor="#E4E4E4" Width="400px"></asp:TextBox>
&nbsp;<asp:Button ID="ButtonIssue" runat="server" OnClick="ButtonIssue_Click" Text="Issue" Width="80px" />
        </p>
        <p>
            <asp:Label ID="LabelNewIssue" runat="server" Text="New issue" Width="100px"></asp:Label>
<asp:TextBox ID="TextBoxNewIssue" runat="server" Width="400px"></asp:TextBox>
&nbsp;<asp:Button ID="ButtonCreate" runat="server" OnClick="ButtonCreate_Click" Text="Create" Width="80px" />
        </p>
        <p>
            <asp:Label ID="LabelRequest" runat="server" Text="Request"></asp:Label>
            <asp:TextBox ID="TextBoxRequest" runat="server" Height="60px" ReadOnly="True" TextMode="MultiLine" Width="580px"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="LabelResponse" runat="server" Text="Response"></asp:Label>
            <asp:TextBox ID="TextBoxResponse" runat="server" Height="60px" TextMode="MultiLine" Width="580px"></asp:TextBox>
        </p>
        <asp:Button ID="ButtonReport" runat="server" OnClick="ButtonReport_Click" Text="Report" Width="80px" />
        <br />
        <asp:Chart ID="Chart1" runat="server" Palette="Pastel" Width="580px" BackColor="WhiteSmoke">
            <series>
                <asp:Series ChartType="Bar" Name="Series1" Palette="Pastel">
                </asp:Series>
            </series>
            <chartareas>
                <asp:ChartArea Name="ChartArea1" BackColor="WhiteSmoke">
                </asp:ChartArea>
            </chartareas>
            <Titles>
                <asp:Title Name="Title1">
                </asp:Title>
            </Titles>
        </asp:Chart>
    </form>
</body>
</html>
