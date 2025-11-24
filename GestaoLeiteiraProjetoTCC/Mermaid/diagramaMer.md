---
config:
  layout: elk
---
erDiagram
    direction LR

    PROPRIEDADE {
      int Id PK
      string NomeProprietario
      string NomeSocial
      int Sexo
      string NomePropriedade
      string Localizacao
      double AreaTotal
      int TipoUnidade
      string Senha
      datetime DataCadastro
    }

    RACA {
      int Id PK
      string NomeRaca
      string Status
    }

    ANIMAL {
      int Id PK
      int RacaId FK
      string NomeAnimal
      string NumeroIdentificador
      datetime DataNascimento
      double PesoNascimento
      int Sexo
      int CategoriaAnimal
      boolean Lactante
      string Status
      datetime DataUltimoParto
      int NumeroDePartos
      int NumeroDeAbortos
      int NumeroDeNascimortos
      int MaeId FK
      int PaiId FK
      int PropriedadeId FK
    }

    GESTACAO {
      int Id PK
      int VacaId FK
      int TouroId FK
      datetime DataInicio
      datetime DataConfirmacao
      datetime DataFim
      int TipoCobertura
      string Status
      double ScoreCorporal
      int CriaId FK
      string Observacoes
    }

    LACTACAO {
      int Id PK
      int AnimalId FK
      datetime DataInicio
      datetime DataFim
      int PropriedadeId FK
    }

    PRODUCAO_LEITEIRA {
      int Id PK
      int AnimalId FK
      int LactacaoId FK
      datetime Data
      double Quantidade
      int PropriedadeId FK
    }

    QUANTIDADE_ORDENHA {
      int Id PK
      int Quantidade
      datetime DataRegistro
    }

    PROPRIEDADE ||--o{ ANIMAL : "1:N"
    RACA ||--o{ ANIMAL : "1:N"

    ANIMAL ||--o{ GESTACAO : "vacaId"
    ANIMAL o|--o{ GESTACAO : "touroId"

    ANIMAL ||--o{ LACTACAO : "1:N"
    PROPRIEDADE ||--o{ LACTACAO : "1:N"

    LACTACAO ||--o{ PRODUCAO_LEITEIRA : "1:N"
    ANIMAL ||--o{ PRODUCAO_LEITEIRA : "1:N"
    PROPRIEDADE ||--o{ PRODUCAO_LEITEIRA : "1:N"

    ANIMAL o|--o{ ANIMAL : "MaeId/PaiId"
    GESTACAO o|--o| ANIMAL : "CriaId"
    ANIMAL ||..|| QUANTIDADE_ORDENHA : "usa configuracao"