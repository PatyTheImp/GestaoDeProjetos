using System.Collections.Generic;

namespace GestaoDeProjetos
{
    class Projeto
    {
        private int id;
        private string nome;
        private string descricao;
        private List<Tecnologia> tecnologiasNecessarias;
        private List<ColaboradorAssociado> colaboradoresAssociados;

        public Projeto(int id, string nome, string descricao, List<Tecnologia> tecnologias, List<ColaboradorAssociado> colaboradores)
        {
            this.id = id;
            this.nome = nome;
            this.descricao = descricao;
            tecnologiasNecessarias = tecnologias;
            colaboradoresAssociados = colaboradores;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        public string Descricao
        {
            get { return descricao; }
            set { descricao = value; }
        }

        public List<Tecnologia> TecnologiasNecessarias
        {
            get { return tecnologiasNecessarias; }
            set { tecnologiasNecessarias = value; }
        }

        public List<ColaboradorAssociado> ColaboradoresAssociados
        {
            get { return colaboradoresAssociados; }
            set { colaboradoresAssociados = value; }
        }
    }
}
