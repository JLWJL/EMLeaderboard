namespace EMLeaderboard.Models;

public class CustomerScoreRank
{
    public long CustomerId { get; set; }
    public decimal Score { get; set; }
    public int Rank { get; set; }
}