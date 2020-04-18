using Microsoft.Azure.Cosmos;
using System.Collections.Generic;

namespace Bot.State
{
    public class UserContext
    {
        public string Category { get; set; }
        public string Resource { get; set; }

        public int ProvideQuantity { get; set; }
        public List<Match> ProvideMatches { get; set; }

        public int NeedQuantity { get; set; }
        public bool NeedUnopenedOnly { get; set; }

        public int TimezoneOffset { get; set; }

        public UserContext()
        {
            this.ProvideMatches = new List<Match>();
        }
    }

    public class Match
    {
        public string OrgPhoneNumber { get; set; }
        public string NeedId { get; set; }
    }
}