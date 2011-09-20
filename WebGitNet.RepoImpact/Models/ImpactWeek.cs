namespace WebGitNet.Models
{
    using System;
    using System.Collections.Generic;

    public class ImpactWeek
    {
        public DateTime Week { get; set; }

        public List<UserImpact> Impacts { get; set; }
    }
}
