<%@ Page Title="" Language="C#" MasterPageFile="~/MasterDesign.master" AutoEventWireup="true" CodeFile="Evaluate.aspx.cs" Inherits="comparePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="upperSpec">
        <h3>Är personerna felaktigt kopplade?</h3>
        <asp:Button ID="btnWrongU" CssClass="button" runat="server" Text="Anmäl som felaktig" OnClick="btnWrong_Click" />
        <asp:Button ID="btnBackU" CssClass="button" runat="server" Text="Tillbaka" OnClick="btnBack_Click" />


    </div>
    <div id="height">
        <asp:Literal ID="RaaId" runat="server"></asp:Literal>
        <asp:Literal ID="LibrisId" runat="server"></asp:Literal>
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
    </div>
    <div id="downSpec">
        <p>Rapportera kopplingen som felaktig</p>
        <p>
            <asp:Button ID="btnWrong" CssClass="button" runat="server" Text="Anmäl som felaktig" OnClick="btnWrong_Click" />
            <asp:Button ID="btnBack" CssClass="button" runat="server" Text="Tillbaka" OnClick="btnBack_Click" />
        </p>
    </div>

</asp:Content>

