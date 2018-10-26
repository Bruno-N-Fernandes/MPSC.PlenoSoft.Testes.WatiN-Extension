using System;

namespace MPSC.PlenoSoft.WatiN.Extension.Controle
{
	public class InputSelect : GetSet
	{
		public override void Set(String idOuNome, Object valor, Boolean forceChange = false)
		{
			navegador.SetCombo(idOuNome, Convert.ToString(valor), forceChange);
		}

		public override Object Get(String idOuNome, Boolean isValue)
		{
			return navegador.GetComboValue(idOuNome, isValue);
		}
	}
}