using System;
using System.Collections.Generic;
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
        if (!IsPostBack)
        {
            try
            {
                if (Session["LibrisPersons"] == null || Session["raa"] == null)
                {
                    bool found = false;
                    while (!found)
                    {
                        raaPerson = backEnd.GetRaaPerson();
                        librisPerson = backEnd.GetLibrisPerson(raaPerson.Name, raaPerson.URI);
                        if (librisPerson != null)
                        {
                            found = true;
                        }

                    }
                }
                else
                {
                    List<Person> librisList = (List<Person>)Session["LibrisPersons"];
                    Random random = new Random();
                    librisPerson = librisList[random.Next(0, librisList.Count)];
                    librisList.Remove(librisPerson);
                    raaPerson = (Person)Session["raa"];
                    if (librisList.Count > 0)
                    {
                        Session["LibrisPersons"] = librisList;
                    }
                    else
                    {
                        Session["LibrisPersons"] = null;
                    }
                }
                Session["LibrisPerson"] = librisPerson;
                Session["raa"] = raaPerson;
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
        if (raaPerson.WikipediaLink != null)
        {
            wiki.NavigateUrl = raaPerson.WikipediaLink;
            wiki.Text = "Wikipedia";
            wiki.Target = "_blank";
        }
        else
        {
            wiki.Text = "";
        }
        if (raaPerson.GetImageURL() != "?")
        {
            raaImage.ImageUrl = raaPerson.GetImageURL();
        }
        else
        {
            raaImage.ImageUrl = "~/Images/QuestionMark.JPG";
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
    /// a positive vote is added
    /// </summary>
    /// <param name="sender">The object that triggerd the listener</param>
    /// <param name="e">Event Arguments</param>
    protected void btnYes_Click(object sender, EventArgs e)
    {
        try
        {
            librisPerson = (Person)Session["LibrisPerson"];
            raaPerson = (Person)Session["raa"];
            backEnd.AddPositiveVote(raaPerson, librisPerson);
            Session["LibrisPerson"] = null;
            Session["raa"] = null;
            Response.Redirect("~/comparePage.aspx");
        }
        catch (MySqlException)
        {
            Response.Redirect("~/ErrorPages/sql.aspx");
        }
    }
    /// <summary>
    /// a negative vote is added
    /// </summary>
    /// <param name="sender">The object that triggerd the listener</param>
    /// <param name="e">Event Arguments</param>
    protected void btnNo_Click(object sender, EventArgs e)
    {
        try
        {
            raaPerson = (Person)Session["raa"];
            librisPerson = (Person)Session["LibrisPerson"];
            backEnd.AddNegativeVote(raaPerson, librisPerson);
            Response.Redirect("~/comparePage.aspx");
        }
        catch (MySqlException)
        {
            Response.Redirect("~/ErrorPages/sql.aspx");
        }
    }
    /// <summary>
    /// The page is reloded and a new person is displayed
    /// </summary>
    /// <param name="sender">The object that triggerd the listener</param>
    /// <param name="e">Event Arguments</param>
    protected void skipp_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/comparePage.aspx");
    }
}
