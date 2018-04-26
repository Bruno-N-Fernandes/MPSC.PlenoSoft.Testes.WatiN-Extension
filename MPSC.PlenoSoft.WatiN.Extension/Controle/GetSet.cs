using System;

namespace MPSC.PlenoSoft.WatiN.Extension.Controle
{
	public abstract class GetSet
	{
		protected Navegador navegador;
		public void Configurar(Navegador iNavegador)
		{
			navegador = iNavegador;
		}

		public abstract void Set(String idOuNome, Object valor, Boolean forceChange = false);
		public abstract Object Get(String idOuNome, Boolean isValue);
	}
}