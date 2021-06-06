using System.Collections.Generic;

namespace GestaoDeProjetos
{
    class Redes : Colaborador
    {
        public Redes(int nif, string nome, string morada, int nivelDeHabilitacao, double valorHora, List<Tecnologia> tecnologiasDeDominio,
            byte[] foto) : base(nif, nome, morada, nivelDeHabilitacao, valorHora, tecnologiasDeDominio, foto)
        {
            tipoColaborador = "Gestor de Redes";
        }
    }
}
