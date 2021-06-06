namespace GestaoDeProjetos
{
    class Tecnologia
    {
        private int id;
        private string nome;
        private string areaDeAtividade;
        private double custo;

        public Tecnologia(int id, string nome, string areaDeAtividade, double custo)
        {
            this.id = id;
            this.nome = nome;
            this.areaDeAtividade = areaDeAtividade;
            this.custo = custo;
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

        public string AreaDeAtividade
        {
            get { return areaDeAtividade; }
            set { areaDeAtividade = value; }
        }

        public double Custo
        {
            get { return custo; }
            set { custo = value; }
        }
    }
}
