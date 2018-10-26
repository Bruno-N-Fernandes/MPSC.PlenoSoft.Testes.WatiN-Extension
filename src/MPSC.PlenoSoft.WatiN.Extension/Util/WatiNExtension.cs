using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using WatiN.Core;
using WatiN.Core.Exceptions;
using WatiN.Core.Native.Windows;

namespace MPSC.PlenoSoft.WatiN.Extension.Util
{
	public enum TipoNavegador
	{
		InternetExplorer = 0,
		FireFox = 1,
	}

	public static class WatiNExtension
	{
		private static readonly TimeOut tempoDecorrido = new TimeOut();

		public static Browser ObterNavegador(TipoNavegador tipoNavegador)
		{
			switch (tipoNavegador)
			{
				case TipoNavegador.InternetExplorer:
					return ObterNavegador<IE>();
				case TipoNavegador.FireFox:
					return ObterNavegador<FireFox>();
				default:
					return ObterNavegador<IE>();
			}
		}

		public static Browser ObterNavegador<TBrowser>() where TBrowser : Browser, new()
		{
			var browser = new TBrowser();
			browser.ShowWindow(NativeMethods.WindowShowStyle.Maximize);
			browser.BringToFront();
			return browser;
		}

		public static Boolean IrParaEndereco(this Browser browser, String endereco, Int32 loop)
		{
			try
			{
				browser.GoTo(endereco);
				Wait();
				return browser.Title != null;
			}
			catch (COMException) { return false; }
			catch (Exception)
			{
				Wait();
				return (loop < 5) ? IrParaEndereco(browser, endereco, loop + 1) : false;
			}
		}

		public static Boolean EstaPreenchido(this Document document, String idOrName, String valorDefault)
		{
			try
			{
				var element = document.TextField(e => e.FindByIdOrName(idOrName));
				valorDefault = ((element != null) && element.Exists && element.Enabled) ? element.Value : null;
			}
			catch (Exception) { /* Ignorar o erro */ }

			return !String.IsNullOrWhiteSpace(valorDefault);
		}

		public static Boolean ContainsAllText(this Document document, params String[] textos)
		{
			Wait();
			return containsTextImpl(document, true, textos);
		}

		public static Boolean ContainsAnyText(this Document document, params String[] textos)
		{
			Wait();
			return containsTextImpl(document, false, textos)
				|| containsTextImpl(document, false, textos.ToUpper())
				|| containsTextImpl(document, false, textos.ToLower())
			;
		}

		public static Boolean WaitContainsAllText(this Document document, Int32 segundos, params String[] textos)
		{
			return waitContainsTextImpl(document, segundos, true, textos);
		}

		public static Boolean WaitContainsAnyText(this Document document, Int32 segundos, params String[] textos)
		{
			return waitContainsTextImpl(document, segundos, false, textos);
		}

		public static void Wait(TimeSpan? timeOut = null)
		{
			tempoDecorrido.ReIniciar();
			while (tempoDecorrido.MenorQue(timeOut))
				Thread.Sleep(50);
		}

		public static Boolean FindByAny(this Element element, String pesquisa)
		{
			return element.FindByIdOrName(pesquisa) || element.FindByText(pesquisa) || element.FindByClassName(pesquisa);
		}

		public static Boolean FindByIdOrName(this Element element, String idOrName)
		{
			var achou = false;
			var erro = true;

			tempoDecorrido.ReIniciar();
			while (erro && (element != null) && tempoDecorrido.MenorQue(TimeOut.CincoSegundos))
			{
				try
				{
					var nivel = 0;
					while ((nivel++ < 16) && !achou)
					{
						achou = achou || comparar(element.Id, idOrName, nivel);
						achou = achou || comparar(element.Name, idOrName, nivel);
					}
					achou = achou || FindByValue(element as Button, idOrName);

					erro = false;
				}
				catch (ElementNotFoundException)
				{
					erro = false;
					achou = false;
				}
				catch (Exception)
				{
					erro = true;
					Wait();
				}
			}

			return achou;
		}

		public static Boolean FindByValue(this Button element, String texto)
		{
			var achou = false;
			var erro = true;

			tempoDecorrido.ReIniciar();
			while (erro && (element != null) && tempoDecorrido.MenorQue(TimeOut.CincoSegundos))
			{
				try
				{
					var nivel = 0;
					while ((nivel++ < 16) && !achou)
					{
						achou = achou || comparar(element.Value, texto, nivel);
					}
					erro = false;
				}
				catch (ElementNotFoundException)
				{
					erro = false;
					achou = false;
				}
				catch (Exception)
				{
					erro = true;
					Wait();
				}
			}

			return achou;
		}

		public static Boolean FindByText(this Element element, String texto)
		{
			var achou = false;
			var erro = true;

			tempoDecorrido.ReIniciar();
			while (erro && (element != null) && tempoDecorrido.MenorQue(TimeOut.CincoSegundos))
			{
				try
				{
					var nivel = 0;
					while ((nivel++ < 16) && !achou)
					{
						achou = achou || comparar(element.Text, texto, nivel);
						achou = achou || comparar(element.OuterText, texto, nivel);
						achou = achou || comparar(element.OuterHtml, texto, nivel);
						achou = achou || comparar(element.InnerHtml, texto, nivel);
					}
					achou = achou || FindByValue(element as Button, texto);

					erro = false;
				}
				catch (ElementNotFoundException)
				{
					erro = false;
					achou = false;
				}
				catch (Exception)
				{
					erro = true;
					Wait();
				}
			}

			return achou;
		}

		public static Boolean FindByClassName(this Element element, String classe)
		{
			var achou = false;
			var erro = true;

			tempoDecorrido.ReIniciar();
			while (erro && (element != null) && tempoDecorrido.MenorQue(TimeOut.CincoSegundos))
			{
				try
				{
					var nivel = 0;
					while ((nivel++ < 16) && !achou)
						if (!achou) achou = comparar(element.ClassName, classe, nivel);

					erro = false;
				}
				catch (ElementNotFoundException)
				{
					erro = false;
					achou = false;
				}
				catch (Exception)
				{
					erro = true;
					Wait();
				}
			}

			return achou;
		}

		public static Link Select(this Link element, Boolean click, Boolean forceChange)
		{
			if (element != null)
			{
				try
				{
					if (element.Exists && element.HabilitarByJavaScript(true))
					{
						if (forceChange || !click)
						{
							element.Focus();
							element.MouseEnter();
						}

						if (click)
						{
							if (forceChange) element.MouseDown();
							element.Click();
							if (forceChange) element.MouseUp();
						}
					}
				}
				catch (COMException) { element = null; }
				catch (Exception) { /* Ignorar */ }
			}
			return element;
		}

		public static Button Select(this Button element, Boolean click, Boolean forceChange)
		{
			if (element != null)
			{
				try
				{
					if (element.Exists && element.HabilitarByJavaScript(true))
					{
						if (forceChange || !click)
						{
							element.Focus();
							element.MouseEnter();
						}

						if (click)
						{
							if (forceChange && element.Exists) element.MouseDown();
							element.Click();
							if (forceChange && element.Exists) element.MouseUp();
						}
					}
				}
				catch (COMException) { element = null; }
				catch (Exception) { /* Ignorar */ }
			}
			return element;
		}

		public static TextField Select(this TextField element, String text, Boolean forceChange)
		{
			if (element != null)
			{
				try
				{
					if (element.Exists && element.HabilitarByJavaScript(true))
					{
						if (forceChange)
						{
							element.MouseDown();
							element.MouseEnter();
							element.Click();
							element.Focus();
							element.MouseUp();
							element.Select();
							element.Clear();
							element.TypeText(text ?? String.Empty);
						}

						element.Value = text ?? String.Empty;

						if (forceChange)
						{
							element.Refresh();
							element.RefreshNativeElement();
							element.KeyDown();
							element.KeyPress();
							element.KeyUp();
							element.Change();
							element.Blur();
							if (!element.forceChangeByJQuery())
								element.forceChangeByJavaScript();
						}
					}
				}
				catch (COMException) { element = null; }
				catch (Exception) { /* Ignorar */ }
			}
			return element;
		}

		public static SelectList Select(this SelectList element, Object valor, Boolean forceChange)
		{
			var texto = Convert.ToString(valor).ToUpper().Trim();
			if (texto.StartsWith("TEXT="))
				SelectByText(element as SelectList, Convert.ToString(valor).Substring(5), forceChange);
			else if (texto.StartsWith("VALUE="))
				SelectByValue(element as SelectList, Convert.ToString(valor).Substring(6), forceChange);
			else
				Select(element as SelectList, Convert.ToString(valor), forceChange);
			return element;
		}

		public static SelectList Select(this SelectList element, String textOrValue, Boolean forceChange)
		{
			if (textOrValue.All(chr => Char.IsDigit(chr)))
				SelectByValue(element as SelectList, textOrValue, forceChange);
			else
				SelectByText(element as SelectList, textOrValue, forceChange);
			return element;
		}

		public static SelectList Select(this SelectList element, Int64 value, Boolean forceChange)
		{
			return SelectByValue(element, value.ToString(), forceChange);
		}

		public static SelectList SelectByValue(this SelectList element, String value, Boolean forceChange)
		{
			if (element != null)
			{
				try
				{
					if (element.Exists && element.HabilitarByJavaScript(true))
					{
						if (forceChange)
							element.Focus();

						element.SelectByValue(value);

						if (forceChange)
						{
							element.Change();
							element.Blur();
							if (!element.forceChangeByJQuery())
								element.forceChangeByJavaScript();
						}
					}
				}
				catch (COMException) { element = null; }
				catch (Exception) { /* Ignorar */ }
			}
			return element;
		}

		public static SelectList SelectByText(this SelectList element, String text, Boolean forceChange)
		{
			if (element != null)
			{
				try
				{
					if (element.Exists && element.HabilitarByJavaScript(true))
					{
						if (forceChange)
							element.Focus();

						element.Select(text);

						if (forceChange)
						{
							element.Change();
							element.Blur();
							if (!element.forceChangeByJQuery())
								element.forceChangeByJavaScript();
						}
					}
				}
				catch (COMException) { element = null; }
				catch (Exception) { /* Ignorar */ }
			}
			return element;
		}



		public static Boolean HabilitarByJavaScript(this Element element, Boolean enabled)
		{
			try
			{
				if (!element.Enabled)
				{
					var id = element.Id ?? element.GetJavascriptElementReference();
					var script = String.Format("document.getElementById('{0}').disabled={1};", id, Convert.ToString(!enabled).ToLower());
					element.DomContainer.Eval(script);
				}
			}
			catch (Exception) { }

			return element.Enabled == enabled;
		}

		private static Boolean forceChangeByJavaScript(this Element element)
		{
			try
			{
				var id = element.Id ?? element.GetJavascriptElementReference();
				var script = String.Format("document.getElementById('{0}').click();", id);

				element.FireEvent("onchange");
				element.DomContainer.Eval(script);
				return true;
			}
			catch (Exception) { return false; }
		}

		private static Boolean forceChangeByJQuery(this Element element)
		{
			try
			{
				var id = element.Id ?? element.GetJavascriptElementReference();
				var script = String.Format("$('#{0}').change();", id);

				element.DomContainer.Eval(script);
				return true;
			}
			catch (Exception) { return false; }
		}

		private static Boolean waitContainsTextImpl(this Document document, Int32 segundos, Boolean all, params String[] textos)
		{
			Wait();
			while ((--segundos > 0) && !containsTextImpl(document, all, textos))
				waitContainsTextImpl(document, segundos, all, textos);
			return containsTextImpl(document, all, textos);
		}

		private static Boolean containsTextImpl(this Document document, Boolean all, params String[] textos)
		{
			var achou = all;
			foreach (var texto in textos)
			{
				try
				{
					if (all)
						achou = achou && document.ContainsText(texto);
					else
						achou = achou || document.ContainsText(texto);
				}
				catch (COMException) { achou = false; }
				catch (Exception) { }
			}
			return achou;
		}

		private static Boolean comparar(String propriedade, String valor, Int32 nivel)
		{
			var achou = false;
			if ((propriedade != null) && (valor != null))
			{
				if ((nivel == 01) && !achou) achou = propriedade.Equals(valor);
				if ((nivel == 02) && !achou) achou = propriedade.ToUpper().Equals(valor.ToUpper());
				if ((nivel == 03) && !achou) achou = propriedade.Trim().Equals(valor.Trim());
				if ((nivel == 04) && !achou) achou = propriedade.Trim().ToUpper().Equals(valor.Trim().ToUpper());

				if ((nivel == 05) && !achou) achou = propriedade.EndsWith(valor);
				if ((nivel == 06) && !achou) achou = propriedade.ToUpper().EndsWith(valor.ToUpper());
				if ((nivel == 07) && !achou) achou = propriedade.Trim().EndsWith(valor.Trim());
				if ((nivel == 08) && !achou) achou = propriedade.Trim().ToUpper().EndsWith(valor.Trim().ToUpper());

				if ((nivel == 09) && !achou) achou = propriedade.StartsWith(valor);
				if ((nivel == 10) && !achou) achou = propriedade.ToUpper().StartsWith(valor.ToUpper());
				if ((nivel == 11) && !achou) achou = propriedade.Trim().StartsWith(valor.Trim());
				if ((nivel == 12) && !achou) achou = propriedade.Trim().ToUpper().StartsWith(valor.Trim().ToUpper());

				if ((nivel == 13) && !achou) achou = propriedade.Contains(valor);
				if ((nivel == 14) && !achou) achou = propriedade.ToUpper().Contains(valor.ToUpper());
				if ((nivel == 15) && !achou) achou = propriedade.Trim().Contains(valor.Trim());
				if ((nivel == 16) && !achou) achou = propriedade.Trim().ToUpper().Contains(valor.Trim().ToUpper());
			}
			return achou;
		}
	}
}