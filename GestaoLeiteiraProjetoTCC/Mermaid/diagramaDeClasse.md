---
config:
    layout: elk
---
classDiagram
    direction LR
    class Propriedade {
      +int Id
      +string NomeProprietario
      +string NomeSocial
      +int Sexo
      +string NomePropriedade
      +string Localizacao
      +double AreaTotal
      +int TipoUnidade
      +string Senha
      +DateTime DataCadastro
    }

    class Raca {
      +int Id
      +string NomeRaca
      +string Status
    }

    class Animal {
      +int Id
      +int RacaId
      +string NomeAnimal
      +string NumeroIdentificador
      +DateTime DataNascimento
      +double PesoNascimento
      +string Sexo
      +string CategoriaAnimal
      +bool Lactante
      +bool Prenha
      +string Status
      +DateTime DataUltimoParto
      +int NumeroDePartos
      +int NumeroDeAbortos
      +int NumeroDeNascimortos
      +int MaeId
      +int PaiId
      +int PropriedadeId
    }

    class Gestacao {
      +int Id
      +int VacaId
      +int TouroId
      +DateTime DataInicio
      +DateTime DataConfirmacao
      +DateTime DataFim
      +string TipoCobertura
      +string Status
      +double ScoreCorporal
      +int CriaId
      +string Observacoes
    }

    class Lactacao {
      +int Id
      +int AnimalId
      +DateTime DataInicio
      +DateTime DataFim
      +int PropriedadeId
    }

    class ProducaoLeiteira {
      +int Id
      +int AnimalId
      +int LactacaoId
      +DateTime Data
      +double Quantidade
      +int PropriedadeId
    }

    class QuantidadeOrdenha {
      +int Id
      +int Quantidade
      +DateTime DataRegistro
    }

    Propriedade --> "1..*" Animal : possui
    Propriedade --> "1..*" Lactacao : possui
    Propriedade --> "1..*" ProducaoLeiteira : gera

    Raca --> "1..*" Animal : classifica

    Animal --> "1..*" Lactacao : inicia
    Lactacao --> "1..*" ProducaoLeiteira : registra
    Animal --> "0..*" ProducaoLeiteira : produz

    Animal --> "0..*" Gestacao : vaca
    Animal --> "0..*" Gestacao : touro
    Gestacao --> "0..1" Animal : cria

    Animal --> "0..*" Animal : mae
    Animal --> "0..*" Animal : pai

    Animal .. QuantidadeOrdenha : usaConfiguracao
```
