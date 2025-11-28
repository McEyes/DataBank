using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Application.Topics.Services;
using Furion.UnifyResult;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DataTopicStore.Application.DocumentFilters
{
    public class TopicDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var tag = new OpenApiTag { Name = "Data Topic API", Description = "Data Topic API" };
            var topicService = App.GetRequiredService<ITopicService>();
            var topicParameters = topicService.GetSwaggerTopicApiParameters();

            foreach (var topicParameter in topicParameters)
            {
                var parameters = topicParameter.GetInputParameters();
                (var type, var ispaged, var outputParameters) = topicParameter.GetOutputParameters();
                var responseSchema = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["statusCode"] = new OpenApiSchema { Type = "integer" },
                        ["data"] = new OpenApiSchema
                        {
                            Type = type,
                            Items = new OpenApiSchema { Type = "object", Properties = outputParameters },
                            Properties = outputParameters,
                        },
                        ["succeeded"] = new OpenApiSchema { Type = "boolean" },
                        ["errors"] = new OpenApiSchema { Type = "string" },
                        ["extras"] = new OpenApiSchema { Type = "string" },
                        ["timestamp"] = new OpenApiSchema { Type = "integer" },
                    }
                };

                if (ispaged)
                {
                    responseSchema.Properties["data"].Type = "object";
                    responseSchema.Properties["data"].Items = null;
                    responseSchema.Properties["data"].Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["data"] = new OpenApiSchema
                        {
                            Type = type,
                            Items = new OpenApiSchema { Type = "object", Properties = outputParameters }
                        },
                        ["totalCount"] = new OpenApiSchema { Type = "integer" }
                    };
                }

                swaggerDoc.Paths.Add($"/api/topic/{topicParameter.TopicId}", new OpenApiPathItem
                {
                    Summary = topicParameter.Descritption,
                    Description = topicParameter.Descritption,
                    Operations =
                    {
                        [OperationType.Get] = new OpenApiOperation
                        {
                            Tags =new List<OpenApiTag>{ tag },
                            Parameters = parameters,
                            Summary = topicParameter.Name,
                            Description = topicParameter.Descritption,
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse {
                                    Description = "OK",
                                    Content = new Dictionary<string,OpenApiMediaType>{
                                        ["application/json"] = new OpenApiMediaType{

                                            Schema = responseSchema
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }
    }
}
