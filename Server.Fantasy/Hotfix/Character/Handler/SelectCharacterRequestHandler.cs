using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Character.Handler;

public sealed class SelectCharacterRequestHandler : MessageRPC<C2G_SelectCharacterRequest, G2C_SelectCharacterResponse>
{
    protected override async FTask Run(Session session, C2G_SelectCharacterRequest request, G2C_SelectCharacterResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        var result = SheepServices.Characters.Select(profile.PlayerId, request.CharacterId);
        response.Success = result.Success;
        response.Message = result.Message;
        response.SelectedHeroId = result.SelectedHeroId;
        response.SelectedGhostId = result.SelectedGhostId;
        response.Characters.AddRange(result.Characters);
        if (response.Success)
        {
            SheepServices.Tasks.AddProgress(profile.PlayerId, "Character.Select.Count", 1);
        }
        Log.Info($"玩家选择角色：玩家ID={profile.PlayerId}，角色ID={request.CharacterId}，成功={response.Success}");
        await FTask.CompletedTask;
    }
}
