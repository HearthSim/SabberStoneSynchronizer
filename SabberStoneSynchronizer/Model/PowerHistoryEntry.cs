﻿using SabberStoneCore.Enums;

namespace SabberStoneSynchronizer.Model
{
	public abstract class PowerHistoryEntry
	{
		public PowerType PowerType { get; set; }

		public abstract void Process(PowerGame powerGame);

	}
}
