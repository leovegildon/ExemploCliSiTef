using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AtivacaoGoftCard
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int tamanhoBuffer = 20000;
        int comando = 0;
        int tipoCampo = 0;
        int tamanhoMin = 0;
        int tamanhoMax = 0;
        byte[] buffer = new byte[10000];
        byte[] bufferEnvio = new byte[10000];
        int retornoContinua = -999;
        int continua = 0;
        int ponteiro = 0;
        string valorTMP;

        private void button1_Click(object sender, EventArgs e)
        {
            lblStatusFluxo.Text = "Fluxo Iniciado.";
            lblStatusFluxo.ForeColor = Color.Green;
            //Reiniciando Data e Hora
            txtData.Text = DateTime.Now.ToString("yyyyMMdd");
            txtHora.Text = DateTime.Now.ToString("HHmmss");

            //Caso a comunicação não esteja configurada (IniciaSiTefInterativo)
            if (txtRetornoSitef.Text == "")
                MessageBox.Show("Configure a comunicação primeiro");
            else
            {
                int retornoInicio = CliSiTef.IniciaFuncaoSiTefInterativo(Convert.ToInt32(txtCodBarras.Text), 
                    txtValor.Text, 
                    txtCupom.Text, 
                    txtData.Text, 
                    txtHora.Text, 
                    txtOperador.Text,
                    "'"+txtRestricoes.Text+"'");
                txtRetornoIniciaSitef.Text = retornoInicio.ToString();
                if (retornoInicio == 10000)
                {
                    timer1.Start();
                }
            }
        }

            public int ConfiguraSiTef()
            {
            string resultado = "";
            txtRetornoSitef.Text = "";
            int online = CliSiTef.ConfiguraIntSiTefInterativoEx(txtIpSitef.Text, txtEmpresaSitef.Text, txtTermSitef.Text, (0), ("[MultiplosCupons=1]"));
            if (online == 0) { resultado = "SiTef Conectado."; }
            if (online == 1) { resultado = "Erro: Endereço de IP inválido ou não resolvido."; }
            if (online == 2) { resultado = "Erro: Código de empresa SiTef inválido."; }
            if (online == 3) { resultado = "Erro: Código de terminal inválido."; }
            if (online == 6) { resultado = "Erro: Erro na inicialização TCP/IP"; }
            if (online == 7) { resultado = "Erro: Memória insuficiente."; }
            if (online == 8) { resultado = "Erro: Não encontrou o CliSitef ou está corrompida."; }
            if (online == 10) { resultado = "Erro: Sem acesso à pasta da CliSitef ou privilégios insuficientes."; }
            txtRetornoSitef.Text = resultado;
            return 1;
            }

            private void Form1_Load(object sender, EventArgs e)
            {
                txtData.Text = DateTime.Now.ToString("yyyyMMdd");
                txtHora.Text = DateTime.Now.ToString("HHmmss");
                timer1.Interval = 150;

            }

            private void button2_Click(object sender, EventArgs e)
            {
                valorTMP = System.Text.Encoding.ASCII.GetString(buffer);
                richTituloVisor.Text = ""; //Armengue para limpar o título do visor pois a CliSiTef não está comandando essa ação.
                bufferEnvio = Encoding.ASCII.GetBytes(txtBufferEnvio.Text);
                retornoContinua = CliSiTef.ContinuaFuncaoSiTefInterativo(ref comando, ref tipoCampo, ref tamanhoMin, ref tamanhoMax, bufferEnvio, tamanhoBuffer, continua);
                timer1.Start();
            }

            private void txtBufferEnvio_KeyPress(object sender, KeyPressEventArgs e)
            {
                if ((Keys)e.KeyChar == Keys.Enter)
                {

                    //nada

                }
            }

            private void timer1_Tick(object sender, EventArgs e)
            {
                lblStatusFluxo.Text = "CliSiTef Devolvendo dados.";
                lblStatusFluxo.ForeColor = Color.Yellow;
                tamanhoBuffer = 20000;
                buffer = new byte[10000]; // Reiniciando o buffer
                retornoContinua = CliSiTef.ContinuaFuncaoSiTefInterativo(ref comando, ref tipoCampo, ref tamanhoMin, ref tamanhoMax, buffer, tamanhoBuffer, continua);
                txtMinBuffer.Text = tamanhoMin.ToString();
                txtMaxBuffer.Text = tamanhoMax.ToString();
                string Mensagem = System.Text.Encoding.ASCII.GetString(buffer);
                
                
                    txtRetornoContinua.Text = retornoContinua.ToString();
                    System.IO.StreamWriter log = new System.IO.StreamWriter("log.txt", true);
                    txtRetornoComando.Text = comando.ToString();
                    txtRetornoTipoCampo.Text = tipoCampo.ToString();
                    log.WriteLine(comando+"|"+tipoCampo+"| "+Mensagem);
                    log.Close();
                    richBufferRetorno.Text = Mensagem;//Preenchendo o valor retornado no buffer para apresentar no visor

                    //tratamento do comando para visor
                    if (comando == 1) { ritchVisor.Text = Mensagem; }
                    if (comando == 3) { ritchVisor.Text = Mensagem; }
                    if (comando == 4) { richTituloVisor.Text = Mensagem; }
                    if (comando == 13) { ritchVisor.Text = ""; richTituloVisor.Text = ""; }
                    if (comando == 14) { richTituloVisor.Text = ""; }
                    if (comando == 21) { ritchVisor.Text = Mensagem; }
                    if (comando == 22) { ritchVisor.Text = Mensagem; }
                    if (comando == 30) { ritchVisor.Text = Mensagem; }

                    //Tratamento do comando para comprovante
                    if (tipoCampo == 121)
                    {
                        FormComprovante comprovanteObj = new FormComprovante(Mensagem);
                        comprovanteObj.Show();
                        DialogResult efetuar = MessageBox.Show("Selecione SIM para EFETUAR a transação, NÃO para DESFAZER a transação ou CANCELAR para manter a transação PENDENTE", "Efetuar?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                        if(efetuar == DialogResult.Yes)
                        {
                            CliSiTef.FinalizaTransacaoSiTefInterativo(1, txtCupom.Text, txtData.Text, txtHora.Text);
                        }

                        if (efetuar == DialogResult.No)
                        {
                            CliSiTef.FinalizaTransacaoSiTefInterativo(0, txtCupom.Text, txtData.Text, txtHora.Text);
                        }
                    }

                    
                    //Tratamento dos retornos e comandos para encerramento do fluxo
                    if ((retornoContinua == -9) || (comando == 21) || (comando == 22) || (comando == 29) || (comando == 30) || (comando == 31) || (comando == 34) || (comando == 35) || (comando == 41) || (comando == 42))
                    {
                        lblStatusFluxo.Text = "Aguardando dado no Buffer.";
                        lblStatusFluxo.ForeColor = Color.Yellow;

                        if (retornoContinua == -9)
                        {
                            ritchVisor.Text = ""+
"  "+
"                   ** LEOVEGILDO ** "+
"                     ** SISTEMAS **" +
"";
                            lblStatusFluxo.Text = "Fluxo Encerrado.";
                            lblStatusFluxo.ForeColor = Color.Red;
                        } //Se fluxo encerrado

                        timer1.Stop();
                    }
                //MessageBox.Show("Saiu do laço");
            }

            private void button3_Click(object sender, EventArgs e)
            {
                ConfiguraSiTef();
                int pinpad = CliSiTef.VerificaPresencaPinPad();
                if (pinpad == 1) { lblPinpad.Text = ": Online"; lblPinpad.ForeColor = Color.Green; }
                if (pinpad == 0) { lblPinpad.Text = ": Ausente"; lblPinpad.ForeColor = Color.Red; }
                CliSiTef.EscreveMensagemPermanentePinPad(txtMensagemPinpad.Text);
            }

            private void button4_Click(object sender, EventArgs e)
            {
                timer1.Start();
            }
    }
}
