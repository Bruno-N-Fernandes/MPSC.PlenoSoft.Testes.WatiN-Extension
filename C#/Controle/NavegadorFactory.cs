using WatiN.Core;

namespace MPSC.PlenoSoft.Testes.WatiN.Extension.Controle
{
	public enum TipoNavegador
	{
		InternetExplorer = 0,
		FireFox = 1,
	}

	public interface INavegadorFactory
	{
		Browser ObterNavegador(TipoNavegador tipoNavegador);
		Browser ObterNavegador<TBrowser>() where TBrowser : Browser, new();
	}

	public class NavegadorFactory : INavegadorFactory
	{
		public virtual Browser ObterNavegador(TipoNavegador tipoNavegador)
		{
			switch (tipoNavegador)
			{
				case TipoNavegador.InternetExplorer:
					return WatiNExtension.ObterNavegador<IE>();
				case TipoNavegador.FireFox:
					return WatiNExtension.ObterNavegador<FireFox>();
				default:
					return WatiNExtension.ObterNavegador<IE>();
			}
		}

		public virtual Browser ObterNavegador<TBrowser>() where TBrowser : Browser, new()
		{
			return WatiNExtension.ObterNavegador<TBrowser>();
		}
	}
}