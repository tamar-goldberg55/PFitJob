using System.Collections.Generic;

namespace Service.Dto
{
    public class EmployerOffersDto
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        public int TotalOffers { get; set; }
        public List<OfferDetailDto> Offers { get; set; }
    }

    public class OfferDetailDto
    {
        public int MatchId { get; set; }
        public string CandidateName { get; set; }
        public string CandidateCity { get; set; }
        public double MatchScore { get; set; }
        public string Status { get; set; }
        public bool IsSelectedByAlgorithm { get; set; }
    }
}
