using Fantasy.Async;
using GameLogic.SheepBattle.Event;
using GameLogic.SheepBattle.Common;
using GameLogic.SheepBattle.Network;
using TEngine;

namespace GameLogic.SheepBattle.Character
{
    public sealed class CharacterController
    {
        public static CharacterController Instance { get; } = new CharacterController();
        public CharacterViewModel Model { get; } = new CharacterViewModel();

        private CharacterController()
        {
        }

        public async FTask<CharacterViewModel> RefreshAsync()
        {
            var response = await SheepNetworkService.Instance.RequestCharacterListAsync();
            Model.Apply(response);
            GameEvent.Send(new CharacterViewChangedEvent(Model));
            return Model;
        }

        public async FTask SelectAsync(int characterId)
        {
            var response = await SheepNetworkService.Instance.SelectCharacterAsync(characterId);
            if (!response.Success)
            {
                CommonNoticeService.Show(response.Message);
                return;
            }

            Model.Apply(response);
            GameEvent.Send(new CharacterViewChangedEvent(Model));
        }

        public CharacterViewModel GetCurrentView()
        {
            return Model;
        }
    }
}
