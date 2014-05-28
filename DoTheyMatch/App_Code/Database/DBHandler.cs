using System;
using System.Collections.Generic;
using System.Web.Configuration;
using MySql.Data.MySqlClient;



namespace Database
{
    /// <summary>
    /// Handles the communication with the database
    /// </summary>
    public class DBHandler : IDisposable
    {
        private readonly MySqlConnection mySqlConnection;
        private readonly String connectionString = WebConfigurationManager.ConnectionStrings["DB"].ConnectionString;

        public DBHandler()
        {
            mySqlConnection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// checks if a person is linked with libris or if it's unlikable
        /// </summary>
        /// <param name="raaId">unique URI from Riksantikvarieämbetet</param>
        /// <returns>true if person is linked</returns>
        public bool IsPersonLinked(String raaId)
        {
            bool isInList = false;
            const string sql = "select contentid from content  where objecturi = @id";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            command.Parameters.AddWithValue("@id", raaId);
            try
            {
                mySqlConnection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isInList = true;
                }
                reader.Close();
            }
            finally
            {
                mySqlConnection.Close();
            }
            if (!isInList)
            {
                isInList = IsUnlinkAble(raaId);
            }

            return isInList;
        }

        /// <summary>
        /// Adds a positive vote for a connection
        /// </summary>
        /// <param name="raaPerson">a Person object from Riksantikvarieämbetet</param>
        /// <param name="librisPerson">a Person object from Libris</param>
        public void AddPositiveVote(Person raaPerson, Person librisPerson)
        {
            if (!CheckMatchForVoting(raaPerson, librisPerson))
            {
                InsertMatch(raaPerson, librisPerson);
            }

            const string sql =
                "UPDATE votecount SET PositiveVotes = PositiveVotes + 1 WHERE RAAId = @raaId and LibrisId = @LibrisId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };

            command.Parameters.AddWithValue("@raaId", raaPerson.URI);
            command.Parameters.AddWithValue("@LibrisId", librisPerson.URI);
            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        /// <summary>
        /// checks if a couple of URIs has bean handled before
        /// </summary>
        /// <param name="raaPerson">a Person object from Riksantikvarieämbetet</param>
        /// <param name="librisPerson">a Person object from Libris</param>
        /// <returns>true if a match is found</returns>
        private bool CheckMatchForVoting(Person raaPerson, Person librisPerson)
        {
            bool exists = false;
            const string sql = "SELECT * FROM votecount WHERE RAAId = @raaId && LibrisId = @LibrisId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };

            command.Parameters.AddWithValue("@raaId", raaPerson.URI);
            command.Parameters.AddWithValue("@LibrisId", librisPerson.URI);

            try
            {
                mySqlConnection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    exists = true;
                }
                reader.Close();
            }
            finally
            {
                mySqlConnection.Close();
            }
            return exists;
        }

        /// <summary>
        /// Adds a connection between two URIs for further voting
        /// </summary>
        /// <param name="raaPerson">a Person object from Riksantikvarieämbetet</param>
        /// <param name="librisPerson">a Person object from Libris</param>
        private void InsertMatch(Person raaPerson, Person librisPerson)
        {
            const string sql = "INSERT into votecount (RAAId, LibrisId) VALUES (@raaId, @LibrisId)";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };

            command.Parameters.AddWithValue("@raaId", raaPerson.URI);
            command.Parameters.AddWithValue("@LibrisId", librisPerson.URI);
            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        /// <summary>
        /// Adds a negative vote for a connection
        /// </summary>
        /// <param name="raaPerson">a Person object from Riksantikvarieämbetet</param>
        /// <param name="librisPerson">a Person object from Libris</param>
        public void AddNegativeVote(Person raaPerson, Person librisPerson)
        {
            if (!CheckMatchForVoting(raaPerson, librisPerson))
            {
                InsertMatch(raaPerson, librisPerson);
            }

            const string sql =
                "UPDATE votecount SET NegativeVotes = NegativeVotes + 1 WHERE RAAId = @raaId and LibrisId = @LibrisId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };

            command.Parameters.AddWithValue("@raaId", raaPerson.URI);
            command.Parameters.AddWithValue("@LibrisId", librisPerson.URI);
            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        /// <summary>
        /// Adds a vote for a couple of URIs as not the same
        /// </summary>
        /// <param name="raaPerson">a Person object from Riksantikvarieämbetet</param>
        /// <param name="librisPerson">a Person object from Libris</param>
        public void ReportMisstake(Person raaPerson, Person librisPerson)
        {
            if (IsLinkReported(raaPerson.URI))
            {
                AddToMisstakeCount(raaPerson.URI);
            }
            else
            {
                InsertIntoReportedfaults(raaPerson.URI, librisPerson.URI);
            }
        }
        /// <summary>
        /// Adds the URIs to the database
        /// </summary>
        /// <param name="raaId">a URI from Riksantikvarieämbetet</param>
        /// <param name="librisId">a URI from Libris</param>
        private void InsertIntoReportedfaults(String raaId, String librisId)
        {
            const string sql = "insert into reportedfaults set RAAId = @RAAId, LibrisId = @LibrisId, ReportedTimes = 1";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            command.Parameters.AddWithValue("@RAAId", raaId);
            command.Parameters.AddWithValue("@LibrisId", librisId);
            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        /// <summary>
        /// Adds a vote. called if the URIs is already in the database
        /// </summary>
        /// <param name="raaId">URI from Riksantikvarieämbetet </param>
        private void AddToMisstakeCount(String raaId)
        {
            const string sql = "UPDATE reportedfaults SET ReportedTimes = ReportedTimes + 1 WHERE RAAId = @raaId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };

            command.Parameters.AddWithValue("@raaId", raaId);
            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        private bool IsLinkReported(String raaId)
        {
            bool isInList = false;
            const string sql = "select LibrisId from reportedfaults where RAAId = @id";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            command.Parameters.AddWithValue("@id", raaId);
            try
            {
                mySqlConnection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isInList = true;
                }
                reader.Close();
            }
            catch (MySqlException)
            {
                isInList = false;
            }
            finally
            {
                mySqlConnection.Close();
            }
            return isInList;
        }

        /// <summary>
        /// Gets the vote count for a pair of URIs
        /// </summary>
        /// <param name="raaPerson">a Person object from Riksantikvarieämbetet</param>
        /// <param name="librisPerson">a Person object from Libris</param>
        /// <returns>the nr of votes in a VOTE object</returns>
        public Votes GetVotes(Person raaPerson, Person librisPerson)
        {
            Votes vote = new Votes();
            const string sql =
                "select PositiveVotes, NegativeVotes from votecount  where RAAId = @raaId and LibrisId = @librisId ";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            command.Parameters.AddWithValue("@raaId", raaPerson.URI);
            command.Parameters.AddWithValue("@librisId", librisPerson.URI);
            try
            {
                mySqlConnection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    vote = new Votes(reader.GetInt32(0), reader.GetInt32(1));
                }
                reader.Close();
            }
            catch (MySqlException)
            {
                vote = new Votes();
            }
            finally
            {
                mySqlConnection.Close();
            }
            return vote;
        }

        /// <summary>
        /// Gets the total count of made links
        /// </summary>
        /// <returns>Nr of connected URIs</returns>
        public int GetNrOfLinkedPersons()
        {
            int count = 0;
            const string sql = "select count(*) from content";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            try
            {
                mySqlConnection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
                reader.Close();
            }
            catch (MySqlException)
            {
                count = 0;
            }
            finally
            {
                mySqlConnection.Close();
            }
            return count;
        }

        /// <summary>
        /// gets the count of persons that has no matches in Libris
        /// </summary>
        /// <returns>Nr of unlikable</returns>
        public int GetCountOfUnlinkable()
        {
            int count = 0;
            const string sql = "select count(*) from nomatches";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            try
            {
                mySqlConnection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
                reader.Close();
            }
            catch (MySqlException)
            {
                count = 0;
            }
            finally
            {
                mySqlConnection.Close();
            }
            return count;
        }

        /// <summary>
        /// Get a list of all linked entities 
        /// </summary>
        /// <returns>a List of persons containing libris and RAA URIs</returns>
        public List<Person> GetListOfLinkedPersons()
        {
            List<Person> list = new List<Person>();
            const string sql = "select objecturi, relatedto from content";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            try
            {
                mySqlConnection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Person person = new Person { URI = reader.GetString(0) };
                    Person samePerson = new Person { URI = reader.GetString(1) };
                    person.SameAs = samePerson;
                    list.Add(person);
                }
                reader.Close();
            }
            catch (MySqlException)
            {
                list = null;
            }
            finally
            {
                mySqlConnection.Close();
            }
            return list;
        }

        /// <summary>
        /// Inserts two URIs in the database as a triplet, whith the predicate SameAs
        /// </summary>
        /// <param name="raaPerson">a Person object from Riksantikvarieämbetet</param>
        /// <param name="librisPerson">a Person object from Libris</param>
        public void CreateLink(Person raaPerson, Person librisPerson)
        {
            const string sql =
                "insert into content set objecturi = @URI, createdate  = @createdate, username = @username, applicationid = @applicationid , relationtype = 'sameAs', relatedto = @relatedto, updatedate = @createdate";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            command.Parameters.AddWithValue("@RAAId", raaPerson.URI);
            command.Parameters.AddWithValue("@URI", raaPerson.URI);
            command.Parameters.AddWithValue("@createdate", DateTime.Now);
            command.Parameters.AddWithValue("@username", "UUStudenter");
            command.Parameters.AddWithValue("@applicationid", 1212);
            command.Parameters.AddWithValue("@relatedto", librisPerson.URI);
            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
                RemoveVotes(raaPerson.URI, librisPerson.URI);
            }
        }

        /// <summary>
        /// Adds two URIs as not the same in the database
        /// </summary>
        /// <param name="raaId">a URI from Riksantikvarieämbetet</param>
        /// <param name="librisId">a URI from Libris</param>
        public void CreateNotTheSameLink(String raaId, String librisId)
        {
            const string sql = "insert into notcorrect set RaaId = @RAAID, LibrisId = @LibrisId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            command.Parameters.AddWithValue("@RAAId", raaId);
            command.Parameters.AddWithValue("@LibrisId", librisId);
            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        /// <summary>
        /// Adds a URI from Riksantikvarieämbetet as unlinkable
        /// </summary>
        /// <param name="raaId">URI from Riksantikvarieämbetet</param>
        public void AddToUnlinkable(String raaId)
        {
            const string sql = "insert into nomatches set RaaId = @RAAId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            command.Parameters.AddWithValue("@RAAId", raaId);

            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
            }
        }



        /// <summary>
        /// check if a URI is in the list of unlinkable
        /// </summary>
        /// <param name="raaURI">URI from Riksantikvarieämbetet</param>
        /// <returns>true if a match is found</returns>
        private bool IsUnlinkAble(String raaURI)
        {
            bool isInList = false;
            const string sql = "select count(RaaId) from nomatches where RaaId = @RaaId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            command.Parameters.AddWithValue("@raaId", raaURI);
            try
            {
                mySqlConnection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader.GetInt32(0) > 0)
                {
                    isInList = true;
                }
            }
            finally
            {
                mySqlConnection.Close();
            }
            return isInList;
        }

        /// <summary>
        /// get count of reported faults for a SameAs connection
        /// </summary>
        /// <param name="raaId">URI from Riksantikvarieämbetet</param>
        /// <returns>the nr of times the link has been reported</returns>
        public int GetCountOfErrorReports(String raaId)
        {
            int nr = 0;
            const string sql = "select ReportedTimes from reportedfaults where RaaId = @RaaId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            command.Parameters.AddWithValue("@raaId", raaId);
            try
            {
                mySqlConnection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    nr = reader.GetInt32(0);
                }
            }
            finally
            {
                mySqlConnection.Close();
            }
            return nr;
        }

        /// <summary>
        /// Removes a SameAs tripplet
        /// </summary>
        /// <param name="raaId">URI from Riksantikvarieämbetet</param>
        public void RemoveTripplet(String raaId)
        {
            const string sql = "delete from content WHERE objecturi = @raaId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            command.Parameters.AddWithValue("@raaId", raaId);
            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        /// <summary>
        /// Removes the votes made for a pair of URIs. should be called after a SameAs link is created
        /// </summary>
        /// <param name="raaUri">URI from Riksantikvarieämbetet</param>
        /// <param name="librisUri">URI from Libris</param>
        public void RemoveVotes(String raaUri, String librisUri)
        {
            const string sql = "delete from votecount WHERE RAAId = @raaId and LibrisId = @librisId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };

            command.Parameters.AddWithValue("@raaId", raaUri);
            command.Parameters.AddWithValue("@librisId", librisUri);
            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        /// <summary>
        /// Removes the two URIs from the table of reported Erros
        /// </summary>
        /// <param name="raaUri">URI from Riksantikvarieämbetet</param>
        /// <param name="librisUri">URI from Libris</param>
        public void RemoveReportedfaults(String raaUri, String librisUri)
        {
            const string sql = "delete from reportedfaults WHERE RAAId = @raaId and LibrisId = @librisId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };

            command.Parameters.AddWithValue("@raaId", raaUri);
            command.Parameters.AddWithValue("@librisId", librisUri);
            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
            }
        }
        /// <summary>
        /// Gets the list of URIs that are listed as not the same
        /// </summary>
        /// <returns>A list of person objects with only URI and sameAs properties</returns>
        public List<Person> GetListOfNotTheSamePersons()
        {
            List<Person> list = new List<Person>();
            const string sql = "SELECT RaaId,LibrisId FROM notcorrect";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            try
            {
                mySqlConnection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Person p = new Person { URI = reader.GetString(0), SameAs = new Person { URI = reader.GetString(1) } };
                    list.Add(p);
                }
                reader.Close();
            }
            finally
            {
                mySqlConnection.Close();
            }
            return list;
        }
        /// <summary>
        /// gets the total nr of times a not the same link has been reported
        /// </summary>
        /// <param name="raaId">URI from Riksantikvarieämbetet</param>
        /// <param name="librisId">URI from Libris</param>
        /// <returns>amount of votes</returns>
        public int GetCountOfErrorReportsNotSame(String raaId, String librisId)
        {
            int nr = 0;
            const string sql = "select NegativeVotes from notCorrect where RaaId = @RaaId and LibrisId = @LibrisId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            command.Parameters.AddWithValue("@raaId", raaId);
            command.Parameters.AddWithValue("@LibrisId", librisId);
            try
            {
                mySqlConnection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    nr = reader.GetInt32(0);
                }
            }
            finally
            {
                mySqlConnection.Close();
            }
            return nr;
        }
        /// <summary>
        /// remove a link between two URTs that are not the same
        /// </summary>
        /// <param name="raaUri">URI from Riksantikvarieämbetet</param>
        /// <param name="librisUri">URI from Libris</param>
        public void RemoveNotTheSame(String raaUri, String librisUri)
        {
            const string sql = "delete from notCorrect WHERE RAAId = @raaId and LibrisId = @librisId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };

            command.Parameters.AddWithValue("@raaId", raaUri);
            command.Parameters.AddWithValue("@librisId", librisUri);
            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
            }
        }
        /// <summary>
        /// Adds a vote fore two URIs as not the same
        /// </summary>
        /// <param name="raaPerson">a Person object from Riksantikvarieämbetet</param>
        /// <param name="librisPerson">a Person object from Libris</param>
        public void AddReportNotSame(Person raaPerson, Person librisPerson)
        {
            if (!CheckMatchForVoting(raaPerson, librisPerson))
            {
                InsertMatch(raaPerson, librisPerson);
            }

            const string sql =
                "UPDATE notcorrect SET NegativeVotes = NegativeVotes + 1 WHERE RAAId = @raaId and LibrisId = @LibrisId";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };

            command.Parameters.AddWithValue("@raaId", raaPerson.URI);
            command.Parameters.AddWithValue("@LibrisId", librisPerson.URI);
            try
            {
                mySqlConnection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                mySqlConnection.Close();
            }
        }
        /// <summary>
        /// gets the total nr of pairs linked as not the same
        /// </summary>
        /// <returns>the count of not the same links</returns>
        public int GetCountOfNotSame()
        {
            int count = 0;
            const string sql = "select count(*) from notcorrect";
            MySqlCommand command = new MySqlCommand { Connection = mySqlConnection, CommandText = sql };
            try
            {
                mySqlConnection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
                reader.Close();
            }
            finally
            {
                mySqlConnection.Close();
            }
            return count;
        }

        public void Dispose()
        {
            mySqlConnection.Dispose();
        }
    }
}