using System.Collections.Generic;

namespace GestaoDeProjetos
{
    interface IGestorProjetos
    {
        bool EditaColaborador(Colaborador colaborador, int[] idTecnologias);
        bool InsereColaborador(Colaborador colaborador, int[] idTecnologias);
        bool EliminaColaborador(int nif);
        bool EliminaColaboradorDoProjeto(int nif, int idProjeto);

        bool InsereTecnologiaProjeto(int idTecnologia, int idProjeto);
        bool InsereColaboradorProjeto(ColaboradorAssociado colaborador, int idProjeto);
        bool InsereProjeto(string nomeProjeto, string descricao, int[] idTecnologias, List<ColaboradorAssociado> colaboradores);
        bool EliminaProjeto(int idProjeto);
        bool EditaProjeto(Projeto projeto);
        bool EditaColaboradoresProjeto(Projeto projetoAntigo, Projeto projetoNovo);
        bool EditaTecnologiasColaboradoresProjeto(Projeto projetoAntigo, Projeto projetoNovo);

        bool InsereTecnologia(Tecnologia tecnologia);
        bool EditaTecnologia(Tecnologia tecnologia);
        bool EliminaTecnologia(int idTecnologia);

        Colaborador ProcuraColaborador(int nif);
        List<Colaborador> ProcuraTodosColaboradores();
        List<Colaborador> ProcuraColaboradoresPorTec(int idTecnologia);
        List<ColaboradorAssociado> ProcuraProjetoColaboradores(int idProjeto);

        List<Tecnologia> ProcuraColaboradorTecnologias(int nif);
        List<Tecnologia> ProcuraProjetoTecnologias(int idProjeto);
        List<Tecnologia> ProcuraTodasTecnologias();
        Tecnologia ProcuraTecnologia(int idTecnologia);

        List<Projeto> ProcuraTodosProjetos();
        List<Projeto> ProcuraProjetoPorColaborador(int nif);
        List<Projeto> ProcuraProjetoPorTecnologia(int idTecnologia);
        Projeto ProcuraProjeto(int idProjeto);
        int BuscaIdProjeto(string nomeProjeto, string descricao);

        double[] CustoProjeto(int idProjeto);
    }
}
