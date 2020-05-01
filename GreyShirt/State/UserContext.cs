using System.Collections.Generic;

namespace Greyshirt.State
{
    public class UserContext
    {
        public List<Match> Matches { get; set; }

        public UserContext()
        {
            this.Matches = new List<Match>();
        }
    }

    public class Match
    {
        public string PhoneNumber { get; set; }
        public string MissionId { get; set; }
    }
}