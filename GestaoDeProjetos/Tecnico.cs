using System.Collections.Generic;

namespace GestaoDeProjetos
{
    class Tecnico : Colaborador
    {
        public Tecnico(int nif, string nome, string morada, int nivelDeHabilitacao, double valorHora, List<Tecnologia> tecnologiasDeDominio,
            byte[] foto) : base(nif, nome, morada, nivelDeHabilitacao, valorHora, tecnologiasDeDominio, foto)
        {
            tipoColaborador = "Operacional Técnico";
        }
    }
}
