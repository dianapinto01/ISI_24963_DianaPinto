TP01 – ETL para Automação da Faturação em Loja Online

Autora: Diana Pinto
N.º de Aluna: 24963
Curso: Licenciatura em Engenharia de Sistemas Informáticos
Ano: 3.º Ano – 2025/2026
Instituição: IPCA

1. Enquadramento

O presente trabalho foi desenvolvido no âmbito da unidade curricular de Integração de Sistemas de Informação (ISI) e tem como objetivo a criação de um processo ETL (Extract, Transform and Load) que permita automatizar o processo de faturação de uma loja online.

O sistema foi desenhado para integrar dados provenientes de ficheiros CSV, validar e transformar a informação no KNIME Analytics Platform, armazenar os resultados numa base de dados MySQL, e exportar os dados tratados em formato XML.
Esses dados são posteriormente utilizados em relatórios e dashboards desenvolvidos em Power BI, que permitem uma análise mais intuitiva dos principais indicadores de faturação.
Foi ainda criada uma API em C# (.NET) para disponibilizar os registos em formato XML através de um endpoint.

2. Estrutura do Projeto
```
ISI_LESI_DianaPinto_N24963/
│
├── README.md
│
├── doc/
│   └── Relatório - Diana Pinto - nº 24963 - ISI - 3º Ano LESI - 2025.pdf
│
├── dataint/
│   └── ETL_Faturacao_DianaPinto_N24963_ISI_LESI.knwf
│
├── data/
│   ├── input/
│   │   └── faturas.csv
│   │
│   └── output/
│       ├── File_0.xml
│       ├── Relatorio_Fatura.pdf
│       └── PowerBI_DianaPinto_N24963_LESI.pbix
│
└── src/
    └── Faturacao_DianaPinto_N24963_ISI_LESI/
        ├── Faturacao.sln
        ├── Program.cs
        └── Controladores/
            └── Controlador.cs
```
    
4. Ferramentas Utilizadas
Ferramenta	Finalidade
KNIME Analytics Platform	Criação dos fluxos ETL (extração, transformação e carga)
MySQL	Armazenamento relacional dos dados processados
C# (.NET 6)	Implementação da API para exposição dos dados em XML
Power BI Desktop	Desenvolvimento de dashboards e relatórios interativos
CSV / XML / PDF / PBIX	Formatos utilizados ao longo do processo

5. Instruções de Execução
5.1 Pré-requisitos

Antes de executar o projeto, devem estar instaladas as seguintes ferramentas:

KNIME Analytics Platform

MySQL Server

Visual Studio (com .NET 6 SDK)

Power BI Desktop

5.2 Criação da Base de Dados MySQL

A base de dados utilizada para armazenar os registos de faturação foi criada com os seguintes comandos:

CREATE DATABASE faturacao
USE faturacao;

```
CREATE TABLE faturas (
  id_fatura INT AUTO_INCREMENT PRIMARY KEY,
  numero_fatura VARCHAR(30) NOT NULL,
  data_fatura DATETIME NOT NULL,
  numero_encomenda VARCHAR(30),
  data_encomenda DATETIME,
  estado_encomenda VARCHAR(40),
  cliente_nome VARCHAR(120),
  cliente_ident_tipo VARCHAR(10),
  cliente_ident_num VARCHAR(30),
  cliente_email VARCHAR(120),
  cliente_pais_iso2 CHAR(2),
  morada_faturacao VARCHAR(255),
  morada_entrega VARCHAR(255),
  produto_sku VARCHAR(40),
  produto_designacao VARCHAR(150),
  categoria_produto VARCHAR(80),
  quantidade INT,
  preco_unitario DECIMAL(10,2),
  desconto_percent DECIMAL(5,2) DEFAULT 0,
  iva_percent DECIMAL(5,2),
  subtotal_linha DECIMAL(12,2),
  valor_iva_linha DECIMAL(12,2),
  total_linha DECIMAL(12,2),
  portes DECIMAL(12,2) DEFAULT 0,
  total_subtotal DECIMAL(12,2),
  total_iva DECIMAL(12,2),
  total_final DECIMAL(12,2),
  tipo_pagamento VARCHAR(60),
  referencia_pagamento VARCHAR(80),
  data_pagamento DATETIME,
  transportadora VARCHAR(100),
  origem_dados VARCHAR(100),
  data_processamento DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY uq_numero_fatura (numero_fatura, produto_sku, quantidade, preco_unitario, iva_percent),
  INDEX idx_numero_fatura (numero_fatura),
  INDEX idx_numero_encomenda (numero_encomenda),
  INDEX idx_cliente_email (cliente_email),
  INDEX idx_produto_sku (produto_sku),
  INDEX idx_data_fatura (data_fatura)
);

CREATE TABLE etl_controle (
  id_execucao INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  processo VARCHAR(80) NOT NULL,
  data_inicio DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  data_fim DATETIME NULL,
  duracao_ms BIGINT UNSIGNED NULL,
  registos_lidos INT UNSIGNED DEFAULT 0,
  registos_rejeitados INT UNSIGNED DEFAULT 0,
  registos_carregados INT UNSIGNED DEFAULT 0,
  estado_execucao ENUM('Sucesso','Erro','Parcial') NOT NULL,
  mensagem VARCHAR(255),
  host VARCHAR(100),
  INDEX idx_processo_data (processo, data_inicio),
  INDEX idx_estado (estado_execucao)
);

CREATE TABLE logs_rejeicoes (
  id_registo INT AUTO_INCREMENT PRIMARY KEY,
  id_execucao INT UNSIGNED,
  numero_fatura VARCHAR(30) NOT NULL,
  motivo_erro VARCHAR(255) NOT NULL,
  severidade ENUM('INFO','WARN','ERROR') DEFAULT 'ERROR',
  data_registo DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (id_execucao) REFERENCES etl_controle(id_execucao)
    ON UPDATE CASCADE
    ON DELETE SET NULL
);
```

5.3 Execução do Workflow no KNIME

Abrir o KNIME Analytics Platform.

Importar o workflow ETL_Faturacao_DianaPinto_N24963_ISI_LESI.knwf.

Configurar o Database Connector com as credenciais do MySQL local.

Executar o workflow (F7).

Durante a execução, o processo realiza:

Extração: leitura do ficheiro faturas.csv;

Transformação: validação, normalização e cálculos automáticos;

Carga: gravação dos dados validados na tabela faturas;

Exportação: criação do ficheiro File_0.xml e do relatório Relatorio_Fatura.pdf.

5.4 Execução da API em C#

Abrir o projeto src/Faturacao_DianaPinto_N24963_ISI_LESI/Faturacao.sln no Visual Studio.

Executar o projeto (F5).

O endpoint principal disponibiliza os registos em formato XML:

https://localhost:<5000>/faturas/xml

5.5 Visualização no Power BI

Abrir o ficheiro PowerBI_DianaPinto_N24963_LESI.pbix.

Ligar à fonte de dados File_0.xml.

Explorar os dashboards com os principais indicadores:

Total faturado por mês

Número de faturas emitidas

Produtos mais vendidos

Encomendas pendentes

6. Indicadores Principais (KPIs)
Indicador	Descrição
Total Faturado (€)	Soma do valor total das faturas emitidas
Número de Faturas Emitidas	Contagem de faturas distintas
Produtos Mais Vendidos	Ranking por SKU
Encomendas Pendentes	Encomendas ainda sem estado “Paga”
Ticket Médio (€)	Valor médio por fatura

7. Estrutura do Processo ETL

O processo desenvolvido no KNIME foi dividido em quatro fases principais:

Extração – Leitura dos ficheiros CSV de faturação.

Transformação – Validação e normalização de dados (emails, códigos ISO2, totais, datas).

Carga – Inserção dos dados validados na base de dados MySQL.

Exportação – Geração dos ficheiros XML e PDF para posterior análise no Power BI.

8. Conteúdo do ZIP Final
Pasta / Ficheiro	Descrição
doc/	Relatório final do trabalho (.docx)
dataint/	Workflow KNIME (.knwf)
data/input/	Ficheiro de entrada CSV
data/output/	Ficheiros XML, PDF e Power BI
src/	Código-fonte da API em C#
README.md	Documento de referência e instruções

9. Link vídeo de demonstração
https://alunosipca-my.sharepoint.com/:u:/g/personal/a24963_alunos_ipca_pt/EZhV-oHT9MpCpb4AW59h2O4BXLAI5FmmU9pSY6nhohh_Xw?e=OmjjMI

10. Link do repositório
https://github.com/dianapinto01/ISI_24963_DianaPinto.git

9. Link do repositório
https://github.com/dianapinto01/ISI_24963_DianaPinto.git
