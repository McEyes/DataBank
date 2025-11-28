using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Enums
{
    public enum ProgressStatus : byte
    {
        Waiting = 0,
        InEvaluation = 1,
        Finished = 2
    }
}