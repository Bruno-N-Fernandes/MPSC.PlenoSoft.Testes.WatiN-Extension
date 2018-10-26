using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSC.PlenoSoft.WatiN.Extension.Util;
using WatiN.Core;

namespace MPSC.PlenoSoft.WatiN.Extension.Testes.Unidade
{
	[TestClass]
	public class Testando_WatiN
	{
		[TestMethod]
		public void Exemplo_De_Como_Automatizar_Pesquisa_No_Google()
		{
			var browser = WatiNExtension.ObterNavegador<IE>();
			browser.GoTo("https://www.google.com/search?q=WatiN");
			var html = browser.Html;
			Assert.IsNotNull(html);
		}

		[TestMethod]
		public void Exemplo_De_Como_Automatizar_Pesquisa_De_CEP_Nos_Correios()
		{
			var navegador = Navegador.New(TipoNavegador.InternetExplorer);
			navegador.IrPara("http://www.buscacep.correios.com.br/servicos/dnec/menuAction.do?Metodo=menuCep", TimeSpan.FromSeconds(10), "CEP");
			navegador.SetText("relaxation", "20090000", false);
			navegador.ClickButton("Buscar", true, true);
			var html = navegador.GetHtml();
			navegador.Fechar();
			Assert.IsNotNull(html);
		}
	}
}