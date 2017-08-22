using System;
using System.Linq;
using SabberStoneCore.Actions;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCore.Kettle;
using SabberStoneCore.Model;
using SabberStoneCore.Model.Entities;
using SabberStoneSynchronizer.Model;

namespace SabberStoneSynchronizer.Sync
{
	public class SyncedGame : Game
	{
		private readonly PowerGame _powerGame;

		public SyncedGame(PowerGame powerGame) : base(new GameConfig())
		{
			_powerGame = powerGame;
		}

		public void Sync()
		{

			State = _powerGame.Game.Data[GameTag.STATE].ParseEnum<State>();
			Step = _powerGame.Game.Data[GameTag.STEP].ParseEnum<Step>();
			NextStep = _powerGame.Game.Data[GameTag.NEXT_STEP].ParseEnum<Step>();

			Player1 = ExtractPlayer(_powerGame.Player1);
			Player2 = ExtractPlayer(_powerGame.Player2);



			Player1.DeckCards.AddRange(_powerGame.Player1.CardEntities.Where(a => a.ValueEquals(GameTag.ZONE, "DECK")).Select(a => Cards.FromId(a.Data[GameTag.CARD_ID])));
			Player1.DeckCards.AddRange(_powerGame.Player1.CardEntities.Where(a => a.ValueEquals(GameTag.ZONE, "DECK")).Select(a => Cards.FromId(a.Data[GameTag.CARD_ID])));



		}

		public Controller ExtractPlayer(Player powerPlayer)
		{
			var player = new Controller(this, powerPlayer.Name, powerPlayer.PlayerId, int.Parse(powerPlayer.Id));
			if (powerPlayer.CurrentChoice != null)
			{
				var choice = new Choice(player)
				{
					ChoiceType = powerPlayer.CurrentChoice.Type,
					TargetIds = powerPlayer.CurrentChoice.ChoiceEntities.Select(a => int.Parse(a.Id)).ToList(),
					SourceId = powerPlayer.CurrentChoice.SourceId
				};

				switch (choice.ChoiceType)
				{
					case ChoiceType.MULLIGAN:
						choice.ChoiceAction = ChoiceAction.HAND;
						break;
					case ChoiceType.INVALID:
					case ChoiceType.GENERAL:
						throw new NotImplementedException();
				}
				player.Choice = choice;
			}
			return player;
		}
	}
}
