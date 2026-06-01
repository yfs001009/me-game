using Fantasy;
using Fantasy.Async;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Hotfix.Shared;

namespace Hotfix.Character.Handler;

public sealed class CharacterListRequestHandler : MessageRPC<C2G_CharacterListRequest, G2C_CharacterListResponse>
{
    protected override async FTask Run(Session session, C2G_CharacterListRequest request, G2C_CharacterListResponse response, Action reply)
    {
        if (!SheepServices.Auth.TryRequireProfile(request.Token, out var profile, out var message))
        {
            response.ErrorCode = 401;
            response.Success = false;
            response.Message = message;
            return;
        }

        var result = SheepServices.Characters.GetList(profile.PlayerId);
        response.Success = result.Success;
        response.Message = result.Message;
        response.SelectedHeroId = result.SelectedHeroId;
        response.SelectedGhostId = result.SelectedGhostId;
        response.Characters.AddRange(result.Characters);
        Log.Info($"玩家请求角色列表：玩家ID={profile.PlayerId}，角色数量={response.Characters.Count}");
        await FTask.CompletedTask;
    }
}
