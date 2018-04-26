using System;

namespace MPSC.PlenoSoft.WatiN.Extension.Util
{
	public class TimeOut
	{
		public static readonly TimeSpan CincoSegundos = TimeSpan.FromSeconds(5);

		private DateTime _inicio = DateTime.Now;
		public TimeSpan TempoDecorrido { get { return DateTime.Now - _inicio; } }

		public void ReIniciar()
		{
			_inicio = DateTime.Now;
		}

		public Boolean MenorQue(TimeSpan? tempo)
		{
			return TempoDecorrido.TotalMilliseconds < (tempo?.TotalMilliseconds ?? 250.0);
		}
	}
}