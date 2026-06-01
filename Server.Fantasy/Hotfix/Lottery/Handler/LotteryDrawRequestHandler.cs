using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Lottery.Handler;

public sealed class LotteryDrawRequestHandler : MessageRPC<C2G_LotteryDrawRequest, G2C_LotteryDrawResponse>
{
    protected override async FTask Run(Session session, C2G_LotteryDrawRequest request, G2C_LotteryDrawResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        var result = SheepServices.Lottery.Draw(profile.PlayerId, request.Pool, request.Count);
        response.Success = result.Success;
        response.Message = result.Message;
        response.Results.AddRange(result.Results);
        response.Snapshot = result.Snapshot;
        await FTask.CompletedTask;
    }
}
