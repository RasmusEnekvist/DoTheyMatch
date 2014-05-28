using System;
using System.Collections.Generic;
using System.Web;
using Database;
using SPARQL.Libris;
using SPARQL.RAA;


/// <summary>
/// Handles all communication between the UI and the logic
/// </summary>
public class BackEnd
{
    private readonly DBHandler db;
    private readonly RAASPARQLHandler raasparql;
    private readonly LibrisSPARQLHandler librisSparql;

    public BackEnd()
    {
        db = new DBHandler();
        raasparql = new RAASPARQLHandler();
        librisSparql = new LibrisSPARQLHandler();
    }
    /// <summary>
    /// Get the percentage of persons that’s been linked/handled
    /// </summary>
    /// <returns>an int representation as percentage</returns>
    public int GetProcentageOfLinkedPersons()
    {
        float linked = db.GetCountOfUnlinkable();
        linked += db.GetNrOfLinkedPersons();
        linked += raasparql.GetNrOfLinkedPersons();
        linked += db.GetCountOfNotSame();
        float total = raasparql.GetNrOfPersons();
        float sum = (linked / total) * 100;
        return (int)sum;
    }
    /// <summary>
    /// Gets a random person from Riksantikvarieämbetet
    /// </summary>
    /// <returns>An architect as a person object</returns>
    public Person GetRaaPerson()
    {
        Random random = new Random(DateTime.Now.Millisecond);
        Person toReturn = new Person();
        bool found = false;
        while (!found)
        {
            int total = raasparql.GetNrOfPersons() + 1;
            int randoms = random.Next(0, total);

            toReturn.URI = raasparql.GetRandomPersonId(randoms);
            if (!db.IsPersonLinked(toReturn.URI))
            {
                found = true;
                toReturn = raasparql.GetPersonByURI(toReturn.URI);
            }
        }
        return toReturn;
    }
    /// <summary>
    /// Returns a person with the same name as param, if more than one i found the rest is stored in Sesson LibrisPerson 
    /// </summary>
    /// <param name="name">Name of the person</param>
    ///  <param name="raaUri">Unique URI from Riksantikvarieämbetet</param>
    /// <returns>A person object with data from Libris</returns>
    public Person GetLibrisPerson(String name, String raaUri)
    {
        Random random = new Random();
        Person toReturn;
        List<Person> persons = librisSparql.GetPersons(name);
        if (persons.Count > 1)
        {
            int randomIndex = random.Next(0, persons.Count);
            toReturn = persons[randomIndex];
            persons.RemoveAt(randomIndex);
            HttpContext.Current.Session["LibrisPersons"] = persons;
        }
        else if (persons.Count == 0)
        {
            toReturn = null;
            db.AddToUnlinkable(raaUri);
        }
        else
        {
            toReturn = persons[0];
        }


        return toReturn;
    }

    /// <summary>
    /// Gets a list of all persons that are linked
    /// </summary>
    /// <returns>A list of person objects</returns>
    public List<Person> GetListOfLinkedPersons()
    {
        List<Person> list = db.GetListOfLinkedPersons();
        List<Person> updatedList = new List<Person>();
        foreach (Person p in list)
        {
            Person updatedRaaPerson = raasparql.GetPersonByURI(p.URI);
            updatedRaaPerson.SameAs = p.SameAs;
            updatedList.Add(updatedRaaPerson);
        }
        return updatedList;
    }
    /// <summary>
    /// Gets a list of persons that are linked as not the same
    /// </summary>
    /// <returns>A list of person objects</returns>
    public List<Person> GetListOfNotTheSame()
    {
        List<Person> list = db.GetListOfNotTheSamePersons();
        List<Person> updatedList = new List<Person>();
        foreach (Person person in list)
        {
            Person updated = raasparql.GetPersonByURI(person.URI);
            updated.SameAs = person.SameAs;
            updatedList.Add(updated);
        }
        return updatedList;
    }


    /// <summary>
    /// Adds a positive vote for a couple of URIs as same if there's been enough votes a triplet is created
    /// </summary>
    /// <param name="raaPerson">URI from Riksantikvarieämbetet</param>
    /// <param name="librisPerson">URI from Libris</param>
    public void AddPositiveVote(Person raaPerson, Person librisPerson)
    {
        Votes votes = db.GetVotes(raaPerson, librisPerson);

        if (votes.NegativeVotes == 0)
        {
            if (votes.PositiveVotes == 3)
            {
                db.CreateLink(raaPerson, librisPerson);
                db.RemoveVotes(raaPerson.URI, librisPerson.URI);
            }
            else
            {
                db.AddPositiveVote(raaPerson, librisPerson);
            }
        }
        else
        {
            if ((double)votes.PositiveVotes / votes.GetNrOfVotes() >= 0.75 && votes.GetNrOfVotes() > 3)
            {
                db.CreateLink(raaPerson, librisPerson);
                db.RemoveVotes(raaPerson.URI, librisPerson.URI);
            }
            else
            {
                db.AddPositiveVote(raaPerson, librisPerson);
            }
        }
    }

    /// <summary>
    /// Adds a negative vote for a couple of URIs, if there's been enough negative votes a not the same link is created
    /// </summary>
    /// <param name="raaPerson">URI from Riksantikvarieämbetet</param>
    /// <param name="librisPerson">URI from Libris</param>
    public void AddNegativeVote(Person raaPerson, Person librisPerson)
    {
        Votes votes = db.GetVotes(raaPerson, librisPerson);

        if (votes.PositiveVotes == 0)
        {
            if (votes.NegativeVotes == 3)
            {
                db.CreateNotTheSameLink(raaPerson.URI, librisPerson.URI);
                db.RemoveVotes(raaPerson.URI, librisPerson.URI);
            }
            else
            {
                db.AddNegativeVote(raaPerson, librisPerson);
            }
        }
        else
        {
            if ((double)votes.NegativeVotes / votes.GetNrOfVotes() >= 0.75 && votes.GetNrOfVotes() > 3)
            {
                db.CreateNotTheSameLink(raaPerson.URI, librisPerson.URI);
                db.RemoveVotes(raaPerson.URI, librisPerson.URI);
            }
            else
            {
                db.AddNegativeVote(raaPerson, librisPerson);
            }
        }
    }

    /// <summary>
    /// Gets a person from Riksantikvarieämbetet based on a URI
    /// </summary>
    /// <param name="raaId">URI for the person</param>
    /// <returns>The person as a Person object</returns>
    public Person GetRaaPersonById(String raaId)
    {
        return raasparql.GetPersonByURI(raaId);
    }
    /// <summary>
    /// Gets a person from Libris based on a URI
    /// </summary>
    /// <param name="librisId">URI for the person</param>
    /// <returns>The person as a Person object</returns>
    public Person GetLibrisPersonById(String librisId)
    {
        return librisSparql.GetPerson(librisId);
    }
    /// <summary>
    /// Adds a vote for a SameAs link being faulty
    /// </summary>
    /// <param name="raaPerson"></param>
    /// <param name="librisPerson"></param>
    public void ReportFault(Person raaPerson, Person librisPerson)
    {
        if (db.GetCountOfErrorReports(raaPerson.URI) >= 4)
        {
            db.RemoveTripplet(raaPerson.URI);
            db.RemoveReportedfaults(raaPerson.URI, librisPerson.URI);
        }
        else
        {
            db.ReportMisstake(raaPerson, librisPerson);
        }


    }
    /// <summary>
    /// Adds a vote for a nottheSameAs link being faulty
    /// </summary>
    /// <param name="raaPerson"></param>
    /// <param name="librisPerson"></param>
    public void ReportFaultNotSame(Person raaPerson, Person librisPerson)
    {
        if (db.GetCountOfErrorReportsNotSame(raaPerson.URI, librisPerson.URI) >= 4)
        {
            db.RemoveNotTheSame(raaPerson.URI, librisPerson.URI);
        }
        else
        {
            db.AddReportNotSame(raaPerson, librisPerson);
        }

    }
}