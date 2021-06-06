using System.Collections.Generic;

namespace GestaoDeProjetos
{
    class Programador : Colaborador
    {
        public Programador(int nif, string nome, string morada, int nivelDeHabilitacao, double valorHora, List<Tecnologia> tecnologiasDeDominio,
            byte[] foto) : base(nif, nome, morada, nivelDeHabilitacao, valorHora, tecnologiasDeDominio, foto)
        {
            tipoColaborador = "Programador";
        }
    }
}
