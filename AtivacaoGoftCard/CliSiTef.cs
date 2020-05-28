using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace AtivacaoGoftCard
{
    class CliSiTef
    {

[DllImport("CliSiTef32I.dll")]
public static extern int ConfiguraIntSiTefInterativoEx(string EnderecoServidorSiTef, string IdentificacaoLoja, string IdentificacaoTerminal, short Reservado, string ParamAdic);

[DllImport("CliSiTef32I.dll")]
public static extern int VerificaPresencaPinPad();

[DllImport("CliSiTef32I.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "IniciaFuncaoSiTefInterativo", CharSet = CharSet.Auto, SetLastError = true)]
public static extern int IniciaFuncaoSiTefInterativo(int Funcao, string ValorOperacao, string COOOperacao, string DataFiscalOperacao, string HoraOperacao, string Operador, string ParametrosAdicionais);

[DllImport("CliSiTef32I.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ContinuaFuncaoSiTefInterativo", CharSet = CharSet.Auto, SetLastError = true)]
public static extern int ContinuaFuncaoSiTefInterativo(ref int comando, ref int tipoCampo, ref int TamMin, ref int TamMax, byte[] Buffer, int TamBuffer, int Continua);

[DllImport("CliSiTef32I.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "FinalizaTransacaoSiTefInterativo", CharSet = CharSet.Auto, SetLastError = true)]
public static extern int FinalizaTransacaoSiTefInterativo(short Confirma, string COOOperacao, string DataFiscalOperacao, string HoraOperacao);

[DllImport("CliSiTef32I.dll")]
public static extern int EscreveMensagemPermanentePinPad(string mensagem);
    }
}
