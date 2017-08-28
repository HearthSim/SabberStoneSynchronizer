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

			Player1 = ExtractPlayer(_powerGame.Player1);
			Player2 = ExtractPlayer(_powerGame.Player2);
			FirstPlayer = int.Parse(_powerGame.Player1.GetValue(GameTag.CONTROLLER)) == 1 ? Player1 : Player2;
			CurrentPlayer = Player1;

			State = _powerGame.Game.Data[GameTag.STATE].ParseEnum<State>();
			Step = _powerGame.Game.Data[GameTag.STEP].ParseEnum<Step>();
			NextStep = _powerGame.Game.Data[GameTag.NEXT_STEP].ParseEnum<Step>();

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
					SourceId = powerPlayer.CurrentChoice.SourceId,
					Choices = powerPlayer.CurrentChoice.ChoiceEntities.Select(a => int.Parse(a.Id)).ToList()
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
			foreach (var cardEntity in powerPlayer.CardEntities)
			{
				switch (cardEntity.GetValue(GameTag.ZONE).ParseEnum<Zone>())
				{
					case SabberStoneCore.Enums.Zone.INVALID:
						break;
					case SabberStoneCore.Enums.Zone.PLAY:
						break;
					case SabberStoneCore.Enums.Zone.DECK:
						FromCard(player, Cards.FromId(cardEntity.GetValue(GameTag.CARD_ID)), zone: player.DeckZone);
						break;
					case SabberStoneCore.Enums.Zone.HAND:
						FromCard(player, Cards.FromId(cardEntity.GetValue(GameTag.CARD_ID)), zone: player.HandZone);
						break;
					case SabberStoneCore.Enums.Zone.GRAVEYARD:
						break;
					case SabberStoneCore.Enums.Zone.REMOVEDFROMGAME:
						break;
					case SabberStoneCore.Enums.Zone.SETASIDE:
						break;
					case SabberStoneCore.Enums.Zone.SECRET:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			player.PlayState = powerPlayer.GetValue(GameTag.PLAYSTATE).ParseEnum<PlayState>();
			var heroCard = _powerGame.Entities.Values.Single(a => a.ValueEquals(GameTag.CARDTYPE, "HERO") && a.ValueEquals(GameTag.CONTROLLER, controllerNum => int.Parse(controllerNum) == powerPlayer.PlayerId));
			player.AddHeroAndPower(Cards.FromId(heroCard.GetValue(GameTag.CARD_ID)));
			return player;
		}
	}
}
