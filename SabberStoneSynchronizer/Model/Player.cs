using System.Collections.Generic;
using System.Linq;
using SabberStoneCore.Enums;

namespace SabberStoneSynchronizer.Model
{
	public class Player : PowerEntity
	{
		private readonly PowerGame _game;
		public string Name { get; set; }

		public Player(PowerGame game, int playerId)
		{
			_game = game;
			PlayerId = playerId;
		}

		public IEnumerable<PowerEntity> CardEntities
		{
			get
			{
				return _game.Entities.Values.Where(a => !string.IsNullOrEmpty(a.GetValue(GameTag.CARD_ID))
														&& !a.ValueEquals(GameTag.CARDTYPE, "HERO")
														&& !a.ValueEquals(GameTag.CARDTYPE, "HERO_POWER")
														&& a.ValueEquals(GameTag.CONTROLLER, PlayerId.ToString())).ToList();
			}
		}
		
		public int PlayerId { get; }
		public PowerChoice CurrentChoice { get; set; }
	}
}
