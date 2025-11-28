using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Models;
using Microsoft.OpenApi.Models;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class SwaggerTopicApiParameters
    {
        public SwaggerTopicApiParameters() { }
        public long TopicId { get; set; }
        public string Name { get; set; }
        public string Descritption { get; set; }
        public ParametersInputSettingModel Input { get; set; }
        public ParametersOutputSettingModel Output { get; set; }

        public IList<OpenApiParameter> GetInputParameters()
        {
            List<OpenApiParameter> parameters = [];
            if (Input is ParametersInputSettingModel { Parameters.Count: > 0 })
            {
                foreach (var item in Input.Parameters)
                {
                    var schema = new OpenApiSchema
                    {
                        Type = item.DataType,
                        Format = item.Format
                    };

                    var param = new OpenApiParameter
                    {
                        In = ParameterLocation.Query,
                        Name = item.Name,
                        Description = item.Description,
                        Required = item.Required,
                    };
                    if (item.DataType == "array")
                    {
                        schema.Format = null;
                        schema.Items = new OpenApiSchema { Type = item.Format };
                    }
                    param.Schema = schema;
                    parameters.Add(param);
                }
            }

            return parameters;
        }

        public (string, bool, Dictionary<string, OpenApiSchema>) GetOutputParameters()
        {
            Dictionary<string, OpenApiSchema> parameters = [];
            if (Output is ParametersOutputSettingModel { Parameters.Count: > 0 })
            {
                foreach (var item in Output.Parameters)
                {
                    parameters.Add(item.Name, new OpenApiSchema { Type = item.DataType, Format = item.Format });
                }

                return (Output.Type, Output.IsPaged, parameters);
            }

            return (null, false, null);
        }
    }
}
