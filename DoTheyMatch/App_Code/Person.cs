using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Represents a person, contains information about the person
/// </summary>
public class Person
{


    private readonly List<String> links = new List<string>();
    private readonly List<Link> externalEntities = new List<Link>();
    private String imageURL;

    public Person()
    {
        URI = "?";
        Name = "?";
        BirthYear = "?";
        DeathYear = "?";
        links.Add("?");
        imageURL = "?";
        externalEntities.Add(new Link());
    }
    /// <summary>
    /// generates a md5 hash based on the input
    /// </summary>
    /// <param name="input">Image filename</param>
    /// <returns>the file namn as a md5 hash</returns>
    private string CreateMD5Hash(string input)
    {
        MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("x2"));
        }
        return sb.ToString();
    }
    /// <summary>
    /// Adds a iamge url to the person if the image is a wikicommons link with file: 
    /// in the path an attempt is made to generate a working url
    /// </summary>
    /// <param name="url">url to the image</param>
    public void SetImageUrl(String url)
    {
        if (url.Contains("File:"))
        {
            String updatedWikiURL = "http://upload.wikimedia.org/wikipedia/commons/thumb/";
            String[] partialURL = url.Split(':');
            String filename = partialURL[2];
            String hash = CreateMD5Hash(filename);
            updatedWikiURL += (hash.ToCharArray()[0] + "/" + hash.ToCharArray()[0] + hash.ToCharArray()[1] + "/");
            updatedWikiURL += filename + "/200px-" + filename;
            url = updatedWikiURL;
        }
        imageURL = url;
    }
    /// <summary>
    /// Gets the Url for the image
    /// </summary>
    /// <returns></returns>
    public String GetImageURL()
    {
        return imageURL;
    }
    /// <summary>
    /// A person that is linked as SameAs this
    /// </summary>
    public Person SameAs { get; set; }

    /// <summary>
    /// unique URI for this person
    /// </summary>
    public string URI { get; set; }


    /// <summary>
    /// name for the person
    /// </summary>
    public string Name { get; set; }


    /// <summary>
    /// year or date of birth
    /// </summary>
    public string BirthYear { get; set; }


    /// <summary>
    /// year or date for death
    /// </summary>
    public string DeathYear { get; set; }


    /// <summary>
    /// Link to wikipedia
    /// </summary>
    public string WikipediaLink { get; set; }

    /// <summary>
    /// Adds a Link to the list
    /// </summary>
    /// <param name="link">External link</param>
    public void AddExternalEntitie(Link link)
    {
        if (externalEntities[0].Title == "?")
        {
            externalEntities.RemoveAt(0);
        }
        externalEntities.Add(link);
    }
    /// <summary>
    /// Gets a list of links
    /// </summary>
    /// <returns></returns>
    public List<Link> GetListOfExternalEntitie()
    {
        return externalEntities;
    }
    /// <summary>
    /// Add a list of links
    /// </summary>
    /// <param name="inEntities"></param>
    public void AddListOfExternalEntitie(List<Link> inEntities)
    {

        foreach (Link inEntity in inEntities)
        {
            AddExternalEntitie(inEntity);
        }
    }

}