using System;
using System.Collections.Generic;
using VDS.RDF.Query;

namespace SPARQL.Libris
{
    /// <summary>
    /// the class handles all communication  with the Libris-SPARQL endpoint
    /// </summary>
    public class LibrisSPARQLHandler
    {
        private const String URI = "http://libris.kb.se/sparql/";

        /// <summary>
        /// Gets a list of person objects where the name matches the param
        /// </summary>
        /// <param name="fullName">Name of the person</param>
        /// <returns>A list with all persons with the given name</returns>
        public List<Person> GetPersons(String fullName)
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(URI));
            SparqlResultSet results = endpoint.QueryWithResultSet(
                "PREFIX date: <http://dbpedia.org/property/> " +
                "PREFIX foaf: <http://xmlns.com/foaf/0.1/> " +
                "select ?a ?birth ?death where{" +
                    "?a foaf:name" + "'" + fullName + "'" + ". " +
                    "OPTIONAL {" +
                    "?a date:birthYear ?birth." +
                    "}OPTIONAL {" +
                    "?a date:deathYear ?death." +
                    "}}");

            List<Person> listOfPersons = new List<Person>();
            foreach (SparqlResult result in results)
            {
                Person person = new Person { Name = fullName };
                if (result.Value("a") != null)
                {
                    person.URI = result.Value("a").ToString();
                }
                if (result.Value("birth") != null)
                {
                    person.BirthYear = result.Value("birth").ToString();
                }
                if (result.Value("death") != null)
                {
                    person.DeathYear = result.Value("death").ToString();
                }

                person.AddListOfExternalEntitie(GetListOfBooksAbout(person.URI));
                person.AddListOfExternalEntitie(GetListOfBooksBy(person.URI));
                listOfPersons.Add(person);
            }
            return listOfPersons;
        }

        /// <summary>
        /// Gets all information avalible for the person with the given URI
        /// </summary>
        /// <param name="librisId">Unique URI from Libris</param>
        /// <returns>a Person object filld with data from Libris</returns>
        public Person GetPerson(String librisId)
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(URI));
            SparqlResultSet results = endpoint.QueryWithResultSet(
                " PREFIX foaf: <http://xmlns.com/foaf/0.1/> " +
                " PREFIX date: <http://dbpedia.org/property/> " +
                " PREFIX links: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> " +
                " select ?name ?birth ?death ?links where{<" +
                    librisId + "> foaf:name ?name." +
                    "OPTIONAL {<" +
                    librisId + "> date:birthYear ?birth." +
                    "}OPTIONAL {<" +
                     librisId + "> date:deathYear ?death." +
                    " }}");

            Person person = new Person();
            foreach (SparqlResult result in results)
            {
                if (result.Value("name") != null)
                {
                    person.Name = result.Value("name").ToString();
                }
                if (result.Value("birth") != null)
                {
                    person.BirthYear = result.Value("birth").ToString();
                }
                if (result.Value("death") != null)
                {
                    person.DeathYear = result.Value("death").ToString();
                }

            }
            person.URI = librisId;
            person.AddListOfExternalEntitie(GetListOfBooksAbout(librisId));
            person.AddListOfExternalEntitie(GetListOfBooksBy(librisId));
            return person;
        }

        /// <summary>
        /// Gets a list of book writen by a person
        /// </summary>
        /// <param name="librisId">URI for the writer</param>
        /// <returns>a list of books with a title and a link to Libris</returns>
        private List<Link> GetListOfBooksBy(String librisId)
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(URI));
            SparqlResultSet results = endpoint.QueryWithResultSet(
                "PREFIX by: <http://purl.org/dc/elements/1.1/>" +
                    "select ?book where{" +
                    "?book by:creator + <" + librisId +
                    ">}");

            List<String> bookURIs = new List<String>();
            List<Link> bookList = new List<Link>();
            foreach (SparqlResult result in results)
            {
                if (result.Value("book") != null)
                {
                    String bookURI = result.Value("book").ToString();
                    bookURIs.Add(bookURI);
                }
            }
            foreach (String bookURI in bookURIs)
            {
                Link book = GetBook(bookURI);
                book.MadeBy = true;
                bookList.Add(book);
            }
            return bookList;
        }

        /// <summary>
        /// Gets a list of book about a person
        /// </summary>
        /// <param name="librisId">URI for the person</param>
        /// <returns>a list of books with a title and a link to Libris</returns>
        private List<Link> GetListOfBooksAbout(String librisId)
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(URI));
            SparqlResultSet results = endpoint.QueryWithResultSet(
            "PREFIX by: <http://purl.org/dc/elements/1.1/>" +
            " select ?book where{" +
                "?book by:subject + <" + librisId +
                ">}");

            List<String> bookURIs = new List<String>();
            List<Link> bookList = new List<Link>();
            foreach (SparqlResult result in results)
            {
                if (result.Value("book") != null)
                {
                    String bookURI = result.Value("book").ToString();
                    bookURIs.Add(bookURI);
                }
            }
            foreach (String book in bookURIs)
            {
                bookList.Add(GetBook(book));
            }
            return bookList;
        }

        /// <summary>
        /// gets the title for a book
        /// </summary>
        /// <param name="bookUri">Unique URI for the book</param>
        /// <returns>A book object with title and URI</returns>
        private Link GetBook(String bookUri)
        {
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri(URI));
            SparqlResultSet results = endpoint.QueryWithResultSet(
              "PREFIX book: <http://purl.org/dc/elements/1.1/>" +
                 "select ?title{<" +
                  bookUri + "> book:title ?title.}");

            Link book = new Link();
            foreach (SparqlResult result in results)
            {
                if (result.Value("title") != null)
                {
                    book.Title = result.Value("title").ToString().Split('@')[0];
                    book.Uri = bookUri;
                }
            }
            return book;
        }
    }
}