using Fantasy;
using GameConfig.role;
using Hotfix.Config;
using Hotfix.Shared;

namespace Hotfix.Character.Service;

public sealed class CharacterService
{
    private const string HeroCategory = "Hero";
    private const string GhostCategory = "Ghost";
    private readonly object gate = new();
    private readonly Dictionary<long, PlayerCharacterState> states = new();

    public G2C_CharacterListResponse GetList(long playerId)
    {
        lock (gate)
        {
            var state = GetOrCreateState(playerId);
            var response = new G2C_CharacterListResponse
            {
                Success = true,
                Message = "角色列表获取成功。",
                SelectedHeroId = state.SelectedHeroId,
                SelectedGhostId = state.SelectedGhostId
            };

            FillCharacters(response.Characters, state);
            return response;
        }
    }

    public G2C_SelectCharacterResponse Select(long playerId, int characterId)
    {
        lock (gate)
        {
            var state = GetOrCreateState(playerId);
            var config = ConfigSystem.Instance.Tables.TbCharacter.GetOrDefault(characterId);
            var response = new G2C_SelectCharacterResponse();

            if (config == null)
            {
                response.Success = false;
                response.Message = "角色不存在。";
                FillResponse(response, state);
                return response;
            }

            if (!SheepServices.Assets.HasCharacter(playerId, characterId))
            {
                response.Success = false;
                response.Message = "角色尚未解锁。";
                FillResponse(response, state);
                return response;
            }

            if (IsCategory(config, HeroCategory))
            {
                state.SelectedHeroId = characterId;
            }
            else if (IsCategory(config, GhostCategory))
            {
                state.SelectedGhostId = characterId;
            }
            else
            {
                response.Success = false;
                response.Message = "角色类型无效。";
                FillResponse(response, state);
                return response;
            }

            response.Success = true;
            response.Message = "角色选择成功。";
            FillResponse(response, state);
            return response;
        }
    }

    private PlayerCharacterState GetOrCreateState(long playerId)
    {
        if (states.TryGetValue(playerId, out var state))
        {
            return state;
        }

        state = new PlayerCharacterState
        {
            PlayerId = playerId
        };
        foreach (var config in ConfigSystem.Instance.Tables.TbCharacter.DataList)
        {
            if (!config.IsInitial)
            {
                continue;
            }

            SheepServices.Assets.EnsureCharacterUnlocked(playerId, config.CharacterId);
            if (IsCategory(config, HeroCategory) && state.SelectedHeroId == 0)
            {
                state.SelectedHeroId = config.CharacterId;
            }
            else if (IsCategory(config, GhostCategory) && state.SelectedGhostId == 0)
            {
                state.SelectedGhostId = config.CharacterId;
            }
        }

        states.Add(playerId, state);
        return state;
    }

    private static void FillResponse(G2C_SelectCharacterResponse response, PlayerCharacterState state)
    {
        response.SelectedHeroId = state.SelectedHeroId;
        response.SelectedGhostId = state.SelectedGhostId;
        FillCharacters(response.Characters, state);
    }

    private static void FillCharacters(ICollection<CharacterInfo> output, PlayerCharacterState state)
    {
        foreach (var config in ConfigSystem.Instance.Tables.TbCharacter.DataList.OrderBy(v => v.Category).ThenBy(v => v.SortOrder))
        {
            output.Add(ToInfo(config, state));
        }
    }

    private static CharacterInfo ToInfo(CharacterConfig config, PlayerCharacterState state)
    {
        return new CharacterInfo
        {
            CharacterId = config.CharacterId,
            Category = config.Category,
            Race = config.Race,
            Name = config.Name,
            AbilityId = config.AbilityId,
            AbilityName = config.AbilityName,
            AbilityDesc = config.AbilityDesc,
            IconAsset = config.IconAsset,
            PrefabAsset = config.PrefabAsset,
            IsUnlocked = SheepServices.Assets.HasCharacter(state.PlayerId, config.CharacterId),
            IsSelected = config.CharacterId == state.SelectedHeroId || config.CharacterId == state.SelectedGhostId,
            Description = config.Description
        };
    }

    private static bool IsCategory(CharacterConfig config, string category)
    {
        return string.Equals(config.Category, category, StringComparison.OrdinalIgnoreCase);
    }
}

internal sealed class PlayerCharacterState
{
    public long PlayerId { get; set; }
    public int SelectedHeroId { get; set; }
    public int SelectedGhostId { get; set; }
}
