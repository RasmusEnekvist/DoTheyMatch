<%@ Page Title="" Language="C#" MasterPageFile="~/MasterDesign.master" AutoEventWireup="true" CodeFile="comparePage.aspx.cs" Inherits="comparePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="upperSpec">
        <h3>Är personerna desamma?</h3>
        <asp:Button ID="btnYesU" CssClass="button" runat="server" Text="Ja" OnClick="btnYes_Click" />
        <asp:Button ID="btnNoU" CssClass="button" runat="server" Text="Nej" OnClick="btnNo_Click" />
        <asp:Button ID="btnSkippU" CssClass="button" runat="server" Text="Vet ej" OnClick="skipp_Click" />
    </div>
    <div id="height">





        <div id="leftSpec">

            <h3 class="center">Arkitekt från Riksantikvarieämbetet</h3>
            <p>
                <asp:Image runat="server" CssClass="thumb" Width="200px" ID="raaImage" />
                <b>Namn:</b>
                <asp:Literal ID="bName" runat="server"></asp:Literal>
            </p>
            <p>
                <b>Födelseår:</b>
                <asp:Literal ID="bBornYear" runat="server"></asp:Literal>
            </p>
            <p>
                <b>Dödsår:</b>
                <asp:Literal ID="bdied" runat="server"></asp:Literal>
            </p>
            <p>
                <b>Externalänkar:</b>
                <asp:HyperLink runat="server" ID="wiki"></asp:HyperLink>
            </p>
            <p><b>Byggnader av personen:</b></p>
            <asp:Panel runat="server" CssClass="panel" ID="panelBuildingsList">
            </asp:Panel>

            <!--Slut på leftSpec-->
        </div>


        <div id="rightSpec">
            <h3 class="center">Person från Libris</h3>
            <p>
                <asp:Image CssClass="thumb" runat="server" Width="200px" ID="librisImage" />
                <b>Namn:</b>
                <asp:Literal ID="LName" runat="server"></asp:Literal>
            </p>
            <p>
                <b>Födelseår:</b>
                <asp:Literal ID="LBirthYear" runat="server"></asp:Literal>
            </p>
            <p>
                <b>Dödsår:</b>
                <asp:Literal ID="LDeathYear" runat="server"></asp:Literal>
            </p>
            <p>
                <b>Externlänk:</b>
                <asp:HyperLink runat="server" ID="linkWiki"></asp:HyperLink>
            </p>
            <p><b>Böcker om:</b></p>
            <asp:Panel runat="server" CssClass="panel" ID="panelBooksBy">
            </asp:Panel>
            <p><b>Böcker av</b>:</p>
            <asp:Panel runat="server" CssClass="panel" ID="panelBooksAbout">
            </asp:Panel>

            <!--Slut på rightSpec-->
        </div>

        <!--Slut på height-->
    </div>
    <div id="downSpec">
        <p>Är det samma person?</p>
        <p>
            <asp:Button ID="btnYes" CssClass="button" runat="server" Text="Ja" OnClick="btnYes_Click" />
            <asp:Button ID="btnNo" CssClass="button" runat="server" Text="Nej" OnClick="btnNo_Click" />
            <asp:Button ID="skipp" CssClass="button" runat="server" Text="Vet ej" OnClick="skipp_Click" />
        </p>
    </div>
</asp:Content>
