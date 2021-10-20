using System;
using System.Runtime.Serialization;

namespace SCILL.Model
{
   
    /// <summary>
    /// The Leaderboard object contains information about the leaderboard itself like the name and the id
    /// </summary>
    public partial class LeaderboardMemberRanking
    {
        /// <summary>
        /// The name of the leaderboard
        /// </summary>
        [Obsolete("Use leaderboard_name instead.")]
        public string name => leaderboard_name;

        /// <summary>
        /// Returns the LeaderboardRanking object containing user ranking information.
        /// </summary>
        [Obsolete("Use leaderboard_member instead.")]
        public LeaderboardRanking member
        {
            get
            {
                if(null != leaderboard_member)
                    return leaderboard_member.ToLeaderboardRanking();
                return null;
            }
        }
    }
    
}