using System;
using System.Collections.Generic;
using System.Linq;

namespace MPSC.PlenoSoft.WatiN.Extension.Util
{
	public static class UtilExtension
	{
		public static Int32 ToInt32(this Double source)
		{
			return Convert.ToInt32(source);
		}

		public static Int32 GetTotalSeconds(this TimeSpan source)
		{
			return source.TotalSeconds.ToInt32();
		}

		public static String[] ToUpper(this IEnumerable<String> source)
		{
			return source.Process(s => s.ToUpper());
		}

		public static String[] ToLower(this IEnumerable<String> source)
		{
			return source.Process(s => s.ToLower());
		}

		private static String[] Process(this IEnumerable<String> source, Func<String, String> seletor)
		{
			return (source != null) ? source.Select(seletor).ToArray() : new String[0];
		}
	}
}