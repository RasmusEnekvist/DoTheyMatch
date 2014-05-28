using System;
using System.Globalization;
using MySql.Data.MySqlClient;


public partial class _Default : System.Web.UI.Page
{
    private BackEnd backEnd = new BackEnd();
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            proc.Text = "<h2>Just nu är " + backEnd.GetProcentageOfLinkedPersons().ToString(CultureInfo.InvariantCulture) + "% av arkitekterna hanterade</h2>";
        }
        catch (MySqlException)
        {
            Response.Redirect("~/ErrorPages/sql.aspx");
        }
        catch (Exception)
        {
            Response.Redirect("~/ErrorPages/sparql.aspx");
        }

    }

}