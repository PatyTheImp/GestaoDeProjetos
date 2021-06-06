using FontAwesome5;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GestaoDeProjetos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Color darkColor;
        Color lightColor;
        IGestorProjetos gestor;
        bool paraEditar;
        Projeto projetoEscolhido;
        string modoEdicao;
        Tecnologia tecnologiaEscolhida;

        public MainWindow()
        {
            InitializeComponent();

            darkColor = Color.FromRgb(42, 42, 42);
            lightColor = Color.FromRgb(213, 213, 213);
            gestor = new GestorProjetos();
            paraEditar = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Tem certeza que quer sair da aplicação?", "Aviso", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Muda o tema da aplicação (Dark Mode/Light Mode)
        private void Toggle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush corLetra = this.FindResource("themeColor") as SolidColorBrush;
            SolidColorBrush corFundo = this.FindResource("fundo") as SolidColorBrush;

            //Verifica se o icone é o sol (se for muda para Light Mode)
            if (sunMoon.Icon == EFontAwesomeIcon.Solid_Sun)
            {
                sunMoon.Icon = EFontAwesomeIcon.Regular_Moon;
                toggle.Icon = EFontAwesomeIcon.Solid_ToggleOn;

                corLetra.Color = darkColor;
                corFundo.Color = lightColor;
            }
            else
            {
                sunMoon.Icon = EFontAwesomeIcon.Solid_Sun;
                toggle.Icon = EFontAwesomeIcon.Solid_ToggleOff;

                corLetra.Color = lightColor;
                corFundo.Color = darkColor;
            }
        }

        //Muda para o separador de gestão de colaboradores
        private void StckpGestaoColab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbiInserirColaborador.IsSelected || tbiInserirProjeto.IsSelected)
            {
                if (MessageBox.Show("Tem certeza que quer sair sem guardar?", "Aviso", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    VaiParaGestaoColaboradoes();
            }
            else
                VaiParaGestaoColaboradoes();
        }

        //Muda para o separador de gestão de projetos
        private void StckpGestaoProjeto_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbiInserirColaborador.IsSelected || tbiInserirProjeto.IsSelected)
            {
                if (MessageBox.Show("Tem certeza que quer sair sem guardar?", "Aviso", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    VaiParaGestaoProjetos();
            }
            else
                VaiParaGestaoProjetos();
        }

        private void StckpGestaoTecnologia_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbiInserirColaborador.IsSelected || tbiInserirProjeto.IsSelected)
            {
                if (MessageBox.Show("Tem certeza que quer sair sem guardar?", "Aviso", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    VaiParaGestaoTecnologias();
            }
            else
                VaiParaGestaoTecnologias();
        }

        //-----------Eventos da gestão de colaboradores-------------------------------------------------------------------------------------

        //Filtra para programadores
        private void CheckbProgramador_Checked(object sender, RoutedEventArgs e)
        {
            checkbRedes.IsChecked = false;
            checkbTecnico.IsChecked = false;

            CarregaColaboradores("Programador", txtbFiltrarNif.Text, txtbFiltrarNome.Text);
        }

        //Filtra para gestor de redes
        private void CheckbRedes_Checked(object sender, RoutedEventArgs e)
        {
            checkbProgramador.IsChecked = false;
            checkbTecnico.IsChecked = false;

            CarregaColaboradores("Gestor de Redes", txtbFiltrarNif.Text, txtbFiltrarNome.Text);
        }

        //Filtra para operacional técnico
        private void CheckbTecnico_Checked(object sender, RoutedEventArgs e)
        {
            checkbProgramador.IsChecked = false;
            checkbRedes.IsChecked = false;

            CarregaColaboradores("Operacional Técnico", txtbFiltrarNif.Text, txtbFiltrarNome.Text);
        }

        //Disfiltra para operacional técnico
        private void CheckbTecnico_Unchecked(object sender, RoutedEventArgs e)
        {
            CarregaColaboradores(string.Empty, txtbFiltrarNif.Text, txtbFiltrarNome.Text);
        }

        //Disfiltra para programadores
        private void CheckbProgramador_Unchecked(object sender, RoutedEventArgs e)
        {
            CarregaColaboradores(string.Empty, txtbFiltrarNif.Text, txtbFiltrarNome.Text);
        }

        //Disfiltra para gestor de redes
        private void CheckbRedes_Unchecked(object sender, RoutedEventArgs e)
        {
            CarregaColaboradores(string.Empty, txtbFiltrarNif.Text, txtbFiltrarNome.Text);
        }

        //Filtra por nif 
        private void TxtbFiltrarNif_TextChanged(object sender, TextChangedEventArgs e)
        {
            CarregaColaboradores(QualAreaChecked(), txtbFiltrarNif.Text, txtbFiltrarNome.Text);
        }

        //Filtra por nome
        private void TxtbFiltrarNome_TextChanged(object sender, TextChangedEventArgs e)
        {
            CarregaColaboradores(QualAreaChecked(), txtbFiltrarNif.Text, txtbFiltrarNome.Text);
        }

        //Muda para o separador de inserir colaborador
        private void BtnInserirColab_Click(object sender, RoutedEventArgs e)
        {
            retanguloColab.Visibility = Visibility.Hidden;
            retanguloProjetos.Visibility = Visibility.Hidden;
            paraEditar = false;

            LimpaCamposInserirColaborador();
            txtbNIFColab.IsEnabled = true;
            tbiInserirColaborador.IsSelected = true;
        }

        //Muda para o separador de consultar colaborador, se um item da datagrid tiver seleccionado
        private void BtnConsultarColab_Click(object sender, RoutedEventArgs e)
        {
            if (dgColaboradores.SelectedItem == null)
                MessageBox.Show("Seleccione um colaborador da tabela", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                retanguloColab.Visibility = Visibility.Hidden;
                retanguloProjetos.Visibility = Visibility.Hidden;

                Colaborador colaborador = (Colaborador)dgColaboradores.SelectedItem;
                PreencheConsultarColaborador(colaborador);
                tbiConsultarColaborador.IsSelected = true;
            }
        }

        //--------------------------Eventos do inserir/editar colaborador-------------------------------------------------------------------------------------

        //Quando uma função/área é escolhida na combobox, a listbox é preenchida com as tecnologias associadas a essa função 
        private void CmbFuncao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Tecnologia> tecnologias = gestor.ProcuraTodasTecnologias();
            lbTecnologiasEscolhidas.Items.Clear();
            lbTecnologiasDisponiveis.Items.Clear();

            if (cmbFuncao.SelectedItem != null)
            {
                string area = cmbFuncao.SelectedValue.ToString();

                foreach (Tecnologia tecnologia in tecnologias)
                {
                    if (tecnologia.AreaDeAtividade == area)
                        lbTecnologiasDisponiveis.Items.Add(tecnologia);
                }
            }
        }

        //Adciona todas as tecnologias disponiveis ás escolhidas
        private void IconSelecionarTodas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lbTecnologiasDisponiveis.Items.Count > 0)
            {
                foreach (Tecnologia item in lbTecnologiasDisponiveis.Items)
                    lbTecnologiasEscolhidas.Items.Add(item);

                lbTecnologiasDisponiveis.Items.Clear();
            }
        }

        //Adciona a tecnologia disponivel seleccionada ás escolhidas
        private void IconSelecionarUma_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lbTecnologiasDisponiveis.Items.Count > 0)
            {
                if (lbTecnologiasDisponiveis.SelectedItem != null)
                {
                    lbTecnologiasEscolhidas.Items.Add(lbTecnologiasDisponiveis.SelectedItem);
                    lbTecnologiasDisponiveis.Items.Remove(lbTecnologiasDisponiveis.SelectedItem);
                }
                else
                    MessageBox.Show("Seleccione uma tecnologia", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Retira a tecnologia escolhida seleccionada e devolve-a ás disponiveis
        private void IconRetirarUma_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lbTecnologiasEscolhidas.Items.Count > 0)
            {
                if (lbTecnologiasEscolhidas.SelectedItem != null)
                {
                    lbTecnologiasDisponiveis.Items.Add(lbTecnologiasEscolhidas.SelectedItem);
                    lbTecnologiasEscolhidas.Items.Remove(lbTecnologiasEscolhidas.SelectedItem);
                }
                else
                    MessageBox.Show("Seleccione uma tecnologia", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Retira todas as tecnologias escolhidas e devolve-as ás disponiveis
        private void IconRetirarTodas_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (lbTecnologiasEscolhidas.Items.Count > 0)
            {
                foreach (Tecnologia item in lbTecnologiasEscolhidas.Items)
                    lbTecnologiasDisponiveis.Items.Add(item);

                lbTecnologiasEscolhidas.Items.Clear();
            }
        }

        //Abre um dialogo de ficheiros e insere a imagem escolhida.
        //Se um ficheiro que não é de imagem for escolhido, uma mesagem de erro aparece
        private void BtnInserirFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();

            if (openFileDialog.ShowDialog() == true)
            {
                Uri ficheiro = new(openFileDialog.FileName);
                try
                {
                    imgFoto.Source = new BitmapImage(ficheiro);
                }
                catch (NotSupportedException)
                {
                    MessageBox.Show("Extensão de ficheiro não suportada.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //Dependendo se estamos no modo editar ou modo inserir, este botão ativa os metodos correspondentes
        private void BtnGuardarInserirColaborador_Click(object sender, RoutedEventArgs e)
        {
            if (paraEditar)
                EditaColaboradorNaBD();
            else
                InsereColaboradorNaBD();
        }

        private void BtnCancelarColaborador_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Tem certeza que quer sair sem guardar?", "Aviso", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (paraEditar)
                {
                    int nif = int.Parse(txtbNIFColab.Text);
                    Colaborador colaborador = gestor.ProcuraColaborador(nif);
                    PreencheConsultarColaborador(colaborador);
                    tbiConsultarColaborador.IsSelected = true;
                }
                else
                    VaiParaGestaoColaboradoes();
            }
        }

        //-----------Eventos do consultar colaborador-------------------------------------------------------------------------------------------

        private void BtnEliminarColab_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Tem certeza que quer apagar este colaborador da base de dados?", "Aviso", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                bool resultado = gestor.EliminaColaborador((int)lblNifColab.Content);

                if (resultado)
                {
                    MessageBox.Show("Colaborador eliminado com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    VaiParaGestaoColaboradoes();
                }
                else
                    MessageBox.Show("Algo correu mal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Vai para o separador inserir colaborador mas no modo edição
        private void BtnAlterarColab_Click(object sender, RoutedEventArgs e)
        {
            Colaborador colaborador = gestor.ProcuraColaborador((int)lblNifColab.Content);
            LimpaCamposInserirColaborador();
            PreecheEditarColaborador(colaborador);

            txtbNIFColab.IsEnabled = false;
            paraEditar = true;
            tbiInserirColaborador.IsSelected = true;
        }

        //Consulta o projeto em da tabela dos projetos associados ao colaborador
        private void BtnConsultarProjetoAssociado_Click(object sender, RoutedEventArgs e)
        {
            retanguloColab.Visibility = Visibility.Hidden;
            retanguloProjetos.Visibility = Visibility.Hidden;

            projetoEscolhido = (Projeto)dgProjetosAssociados.SelectedItem;
            PopulaConsultarProjeto();

            tbiConsultarProjeto.IsSelected = true;
        }

        //-----------Eventos da gestão de projetos------------------------------------------------------------------------------------------------------

        //Filtra os projetos da tabela pelo nome
        private void TxtbFiltrarNomeProjeto_TextChanged(object sender, TextChangedEventArgs e)
        {
            CarregaProjetos(txtbFiltrarNomeProjeto.Text);
        }

        //Vai para o separador dos Custo do Projeto escolhido
        private void BtnCustos_Click(object sender, RoutedEventArgs e)
        {
            //Se já estiver no separador Consultar Projeto popula os custos com o projeto já consultado
            if (tbiConsultarProjeto.IsSelected)
            {
                PopulaCustos(projetoEscolhido);
                custosProjeto.Visibility = Visibility.Visible;
            }
            else if (dgProjetos.SelectedItem == null)
                MessageBox.Show("Seleccione um projeto da tabela", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                projetoEscolhido = (Projeto)dgProjetos.SelectedItem;
                PopulaCustos(projetoEscolhido);

                custosProjeto.Visibility = Visibility.Visible;
            }
        }

        //Vai para o separador Consultar Projeto e atribui o projeto selecionado a variável global projetoEscolhido
        private void BtnConsultarProjeto_Click(object sender, RoutedEventArgs e)
        {
            if (dgProjetos.SelectedItem == null)
                MessageBox.Show("Seleccione um projeto da tabela", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                retanguloColab.Visibility = Visibility.Hidden;
                retanguloProjetos.Visibility = Visibility.Hidden;

                projetoEscolhido = (Projeto)dgProjetos.SelectedItem;
                PopulaConsultarProjeto();

                tbiConsultarProjeto.IsSelected = true;
            }
        }

        //Vai para o separador Inserir Projeto
        private void BtnInserirProjeto_Click(object sender, RoutedEventArgs e)
        {
            retanguloColab.Visibility = Visibility.Hidden;
            retanguloProjetos.Visibility = Visibility.Hidden;
            paraEditar = false;

            LimpaCamposInserirProjeto();

            List<Tecnologia> tecnologias = gestor.ProcuraTodasTecnologias();
            foreach (Tecnologia tecnologia in tecnologias)
                lvTecsDisponiveisProjeto.Items.Add(tecnologia);

            modoEdicao = "inserir";
            MudaVisibilidadesEditarProjeto();

            tbiInserirProjeto.IsSelected = true;
        }

        private void BtnFecharCustos_Click(object sender, RoutedEventArgs e)
        {
            custosProjeto.Visibility = Visibility.Hidden;
        }

        //-----------Eventos do Inserir/Editar Projeto------------------------------------------------------------------------------------------------------

        //Adciona todas as tecnologias disponiveis ás escolhidas
        private void IconSelecTodasTec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lvTecsDisponiveisProjeto.Items.Count > 0)
            {
                foreach (Tecnologia tecnologia in lvTecsDisponiveisProjeto.Items)
                    lvTecsEscolhidasProjeto.Items.Add(tecnologia);

                lvTecsDisponiveisProjeto.Items.Clear();

                //Visto todas as tecnologias disponiveis estarem associadas ao projeto, todos os colaboradores passam a ser
                //qualificados, por isso, adcionam-se todos os colaboradores á ListView dos qualificados com excepção daqueles
                //que já foram escolhidos para o projeto
                List<Colaborador> colaboradores = gestor.ProcuraTodosColaboradores();
                lvColabsQualificados.Items.Clear();

                foreach (Colaborador colaborador in colaboradores)
                {
                    lvColabsQualificados.Items.Add(colaborador);
                    foreach (ColaboradorAssociado colaboradorAssociado in lvColabsAssociados.Items)
                    {
                        if (colaborador.Nif == colaboradorAssociado.Colaborador.Nif)
                        {
                            lvColabsQualificados.Items.Remove(colaborador);
                            break;
                        }
                    }
                }
            }
        }

        //Adciona a tecnologia disponivel seleccionada ás escolhidas
        private void IconSelecUmaTec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lvTecsDisponiveisProjeto.Items.Count > 0)
            {
                if (lvTecsDisponiveisProjeto.SelectedItem != null)
                {
                    Tecnologia tecnologia = (Tecnologia)lvTecsDisponiveisProjeto.SelectedItem;
                    lvTecsEscolhidasProjeto.Items.Add(tecnologia);

                    //Quando se associa uma tecnologia ao projeto, os colaboradores que dominam essa tecnologia
                    //passam a ser qualificados para fazer parte do projeto                
                    AdcionaColaboradoresQualificados(tecnologia);

                    lvTecsDisponiveisProjeto.Items.Remove(tecnologia);
                }
                else
                    MessageBox.Show("Seleccione uma tecnologia", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Retira a tecnologia escolhida seleccionada e devolve-a ás disponiveis
        private void IconRetiraUmaTec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lvTecsEscolhidasProjeto.Items.Count > 0)
            {
                if (lvTecsEscolhidasProjeto.SelectedItem != null)
                {
                    Tecnologia tecnologia = (Tecnologia)lvTecsEscolhidasProjeto.SelectedItem;
                    string tipoColaborador;
                    bool aindaQualificado = false;
                    List<Colaborador> colaboradoresNaoQualificados = new();
                    List<ColaboradorAssociado> associadosNaoQualificados = new();

                    lvTecsDisponiveisProjeto.Items.Add(tecnologia);
                    lvTecsEscolhidasProjeto.Items.Remove(tecnologia);

                    if (tecnologia.AreaDeAtividade == "Programação")
                        tipoColaborador = "Programador";
                    else if (tecnologia.AreaDeAtividade == "Redes")
                        tipoColaborador = "Gestor de Redes";
                    else
                        tipoColaborador = "Operacional Técnico";

                    foreach (Colaborador colaborador in lvColabsQualificados.Items)
                    {
                        if (colaborador.TipoColaborador == tipoColaborador)
                        {
                            foreach (Tecnologia colaboradorTec in colaborador.TecnologiasDeDominio)
                            {
                                foreach (Tecnologia tecEscolhida in lvTecsEscolhidasProjeto.Items)
                                {
                                    if (colaboradorTec.Id == tecEscolhida.Id)
                                    {
                                        aindaQualificado = true;
                                        break;
                                    }
                                }
                                if (aindaQualificado)
                                    break;
                            }
                            if (!aindaQualificado)
                                colaboradoresNaoQualificados.Add(colaborador);

                            aindaQualificado = false;
                        }
                    }

                    foreach (Colaborador colab in colaboradoresNaoQualificados)
                        lvColabsQualificados.Items.Remove(colab);

                    aindaQualificado = false;
                    foreach (ColaboradorAssociado colaboradorAssociado in lvColabsAssociados.Items)
                    {
                        if (colaboradorAssociado.Colaborador.TipoColaborador == tipoColaborador)
                        {
                            foreach (Tecnologia colaboradorTec in colaboradorAssociado.Colaborador.TecnologiasDeDominio)
                            {
                                foreach (Tecnologia tecEscolhida in lvTecsEscolhidasProjeto.Items)
                                {
                                    if (colaboradorTec.Id == tecEscolhida.Id)
                                    {
                                        aindaQualificado = true;
                                        break;
                                    }
                                }
                                if (aindaQualificado)
                                    break;
                            }
                            if (!aindaQualificado)
                                associadosNaoQualificados.Add(colaboradorAssociado);

                            aindaQualificado = false;
                        }
                    }

                    foreach (ColaboradorAssociado colabAssoc in associadosNaoQualificados)
                        lvColabsAssociados.Items.Remove(colabAssoc);
                }
                else
                    MessageBox.Show("Seleccione uma tecnologia", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Retira todas as tecnologias escolhidas e devolve-as ás disponiveis
        private void IconRetiraTodasTec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lvTecsEscolhidasProjeto.Items.Count > 0)
            {
                foreach (Tecnologia tecnologia in lvTecsEscolhidasProjeto.Items)
                    lvTecsDisponiveisProjeto.Items.Add(tecnologia);

                lvTecsEscolhidasProjeto.Items.Clear();
                //Também limpa-se as ListViews dos colaboradores, visto não haver mais tecnologias associadas
                //também não pode haver colaboradores associados
                lvColabsQualificados.Items.Clear();
                lvColabsAssociados.Items.Clear();
            }
        }

        //Torna visivel uma grid que tem o papel de inputbox, para poder introduzir aí as horas que um 
        //colaborador está a participar em um projeto
        private void IconSelecUmColab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lvColabsQualificados.Items.Count > 0)
            {
                if (lvColabsQualificados.SelectedItem != null)
                {
                    inputHorasAfeto.Visibility = Visibility.Visible;
                    txtbHorasAfeto.Focus();
                }
                else
                    MessageBox.Show("Seleccione um colaborador", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Utilizo Regex (regular expressions) para evitar que o utilizador introduza qualquer coisa que
        //não seja um valor numérico
        private void TxtbHorasAfeto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnCancelarHoras_Click(object sender, RoutedEventArgs e)
        {
            inputHorasAfeto.Visibility = Visibility.Hidden;
        }

        //Quando se clica no botão de OK da inputbox e tendo sido introduzido um valor numérico válido, 
        //cria-se um ColaboradorAssociado e introduz-se na listview dos calaboradores associados
        private void BtnOkHoras_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtbHorasAfeto.Text, out int horas))
            {
                if (horas <= 0)
                    MessageBox.Show("Introduza um valor maior que zero", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                {
                    Colaborador colaborador = (Colaborador)lvColabsQualificados.SelectedItem;
                    int horasAfeto = int.Parse(txtbHorasAfeto.Text);
                    ColaboradorAssociado colaboradorAssociado = new(colaborador, horasAfeto);
                    lvColabsAssociados.Items.Add(colaboradorAssociado);
                    lvColabsQualificados.Items.Remove(colaborador);

                    inputHorasAfeto.Visibility = Visibility.Hidden;
                    txtbHorasAfeto.Clear();
                }
            }
            else
                MessageBox.Show("Introduza um valor numérico válido", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void IconRetiraTodosColabs_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lvColabsAssociados.Items.Count > 0)
            {
                foreach (ColaboradorAssociado colaboradorAssociado in lvColabsAssociados.Items)
                {
                    Colaborador colaborador = gestor.ProcuraColaborador(colaboradorAssociado.Colaborador.Nif);
                    lvColabsQualificados.Items.Add(colaborador);
                }

                lvColabsAssociados.Items.Clear();
            }
        }

        private void IconRetiraUmColab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lvColabsAssociados.Items.Count > 0)
            {
                if (lvColabsAssociados.SelectedItem != null)
                {
                    ColaboradorAssociado colaboradorAssociado = (ColaboradorAssociado)lvColabsAssociados.SelectedItem;
                    Colaborador colaborador = gestor.ProcuraColaborador(colaboradorAssociado.Colaborador.Nif);
                    lvColabsQualificados.Items.Add(colaborador);
                    lvColabsAssociados.Items.Remove(colaboradorAssociado);
                }
                else
                    MessageBox.Show("Seleccione um colaborador", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnAlterarTecsProjeto_Click(object sender, RoutedEventArgs e)
        {
            modoEdicao = "editar tecnologias";
            MudaVisibilidadesEditarProjeto();
        }

        private void BtnAlterarColabsProjeto_Click(object sender, RoutedEventArgs e)
        {
            modoEdicao = "editar colaboradores";
            MudaVisibilidadesEditarProjeto();
        }

        private void BtnGuardarAlteracoesProjeto_Click(object sender, RoutedEventArgs e)
        {
            if (paraEditar)
            {
                if (modoEdicao == "editar projeto")
                    EditaProjetoNaBD();
                else if (modoEdicao == "editar tecnologias")
                    EditaTecnologiaProjetoNaBD();
                else
                    EditaColaboradorProjetoNaBD();
            }
            else
                InsereProjetoNaBD();
        }

        private void BtnCancelarProjeto_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Tem certeza que quer sair sem guardar?", "Aviso", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (paraEditar)
                {
                    PopulaConsultarProjeto();
                    tbiConsultarProjeto.IsSelected = true;
                }
                else
                    VaiParaGestaoProjetos();
            }
        }

        //-----------------Eventos do Consultar Projeto---------------------------------------------------------------------------------------------

        private void BtnEliminarProjeto_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Tem certeza que quer apagar este projeto da base de dados?", "Aviso", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                bool resultado = gestor.EliminaProjeto(projetoEscolhido.Id);

                if (resultado)
                {
                    MessageBox.Show("Projeto eliminado com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    VaiParaGestaoProjetos();
                }
                else
                    MessageBox.Show("Algo correu mal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAlterarProjeto_Click(object sender, RoutedEventArgs e)
        {
            retanguloColab.Visibility = Visibility.Hidden;
            retanguloProjetos.Visibility = Visibility.Hidden;
            paraEditar = true;

            LimpaCamposInserirProjeto();
            PreencheEditarProjeto();

            modoEdicao = "editar projeto";
            MudaVisibilidadesEditarProjeto();

            tbiInserirProjeto.IsSelected = true;
        }

        private void BtnConsultarColaboradorAssociado_Click(object sender, RoutedEventArgs e)
        {
            retanguloColab.Visibility = Visibility.Hidden;
            retanguloProjetos.Visibility = Visibility.Hidden;

            ColaboradorAssociado colaborador = (ColaboradorAssociado)dgColabsAssociados.SelectedItem;
            PreencheConsultarColaborador(colaborador.Colaborador);
            tbiConsultarColaborador.IsSelected = true;
        }

        //---------------------Eventos da Gestão de Tecnologias---------------------------------------------------------------------------------------

        private void CheckbProgramacaoTec_Checked(object sender, RoutedEventArgs e)
        {
            checkbRedesTec.IsChecked = false;
            checkbTecnicoTec.IsChecked = false;

            CarregaTecnologias("P", txtbfiltrarTecNome.Text.Trim());
        }

        private void CheckbProgramacaoTec_Unchecked(object sender, RoutedEventArgs e)
        {
            CarregaTecnologias(string.Empty, txtbfiltrarTecNome.Text.Trim());
        }

        private void CheckbRedesTec_Checked(object sender, RoutedEventArgs e)
        {
            checkbProgramacaoTec.IsChecked = false;
            checkbTecnicoTec.IsChecked = false;

            CarregaTecnologias("R", txtbfiltrarTecNome.Text.Trim());
        }

        private void CheckbRedesTec_Unchecked(object sender, RoutedEventArgs e)
        {
            CarregaTecnologias(string.Empty, txtbfiltrarTecNome.Text.Trim());
        }

        private void CheckbTecnicoTec_Checked(object sender, RoutedEventArgs e)
        {
            checkbProgramacaoTec.IsChecked = false;
            checkbRedesTec.IsChecked = false;

            CarregaTecnologias("O", txtbfiltrarTecNome.Text.Trim());
        }

        private void TxtbfiltrarTecNome_TextChanged(object sender, TextChangedEventArgs e)
        {
            CarregaTecnologias(QualAreaTec(), txtbfiltrarTecNome.Text.Trim());
        }

        private void CheckbTecnicoTec_Unchecked(object sender, RoutedEventArgs e)
        {
            CarregaTecnologias(string.Empty, txtbfiltrarTecNome.Text.Trim());
        }

        private void BtnProjetoTec_Click(object sender, RoutedEventArgs e)
        {
            Tecnologia tecnologia = (Tecnologia)dgTecnologias.SelectedItem;
            List<Projeto> projetos = gestor.ProcuraProjetoPorTecnologia(tecnologia.Id);

            if (projetos.Count > 0)
            {
                dgProjectTec.ItemsSource = projetos;
                projetosAssociados.Visibility = Visibility.Visible;
            }
            else
                MessageBox.Show("Não há projetos associados a esta tecnologia", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnFecharProjetos_Click(object sender, RoutedEventArgs e)
        {
            projetosAssociados.Visibility = Visibility.Hidden;
        }

        private void BtnConsultarProjetoTec_Click(object sender, RoutedEventArgs e)
        {
            projetoEscolhido = (Projeto)dgProjectTec.SelectedItem;
            PopulaConsultarProjeto();

            retanguloTec.Visibility = Visibility.Hidden;
            projetosAssociados.Visibility = Visibility.Hidden;

            tbiConsultarProjeto.IsSelected = true;
        }

        private void BtnColabTec_Click(object sender, RoutedEventArgs e)
        {
            Tecnologia tecnologia = (Tecnologia)dgTecnologias.SelectedItem;
            List<Colaborador> colaboradores = gestor.ProcuraColaboradoresPorTec(tecnologia.Id);

            if (colaboradores.Count > 0)
            {
                dgColabTec.ItemsSource = colaboradores;
                colaboradoresAssociados.Visibility = Visibility.Visible;
            }
            else
                MessageBox.Show("Não há colaboradores associados a esta tecnologia", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnFecharColaboradores_Click(object sender, RoutedEventArgs e)
        {
            colaboradoresAssociados.Visibility = Visibility.Hidden;
        }

        private void BtnConsultarColabTec_Click(object sender, RoutedEventArgs e)
        {
            Colaborador colaborador = (Colaborador)dgColabTec.SelectedItem;
            PreencheConsultarColaborador(colaborador);

            retanguloTec.Visibility = Visibility.Hidden;
            colaboradoresAssociados.Visibility = Visibility.Hidden;

            tbiConsultarColaborador.IsSelected = true;
        }

        private void BtnCancelarInserirTec_Click(object sender, RoutedEventArgs e)
        {
            inserirTec.Visibility = Visibility.Hidden;
        }

        private void BtnInserirTec_Click(object sender, RoutedEventArgs e)
        {
            LimpaInserirTec();

            paraEditar = false;

            inserirTec.Visibility = Visibility.Visible;
            stckAreaTec.Visibility = Visibility.Visible;
        }

        private void BtnEditarTec_Click(object sender, RoutedEventArgs e)
        {
            LimpaInserirTec();

            paraEditar = true;

            tecnologiaEscolhida = (Tecnologia)dgTecnologias.SelectedItem;
            txtbInserirTecNome.Text = tecnologiaEscolhida.Nome;
            txtbInserirTecCusto.Text = tecnologiaEscolhida.Custo.ToString();

            stckAreaTec.Visibility = Visibility.Collapsed;
            inserirTec.Visibility = Visibility.Visible;
        }

        private void BtnOkInserirTec_Click(object sender, RoutedEventArgs e)
        {
            if (paraEditar)
                EditaTecnologiaNaBD();
            else
                InsereTecnologiaNaBD();
        }

        private void BtnApagarTec_Click(object sender, RoutedEventArgs e)
        {
            tecnologiaEscolhida = (Tecnologia)dgTecnologias.SelectedItem;

            if (VerificaTecsSuficientes())
            {
                if (VerificaColabTecsSuficientes())
                    EliminaTecnologiaNaBD();
                else
                    MessageBox.Show("A tecnologia escolhida não pode ser eliminada pois há pelo menos um coloborador " +
                        "que só tem essa tecnologia associada", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("A tecnologia escolhida não pode ser eliminada pois tem de haver pelo menos 4 tecnologias de cada área",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void TxtbInserirTecCusto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //---------------Meus Métodos-------------------------------------------------------------------------------------------------------------

        //Carrega a datagrid dos colaboradores conforme as opções de filtragem
        private void CarregaColaboradores(string area, string nif, string nome)
        {
            List<Colaborador> colaboradores = gestor.ProcuraTodosColaboradores();
            List<Colaborador> colaboradoresParaFiltrar = new();

            if (area != string.Empty || nif != string.Empty || nome != string.Empty)
            {
                foreach (Colaborador colaborador in colaboradores)
                {
                    if (!colaborador.Nif.ToString().StartsWith(nif) || !colaborador.TipoColaborador.StartsWith(area)
                        || !colaborador.Nome.ToLower().StartsWith(nome.ToLower()))
                        colaboradoresParaFiltrar.Add(colaborador);
                }

                foreach (Colaborador colaborador in colaboradoresParaFiltrar)
                    colaboradores.Remove(colaborador);
            }

            dgColaboradores.ItemsSource = colaboradores;
        }

        private void CarregaProjetos(string nome)
        {
            List<Projeto> projetos = gestor.ProcuraTodosProjetos();
            List<Projeto> projetosParaFiltrar = new();

            if (nome != string.Empty)
            {
                foreach (Projeto projeto in projetos)
                {
                    if (!projeto.Nome.ToLower().StartsWith(nome.ToLower()))
                        projetosParaFiltrar.Add(projeto);
                }

                foreach (Projeto proj in projetosParaFiltrar)
                    projetos.Remove(proj);
            }

            dgProjetos.ItemsSource = projetos;
        }

        //Verifica qual checkbox na gestão de colaboradores está checked
        private string QualAreaChecked()
        {
            if (checkbProgramador.IsChecked == true)
                return "Programador";
            if (checkbRedes.IsChecked == true)
                return "Gestor de Redes";
            if (checkbTecnico.IsChecked == true)
                return "Operacional Técnico";

            return string.Empty;
        }

        private void LimpaFiltrosDosColaboradores()
        {
            checkbProgramador.IsChecked = false;
            checkbRedes.IsChecked = false;
            checkbTecnico.IsChecked = false;
            txtbFiltrarNif.Clear();
            txtbFiltrarNome.Clear();
        }

        private void LimpaCamposInserirColaborador()
        {
            txtbNomeColab.Clear();
            txtbNIFColab.Clear();
            txtbMoradaColab.Clear();
            sldrNivelHabil.Value = 1;
            txtbValorHora.Clear();
            cmbFuncao.SelectedItem = null;
            imgFoto.Source = null;
        }

        //Verifica se os campos obrigatórios em Inserir/Editar Colaborador estão preenchidos e se têm valores válidos. 
        private bool VerificaInserirColaborador()
        {
            if (lbTecnologiasEscolhidas.Items.Count <= 0 || txtbNomeColab.Text.Trim() == string.Empty || txtbNIFColab.Text.Trim() == string.Empty
                            || txtbMoradaColab.Text.Trim() == string.Empty || txtbValorHora.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Ainda há campos por preencher", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (!int.TryParse(txtbNIFColab.Text, out _))
            {
                MessageBox.Show("NIF inválido", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                txtbNIFColab.Focus();
                return false;
            }

            if (txtbNIFColab.Text.Length != 9)
            {
                MessageBox.Show("O NIF tem de ter 9 algarismos", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                txtbNIFColab.Focus();
                return false;
            }

            if (!double.TryParse(txtbValorHora.Text, out _))
            {
                MessageBox.Show("Valor hora inválido", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                txtbValorHora.Focus();
                return false;
            }

            return true;
        }

        //Copiei este código do stackoverflow. Cria um bitmapimage apartir de um array de bytes.
        //Como imagens são guardadas na base de dados como byte[], esta conversão tem de ser feita.
        private static BitmapImage CriarImagem(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            var image = new BitmapImage();

            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();

            return image;
        }

        //Copiei este código do stackoverflow. Cria um array de bytes apartir de um bitmapimage.
        //Como imagens são guardadas na base de dados como byte[], esta conversão tem de ser feita.
        private byte[] CriarByteArray(BitmapImage bitmapImage)
        {
            byte[] data;

            if (bitmapImage == null)
                data = Array.Empty<byte>();
            else
            {
                JpegBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

                using (MemoryStream ms = new())
                {
                    encoder.Save(ms);
                    data = ms.ToArray();
                }
            }

            return data;
        }

        //Preenche os campos no separador Consultar Colaborador com os dados do colaborador seleccionado
        private void PreencheConsultarColaborador(Colaborador colaborador)
        {
            lblNomeColab.Content = colaborador.Nome;
            lblNifColab.Content = colaborador.Nif;
            lblMorada.Content = colaborador.Morada;
            lblNivelHabilConsult.Content = colaborador.NivelDeHabilitacao;
            lblValorHora.Content = String.Format("{0:C2}", colaborador.ValorHora);
            lblFuncao.Content = colaborador.TipoColaborador;

            lbTecnologiasColab.Items.Clear();
            foreach (Tecnologia tecnologia in colaborador.TecnologiasDeDominio)
                lbTecnologiasColab.Items.Add(tecnologia);

            imgFotoConsult.Source = null;
            if (colaborador.Foto != Array.Empty<byte>())
                imgFotoConsult.Source = CriarImagem(colaborador.Foto);

            List<Projeto> projetosAssociados = gestor.ProcuraProjetoPorColaborador(colaborador.Nif);
            dgProjetosAssociados.Items.Clear();
            foreach (Projeto projeto in projetosAssociados)
                dgProjetosAssociados.Items.Add(projeto);
        }

        //Preenche os campos no separador Inserir Colaborador com os dados do colaborador seleccionado, para poder ser editado
        private void PreecheEditarColaborador(Colaborador colaborador)
        {
            txtbNomeColab.Text = colaborador.Nome;
            txtbNIFColab.Text = colaborador.Nif.ToString();
            txtbMoradaColab.Text = colaborador.Morada;
            sldrNivelHabil.Value = colaborador.NivelDeHabilitacao;
            txtbValorHora.Text = colaborador.ValorHora.ToString();

            if (colaborador.TipoColaborador == "Programador")
                cmbFuncao.SelectedIndex = 0;
            else if (colaborador.TipoColaborador == "Gestor de Redes")
                cmbFuncao.SelectedIndex = 1;
            else
                cmbFuncao.SelectedIndex = 2;

            List<Tecnologia> totalTecnologias = gestor.ProcuraTodasTecnologias();

            foreach (Tecnologia tecnologia in colaborador.TecnologiasDeDominio)
            {
                lbTecnologiasEscolhidas.Items.Add(tecnologia);
                //lbTecnologiasDisponiveis.Items.Remove(tecnologia); => isto não da em nada pois os objetos são diferentes

                //As tecnologias escolhidas são retiradas do total de tecnologias para posteriormente, as tecnologias não escolhidas
                //serem adcionadas a listbox das tecnologias disponiveis
                for (int i = 0; i < totalTecnologias.Count; i++)
                {
                    if (totalTecnologias[i].Id == tecnologia.Id)
                    {
                        totalTecnologias.RemoveAt(i);
                        break;
                    }
                }
            }

            //Visto esta listbox estar programada para ser preenchida toda vez que o valor da combo box das funções muda,
            //uma limpeza tem de ser feita
            lbTecnologiasDisponiveis.Items.Clear();

            //Da lista das tecnologias não escolhidas, escolhe-se as que são da mesma área que o colaborador e adciona-se á 
            //listbox das tecnologias disponiveis
            foreach (Tecnologia tecnologia in totalTecnologias)
            {
                if (tecnologia.AreaDeAtividade == cmbFuncao.SelectedValue.ToString())
                    lbTecnologiasDisponiveis.Items.Add(tecnologia);
            }


            imgFoto.Source = null;
            if (colaborador.Foto != Array.Empty<byte>())
                imgFoto.Source = CriarImagem(colaborador.Foto);

        }

        private void InsereColaboradorNaBD()
        {
            if (VerificaInserirColaborador())
            {
                List<Tecnologia> tecnologias = new();
                int[] idTecnologias = new int[lbTecnologiasEscolhidas.Items.Count];
                int i = 0;
                foreach (Tecnologia item in lbTecnologiasEscolhidas.Items)
                {
                    tecnologias.Add(item);
                    idTecnologias[i++] = item.Id;
                }

                string area = cmbFuncao.SelectedValue.ToString();
                Colaborador colaborador = null;
                int nif = int.Parse(txtbNIFColab.Text);
                string nome = txtbNomeColab.Text;
                string morada = txtbMoradaColab.Text;
                int nivelHabilitação = (int)sldrNivelHabil.Value;
                double valorHora = double.Parse(txtbValorHora.Text);
                byte[] foto = CriarByteArray((BitmapImage)imgFoto.Source);

                if (area == "Programação")
                    colaborador = new Programador(nif, nome, morada, nivelHabilitação, valorHora, tecnologias, foto);
                else if (area == "Redes")
                    colaborador = new Redes(nif, nome, morada, nivelHabilitação, valorHora, tecnologias, foto);
                else
                    colaborador = new Tecnico(nif, nome, morada, nivelHabilitação, valorHora, tecnologias, foto);

                bool resultado = gestor.InsereColaborador(colaborador, idTecnologias);

                if (resultado)
                {
                    MessageBox.Show("Colaborador inserido com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    PreencheConsultarColaborador(colaborador);
                    tbiConsultarColaborador.IsSelected = true;
                }
                else
                    MessageBox.Show("Algo correu mal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditaColaboradorNaBD()
        {
            if (VerificaInserirColaborador())
            {
                List<Tecnologia> tecnologias = new();
                int[] idTecnologias = new int[lbTecnologiasEscolhidas.Items.Count];
                int i = 0;
                foreach (Tecnologia item in lbTecnologiasEscolhidas.Items)
                {
                    tecnologias.Add(item);
                    idTecnologias[i++] = item.Id;
                }
                int nif = int.Parse(txtbNIFColab.Text);
                List<Projeto> projetosAssociados = gestor.ProcuraProjetoPorColaborador(nif);

                if (!VerificaAindaQualificado(tecnologias, projetosAssociados, nif))
                    return;

                string area = cmbFuncao.SelectedValue.ToString();
                Colaborador colaborador = null;

                string nome = txtbNomeColab.Text;
                string morada = txtbMoradaColab.Text;
                int nivelHabilitação = (int)sldrNivelHabil.Value;
                double valorHora = double.Parse(txtbValorHora.Text);
                byte[] foto = CriarByteArray((BitmapImage)imgFoto.Source);

                if (area == "Programação")
                    colaborador = new Programador(nif, nome, morada, nivelHabilitação, valorHora, tecnologias, foto);
                else if (area == "Redes")
                    colaborador = new Redes(nif, nome, morada, nivelHabilitação, valorHora, tecnologias, foto);
                else
                    colaborador = new Tecnico(nif, nome, morada, nivelHabilitação, valorHora, tecnologias, foto);

                bool resultado = gestor.EditaColaborador(colaborador, idTecnologias);

                if (resultado)
                {
                    MessageBox.Show("Colaborador alterado com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    PreencheConsultarColaborador(colaborador);
                    tbiConsultarColaborador.IsSelected = true;
                }
                else
                    MessageBox.Show("Algo correu mal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool VerificaAindaQualificado(List<Tecnologia> tecnologias, List<Projeto> projetosAssociados, int nif)
        {
            bool aindaQualificado = false;
            bool jaFoiPerguntado = false;

            foreach (Projeto projeto in projetosAssociados)
            {
                foreach (Tecnologia tecnologiaNecessaria in projeto.TecnologiasNecessarias)
                {
                    foreach (Tecnologia tecnologia in tecnologias)
                    {
                        if (tecnologiaNecessaria.Id == tecnologia.Id)
                        {
                            aindaQualificado = true;
                            break;
                        }
                    }

                    if (aindaQualificado)
                        break;
                }

                if (!aindaQualificado)
                {
                    if (!jaFoiPerguntado)
                    {
                        if (MessageBox.Show("Há pelo menos um projeto onde este colaborador já não é qualificado para participar. " +
                        "Se prosseguir, vai ser eliminado dos projetos onde já não qualifica. Tem certeza que quer prosseguir?",
                        "Aviso", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                            return false;
                        jaFoiPerguntado = true;
                    }

                    gestor.EliminaColaboradorDoProjeto(nif, projeto.Id);
                }

                aindaQualificado = false;
            }

            return true;
        }

        private void EliminaTecnologiaNaBD()
        {
            bool aindaQualificado = false;
            bool jaFoiPerguntado = false;
            List<Projeto> projetosAssociados = gestor.ProcuraProjetoPorTecnologia(tecnologiaEscolhida.Id);
            List<Colaborador> colaboradores = gestor.ProcuraColaboradoresPorTec(tecnologiaEscolhida.Id);

            if (projetosAssociados.Count > 0)
            {
                foreach (Projeto projeto in projetosAssociados)
                {
                    List<Colaborador> colaboradoresComuns = new();

                    foreach (ColaboradorAssociado colaboradorAssociado in projeto.ColaboradoresAssociados)
                    {
                        foreach (Colaborador colaborador in colaboradores)
                        {
                            if (colaboradorAssociado.Colaborador.Nif == colaborador.Nif)
                                colaboradoresComuns.Add(colaborador);
                        }
                    }

                    foreach (Colaborador colab in colaboradoresComuns)
                    {
                        foreach (Tecnologia tecnologia in colab.TecnologiasDeDominio)
                        {
                            foreach (Tecnologia tec in projeto.TecnologiasNecessarias)
                            {
                                if (tec.Id == tecnologia.Id && tec.Id != tecnologiaEscolhida.Id)
                                {
                                    aindaQualificado = true;
                                    break;
                                }
                            }

                            if (aindaQualificado)
                                break;
                        }

                        if (!aindaQualificado)
                        {
                            if (!jaFoiPerguntado)
                            {
                                if (MessageBox.Show("Ao eliminar esta tecnologia, há pelo menos um colaborador que já não é qualificado " +
                                    "para participar em um projeto. Se prosseguir, vai eliminar colaboradores dos projetos onde já não " +
                                    "qualificam. Tem certeza que quer prosseguir?", "Aviso", MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning) == MessageBoxResult.No)
                                    return;
                                jaFoiPerguntado = true;
                            }

                            gestor.EliminaColaboradorDoProjeto(colab.Nif, projeto.Id);
                        }

                        aindaQualificado = false;
                    }
                }
            }
            else
            {
                if (MessageBox.Show("Tem certeza que quer eliminar esta tecnologia da Base de Dados?", "Aviso",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            }

            bool resultado = gestor.EliminaTecnologia(tecnologiaEscolhida.Id);

            if (resultado)
            {
                MessageBox.Show("Tecnologia eliminada com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                CarregaTecnologias(QualAreaTec(), txtbfiltrarTecNome.Text);
            }
            else
                MessageBox.Show("Algo correu mal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //Preenche as labels do separador Custos Atuais com os valores correspondentes ao projeto selecionado
        private void PopulaCustos(Projeto projeto)
        {
            double[] custos = gestor.CustoProjeto(projeto.Id);

            lblNomeProjetoCusto.Content = projeto.Nome;
            lblCustoTecnologia.Content = String.Format("{0:C2}", custos[0]);
            lblCustoColaboradores.Content = String.Format("{0:C2}", custos[1]);
            lblCustoTotal.Content = String.Format("{0:C2}", custos[2]);
        }

        //Preenche oscampos do separador Consultar Projeto com os valores correspondentes ao projeto selecionado
        private void PopulaConsultarProjeto()
        {
            lblNomeConsultarProjeto.Content = projetoEscolhido.Nome;
            txtbDescricaoConsultar.Text = projetoEscolhido.Descricao;

            List<Tecnologia> tecnologias = gestor.ProcuraProjetoTecnologias(projetoEscolhido.Id);
            lvTecnologiasProjeto.ItemsSource = tecnologias;

            List<ColaboradorAssociado> colaboradoresAssociados = gestor.ProcuraProjetoColaboradores(projetoEscolhido.Id);
            dgColabsAssociados.ItemsSource = colaboradoresAssociados;
        }

        private void LimpaCamposInserirProjeto()
        {
            txtbNomeInserirProjeto.Clear();
            txtbDescricaoInserir.Clear();
            lvTecsDisponiveisProjeto.Items.Clear();
            lvTecsEscolhidasProjeto.Items.Clear();
            lvColabsQualificados.Items.Clear();
            lvColabsAssociados.Items.Clear();
        }

        private void InsereProjetoNaBD()
        {
            if (txtbNomeInserirProjeto.Text.Trim() != string.Empty && txtbDescricaoInserir.Text.Trim() != string.Empty)
            {
                string nomeProjeto = txtbNomeInserirProjeto.Text;
                string descricao = txtbDescricaoInserir.Text;
                List<int> idTecs = new();
                List<ColaboradorAssociado> colaboradoresAssociados = new();
                bool resultado;
                int idProjeto;

                if (lvTecsEscolhidasProjeto.Items.Count > 0)
                {
                    foreach (Tecnologia tecnologia in lvTecsEscolhidasProjeto.Items)
                        idTecs.Add(tecnologia.Id);
                }

                if (lvColabsAssociados.Items.Count > 0)
                {
                    foreach (ColaboradorAssociado colaboradorAssociado in lvColabsAssociados.Items)
                        colaboradoresAssociados.Add(colaboradorAssociado);
                }

                resultado = gestor.InsereProjeto(nomeProjeto, descricao, idTecs.ToArray(), colaboradoresAssociados);

                if (resultado)
                {
                    MessageBox.Show("Projeto inserido com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    idProjeto = gestor.BuscaIdProjeto(nomeProjeto, descricao);
                    projetoEscolhido = gestor.ProcuraProjeto(idProjeto);
                    PopulaConsultarProjeto();
                    tbiConsultarProjeto.IsSelected = true;
                }
                else
                    MessageBox.Show("Algo correu mal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Preencha o nome e descrição do projeto", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void VaiParaGestaoProjetos()
        {
            retanguloColab.Visibility = Visibility.Hidden;
            retanguloProjetos.Visibility = Visibility.Visible;
            retanguloTec.Visibility = Visibility.Hidden;

            tbiGestaoProjetos.IsSelected = true;
            txtbFiltrarNomeProjeto.Clear();

            CarregaProjetos(string.Empty);
        }

        private void VaiParaGestaoColaboradoes()
        {
            retanguloColab.Visibility = Visibility.Visible;
            retanguloProjetos.Visibility = Visibility.Hidden;
            retanguloTec.Visibility = Visibility.Hidden;

            tbiGestorColaboradores.IsSelected = true;
            LimpaFiltrosDosColaboradores();

            CarregaColaboradores(string.Empty, string.Empty, string.Empty);
        }

        private void VaiParaGestaoTecnologias()
        {
            retanguloColab.Visibility = Visibility.Hidden;
            retanguloProjetos.Visibility = Visibility.Hidden;
            retanguloTec.Visibility = Visibility.Visible;
            LimpaFiltrosTecnologias();

            CarregaTecnologias(string.Empty, string.Empty);
            tbiGestaoTecnologias.IsSelected = true;
        }

        private void LimpaFiltrosTecnologias()
        {
            checkbProgramacaoTec.IsChecked = false;
            checkbRedesTec.IsChecked = false;
            checkbTecnicoTec.IsChecked = false;
            txtbfiltrarTecNome.Clear();
        }

        private void CarregaTecnologias(string area, string nome)
        {
            dgTecnologias.Items.Clear();
            List<Tecnologia> tecnologias = gestor.ProcuraTodasTecnologias();
            nome = nome.ToLower();

            foreach (Tecnologia tecnologia in tecnologias)
            {
                if (tecnologia.AreaDeAtividade.StartsWith(area) && tecnologia.Nome.ToLower().StartsWith(nome))
                    dgTecnologias.Items.Add(tecnologia);
            }
        }

        private void PreencheEditarProjeto()
        {
            txtbNomeInserirProjeto.Text = projetoEscolhido.Nome;
            txtbDescricaoInserir.Text = projetoEscolhido.Descricao;

            List<Tecnologia> tecnologiasDisponiveis = gestor.ProcuraTodasTecnologias();

            foreach (Tecnologia tecnologia in projetoEscolhido.TecnologiasNecessarias)
            {
                lvTecsEscolhidasProjeto.Items.Add(tecnologia);
                AdcionaColaboradoresQualificados(tecnologia);

                for (int i = 0; i < tecnologiasDisponiveis.Count; i++)
                {
                    if (tecnologiasDisponiveis[i].Id == tecnologia.Id)
                    {
                        tecnologiasDisponiveis.RemoveAt(i);
                        break;
                    }
                }
            }

            lvTecsDisponiveisProjeto.Items.Clear();

            foreach (Tecnologia tec in tecnologiasDisponiveis)
                lvTecsDisponiveisProjeto.Items.Add(tec);

            List<Colaborador> colaboradoresQualificados = new();
            foreach (Colaborador colaborador in lvColabsQualificados.Items)
                colaboradoresQualificados.Add(colaborador);

            foreach (ColaboradorAssociado colaboradorAssociado in projetoEscolhido.ColaboradoresAssociados)
            {
                lvColabsAssociados.Items.Add(colaboradorAssociado);

                for (int i = 0; i < colaboradoresQualificados.Count; i++)
                {
                    if (colaboradoresQualificados[i].Nif == colaboradorAssociado.Colaborador.Nif)
                    {
                        colaboradoresQualificados.RemoveAt(i);
                        break;
                    }
                }
            }

            lvColabsQualificados.Items.Clear();
            foreach (Colaborador colab in colaboradoresQualificados)
                lvColabsQualificados.Items.Add(colab);
        }

        private void AdcionaColaboradoresQualificados(Tecnologia tecnologia)
        {
            List<Colaborador> colaboradoresQualificados = gestor.ProcuraColaboradoresPorTec(tecnologia.Id);
            if (colaboradoresQualificados.Count > 0)
            {
                foreach (Colaborador colaboradorQualificado in colaboradoresQualificados)
                {
                    bool jaEstaNaLista = false;
                    foreach (Colaborador colaboradorNaLista in lvColabsQualificados.Items)
                    {
                        if (colaboradorQualificado.Nif == colaboradorNaLista.Nif)
                        {
                            jaEstaNaLista = true;
                            break;
                        }
                    }
                    if (!jaEstaNaLista)
                    {
                        lvColabsQualificados.Items.Add(colaboradorQualificado);
                        foreach (ColaboradorAssociado colaboradorAssociado in lvColabsAssociados.Items)
                        {
                            if (colaboradorQualificado.Nif == colaboradorAssociado.Colaborador.Nif)
                            {
                                lvColabsQualificados.Items.Remove(colaboradorQualificado);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void MudaVisibilidadesEditarProjeto()
        {
            switch (modoEdicao)
            {
                case "inserir":
                case "editar tecnologias":
                    gridEditarTecsProjeto.Visibility = Visibility.Hidden;
                    btnAlterarTecsProjeto.Visibility = Visibility.Hidden;

                    stckpnlSetasSelectTec.Visibility = Visibility.Visible;
                    stckpnlSetasRetiraTec.Visibility = Visibility.Visible;

                    gridEditarColabsProjeto.Visibility = Visibility.Hidden;
                    btnAlterarColabsProjeto.Visibility = Visibility.Hidden;

                    stckpnlSetasSelectColab.Visibility = Visibility.Visible;
                    stckpnlSetasRetiraColab.Visibility = Visibility.Visible;
                    break;

                case "editar projeto":
                    gridEditarTecsProjeto.Visibility = Visibility.Visible;
                    btnAlterarTecsProjeto.Visibility = Visibility.Visible;

                    stckpnlSetasSelectTec.Visibility = Visibility.Hidden;
                    stckpnlSetasRetiraTec.Visibility = Visibility.Hidden;

                    gridEditarColabsProjeto.Visibility = Visibility.Visible;
                    btnAlterarColabsProjeto.Visibility = Visibility.Visible;

                    stckpnlSetasSelectColab.Visibility = Visibility.Hidden;
                    stckpnlSetasRetiraColab.Visibility = Visibility.Hidden;
                    break;

                case "editar colaboradores":
                    gridEditarTecsProjeto.Visibility = Visibility.Visible;
                    btnAlterarTecsProjeto.Visibility = Visibility.Visible;

                    stckpnlSetasSelectTec.Visibility = Visibility.Hidden;
                    stckpnlSetasRetiraTec.Visibility = Visibility.Hidden;

                    gridEditarColabsProjeto.Visibility = Visibility.Hidden;
                    btnAlterarColabsProjeto.Visibility = Visibility.Hidden;

                    stckpnlSetasSelectColab.Visibility = Visibility.Visible;
                    stckpnlSetasRetiraColab.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void EditaProjetoNaBD()
        {
            if (txtbNomeInserirProjeto.Text.Trim() != string.Empty && txtbDescricaoInserir.Text.Trim() != string.Empty)
            {
                projetoEscolhido.Nome = txtbNomeInserirProjeto.Text;
                projetoEscolhido.Descricao = txtbDescricaoInserir.Text;

                bool resultado = gestor.EditaProjeto(projetoEscolhido);

                if (resultado)
                {
                    MessageBox.Show("Projeto editado com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    projetoEscolhido = gestor.ProcuraProjeto(projetoEscolhido.Id);
                    PopulaConsultarProjeto();
                    tbiConsultarProjeto.IsSelected = true;
                }
                else
                    MessageBox.Show("Algo correu mal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Preencha o nome e descrição do projeto", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditaColaboradorProjetoNaBD()
        {
            if (txtbNomeInserirProjeto.Text.Trim() != string.Empty && txtbDescricaoInserir.Text.Trim() != string.Empty)
            {
                Projeto projetoNovo = new(projetoEscolhido.Id, txtbNomeInserirProjeto.Text, txtbDescricaoInserir.Text,
                    projetoEscolhido.TecnologiasNecessarias, new List<ColaboradorAssociado>());

                foreach (ColaboradorAssociado colaboradorAssociado in lvColabsAssociados.Items)
                    projetoNovo.ColaboradoresAssociados.Add(colaboradorAssociado);

                bool resultado = gestor.EditaColaboradoresProjeto(projetoEscolhido, projetoNovo);

                if (resultado)
                {
                    MessageBox.Show("Projeto editado com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    projetoEscolhido = gestor.ProcuraProjeto(projetoEscolhido.Id);
                    PopulaConsultarProjeto();
                    tbiConsultarProjeto.IsSelected = true;
                }
                else
                    MessageBox.Show("Algo correu mal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Preencha o nome e descrição do projeto", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditaTecnologiaProjetoNaBD()
        {
            if (txtbNomeInserirProjeto.Text.Trim() != string.Empty && txtbDescricaoInserir.Text.Trim() != string.Empty)
            {
                Projeto projetoNovo = new(projetoEscolhido.Id, txtbNomeInserirProjeto.Text, txtbDescricaoInserir.Text,
                    new List<Tecnologia>(), new List<ColaboradorAssociado>());

                foreach (Tecnologia tecnologia in lvTecsEscolhidasProjeto.Items)
                    projetoNovo.TecnologiasNecessarias.Add(tecnologia);

                foreach (ColaboradorAssociado colaboradorAssociado in lvColabsAssociados.Items)
                    projetoNovo.ColaboradoresAssociados.Add(colaboradorAssociado);

                bool resultado = gestor.EditaTecnologiasColaboradoresProjeto(projetoEscolhido, projetoNovo);

                if (resultado)
                {
                    MessageBox.Show("Projeto editado com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    projetoEscolhido = gestor.ProcuraProjeto(projetoEscolhido.Id);
                    PopulaConsultarProjeto();
                    tbiConsultarProjeto.IsSelected = true;
                }
                else
                    MessageBox.Show("Algo correu mal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Preencha o nome e descrição do projeto", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string QualAreaTec()
        {
            if (checkbProgramacaoTec.IsChecked == true)
                return "P";
            if (checkbRedesTec.IsChecked == true)
                return "R";
            if (checkbTecnicoTec.IsChecked == true)
                return "O";

            return string.Empty;
        }

        private bool VerificaInserirTecnologia()
        {
            if (txtbInserirTecNome.Text.Trim() == string.Empty || (cmbArea.SelectedItem == null && !paraEditar) ||
                txtbInserirTecCusto.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Ainda há campos por preencher", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (!double.TryParse(txtbInserirTecCusto.Text, out double custo))
            {
                MessageBox.Show("Insira um valor numérico válido para o custo", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                txtbInserirTecCusto.Focus();
                return false;
            }

            return true;
        }

        private void InsereTecnologiaNaBD()
        {
            if (VerificaInserirTecnologia())
            {
                string nome = txtbInserirTecNome.Text;
                string area = cmbArea.SelectedValue.ToString();
                double custo = double.Parse(txtbInserirTecCusto.Text);

                //O id aqui pode ser qualquer valor, visto que isto é so pra criar um objecto com informação
                //para ser adcionada á BD. O verdadeiro id será criado automaticamente na inserção 
                //dos dados na BD.
                Tecnologia tecnologia = new(0, nome, area, custo);

                bool resultado = gestor.InsereTecnologia(tecnologia);

                if (resultado)
                {
                    MessageBox.Show("Tecnologia inserida com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    CarregaTecnologias(QualAreaTec(), txtbfiltrarTecNome.Text);

                    inserirTec.Visibility = Visibility.Hidden;
                }
                else
                    MessageBox.Show("Algo correu mal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditaTecnologiaNaBD()
        {
            if (VerificaInserirTecnologia())
            {
                string nome = txtbInserirTecNome.Text;
                double custo = double.Parse(txtbInserirTecCusto.Text);

                tecnologiaEscolhida.Nome = nome;
                tecnologiaEscolhida.Custo = custo;

                bool resultado = gestor.EditaTecnologia(tecnologiaEscolhida);

                if (resultado)
                {
                    MessageBox.Show("Tecnologia alterada com sucesso", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    CarregaTecnologias(QualAreaTec(), txtbfiltrarTecNome.Text);

                    inserirTec.Visibility = Visibility.Hidden;
                }
                else
                    MessageBox.Show("Algo correu mal", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimpaInserirTec()
        {
            txtbInserirTecNome.Clear();
            txtbInserirTecCusto.Clear();
            cmbArea.SelectedItem = null;
        }

        //Verifica quantas tecnologias na mesma área qua a tecnologia escolhida há. Caso haja 4, quere dizer que
        //não se pode eliminar tecnologias dessa área, pois tem de haver, pelo menos, 4 tecnologias de cada área. 
        private bool VerificaTecsSuficientes()
        {
            List<Tecnologia> tecnologias = gestor.ProcuraTodasTecnologias();
            List<Tecnologia> tecsMesmaArea = new();
            string area = tecnologiaEscolhida.AreaDeAtividade;

            foreach (Tecnologia tecnologia in tecnologias)
            {
                if (tecnologia.AreaDeAtividade == area)
                    tecsMesmaArea.Add(tecnologia);
            }

            return tecsMesmaArea.Count > 4;
        }

        //Verifica se os colaboradores associados a uma tecnologia tem mais do que 1 tecnologia associada.
        //Caso não tenham, essa tecnologia, sendo a única que esta associada a um colaborador, não pode ser
        //eliminada, pois o colaborador ficaria sem tecnologias associadas e consequentemente sem função.
        private bool VerificaColabTecsSuficientes()
        {
            List<Colaborador> colaboradores = gestor.ProcuraColaboradoresPorTec(tecnologiaEscolhida.Id);

            foreach (Colaborador colaborador in colaboradores)
            {
                if (colaborador.TecnologiasDeDominio.Count == 1)
                    return false;
            }

            return true;
        }
    }
}
