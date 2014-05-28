using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

public partial class comparePage : Page
{
    private readonly BackEnd backEnd = new BackEnd();
    private Person raaPerson;
    private Person librisPerson;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Session["raaid"] == null || Session["librisid"] == null)
            {
                Response.Redirect("~/ListOfLinkedData.aspx");
            }
            String raaId = Session["raaid"].ToString();
            String librisId = Session["librisid"].ToString();

            raaPerson = backEnd.GetRaaPersonById(raaId);
            librisPerson = backEnd.GetLibrisPersonById(librisId);
            FillPageWithdata();
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
    /// <summary>
    /// returns the user to the list page
    /// </summary>
    /// <param name="sender">The object that triggerd the listener</param>
    /// <param name="e">Event Arguments</param>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/ListOfLinkedData.aspx");
    }
    /// <summary>
    /// Fills the page with data about the persons
    /// </summary>
    private void FillPageWithdata()
    {
        panelBuildingsList.Controls.Clear();
        panelBooksBy.Controls.Clear();
        panelBooksAbout.Controls.Clear();
        //RAÄ

        bName.Text = raaPerson.Name;
        bBornYear.Text = raaPerson.BirthYear;
        bdied.Text = raaPerson.DeathYear;
        if (raaPerson.GetImageURL() != "?")
        {
            raaImage.ImageUrl = raaPerson.GetImageURL();
        }
        else
        {
            raaImage.ImageUrl = "~/Images/QuestionMark.JPG";


        }
        if (raaPerson.WikipediaLink != null)
        {
            wiki.NavigateUrl = raaPerson.WikipediaLink;
            wiki.Text = "Wikipedia";
            wiki.Target = "_blank";
        }
        foreach (Link link in raaPerson.GetListOfExternalEntitie())
        {
            HyperLink hyperLink = new HyperLink { Text = link.Title, NavigateUrl = link.Uri, Target = "_blank" };
            panelBuildingsList.Controls.Add(hyperLink);
            panelBuildingsList.Controls.Add(new LiteralControl("<br />"));
        }

        //Libris
        LName.Text = librisPerson.Name;
        LBirthYear.Text = librisPerson.BirthYear;
        LDeathYear.Text = librisPerson.DeathYear;
        if (librisPerson.WikipediaLink != null)
        {
            linkWiki.NavigateUrl = librisPerson.WikipediaLink;
            linkWiki.Text = "Wikipedia";
            linkWiki.Target = "_blank";
        }
        if (librisPerson.GetImageURL() != "?")
        {
            librisImage.ImageUrl = librisPerson.GetImageURL();
        }
        else
        {
            librisImage.ImageUrl = "~/Images/QuestionMark.JPG";


        }
        if (librisPerson.GetListOfExternalEntitie()[0].Title != "?")
        {
            foreach (Link link in librisPerson.GetListOfExternalEntitie())
            {
                HyperLink hyperLink = new HyperLink { Text = link.Title, NavigateUrl = link.Uri, Target = "_blank" };
                if (link.MadeBy)
                {
                    panelBooksBy.Controls.Add(hyperLink);
                    panelBooksBy.Controls.Add(new LiteralControl("<br />"));
                }
                else
                {
                    panelBooksAbout.Controls.Add(hyperLink);
                    panelBooksAbout.Controls.Add(new LiteralControl("<br />"));
                }
            }
        }
    }
    /// <summary>
    /// Adds a vote for the connecton beening wrong
    /// </summary>
    /// <param name="sender">The object that triggerd the listener</param>
    /// <param name="e">Event Arguments</param>
    protected void btnWrong_Click(object sender, EventArgs e)
    {
        try
        {
            if ((bool)Session["same"])
            {
                backEnd.ReportFault(raaPerson, librisPerson);
            }
            else
            {
                backEnd.ReportFaultNotSame(raaPerson, librisPerson);
            }
        }
        catch (MySqlException)
        {
            Response.Redirect("~/ErrorPages/sql.aspx");
        }
        Response.Redirect("~/ThankYou.aspx");

    }
}