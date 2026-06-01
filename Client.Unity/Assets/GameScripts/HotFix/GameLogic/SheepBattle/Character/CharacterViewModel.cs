using System.Collections.Generic;
using System.Linq;
using Fantasy;

namespace GameLogic.SheepBattle.Character
{
    public sealed class CharacterViewModel
    {
        public List<CharacterEntryViewModel> Characters { get; } = new List<CharacterEntryViewModel>();
        public int SelectedHeroId { get; private set; }
        public int SelectedGhostId { get; private set; }

        public void Apply(G2C_CharacterListResponse response)
        {
            Apply(response?.Characters, response?.SelectedHeroId ?? 0, response?.SelectedGhostId ?? 0);
        }

        public void Apply(G2C_SelectCharacterResponse response)
        {
            Apply(response?.Characters, response?.SelectedHeroId ?? 0, response?.SelectedGhostId ?? 0);
        }

        public IReadOnlyList<CharacterEntryViewModel> GetByCategory(string category)
        {
            return Characters
                .Where(v => string.Equals(v.Category, category, System.StringComparison.OrdinalIgnoreCase))
                .OrderBy(v => v.CharacterId)
                .ToList();
        }

        private void Apply(IReadOnlyList<CharacterInfo> characters, int selectedHeroId, int selectedGhostId)
        {
            SelectedHeroId = selectedHeroId;
            SelectedGhostId = selectedGhostId;
            Characters.Clear();
            if (characters == null)
            {
                return;
            }

            foreach (var info in characters)
            {
                Characters.Add(new CharacterEntryViewModel(info));
            }
        }
    }

    public sealed class CharacterEntryViewModel
    {
        public CharacterEntryViewModel(CharacterInfo info)
        {
            CharacterId = info?.CharacterId ?? 0;
            Category = info?.Category ?? string.Empty;
            Race = info?.Race ?? string.Empty;
            Name = info?.Name ?? string.Empty;
            AbilityName = info?.AbilityName ?? string.Empty;
            AbilityDesc = info?.AbilityDesc ?? string.Empty;
            IconAsset = info?.IconAsset ?? string.Empty;
            IsUnlocked = info?.IsUnlocked ?? false;
            IsSelected = info?.IsSelected ?? false;
            Description = info?.Description ?? string.Empty;
        }

        public int CharacterId { get; }
        public string Category { get; }
        public string Race { get; }
        public string Name { get; }
        public string AbilityName { get; }
        public string AbilityDesc { get; }
        public string IconAsset { get; }
        public bool IsUnlocked { get; }
        public bool IsSelected { get; }
        public string Description { get; }
    }
}
