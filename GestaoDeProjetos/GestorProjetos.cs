using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GestaoDeProjetos
{
    class GestorProjetos : IGestorProjetos
    {
        private SqlConnection connection;

        public GestorProjetos()
        {
            connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=K:\OneDrive\informática\tarefas\Csharp\ProjetoFinal\GestaoDeProjetos\GestaoDeProjetos\GestaoProjetosDB.mdf;Integrated Security=True");
        }

        public bool EditaColaborador(Colaborador colaborador, int[] idTecnologias)
        {
            SqlCommand commandColab = new("UPDATE Colaborador " +
                "SET nome = @nome, morada = @morada, nivel_de_habilitacao = @nivel_de_habilitacao, valor_hora = @valor_hora, foto = @foto " +
                "WHERE nif = @nif;", connection);

            commandColab.Parameters.AddWithValue("@nif", colaborador.Nif);
            commandColab.Parameters.AddWithValue("@nome", colaborador.Nome);
            commandColab.Parameters.AddWithValue("@morada", colaborador.Morada);
            commandColab.Parameters.AddWithValue("@nivel_de_habilitacao", colaborador.NivelDeHabilitacao);
            commandColab.Parameters.AddWithValue("@valor_hora", colaborador.ValorHora);
            commandColab.Parameters.AddWithValue("@foto", colaborador.Foto);

            try
            {
                connection.Open();
                commandColab.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            SqlCommand cmdTecnologiaColaborador = new("DELETE FROM TecnologiaColaborador WHERE colaborador_nif = @nif", connection);
            cmdTecnologiaColaborador.Parameters.AddWithValue("@nif", colaborador.Nif);

            try
            {
                connection.Open();
                cmdTecnologiaColaborador.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            foreach (int idTec in idTecnologias)
            {
                SqlCommand commandTecColab = new("INSERT INTO TecnologiaColaborador (tecnologia_id, colaborador_nif) " +
                "VALUES (@tecId, @colabNif);", connection);
                connection.Open();
                commandTecColab.Parameters.AddWithValue("@tecId", idTec);
                commandTecColab.Parameters.AddWithValue("@colabNif", colaborador.Nif);

                try
                {
                    commandTecColab.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception)
                {
                    connection.Close();
                    return false;
                }
            }

            return true;
        }

        //Insere um colaborador na tabela Colaborador e as tecnologias associadas na tabela TecnologiaColaborador
        public bool InsereColaborador(Colaborador colaborador, int[] idTecnologias)
        {
            SqlCommand commandColab = new("INSERT INTO Colaborador VALUES (@nif, @nome, @morada, @nivel_de_habilitacao, " +
                "@valor_hora, @foto);", connection);

            commandColab.Parameters.AddWithValue("@nif", colaborador.Nif);
            commandColab.Parameters.AddWithValue("@nome", colaborador.Nome);
            commandColab.Parameters.AddWithValue("@morada", colaborador.Morada);
            commandColab.Parameters.AddWithValue("@nivel_de_habilitacao", colaborador.NivelDeHabilitacao);
            commandColab.Parameters.AddWithValue("@valor_hora", colaborador.ValorHora);
            commandColab.Parameters.AddWithValue("@foto", colaborador.Foto);
            int rows = 0;

            try
            {
                connection.Open();
                rows += commandColab.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            foreach (int idTec in idTecnologias)
            {
                SqlCommand commandTecColab = new("INSERT INTO TecnologiaColaborador (tecnologia_id, colaborador_nif) " +
                "VALUES (@tecId, @colabNif);", connection);
                connection.Open();
                commandTecColab.Parameters.AddWithValue("@tecId", idTec);
                commandTecColab.Parameters.AddWithValue("@colabNif", colaborador.Nif);

                try
                {
                    rows += commandTecColab.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception)
                {
                    connection.Close();
                    return false;
                }
            }


            //O número de rows afetadas deve ser igual a quantidade de tecnologias associadas ao colaborador (idTecnologias.Length)
            //e +1 que representa o colaborador a ser inserido na tabela Colaborador
            return rows == idTecnologias.Length + 1;
        }

        //Elimina o colaborador das tabelas Colaborador, ColaboradorProjeto e TecnologiaColaborador
        public bool EliminaColaborador(int nif)
        {
            SqlCommand cmdColaboradorProjeto = new("DELETE FROM ColaboradorProjeto WHERE colaborador_nif = @nif", connection);
            cmdColaboradorProjeto.Parameters.AddWithValue("@nif", nif);

            try
            {
                connection.Open();
                cmdColaboradorProjeto.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            SqlCommand cmdTecnologiaColaborador = new("DELETE FROM TecnologiaColaborador WHERE colaborador_nif = @nif", connection);
            cmdTecnologiaColaborador.Parameters.AddWithValue("@nif", nif);

            try
            {
                connection.Open();
                cmdTecnologiaColaborador.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            //Elimina-se primeiro o colaborador das tabelas auxiliares visto conterem chaves estrangeiras.
            SqlCommand cmdColaborador = new("DELETE FROM Colaborador WHERE nif = @nif", connection);
            cmdColaborador.Parameters.AddWithValue("@nif", nif);

            try
            {
                connection.Open();
                cmdColaborador.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            return true;
        }

        public bool EliminaColaboradorDoProjeto(int nif, int idProjeto)
        {
            SqlCommand command = new("DELETE FROM ColaboradorProjeto WHERE colaborador_nif = @nif AND projeto_id = @idProjeto", connection);
            command.Parameters.AddWithValue("@nif", nif);
            command.Parameters.AddWithValue("@idProjeto", idProjeto);
            int rows = 0;

            try
            {
                connection.Open();
                rows = command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            return rows == 1;
        }

        //Insere o id de uma tecnologia e o id do projeto em que está associada, na tabela TecnologiaProjeto
        public bool InsereTecnologiaProjeto(int idTecnologia, int idProjeto)
        {
            SqlCommand command = new("INSERT INTO TecnologiaProjeto (tecnologia_id, projeto_id) VALUES" +
                " (@idTec, @idProjeto);", connection);
            command.Parameters.AddWithValue("@idTec", idTecnologia);
            command.Parameters.AddWithValue("@idProjeto", idProjeto);
            int rows;

            try
            {
                connection.Open();
                rows = command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            return rows == 1;
        }

        //Insere o id de um projeto mais o id de um dos colabaradores que está asociado a esse projeto, assim como
        //a quantidade de horas esse colaborador vai estar no projeto, na tabela ColaboradorProjeto
        public bool InsereColaboradorProjeto(ColaboradorAssociado colaborador, int idProjeto)
        {
            SqlCommand command = new("INSERT INTO ColaboradorProjeto (colaborador_nif, projeto_id, horas_afeto) VALUES" +
                " (@nif, @idProjeto, @horas);", connection);
            command.Parameters.AddWithValue("@nif", colaborador.Colaborador.Nif);
            command.Parameters.AddWithValue("@idProjeto", idProjeto);
            command.Parameters.AddWithValue("@horas", colaborador.HorasAfeto);
            int rows;

            try
            {
                connection.Open();
                rows = command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            return rows == 1;
        }

        //Insere um projeto a tabela projetos, assim como as tecnologias e colaboradores associados ao projeto nas tabelas
        //TecnologiaProjeto e ColaboradorProjeto respectivamente
        public bool InsereProjeto(string nomeProjeto, string descricao, int[] idTecnologias, List<ColaboradorAssociado> colaboradores)
        {
            int idProjeto;
            bool tudoBem = true;

            SqlCommand command = new("INSERT INTO Projeto (nome, descricao) VALUES" +
                " (@nome, @descricao);", connection);
            command.Parameters.AddWithValue("@nome", nomeProjeto);
            command.Parameters.AddWithValue("@descricao", descricao);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            idProjeto = BuscaIdProjeto(nomeProjeto, descricao);

            if (idTecnologias.Length > 0)
            {
                foreach (int idTec in idTecnologias)
                {
                    if (!InsereTecnologiaProjeto(idTec, idProjeto))
                        tudoBem = false;
                }
            }

            if (colaboradores.Count > 0)
            {
                foreach (ColaboradorAssociado colaborador in colaboradores)
                {
                    if (!InsereColaboradorProjeto(colaborador, idProjeto))
                        tudoBem = false;
                }
            }

            //O método BuscaIdProjet devolve zero se não for encontrado nenhum projeto com as caracteristicas indicadas
            return tudoBem && idProjeto != 0;
        }

        public bool EliminaProjeto(int idProjeto)
        {
            SqlCommand cmdColaboradorProjeto = new("DELETE FROM ColaboradorProjeto WHERE projeto_id = @id", connection);
            cmdColaboradorProjeto.Parameters.AddWithValue("@id", idProjeto);

            try
            {
                connection.Open();
                cmdColaboradorProjeto.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            SqlCommand cmdTecnologiaProjeto = new("DELETE FROM TecnologiaProjeto WHERE projeto_id = @id", connection);
            cmdTecnologiaProjeto.Parameters.AddWithValue("@id", idProjeto);

            try
            {
                connection.Open();
                cmdTecnologiaProjeto.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            SqlCommand cmdProjeto = new("DELETE FROM Projeto WHERE id = @id", connection);
            cmdProjeto.Parameters.AddWithValue("@id", idProjeto);

            try
            {
                connection.Open();
                cmdProjeto.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            return true;
        }

        //Edita na tabela Projeto, o nome e a descrição do projeto
        public bool EditaProjeto(Projeto projeto)
        {
            SqlCommand command = new(
                "UPDATE Projeto " +
                "SET nome = @nome, descricao = @descricao " +
                "WHERE id = @id",
                connection);
            int row = 0;
            command.Parameters.AddWithValue("@nome", projeto.Nome);
            command.Parameters.AddWithValue("@descricao", projeto.Descricao);
            command.Parameters.AddWithValue("@id", projeto.Id);

            try
            {
                connection.Open();
                row = command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            return row == 1;
        }

        //Para além de editar a tabela Projeto, também apaga os colaboradores anteriormente associados ao projeto
        //e insere os novos na tabela ColaboradorProjeto
        public bool EditaColaboradoresProjeto(Projeto projetoAntigo, Projeto projetoNovo)
        {
            bool tudoBem = EditaProjeto(projetoNovo);

            if (tudoBem)
            {
                foreach (ColaboradorAssociado colaborador in projetoAntigo.ColaboradoresAssociados)
                {
                    SqlCommand command = new("DELETE FROM ColaboradorProjeto WHERE colaborador_nif = @nif AND projeto_id = @id", connection);
                    command.Parameters.AddWithValue("@nif", colaborador.Colaborador.Nif);
                    command.Parameters.AddWithValue("@id", projetoAntigo.Id);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception)
                    {
                        connection.Close();
                        return false;
                    }
                }

                foreach (ColaboradorAssociado associado in projetoNovo.ColaboradoresAssociados)
                {
                    SqlCommand command = new("INSERT INTO ColaboradorProjeto (colaborador_nif, projeto_id, horas_afeto) " +
                        "VALUES (@nif, @id, @horas)", connection);
                    command.Parameters.AddWithValue("@nif", associado.Colaborador.Nif);
                    command.Parameters.AddWithValue("@id", projetoNovo.Id);
                    command.Parameters.AddWithValue("@horas", associado.HorasAfeto);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception)
                    {
                        connection.Close();
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        //Para além de editar a tabela Projeto e a tabela ColaboradorProjeto, também apaga as tecnologias anteriormente
        //associadas ao projeto e insere as novas na tabela ColaboradorProjeto
        public bool EditaTecnologiasColaboradoresProjeto(Projeto projetoAntigo, Projeto projetoNovo)
        {
            bool tudoBem = EditaColaboradoresProjeto(projetoAntigo, projetoNovo);

            if (tudoBem)
            {
                foreach (Tecnologia tecnologia in projetoAntigo.TecnologiasNecessarias)
                {
                    SqlCommand command = new("DELETE FROM TecnologiaProjeto WHERE tecnologia_id = @idTec AND projeto_id = @idProjeto", connection);
                    command.Parameters.AddWithValue("@idTec", tecnologia.Id);
                    command.Parameters.AddWithValue("@idProjeto", projetoAntigo.Id);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception)
                    {
                        connection.Close();
                        return false;
                    }
                }

                foreach (Tecnologia tecnologia in projetoNovo.TecnologiasNecessarias)
                {
                    SqlCommand command = new("INSERT INTO TecnologiaProjeto (tecnologia_id, projeto_id) " +
                        "VALUES (@idTec, @idProjeto)", connection);
                    command.Parameters.AddWithValue("@idTec", tecnologia.Id);
                    command.Parameters.AddWithValue("@idProjeto", projetoNovo.Id);

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception)
                    {
                        connection.Close();
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public bool InsereTecnologia(Tecnologia tecnologia)
        {
            int row;

            SqlCommand command = new("INSERT INTO Tecnologia (nome, area, custo)" +
                " VALUES (@nome, @area, @custo);", connection);
            command.Parameters.AddWithValue("@nome", tecnologia.Nome);
            command.Parameters.AddWithValue("@area", tecnologia.AreaDeAtividade);
            command.Parameters.AddWithValue("@custo", tecnologia.Custo);

            try
            {
                connection.Open();
                row = command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            return row == 1;
        }

        public bool EditaTecnologia(Tecnologia tecnologia)
        {
            int row;
            SqlCommand command = new(
                "UPDATE Tecnologia " +
                "SET nome = @nome, custo = @custo " +
                "WHERE id = @id", connection);
            command.Parameters.AddWithValue("@nome", tecnologia.Nome);
            command.Parameters.AddWithValue("@custo", tecnologia.Custo);
            command.Parameters.AddWithValue("@id", tecnologia.Id);

            try
            {
                connection.Open();
                row = command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            return row == 1;
        }

        public bool EliminaTecnologia(int idTecnologia)
        {
            SqlCommand commandTecColab = new("DELETE FROM TecnologiaColaborador WHERE tecnologia_id = @id", connection);
            commandTecColab.Parameters.AddWithValue("@id", idTecnologia);

            try
            {
                connection.Open();
                commandTecColab.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            SqlCommand commandTecProjeto = new("DELETE FROM TecnologiaProjeto WHERE tecnologia_id = @id", connection);
            commandTecProjeto.Parameters.AddWithValue("@id", idTecnologia);

            try
            {
                connection.Open();
                commandTecProjeto.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            SqlCommand commandTec = new("DELETE FROM Tecnologia WHERE id = @id", connection);
            commandTec.Parameters.AddWithValue("@id", idTecnologia);

            try
            {
                connection.Open();
                commandTec.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            return true;
        }

        //Devolve o colabor com o nif dado
        public Colaborador ProcuraColaborador(int nif)
        {
            Colaborador colaborador = null;
            byte[] foto;

            connection.Open();
            SqlCommand sqlCommand = new("SELECT * FROM Colaborador WHERE nif = @nif;", connection);
            sqlCommand.Parameters.AddWithValue("@nif", nif);
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            if (dataReader.Read())
            {
                string nome = dataReader["nome"].ToString();
                string morada = dataReader["morada"].ToString();
                int nivelDeHabilitacao = int.Parse(dataReader["nivel_de_habilitacao"].ToString());
                double valorHora = double.Parse(dataReader["valor_hora"].ToString());

                //Se não houver foto na base de dados, devolve um array vazio 
                if (dataReader["foto"] != DBNull.Value)
                    foto = (byte[])dataReader["foto"];
                else
                    foto = Array.Empty<byte>();

                //A conecção tem de ser fechada para poder invocar o método ProcuraColaboradorTecnologias,
                //caso contrário haveria conflito de sqldatareaders 
                connection.Close();

                List<Tecnologia> tecnologiasDeDominio = ProcuraColaboradorTecnologias(nif);

                if (tecnologiasDeDominio[0].AreaDeAtividade == "Programação")
                    colaborador = new Programador(nif, nome, morada, nivelDeHabilitacao, valorHora, tecnologiasDeDominio, foto);
                else if (tecnologiasDeDominio[0].AreaDeAtividade == "Redes")
                    colaborador = new Redes(nif, nome, morada, nivelDeHabilitacao, valorHora, tecnologiasDeDominio, foto);
                else
                    colaborador = new Tecnico(nif, nome, morada, nivelDeHabilitacao, valorHora, tecnologiasDeDominio, foto);
            }
            connection.Close();

            return colaborador;
        }

        //Devolve todos os calaboradores da base de dados
        public List<Colaborador> ProcuraTodosColaboradores()
        {
            List<Colaborador> colaboradores = new();
            //Lista para guardar os NIFs de cada colocaborador, para depois poder invocar o método ProcuraColaborador.
            //Não consigo invocar esse método dentro do while (dataReader.Read()) por que causa conflito de sqldatareaders.
            //Escolhi List e não array por não saber a quantidade exata de registos.
            List<int> nifs = new();

            SqlCommand sqlCommand = new("SELECT nif FROM Colaborador;", connection);
            connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                int nifColaborador = int.Parse(dataReader["nif"].ToString());
                nifs.Add(nifColaborador);
            }
            connection.Close();

            foreach (int nif in nifs)
            {
                Colaborador colaborador = ProcuraColaborador(nif);
                colaboradores.Add(colaborador);
            }

            return colaboradores;
        }

        //Devolve os colaboradores que dominam a tecnologia escolhida
        //Criei este método para facilitar a escolha de colaboradores para um projeto
        public List<Colaborador> ProcuraColaboradoresPorTec(int idTecnologia)
        {
            List<Colaborador> colaboradores = new();
            List<int> nifs = new();

            SqlCommand sqlCommand = new("SELECT * FROM TecnologiaColaborador WHERE tecnologia_id = @id;", connection);
            sqlCommand.Parameters.AddWithValue("@id", idTecnologia);
            connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                int nifColaborador = int.Parse(dataReader["colaborador_nif"].ToString());
                nifs.Add(nifColaborador);
            }
            connection.Close();

            foreach (int nif in nifs)
            {
                Colaborador colaborador = ProcuraColaborador(nif);
                colaboradores.Add(colaborador);
            }

            return colaboradores;
        }

        public List<ColaboradorAssociado> ProcuraProjetoColaboradores(int idProjeto)
        {
            List<ColaboradorAssociado> colaboradoresAssociados = new();
            //Pesquisei na internet sobre dictionaries.
            //A key é o nif do colaborador e o value são as horas em ele está no projeto
            Dictionary<int, int> nifsHoras = new();

            SqlCommand sqlCommand = new("SELECT colaborador_nif, horas_afeto FROM ColaboradorProjeto WHERE projeto_id = @id;", connection);
            sqlCommand.Parameters.AddWithValue("@id", idProjeto);
            connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                int nifColaborador = int.Parse(dataReader["colaborador_nif"].ToString());
                int horasAfeto = int.Parse(dataReader["horas_afeto"].ToString());
                nifsHoras.Add(nifColaborador, horasAfeto);
            }
            connection.Close();

            foreach (KeyValuePair<int, int> nifHora in nifsHoras)
            {
                Colaborador colaborador = ProcuraColaborador(nifHora.Key);
                ColaboradorAssociado colaboradorAssociado = new(colaborador, nifHora.Value);
                colaboradoresAssociados.Add(colaboradorAssociado);
            }

            return colaboradoresAssociados;
        }

        //Devolve as tecnologias associadas ao colaborador com o nif dado
        public List<Tecnologia> ProcuraColaboradorTecnologias(int nif)
        {
            List<Tecnologia> tecnologias = new();

            SqlCommand sqlCommand = new(
                "SELECT  t.id, t.nome, t.area, t.custo " +
                "FROM TecnologiaColaborador AS tc " +
                "JOIN Colaborador AS c " +
                "ON c.nif = tc.colaborador_nif " +
                "JOIN Tecnologia AS t " +
                "ON t.id = tc.tecnologia_id " +
                "WHERE c.nif = @nif;", connection);
            sqlCommand.Parameters.AddWithValue("@nif", nif);
            connection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                Tecnologia tecnologia = null;

                int id = int.Parse(sqlDataReader["id"].ToString());
                string nome = sqlDataReader["nome"].ToString();
                string area = sqlDataReader["area"].ToString();
                double custo = double.Parse(sqlDataReader["custo"].ToString());

                tecnologia = new(id, nome, area, custo);
                tecnologias.Add(tecnologia);
            }
            connection.Close();

            return tecnologias;
        }

        public List<Tecnologia> ProcuraProjetoTecnologias(int idProjeto)
        {
            List<Tecnologia> tecnologias = new();
            List<int> tecIds = new();

            SqlCommand sqlCommand = new("SELECT tecnologia_id FROM TecnologiaProjeto WHERE projeto_id = @id;", connection);
            sqlCommand.Parameters.AddWithValue("@id", idProjeto);
            connection.Open();
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            while (dataReader.Read())
            {
                int tecId = int.Parse(dataReader["tecnologia_id"].ToString());
                tecIds.Add(tecId);
            }
            connection.Close();

            foreach (int id in tecIds)
            {
                Tecnologia tecnologia = ProcuraTecnologia(id);
                tecnologias.Add(tecnologia);
            }

            return tecnologias;
        }

        public List<Tecnologia> ProcuraTodasTecnologias()
        {
            List<Tecnologia> tecnologias = new();

            SqlCommand sqlCommand = new("SELECT * FROM Tecnologia", connection);

            connection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                Tecnologia tecnologia = null;

                int id = int.Parse(sqlDataReader["id"].ToString());
                string nome = sqlDataReader["nome"].ToString();
                string area = sqlDataReader["area"].ToString();
                double custo = double.Parse(sqlDataReader["custo"].ToString());

                tecnologia = new(id, nome, area, custo);
                tecnologias.Add(tecnologia);
            }
            connection.Close();

            return tecnologias;
        }

        public Tecnologia ProcuraTecnologia(int idTecnologia)
        {
            Tecnologia tecnologia = null;

            SqlCommand sqlCommand = new("SELECT * FROM Tecnologia WHERE id = @id", connection);
            sqlCommand.Parameters.AddWithValue("@id", idTecnologia);
            connection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            if (sqlDataReader.Read())
            {
                string nome = sqlDataReader["nome"].ToString();
                string area = sqlDataReader["area"].ToString();
                double custo = double.Parse(sqlDataReader["custo"].ToString());

                tecnologia = new(idTecnologia, nome, area, custo);
            }
            connection.Close();

            return tecnologia;
        }

        public List<Projeto> ProcuraTodosProjetos()
        {
            List<Projeto> projetos = new();
            List<int> projetoIds = new();

            SqlCommand sqlCommand = new("SELECT id FROM Projeto", connection);

            connection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                int id = int.Parse(sqlDataReader["id"].ToString());
                projetoIds.Add(id);
            }
            connection.Close();

            foreach (int projetoId in projetoIds)
            {
                Projeto projeto = ProcuraProjeto(projetoId);
                projetos.Add(projeto);
            }

            return projetos;
        }

        public List<Projeto> ProcuraProjetoPorColaborador(int nif)
        {
            List<Projeto> projetos = new();
            List<int> projetoIds = new();

            SqlCommand sqlCommand = new("SELECT projeto_id FROM ColaboradorProjeto WHERE colaborador_nif = @nif;", connection);
            sqlCommand.Parameters.AddWithValue("@nif", nif);

            connection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                int id = int.Parse(sqlDataReader["projeto_id"].ToString());
                projetoIds.Add(id);
            }
            connection.Close();

            foreach (int projetoId in projetoIds)
            {
                Projeto projeto = ProcuraProjeto(projetoId);
                projetos.Add(projeto);
            }

            return projetos;
        }

        public List<Projeto> ProcuraProjetoPorTecnologia(int idTecnologia)
        {
            List<Projeto> projetos = new();
            List<int> projetoIds = new();

            SqlCommand command = new("SELECT projeto_id FROM TecnologiaProjeto WHERE tecnologia_id = @idTec;", connection);
            command.Parameters.AddWithValue("@idTec", idTecnologia);

            connection.Open();
            SqlDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                int id = int.Parse(dataReader["projeto_id"].ToString());
                projetoIds.Add(id);
            }
            connection.Close();

            foreach (int projetoId in projetoIds)
            {
                Projeto projeto = ProcuraProjeto(projetoId);
                projetos.Add(projeto);
            }

            return projetos;
        }

        public Projeto ProcuraProjeto(int idProjeto)
        {
            Projeto projeto = null;

            SqlCommand sqlCommand = new("SELECT * FROM Projeto WHERE id = @id", connection);
            sqlCommand.Parameters.AddWithValue("@id", idProjeto);
            connection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            if (sqlDataReader.Read())
            {
                string nome = sqlDataReader["nome"].ToString();
                string descricao = sqlDataReader["descricao"].ToString();

                connection.Close();

                List<Tecnologia> tecnologias = ProcuraProjetoTecnologias(idProjeto);
                List<ColaboradorAssociado> colaboradores = ProcuraProjetoColaboradores(idProjeto);

                projeto = new(idProjeto, nome, descricao, tecnologias, colaboradores);
            }
            connection.Close();

            return projeto;
        }

        public int BuscaIdProjeto(string nomeProjeto, string descricao)
        {
            int id = 0;

            SqlCommand command = new("SELECT id FROM Projeto WHERE nome = '" + nomeProjeto + "' " +
                "AND descricao = '" + descricao + "';", connection);
            connection.Open();
            SqlDataReader sqlDataReader = command.ExecuteReader();
            if (sqlDataReader.Read())
                id = int.Parse(sqlDataReader["id"].ToString());

            connection.Close();

            return id;
        }

        public double[] CustoProjeto(int idProjeto)
        {
            double[] custos = new double[3];
            double custoTecnologias = 0;
            double custoColaboradores = 0;

            List<Tecnologia> tecnologiasAssociadas = ProcuraProjetoTecnologias(idProjeto);
            List<ColaboradorAssociado> colaboradoresAssociados = ProcuraProjetoColaboradores(idProjeto);

            if (tecnologiasAssociadas.Count > 0)
            {
                foreach (Tecnologia tecnologia in tecnologiasAssociadas)
                    custoTecnologias += tecnologia.Custo;
            }

            custos[0] = custoTecnologias;

            if (colaboradoresAssociados.Count > 0)
            {
                foreach (ColaboradorAssociado colaborador in colaboradoresAssociados)
                    custoColaboradores += colaborador.CustoNoProjeto;
            }

            custos[1] = custoColaboradores;
            custos[2] = custoTecnologias + custoColaboradores;

            return custos;
        }
    }
}
