using ITPortal.Core;
using ITPortal.Extension.System;
using ITPortal.Search.Application.OpenSearch.Dtos;
using ITPortal.Search.Application.SearchTopic.Dtos;
using ITPortal.Search.Application.SearchTopicGrants.Dtos;
using ITPortal.Search.Application.TopicDocument.Dtos;
using ITPortal.Search.Application.UserSearchHistory.Dtos;
using ITPortal.Search.Core.Models;
using ITPortal.Search.Core.Models.Search;

using Newtonsoft.Json;

namespace ITPortal.Search.Application.Mappers
{
    public class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<SearchTopicDto, SearchTopicEntity>();
            config.ForType<SearchTopicEntity, SearchTopicDto>();
            config.ForType<SearchTopicGrantsResultDto, SearchTopicEntity>();
            config.ForType<SearchTopicEntity, SearchTopicGrantsResultDto>();

            config.ForType<SearchTopicGrantsDto, SearchTopicGrantsEntity>();
            config.ForType<SearchTopicGrantsEntity, SearchTopicGrantsDto>();

            config.ForType<TopicDocumentDto, TopicDocumentModel>()
                .Map(dest => dest.Attachments, src => src.AttachmentList == null ? null : src.AttachmentList.Adapt<TopicDocumentAttachmentsModel>())
                .Map(dest => dest.PayloadStr, src => src.Payload == null ? "" : src.Payload.ToJSON());
            config.ForType<TopicDocumentModel, TopicDocumentDto>()
                .Map(dest => dest.AttachmentList, src => src.Attachments == null ? null : src.Attachments.Adapt<TopicDocumentAttachmentDto>())
                .Map(dest => dest.Payload, src => src.PayloadStr.IsNullOrWhiteSpace() ? null : src.PayloadStr.To<TopicDocumentAttachmentDto>());
            config.ForType<TopicDocumentCreateDto, TopicDocumentModel>();
            config.ForType<TopicDocumentModel, TopicDocumentCreateDto>();
            config.ForType<TopicDocumentUpdateDto, TopicDocumentModel>();
            config.ForType<TopicDocumentModel, TopicDocumentUpdateDto>();
            //config.ForType<TopicDocumentQueryDto, TopicDocumentModel>();
            //config.ForType<TopicDocumentModel, TopicDocumentQueryDto>();

            config.ForType<TopicDocumentAttachmentDto, TopicDocumentAttachmentsModel>();
            config.ForType<TopicDocumentAttachmentsModel, TopicDocumentAttachmentDto>();

            config.ForType<GroupByTopicDocumentQueryDto, GroupByTopicDocumentQueryModel>();
            config.ForType<GroupByTopicDocumentQueryModel, GroupByTopicDocumentQueryDto>();

            config.ForType<TopicDocumentQueryDto, TopicDocumentQueryModel>();
            config.ForType<TopicDocumentQueryModel, TopicDocumentQueryDto>();

            config.ForType<UserSearchHistoryDto, UserSearchHistoryEntity>();
            config.ForType<UserSearchHistoryEntity, UserSearchHistoryDto>();
        }
    }
}
