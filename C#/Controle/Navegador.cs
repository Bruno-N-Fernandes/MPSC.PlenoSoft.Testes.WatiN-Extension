using System;
using WatiN.Core;

namespace MPSC.PlenoSoft.Testes.WatiN.Extension.Controle
{
	public class Navegador : INavegador, IDisposable
	{
		public static INavegadorFactory fabrica = new NavegadorFactory();
		private readonly ITimeOut tempoDecorrido = new TimeOut();
		private readonly IElementContainer _iElementContainer;
		private readonly Browser _browser;

		public Navegador(TipoNavegador tipoNavegador)
		{
			_browser = fabrica.ObterNavegador(tipoNavegador);
			WatiNExtension.Wait();
			_browser.BringToFront();
			WatiNExtension.Wait();
			_iElementContainer = _browser;
		}

		private Navegador(Browser browser, IElementContainer iElementContainer)
		{
			_browser = browser;
			_iElementContainer = iElementContainer;
		}

		public INavegador IrPara(String endereco, Int32 timeOut, String textoParaContinuar)
		{
			if (String.IsNullOrEmpty(textoParaContinuar))
			{
				_browser.GoTo(endereco);
				_browser.WaitForComplete(timeOut);
			}
			else
			{
				_browser.GoToNoWait(endereco);
				tempoDecorrido.ReIniciar();
				while (tempoDecorrido.MenorQue(timeOut).Segundos && !_browser.ContainsText(textoParaContinuar) && !_browser.ContainsText(textoParaContinuar.ToLower()) && !_browser.ContainsText(textoParaContinuar.ToUpper()))
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

		public void SetText(String idOuNome, String valor, Boolean forceChange)
		{
			_iElementContainer.TextField(e => e.FindByIdOrName(idOuNome)).Select(valor, forceChange);
		}

		public void SetCombo(String idOuNome, String valor, Boolean forceChange)
		{
			_iElementContainer.SelectList(e => e.FindByIdOrName(idOuNome)).Select(valor, forceChange);
		}

		public void ClickLink(String idOuNome, Boolean click)
		{
			_iElementContainer.Link(e => e.FindByIdOrName(idOuNome)).Select(click, true);
		}

		public void ClickButton(String idOuNome, Boolean click)
		{
			_iElementContainer.Button(e => e.FindByIdOrName(idOuNome)).Select(click, true);
		}

		public INavegador GetDiv(String idOuNome)
		{
			var element = _iElementContainer.Div(e => e.FindByIdOrName(idOuNome));
			return (element.Exists ? new Navegador(_browser, element) : null);
		}

		~Navegador()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
			try
			{
				if (Object.ReferenceEquals(_browser, _iElementContainer))
				{
					_browser.Close();
					_browser.Dispose();
				}
			}
			catch { }
			finally { GC.Collect(); }
		}

		public String GetHtml()
		{
			return _browser.Html;
		}
	}

	public abstract class GetSet
	{
		protected INavegador _iNavegador;
		public void Configurar(INavegador iNavegador)
		{
			_iNavegador = iNavegador;
		}

		public abstract void Set(String idOuNome, Object valor, Boolean forceChange);
		public abstract Object Get(String idOuNome, Boolean isValue);
	}

	public class InputText : GetSet
	{
		public override void Set(String idOuNome, Object valor, Boolean forceChange)
		{
			_iNavegador.SetText(idOuNome, Convert.ToString(valor), forceChange);
		}

		public override Object Get(String idOuNome, Boolean isValue)
		{
			return _iNavegador.GetTextValue(idOuNome, isValue);
		}
	}

	public class InputSelect : GetSet
	{
		public override void Set(String idOuNome, Object valor, Boolean forceChange)
		{
			_iNavegador.SetCombo(idOuNome, Convert.ToString(valor), forceChange);
		}

		public override Object Get(String idOuNome, Boolean isValue)
		{
			return _iNavegador.GetComboValue(idOuNome, isValue);
		}
	}

	public class InputLink : GetSet
	{
		public override void Set(String idOuNome, Object valor, Boolean forceChange)
		{
			_iNavegador.ClickLink(idOuNome, Convert.ToString(valor).ToUpper().Contains("CLICK") || forceChange);
		}

		public override Object Get(String idOuNome, Boolean isValue)
		{
			return _iNavegador.GetLinkValue(idOuNome, isValue);
		}
	}

	public class InputButton : GetSet
	{
		public override void Set(String idOuNome, Object valor, Boolean forceChange)
		{
			_iNavegador.ClickButton(idOuNome, Convert.ToString(valor).ToUpper().Contains("CLICK") || forceChange);
		}

		public override Object Get(String idOuNome, Boolean isValue)
		{
			return _iNavegador.GetButtonValue(idOuNome, isValue);
		}
	}
}