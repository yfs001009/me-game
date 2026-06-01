using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Task.Handler;

public sealed class OutgameTaskListRequestHandler : MessageRPC<C2G_OutgameTaskListRequest, G2C_OutgameTaskListResponse>
{
    protected override async FTask Run(Session session, C2G_OutgameTaskListRequest request, G2C_OutgameTaskListResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        SheepServices.Assets.BindScene(session.Scene);
        var result = SheepServices.Tasks.GetList(session.Scene, profile.PlayerId, request.TaskType, request.ActivityId, request.FeatureId);
        response.Success = result.Success;
        response.Message = result.Message;
        response.Tasks.AddRange(result.Tasks);
        await FTask.CompletedTask;
    }
}
