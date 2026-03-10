using Service.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IMatch : IService<MatchDto>
    {
        Task<double> CalculateMatchScore(int candidateId, int jobId);//האלגוריתמים שמשווה בין המשרה לבין המועמד 

        Task<List<MatchDto>> RunMatchingAlgorithm(int jobId); //– פונקציה שרצה על כל המועמדים ומעדכנת את טבלת ההתאמות למשרה ספציפית.
        Task<List<MatchDto>> GetTopMatchesForCandidate(int candidateId, int topCount);// – החזרת המשרות הטובות ביותר עבור מועמד מסוים.
    }
}

