using System;

namespace Service.Dto
{
    public class JobSuggestionDto
    {
        public int JobId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal? Payment { get; set; }
        public bool IsRemote { get; set; }
        public bool IsJobWithPeople { get; set; }
        public string LevelLabel { get; set; }
        public double MatchScore { get; set; }
        public DateTime RequiredDate { get; set; }
        public int EmployerId { get; set; }
    }
}
