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
        byte[] buffer = new byte[200];
        int retornoContinua = -999;
        int continua = 0;
        int ponteiro = 0;
        string valorTMP;

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtRetornoSitef.Text == "")
                MessageBox.Show("Configure a comunicação primeiro");
            else
            {
                //button1.Enabled = false;
                int retornoInicio = CliSiTef.IniciaFuncaoSiTefInterativo(Convert.ToInt32(txtCodBarras.Text), txtValor.Text, txtCupom.Text, txtData.Text, txtHora.Text, txtOperador.Text, txtRestricoes.Text);
                //MessageBox.Show("IniciaFuncaoSiTefInterativo = " + retornoInicio.ToString());
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

                timer1.Interval = 350;

            }

            private void button2_Click(object sender, EventArgs e)
            {
                //tamanhoBuffer = txtBufferEnvio.TextLength;
                //buffer = Encoding.ASCII.GetBytes(txtBufferEnvio.Text);
                valorTMP = System.Text.Encoding.ASCII.GetString(buffer);
                buffer = Encoding.ASCII.GetBytes(txtBufferEnvio.Text);
                //MessageBox.Show("Valor do buffer: " + valorTMP + "\n" + "Tamanho do buffer: " + tamanhoBuffer);
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
                txtMinBuffer.Text = tamanhoMin.ToString();
                txtMaxBuffer.Text = tamanhoMax.ToString();
                 tamanhoBuffer = buffer.Length;
                 
                    retornoContinua = CliSiTef.ContinuaFuncaoSiTefInterativo(ref comando, ref tipoCampo, ref tamanhoMin, ref tamanhoMax, buffer, tamanhoBuffer, continua);
                     
                string Mensagem = System.Text.Encoding.ASCII.GetString(buffer);
                
                
                    //richBufferRetorno.Text = Mensagem;
                    txtRetornoContinua.Text = retornoContinua.ToString();
                    System.IO.StreamWriter log = new System.IO.StreamWriter("log.txt", true);
                    txtRetornoComando.Text = comando.ToString();
                    txtRetornoTipoCampo.Text = tipoCampo.ToString();
                    log.WriteLine(comando+"|"+tipoCampo+"| "+Mensagem);
                    log.Close();
                    richBufferRetorno.Text = Mensagem;

                //tratamento do comando para visor
                    if (comando == 1) { ritchVisor.Text = Mensagem; }
                    if (comando == 3) { ritchVisor.Text = Mensagem; }
                    if (comando == 4) { richTituloVisor.Text = Mensagem; }
                    if (comando == 14) { richTituloVisor.Text = ""; }
                    if (comando == 21) { ritchVisor.Text = Mensagem; }
                    if (comando == 22) { ritchVisor.Text = Mensagem; }

                    if (retornoContinua == -9) { ritchVisor.Text = "    ** LEOVEGILDO **\n     ** SISTEMAS **"; }
                    
                        
                    
                    
                    //MessageBox.Show("Comando: " + comando.ToString() + "\n" + "tipoCampo: " + tipoCampo);

                    if ((comando == 21) || (comando ==  29) || (comando ==  30) || (comando ==  31) || (comando ==  34) || (comando ==  35) || (comando ==  41) || (comando ==  42))
                    {
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
