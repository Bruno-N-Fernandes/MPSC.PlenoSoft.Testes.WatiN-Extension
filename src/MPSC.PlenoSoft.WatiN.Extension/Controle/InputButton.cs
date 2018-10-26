using System;

namespace MPSC.PlenoSoft.WatiN.Extension.Controle
{
	public class InputButton : GetSet
	{
		public override void Set(String idOuNome, Object valor, Boolean forceChange = false)
		{
			navegador.ClickButton(idOuNome, Convert.ToString(valor).ToUpper().Contains("CLICK") || forceChange);
		}

		public override Object Get(String idOuNome, Boolean isValue)
		{
			return navegador.GetButtonValue(idOuNome, isValue);
		}
	}
}