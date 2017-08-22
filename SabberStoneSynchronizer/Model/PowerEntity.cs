using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SabberStoneCore.Enums;

namespace SabberStoneSynchronizer.Model
{
	public class PowerEntity
	{
		public Dictionary<GameTag, string> Data;

		public PowerEntity()
		{
			Data = new Dictionary<GameTag, string>();
		}

		internal void Add(string tag, string value)
		{
			var gameTag = Util.ParseTag(tag);
			if (!Data.ContainsKey(gameTag))
			{
				Data.Add(gameTag, value);
			}
			else if (Data[gameTag] == value)
			{
				//Console.WriteLine("Unchanged add tag submited: tag[" + tag + "] value[" + value + "]");
			}
			else
			{
				Console.WriteLine("Changed add tag submited: tag[" + tag + "] oldvalue[" + Data[gameTag] + "] newvalue[" + value + "]");
			}
		}

		internal void Change(GameTag tag, string value)
		{
			Data[tag] = value;
		}

		internal string GetValue(GameTag tag)
		{
			string result = null;
			Data.TryGetValue(tag, out result);
			return result;
		}

		public string Id
		{
			get => GetValue(GameTag.ENTITY_ID);
			set => Add("ENTITY_ID", value);
		}

		public override string ToString()
		{
			var str = new StringBuilder();
			str.Append($"{GetType().Name}:");
			str.AppendLine("[" + Id + "]");
			Data.ToList().ForEach(p =>
			{
				str.AppendLine(" - " + p.Key + " -> " + p.Value);
			});
			return str.ToString();
		}

		public bool ValueEquals(GameTag tag, string value)
		{
			if (!Data.ContainsKey(tag))
				return false;
			return Data[tag] == value;
		}

	}
}
