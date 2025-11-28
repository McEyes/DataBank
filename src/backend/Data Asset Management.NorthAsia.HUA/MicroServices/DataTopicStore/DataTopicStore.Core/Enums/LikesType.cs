using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Core.Enums
{
    public enum LikesType : byte
    {
        Likes,
        Dislikes
    }

    public enum CancelLikesType : byte
    {
        Likes,
        Dislikes,
        CancelLikes,
        CancelDislikes
    }
}
