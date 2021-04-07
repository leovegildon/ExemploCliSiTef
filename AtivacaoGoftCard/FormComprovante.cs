using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AtivacaoGoftCard
{
    public partial class FormComprovante : Form
    {
        string comprovanteRecebido;
        public FormComprovante(string comprovante)
        {
            comprovanteRecebido = comprovante;
            InitializeComponent();
        }

        private void FormComprovante_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = comprovanteRecebido;
        }
    }
}
