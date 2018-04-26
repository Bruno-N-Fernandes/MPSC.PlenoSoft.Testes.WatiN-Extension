using System;
using MPSC.PlenoSoft.WatiN.Extension.Util;
using WatiN.Core;

namespace MPSC.PlenoSoft.WatiN.Extension
{
	public class Navegador : IDisposable
	{
		private readonly TimeOut tempoDecorrido = new TimeOut();
		private readonly IElementContainer _iElementContainer;
		private readonly Browser _browser;

		private Navegador(Browser browser, IElementContainer iElementContainer)
		{
			_browser = browser;
			_iElementContainer = iElementContainer;
		}

		public static Navegador New(TipoNavegador tipoNavegador)
		{
			var browser = WatiNExtension.ObterNavegador(tipoNavegador);
			return new Navegador(browser, browser);
		}

		public static Navegador New<TBrowser>() where TBrowser : Browser, new()
		{
			var browser = WatiNExtension.ObterNavegador<TBrowser>();
			return new Navegador(browser, browser);
		}

		public Navegador IrPara(String endereco, TimeSpan? timeOut = null, String textoParaContinuar = null)
		{
			if (String.IsNullOrEmpty(textoParaContinuar))
			{
				_browser.GoTo(endereco);
				_browser.WaitForComplete((timeOut ?? TimeOut.CincoSegundos).GetTotalSeconds());
			}
			else
			{
				_browser.GoToNoWait(endereco);
				tempoDecorrido.ReIniciar();
				while (tempoDecorrido.MenorQue(timeOut) && !_browser.ContainsAnyText(textoParaContinuar))
					WatiNExtension.Wait();
			}
			return this;
		}

		public String GetTextValue(String idOuNome, Boolean isValue)
		{
			var element = _iElementContainer.TextField(e => e.FindByIdOrName(idOuNome));
			return isValue ? element.Value : element.Text;
		}

		public String GetComboValue(String idOuNome, Boolean isValue)
		{
			var element = _iElementContainer.SelectList(e => e.FindByIdOrName(idOuNome));
			return isValue ? element.SelectedOption.Value : element.Text;
		}

		public String GetButtonValue(String idOuNome, Boolean isValue)
		{
			var element = _iElementContainer.Button(e => e.FindByIdOrName(idOuNome));
			return isValue ? element.Value : element.Text;
		}

		public String GetLinkValue(String idOuNome, Boolean isValue)
		{
			var element = _iElementContainer.Link(e => e.FindByIdOrName(idOuNome));
			return isValue ? element.Url : element.Title;
		}

		public void SetText(String idOuNome, String valor, Boolean forceChange = false)
		{
			_iElementContainer.TextField(e => e.FindByIdOrName(idOuNome)).Select(valor, forceChange);
		}

		public void SetCombo(String idOuNome, String valor, Boolean forceChange = false)
		{
			_iElementContainer.SelectList(e => e.FindByIdOrName(idOuNome)).Select(valor, forceChange);
		}

		public void ClickLink(String idOuNome, Boolean click, Boolean forceChange = false)
		{
			_iElementContainer.Link(e => e.FindByIdOrName(idOuNome)).Select(click, forceChange);
		}

		public void ClickButton(String idOuNome, Boolean click, Boolean forceChange = false)
		{
			_iElementContainer.Button(e => e.FindByIdOrName(idOuNome)).Select(click, forceChange);
		}

		public Navegador GetDiv(String idOuNome)
		{
			var element = _iElementContainer.Div(e => e.FindByIdOrName(idOuNome));
			return (element.Exists ? new Navegador(_browser, element) : null);
		}

		public String GetHtml()
		{
			return _browser.Html;
		}

		public String GetUrl()
		{
			return _browser.Url;
		}

		~Navegador()
		{
			Dispose();
		}

		public void Fechar()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
			try
			{
				if (Object.ReferenceEquals(_browser, _iElementContainer))
				{
					_browser?.Close();
					_browser?.Dispose();
				}
			}
			catch { }
			finally { GC.Collect(); }
		}
	}
}