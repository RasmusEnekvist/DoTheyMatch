<%@ Page Title="" Language="C#" MasterPageFile="~/MasterDesign.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>Välkommen</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">



    <div id="height" class="margin">
        <div id="leftSpec">

            <p>
                Projekt: Do They Match?
            </p>
            <p>
                Observera att Riksantikvarieämbetets SPARQL-endpoint endast är en testversion
            </p>
            <p>
                Här ges du möjlighet att bidra till träffsäkrare sökningar på internet i framtiden 
                        genom att du nu jämför data om två personer och avgör om det är samma person.

            </p>
            <p>
                De personer du kommer få se i den vänstra kolumnet finns i Bebyggelseregistret hos 
                        Riksantikvarieämbetet. Där är personen omtalad som arkitekt för någon eller några byggnader.
            </p>
            <p>
                I högra kolumnen kommer personer med samma namn som arkitekten att dyka upp. Dessa finns 
                    registrerade i Libris databas som personer. 
            </p>
            <p>Din uppgift nu är helt enkelt att avgöra om det är samma person eller om det inte är det.</p>
            <p>Lycka till!!</p>
        </div>
        <div id="rightSpec">
            <span class="center">
                <asp:Literal runat="server" ID="proc"></asp:Literal></span><br />
            <p class="center">


                <asp:Image ID="image2" runat="server" ImageUrl="~/images/200px-Gunnar_Asplund_1940.jpg" Width="150px" />
                <asp:Image ID="image1" runat="server" ImageUrl="~/images/Same.jpg" Width="150px" />
                <asp:Image ID="image3" runat="server" ImageUrl="~/images/ARKM.1988-104-0974.jpg" Width="150px" />
                <br />
            </p>
        </div>
    </div>

</asp:Content>

