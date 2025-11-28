

using System.Text.Json;
using Azure.Core.Serialization;
using DataTopicStore.Core.Enums;
using DataTopicStore.Core.Models;
using Furion.JsonSerialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class TopicDraftDetailsDto
    {
        public long id { get; set; }
        public string name { get; set; }
        public string cover { get; set; }
        
        public string version { get; set; }
        public string description { get; set; }
        public string department { get; set; }
        public string author_id { get; set; }
        public string author { get; set; }
        public string owner_id { get; set; }
        public string owner { get; set; }
        public string owner_email { get; set; }
        public string content { get; set; }
        public string json_data_lineage { get; set; }
        public string json_formula { get; set; }
        public DateTimeOffset created_time { get; set; }
        public DateTimeOffset? updated_time { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public string json_develop_records { get; set; }
        public string json_parameters_input { get; set; }
        public string json_parameters_output { get; set; }
        public string sql_scripts { get; set; }

        public bool? is_verification_passed { get; set; }
        public string verification_failure_reason { get; set; }
        public string json_validation_result { get; set; }
        public TAGS tags { get; set; }
        public ProgressStatus progress { get; set; }
        public bool can_edit { get; set; }

        public CreateITDevelopingDto ITDevelopingSetting
        {
            get
            {
                CreateITDevelopingDto dto = new CreateITDevelopingDto { topic_id = id };
                if (!string.IsNullOrWhiteSpace(this.json_develop_records))
                    dto.Data = JsonConvert.DeserializeObject<CreateITDevelopingModel>(this.json_develop_records);
                return dto;
            }
        }

        public SetParametersInputDto InputParameterSetting
        {
            get
            {
                SetParametersInputDto dto = new SetParametersInputDto { TopicId = id };
                if (!string.IsNullOrWhiteSpace(this.json_parameters_input))
                    dto.InputSetting = JsonConvert.DeserializeObject<ParametersInputSettingModel>(this.json_parameters_input);
                return dto;
            }
        }
        public SetParametersOutputDto OutputParameterSetting
        {
            get
            {
                SetParametersOutputDto dto = new SetParametersOutputDto { TopicId = id };
                if (!string.IsNullOrWhiteSpace(this.json_parameters_output))
                    dto.OutputSetting = JsonConvert.DeserializeObject<ParametersOutputSettingModel>(this.json_parameters_output);
                return dto;
            }
        }

        public Guid? category_id { get; set; }
    }
}
