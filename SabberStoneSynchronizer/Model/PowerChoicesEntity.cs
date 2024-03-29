﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SabberStoneCore.Enums;
using SabberStoneCore.Model;

namespace SabberStoneSynchronizer.Model
{
	internal class PowerChoicesEntity : PowerHistoryEntry
	{
		private static readonly Regex InitialChoiceRegex = new Regex(@"id=(\d) Player=(.+) TaskList=(\d) ChoiceType=([A-Z]+) CountMin=(\d) CountMax=(\d)");
		private static readonly Regex IdPickRegex = new Regex(@"id=(\d+)");
		private int _entityId;
		private string _playerName;
		private int _taskList;

		public PowerChoicesEntity(string contentLine)
		{
			var match = InitialChoiceRegex.Match(contentLine);
			_entityId = int.Parse(match.Groups[1].Value);
			_playerName = match.Groups[2].Value;
			_taskList = int.Parse(match.Groups[3].Value);
			ChoiceType = (ChoiceType)Enum.Parse(typeof(ChoiceType), match.Groups[4].Value);
			Choices = new List<int>();
		}

		public List<int> Choices { get; set; }

		public ChoiceType ChoiceType { get; }

		public void AddChoiceLine(string contentLine)
		{
			if (contentLine.StartsWith("Source"))
				Source = contentLine.Split('=').Last();
			else if (contentLine.StartsWith("Entities["))
			{
				Choices.Add(int.Parse(IdPickRegex.Match(contentLine).Groups[1].Value));
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public string Source { get; private set; }
		public override void Process(PowerGame powerGame)
		{
			switch (ChoiceType)
			{
				case ChoiceType.INVALID:
					throw new NotImplementedException();
				case ChoiceType.MULLIGAN:
					{
						foreach (var choice in Choices)
						{
							var choiceEntities = Choices.Select(a => powerGame.Entities[a]).ToList();
							var player = powerGame.GetIdByName(_playerName);
							(powerGame.GetEntityById(player) as Player).CurrentChoice = new PowerChoice(choiceEntities, ChoiceType.MULLIGAN, powerGame.GetIdByName(Source));

							//todo hack, not sure if we should force these in hand here
							foreach (var choiceEntity in choiceEntities)
							{
								choiceEntity.Change(GameTag.ZONE, "HAND");
							}


						}
					}
					break;
				case ChoiceType.GENERAL:
					throw new NotImplementedException();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public class PowerChoice
	{
		public List<PowerEntity> ChoiceEntities { get; }
		public ChoiceType Type { get; }
		public int SourceId { get; }

		public PowerChoice(List<PowerEntity> choiceEntities, ChoiceType type, int sourceId)
		{
			ChoiceEntities = choiceEntities;
			Type = type;
			SourceId = sourceId;
		}
	}
}
