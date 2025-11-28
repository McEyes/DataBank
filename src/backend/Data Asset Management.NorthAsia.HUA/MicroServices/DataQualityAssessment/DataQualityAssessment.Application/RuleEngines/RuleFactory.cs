using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.RuleEngines.Rules;
using DataQualityAssessment.Core.Enums;
using DataQualityAssessment.Core.Models;

namespace DataQualityAssessment.Application.RuleEngines
{
    public class RuleFactory
    {
        public RuleFactory() { }

        public static IRule CreateRule(RuleValidateType type,RuleDataContext context)
        {
            switch (type)
            {
                case RuleValidateType.Email:
                    return new EmailVerificationRule(context);
                case RuleValidateType.PhoneNumber:
                    return new PhoneNumberVerificationRule(context);
                case RuleValidateType.NullValueRatio:
                    return new NullValueRatioVerificationRule(context);
                //case RuleValidateType.Timeliness:
                //    throw new NotImplementedException();
                case RuleValidateType.ExtraColumns:
                    return new ExtraColumnsVerificationRule(context);
                case RuleValidateType.DataTypeDateType:
                    return new DataTypeDateTypeVerificationRule(context);
                case RuleValidateType.DataTypeEnumType:
                    return new DataTypeEnumTypeVerificationRule(context);
                case RuleValidateType.DataInterpretability:
                    return new DataInterpretabilityVerificationRule(context);
                case RuleValidateType.DataTypeVarcharType:
                    return new DataTypeVarcharTypeVerificationRule(context);
                case RuleValidateType.DataInformationIntegrity:
                    return new DataInformationIntegrityVerificationRule(context);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
