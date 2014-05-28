<%@ Page Title="" Language="C#" MasterPageFile="~/MasterDesign.master" AutoEventWireup="true" CodeFile="ListOfLinkedData.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>Välkommen</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="listSpec">
        <h3 class="center">Lista på färdiga matchningar</h3>

        <p>

            <asp:GridView ID="ListOfLinked" runat="server" OnRowCommand="Click" AutoGenerateColumns="false" DataKeyNames="Name">
                <Columns>
                    <asp:ButtonField DataTextField="Name" HeaderText="Namn" />
                    <asp:BoundField DataField="URI" HeaderText="Raä-URI" />
                    <asp:BoundField DataField="SameAs.URI" HeaderText="Libris-URI" />
                </Columns>
            </asp:GridView>
        </p>
        <h3 class="center">Lista på ickekorrekta</h3>
        <p>

            <asp:GridView ID="ListOfNotCorrect" runat="server" OnRowCommand="ClickNotTheSame" AutoGenerateColumns="false" DataKeyNames="Name">
                <Columns>
                    <asp:ButtonField DataTextField="Name" HeaderText="Namn" />
                    <asp:BoundField DataField="URI" HeaderText="Raä-URI" />
                    <asp:BoundField DataField="SameAs.URI" HeaderText="Libris-URI" />
                </Columns>
            </asp:GridView>
        </p>

    </div>
</asp:Content>

