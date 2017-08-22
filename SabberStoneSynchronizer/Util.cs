using System;
using System.Collections.Generic;
using System.Text;
using SabberStoneCore.Enums;

namespace SabberStoneSynchronizer
{
	public static class Util
	{
		public static T ParseEnum<T>(this string enumValue)
		{
			return (T)Enum.Parse(typeof(T), enumValue);
		}
		public static GameTag ParseTag(string tag)
		{
			return (GameTag)Enum.Parse(typeof(GameTag), tag);
		}
	}
}
