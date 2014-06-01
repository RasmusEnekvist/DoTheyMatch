using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

public partial class _Default : System.Web.UI.Page
{
    private readonly BackEnd backEnd = new BackEnd();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            List<Person> linked = backEnd.GetListOfLinkedPersons();
            ListOfLinked.DataSource = linked;
            ListOfLinked.DataBind();
            List<Person> notLinked = backEnd.GetListOfNotTheSame();
            ListOfNotCorrect.DataSource = notLinked;
            ListOfNotCorrect.DataBind();
        }
        catch (MySqlException)
        {
            Response.Redirect("~/ErrorPages/sql.aspx");
        }

    }

    /// <summary>
    /// Handle click on a SameAs link, and display the data
    /// </summary>
    /// <param name="sender">The object that triggerd the listener</param>
    /// <param name="e">Event Arguments</param>
    protected void Click(object sender, GridViewCommandEventArgs e)
    {
        int index = Convert.ToInt32(e.CommandArgument);
        Session["raaid"] = ListOfLinked.Rows[index].Cells[1].Text;
        Session["librisid"] = ListOfLinked.Rows[index].Cells[2].Text;
        Session["same"] = true;
        Response.Redirect("~/Evaluate.aspx");
    }
    /// <summary>
    /// Handle click on a NotTheSame link, and display the data
    /// </summary>
    /// <param name="sender">The object that triggerd the listener</param>
    /// <param name="e">Event Arguments</param> 
    protected void ClickNotTheSame(object sender, GridViewCommandEventArgs e)
    {
        int index = Convert.ToInt32(e.CommandArgument);
        Session["raaid"] = ListOfNotCorrect.Rows[index].Cells[1].Text;
        Session["librisid"] = ListOfNotCorrect.Rows[index].Cells[2].Text;
        Session["same"] = false;
        Response.Redirect("~/Evaluate.aspx");
    }
}