
/// <summary>
/// represents positive and negative votes about a person
/// </summary>
public class Votes
{
    public int PositiveVotes { get; private set; }
    public int NegativeVotes { get; private set; }

    public Votes(int positiveVotes, int negativeVotes)
    {
        PositiveVotes = positiveVotes;
        NegativeVotes = negativeVotes;
    }

    public Votes() { }
    /// <summary>
    /// Gets the total nr of votes
    /// </summary>
    /// <returns>a count of all votes</returns>
    public int GetNrOfVotes()
    {
        return PositiveVotes + NegativeVotes;
    }
}