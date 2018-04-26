using System;

namespace MPSC.PlenoSoft.WatiN.Extension.Controle
{
	public class InputText : GetSet
	{
		public override void Set(String idOuNome, Object valor, Boolean forceChange = false)
		{
			navegador.SetText(idOuNome, Convert.ToString(valor), forceChange);
		}

		public override Object Get(String idOuNome, Boolean isValue)
		{
			return navegador.GetTextValue(idOuNome, isValue);
		}
	}
}