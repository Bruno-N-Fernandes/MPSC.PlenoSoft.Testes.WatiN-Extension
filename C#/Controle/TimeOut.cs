using System;

namespace MPSC.PlenoSoft.Testes.WatiN.Extension.Controle
{
	public interface ITimeOut
	{
		void ReIniciar();
		IUnidadeTempo MenorQue(Decimal tempo);
	}

	public interface IUnidadeTempo
	{
		Boolean MiliSegundos { get; }
		Boolean Segundos { get; }
		Boolean Minutos { get; }
		Boolean Horas { get; }
	}

	public class TimeOut : ITimeOut, IUnidadeTempo
	{
		private DateTime _inicio = DateTime.Now;
		private Double _tempo = 0;
		public void ReIniciar()
		{
			_inicio = DateTime.Now;
		}

		public IUnidadeTempo MenorQue(Decimal tempo)
		{
			_tempo = Convert.ToDouble(tempo);
			return this;
		}

		Boolean IUnidadeTempo.MiliSegundos
		{
			get { return (DateTime.Now - _inicio).TotalMilliseconds < _tempo; }
		}

		Boolean IUnidadeTempo.Segundos
		{
			get { return (DateTime.Now - _inicio).TotalSeconds < _tempo; }
		}

		Boolean IUnidadeTempo.Minutos
		{
			get { return (DateTime.Now - _inicio).TotalMinutes < _tempo; }
		}

		Boolean IUnidadeTempo.Horas
		{
			get { return (DateTime.Now - _inicio).TotalHours < _tempo; }
		}
	}
}
