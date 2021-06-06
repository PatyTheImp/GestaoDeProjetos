using System.Collections.Generic;

namespace GestaoDeProjetos
{
    abstract class Colaborador
    {
        protected int nif;
        protected string nome;
        protected string morada;
        protected int nivelDeHabilitacao;
        protected double valorHora;
        protected List<Tecnologia> tecnologiasDeDominio;
        //Imagens ficam guardadas na base de dados como um array de bytes
        protected byte[] foto;
        protected string tipoColaborador;

        public Colaborador(int nif, string nome, string morada, int nivelDeHabilitacao, double valorHora,
            List<Tecnologia> tecnologiasDeDominio, byte[] foto)
        {
            this.nif = nif;
            this.nome = nome;
            this.morada = morada;
            this.nivelDeHabilitacao = nivelDeHabilitacao;
            this.valorHora = valorHora;
            this.tecnologiasDeDominio = tecnologiasDeDominio;
            this.foto = foto;
        }

        public int Nif
        {
            get { return nif; }
            set { nif = value; }
        }

        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        public string Morada
        {
            get { return morada; }
            set { morada = value; }
        }

        public int NivelDeHabilitacao
        {
            get { return nivelDeHabilitacao; }
            set { nivelDeHabilitacao = value; }
        }

        public double ValorHora
        {
            get { return valorHora; }
            set { valorHora = value; }
        }

        public List<Tecnologia> TecnologiasDeDominio
        {
            get { return tecnologiasDeDominio; }
            set { tecnologiasDeDominio = value; }
        }

        public byte[] Foto
        {
            get { return foto; }
            set { foto = value; }
        }

        public string TipoColaborador
        {
            get { return tipoColaborador; }
        }
    }
}
