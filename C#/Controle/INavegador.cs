using System;

namespace MPSC.PlenoSoft.Testes.WatiN.Extension.Controle
{
	public interface INavegador
	{
		void ClickButton(String idOuNome, Boolean click);
		void ClickLink(String idOuNome, Boolean click);
		INavegador GetDiv(String idOuNome);
		String GetHtml();
		INavegador IrPara(String endereco, Int32 timeOut, String textoParaContinuar);
		void SetCombo(String idOuNome, String valor, Boolean forceChange);
		void SetText(String idOuNome, String valor, Boolean forceChange);

		String GetTextValue(String idOuNome, Boolean isValue);
		String GetComboValue(String idOuNome, Boolean isValue);
		String GetButtonValue(String idOuNome, Boolean isValue);
		String GetLinkValue(String idOuNome, Boolean isValue);
	}
}