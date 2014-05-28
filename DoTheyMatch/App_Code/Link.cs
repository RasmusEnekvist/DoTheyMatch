/// <summary>
/// Link is used to display hyperlinks connected to a person. could be books or houses
/// </summary>
public class Link
{
    public Link()
    {
        Title = "?";
        Uri = "?";
        MadeBy = false;
    }
    /// <summary>
    /// Webb url for the hyperlink
    /// </summary>
    public string Uri { get; set; }
    /// <summary>
    /// Titel for the link
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// if this entity is made by the person
    /// </summary>
    public bool MadeBy { get; set; }


}