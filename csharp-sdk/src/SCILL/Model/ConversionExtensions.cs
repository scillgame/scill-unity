using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SCILL.Client;

namespace SCILL.Model
{
    public static class ConversionExtensions
    {
        public static List<Leaderboard> ToLeaderboards(this IEnumerable<LeaderboardResults> input)
        {
            List<Leaderboard> result = new List<Leaderboard>();
            foreach (LeaderboardResults leaderboardResults in input)
            {
                result.Add(leaderboardResults.ToLeaderboard());
            }

            return result;
        }
        
        public static Leaderboard ToLeaderboard(this LeaderboardResults input)
        {
            Leaderboard result = new Leaderboard();
            result.leaderboard_id = input.leaderboard_id;
            result.name = input.leaderboard_name;

            if (null != input.leaderboard_results_by_member_type &&
                input.leaderboard_results_by_member_type.Count > 1)
            {
                var teamResults = input.leaderboard_results_by_member_type["team"];
                result.num_teams = teamResults.count;
                result.grouped_by_teams = teamResults.members.ToLeaderboardRankings();
                
                var userResults = input.leaderboard_results_by_member_type["user"];
                result.num_users = userResults.count;
                result.grouped_by_users = userResults.members.ToLeaderboardRankings();
            }
            else
            {
                throw new ApiException(400,
                    "Trying to convert a LeaderboardResults object with less than two entries for leaderboard_results_by_member_type into a Leaderboard object.");
            }

            return result;
        }

        public static List<LeaderboardRanking> ToLeaderboardRankings(this IEnumerable<LeaderboardMember> members)
        {
            List<LeaderboardRanking> rankings = new List<LeaderboardRanking>(members.Count());
            foreach (LeaderboardMember leaderboardMember in members)
            {
                rankings.Add(leaderboardMember.ToLeaderboardRanking());
            }

            return rankings;
        }

        public static LeaderboardRanking ToLeaderboardRanking(this LeaderboardMember member)
        {
            return new LeaderboardRanking(
                member.member_id, member.member_type, member.score, member.rank, member.additional_info
            );
        }
        
        
    }
}