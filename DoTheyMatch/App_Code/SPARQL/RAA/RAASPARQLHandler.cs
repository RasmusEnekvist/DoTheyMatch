using System;
using System.Collections.Generic;
using VDS.RDF.Query;

namespace SPARQL.RAA
{
    /// <summary>
    /// the class handles all communication  with Riksantikvarieämbetets SPARQL-server
    /// </summary>
    public class RAASPARQLHandler
    {
        private const String URI = "http://sparqltest.raa.se/ksamsok/sparql";
        /// <summary>
        /// Gets the total nr of architects from Riksantikvarieämbetet
        /// </summary>
        /// <returns>The total count of architects</returns>
        public int GetNrOfPersons()
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(URI));
            SparqlResultSet results = endpoint.QueryWithResultSet(
                "PREFIX foaf: <http://xmlns.com/foaf/0.1/> prefix ksamsok: <http://kulturarvsdata.se/ksamsok#> prefix type: <http://kulturarvsdata.se/resurser/EntityType#> select (COUNT(DISTINCT ?a) AS ?count) where{ ?a foaf:fullName ?name. ?a ?s type:person. ?f ksamsok:architect ?a. MINUS{ ?a foaf:fullName 'Okänd'}}");

            int count = 0;
            foreach (SparqlResult result in results)
            {
                if (result.Value("count") != null)
                {
                    string s = result.Value("count").ToString();
                    String[] s1 = s.Split('^');
                    count = int.Parse(s1[0].Trim());
                }
            }

            return count;
        }



        /// <summary>
        /// Adds birth and death year for a person if it's available 
        /// </summary>
        /// <param name="inPerson">the person object</param>
        /// <returns>same person object as sent in with added information</returns>
        private Person GetBirthAndDeathYear(Person inPerson)
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(URI));
            SparqlResultSet results = endpoint.QueryWithResultSet(
                "PREFIX ksamsok: <http://kulturarvsdata.se/ksamsok#>" +
                "select ?year where{" +
                    "<" + inPerson.URI + "> ksamsok:context ?b." +
                    "?b ksamsok:toTime ?year." +
                    "}limit 2");

            if (results.Count == 2)
            {
                string year1 = results[0].Value("year").ToString();
                string year2 = results[1].Value("year").ToString();

                if (int.Parse(year1) > int.Parse(year2))
                {
                    inPerson.BirthYear = year2;
                    inPerson.DeathYear = year1;
                }
                else
                {
                    inPerson.BirthYear = year1;
                    inPerson.DeathYear = year2;
                }
            }
            return inPerson;
        }

        /// <summary>
        /// Gets information about a person given the URI
        /// </summary>
        /// <param name="uri">The Unique URI for a person</param>
        /// <returns>a person object with data added</returns>
        public Person GetPersonByURI(String uri)
        {
            Person person = new Person();
            //Endast för test Gunnar Asplund  person.URI = "http://kulturarvsdata.se/raa/bbrp/21600000003542";
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(URI));
            SparqlResultSet results = endpoint.QueryWithResultSet(
                "prefix ksamsok: <http://kulturarvsdata.se/ksamsok#>" +
                "prefix wiki: <http://kulturarvsdata.se/ugc#>" +
                "PREFIX foaf:   <http://xmlns.com/foaf/0.1/>" +
                "select  ?name ?wikiLink ?image ?born ?death" +
                "where{" +
                "<" + uri + "> foaf:fullName ?name." +
                "optional{<" + uri + "> wiki:sameAsWikipedia ?wikiLink}" +
                "optional{<" + uri + "> ksamsok:isVisualizedBy ?image}" +
                "}");
            person.URI = uri;

            foreach (SparqlResult result in results)
            {
                if (result.Value("name") != null)
                {
                    person.Name = result.Value("name").ToString();
                }
                if (result.Value("wikiLink") != null)
                {
                    person.WikipediaLink = result.Value("wikiLink").ToString();
                }
                if (result.Value("image") != null)
                {
                    person.SetImageUrl(result.Value("image").ToString());
                }
                person = GetBirthAndDeathYear(person);
            }
            person.AddListOfExternalEntitie(GetListOfHouses(person.URI));

            return person;
        }

        /// <summary>
        /// Gets a random URI based of the offset parameter.
        /// </summary>
        /// <param name="offset">should be a random nr between 0 and total count</param>
        /// <returns>URI for an architect</returns>
        public String GetRandomPersonId(int offset)
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(URI));
            SparqlResultSet results = endpoint.QueryWithResultSet(
                "prefix type: <http://kulturarvsdata.se/resurser/EntityType#> prefix ksamsok: <http://kulturarvsdata.se/ksamsok#> select distinct ?uri where{?f ksamsok:architect ?uri. ?uri ?s type:person. FILTER(?uri != <http://kulturarvsdata.se/raa/bbrp/-1>)} limit 1  offset " +
                offset);

            String uri = "";
            foreach (SparqlResult result in results)
            {
                if (result.Value("uri") != null)
                {
                    uri = result.Value("uri").ToString();
                }
            }
            return uri;
        }

        /// <summary>
        /// Gets the count of persons already linked to Libris
        /// </summary>
        /// <returns>count of already made links</returns>
        public int GetNrOfLinkedPersons()
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(URI));
            SparqlResultSet results = endpoint.QueryWithResultSet(
                "PREFIX foaf: <http://xmlns.com/foaf/0.1/>" +
                "prefix owl:  <http://www.w3.org/2002/07/owl#>" +
                "prefix ksamsok: <http://kulturarvsdata.se/ksamsok#>" +
                "select (COUNT(DISTINCT ?a) AS ?count)where{" +
                "?a foaf:fullName ?name." +
                "?f ksamsok:architect ?a." +
                "?a owl:sameAs ?link." +
                "FILTER regex(str(?link), '^http://libris.kb.se/resource/auth/') . " +
                "MINUS{ ?a foaf:fullName 'Okänd'}" +
                "}");

            int count = 0;
            foreach (SparqlResult result in results)
            {
                if (result.Value("count") != null)
                {
                    string s = result.Value("count").ToString();
                    String[] s1 = s.Split('^');
                    count = int.Parse(s1[0].Trim());
                }
            }
            return count;
        }

        /// <summary>
        /// Gets a list of houses for a given architect
        /// </summary>
        /// <param name="raaId">URI for the architect</param>
        /// <returns>a list of links objects</returns>
        private List<Link> GetListOfHouses(String raaId)
        {
            List<Link> links = new List<Link>();
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(URI));
            SparqlResultSet results = endpoint.QueryWithResultSet(
                "prefix ksamsok: <http://kulturarvsdata.se/ksamsok#>" +
                "prefix pres: <http://kulturarvsdata.se/presentation#>" +
                "select distinct ?uri where{" +
                "?f ksamsok:architect <" + raaId + ">." +
                "?uri ksamsok:context ?f.}");
            foreach (SparqlResult result in results)
            {
                if (result.HasValue("uri"))
                {
                    links.Add(GetHouse(result.Value("uri").ToString()));
                }
            }
            return links;
        }

        /// <summary>
        /// Get the label and hyperlink for a given house
        /// </summary>
        /// <param name="uri">URI for a house</param>
        /// <returns>house as a Link object</returns>
        private Link GetHouse(String uri)
        {
            Link house = new Link();
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(URI));
            SparqlResultSet results = endpoint.QueryWithResultSet(
                "prefix ksamsok: <http://kulturarvsdata.se/ksamsok#>" +
                "select distinct ?name ?url where{ " +
                "<" + uri + "> ksamsok:itemLabel ?name." +
                "<" + uri + "> ksamsok:url ?url." +
                "}limit 1");
            foreach (SparqlResult result in results)
            {
                if (result.HasValue("url"))
                {
                    house.Uri = result.Value("url").ToString();
                }
                if (result.HasValue("name"))
                {
                    house.Title = result.Value("name").ToString();
                }
                return house;
            }
            return new Link();
        }
    }
}