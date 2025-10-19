using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.Sig;
using System.Text;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Faturacao.Controladores
{
    [ApiController]
    [Route("faturas")]
    public class FaturasController : ControllerBase
    {
        
        private const string ConnStr =
            "Server=localhost;Port=3306;Database=faturacao;User=root;Password=12345;";

        // Todas as faturas em XML
        // GET /faturas/xml
        [HttpGet("xml")]
        public IActionResult GetXml()
        {
            // Abre a ligação à base de dados
            using var conn = new MySqlConnection(ConnStr);
            conn.Open();

            // Comando SQL que seleciona todas as faturas ordenadas por data e número
            var cmd = new MySqlCommand(
                "SELECT * FROM faturacao.faturas ORDER BY data_fatura, numero_fatura", conn);
            using var rdr = cmd.ExecuteReader();

            // Cria um stream em memória para armazenar o XML gerado
            using var ms = new MemoryStream();

            // Configurações do escritor XML
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(false) 
            };

            // Criação do ficheiro XML linha a linha
            using (var xw = XmlWriter.Create(ms, settings))
            {
                xw.WriteStartDocument();
                // Abre o elemento <faturas>
                xw.WriteStartElement("faturas");

                // Percorre todos os registos devolvidos pela query
                while (rdr.Read())
                {
                    // Abre o elemento <fatura>
                    xw.WriteStartElement("fatura");

                    // Escreve cada campo da fatura como um elemento XML
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        var nome = rdr.GetName(i);
                        var val = rdr.GetValue(i);
                        if (val == DBNull.Value) continue;

                        // Conversão do valor para texto com formatação adequada
                        string texto =
                            val is DateTime dt ? dt.ToString("yyyy-MM-dd HH:mm:ss") :
                            val is decimal dec ? dec.ToString("0.00") :
                            val is double dbl ? dbl.ToString("0.###") :
                            val.ToString();

                        xw.WriteElementString(nome, texto);
                    }
                    // Fecha o elemento <fatura>
                    xw.WriteEndElement(); 
                }

                // Fecha o elemento <faturas>
                xw.WriteEndElement();  
                xw.WriteEndDocument();
                xw.Flush();
            }

            // Retorna o conteúdo XML gerado como resposta HTTP
            return File(ms.ToArray(), "application/xml; charset=utf-8");
        }

        // Endpoint que devolve apenas as faturas canceladas em formato XML
        // GET /faturas/xml-nao-pagas
        [HttpGet("xml-nao-pagas")]
        public IActionResult GetXmlNaoPagas()
        {
            // Abre a ligação à base de dados
            using var conn = new MySqlConnection(ConnStr);
            conn.Open();

            // Comando SQL que seleciona apenas faturas com estado diferente de Cancelada de Espanha
            var cmd = new MySqlCommand(
                "SELECT " +
                "numero_fatura, " +
                "data_fatura, " +
                "numero_encomenda, " +
                "cliente_nome, " +
                "cliente_email, " +
                "cliente_pais_iso2, " +
                "produto_sku, " +
                "produto_designacao, " +
                "quantidade, " +
                "preco_unitario, " +
                "total_linha, " +
               "estado_encomenda," +
               "data_processamento " +
                "FROM faturacao.faturas " +
                "WHERE estado_encomenda = 'Cancelada'" +
                "AND cliente_pais_iso2 = 'ES'"+
                "ORDER BY data_fatura DESC, numero_fatura", conn);

      
            using var rdr = cmd.ExecuteReader();


            // Cria um stream em memória para armazenar o XML
            using var ms = new MemoryStream();
            // Configurações do escritor XML
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(false) 
            };

            // Geração do XML com as faturas não pagas
            using (var xw = XmlWriter.Create(ms, settings))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("faturas_nao_pagas");

                while (rdr.Read())
                {
                    xw.WriteStartElement("fatura");

                    // Escreve cada campo da fatura não paga
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        var nome = rdr.GetName(i);
                        var val = rdr.GetValue(i);
                        if (val == DBNull.Value) continue;

                        string texto =
                            val is DateTime dt ? dt.ToString("yyyy-MM-dd HH:mm:ss") :
                            val is decimal dec ? dec.ToString("0.00") :
                            val is double dbl ? dbl.ToString("0.###") :
                            val.ToString();

                        xw.WriteElementString(nome, texto);
                    }

                    xw.WriteEndElement(); 
                }

                xw.WriteEndElement();  
                xw.WriteEndDocument();
                xw.Flush();
            }

            return File(ms.ToArray(), "application/xml; charset=utf-8");
        }
    }
}
