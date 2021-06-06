namespace GestaoDeProjetos
{
    class ColaboradorAssociado
    {
        Colaborador colaborador;
        private int horasAfeto;
        private double custoNoProjeto;

        public ColaboradorAssociado(Colaborador colaborador, int horasAfeto)
        {
            this.colaborador = colaborador;
            this.horasAfeto = horasAfeto;
            custoNoProjeto = horasAfeto * colaborador.ValorHora;
        }

        public Colaborador Colaborador
        {
            get { return colaborador; }
        }

        public int HorasAfeto
        {
            get { return horasAfeto; }
        }

        public double CustoNoProjeto
        {
            get { return custoNoProjeto; }
        }
    }
}
