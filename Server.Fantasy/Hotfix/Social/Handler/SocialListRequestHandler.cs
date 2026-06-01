using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Social.Handler;

public sealed class SocialListRequestHandler : MessageRPC<C2G_SocialListRequest, G2C_SocialListResponse>
{
    protected override async FTask Run(Session session, C2G_SocialListRequest request, G2C_SocialListResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        var result = SheepServices.Social.GetList(profile, request.ViewMode, request.Keyword);
        response.Success = result.Success;
        response.Message = result.Message;
        response.ViewMode = result.ViewMode;
        response.FollowingCount = result.FollowingCount;
        response.FollowerCount = result.FollowerCount;
        response.Players.AddRange(result.Players);
        await FTask.CompletedTask;
    }
}
