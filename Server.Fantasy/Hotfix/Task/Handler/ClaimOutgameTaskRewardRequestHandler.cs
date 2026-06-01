using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Task.Handler;

public sealed class ClaimOutgameTaskRewardRequestHandler : MessageRPC<C2G_ClaimOutgameTaskRewardRequest, G2C_ClaimOutgameTaskRewardResponse>
{
    protected override async FTask Run(Session session, C2G_ClaimOutgameTaskRewardRequest request, G2C_ClaimOutgameTaskRewardResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        SheepServices.Assets.BindScene(session.Scene);
        var result = SheepServices.Tasks.Claim(session.Scene, profile.PlayerId, request.TaskId);
        response.Success = result.Success;
        response.Message = result.Message;
        response.Tasks.AddRange(result.Tasks);
        response.Snapshot = result.Snapshot;
        await FTask.CompletedTask;
    }
}
