using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.Assessments.Dtos;
using DataQualityAssessment.Core.Entities.DataAsset;

namespace DataQualityAssessment.Application
{

    public class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<TableEntity, TablePageItemDto>()
                .Map(dest => dest.TableId, src => src.Id)
                .Map(dest => dest.TableName, src => src.table_name)
                .Map(dest => dest.SourceId, src => src.source_id)
                .Map(dest => dest.TableComment, src => src.table_comment)
                .Map(dest => dest.UpdateFrequency, src => src.update_frequency)
                .Map(dest => dest.CreatedTableTime, src => src.create_time);
        }
    }
}
