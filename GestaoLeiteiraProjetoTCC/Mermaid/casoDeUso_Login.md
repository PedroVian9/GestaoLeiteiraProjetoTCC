---
config:
  layout: elk
---
flowchart LR
 subgraph UC_Login["Caso de Uso: Autenticar e Selecionar Propriedade"]
        uc1{{"Selecionar Propriedade"}}
        uc2{{"Autenticar com Senha"}}
        uc3{{"Carregar Dashboard Inicial"}}
  end
    actorProprietario(["Proprietario"]) --> uc1
    uc1 --> uc2
    uc2 --> uc3