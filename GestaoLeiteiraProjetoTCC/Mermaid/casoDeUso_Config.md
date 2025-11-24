---
config:
    layout: elk
---
flowchart LR
    actorProprietario([Proprietario])
    actorSistema([Sistema])

    subgraph UC_Config ["Caso de Uso: Configurar Sistema"]
        uc13{{"Cadastrar Propriedade"}}
        uc14{{"Editar Dados da Propriedade"}}
        uc15{{"Alterar Senha"}}
        uc16{{"Configurar Quantidade de Ordenhas"}}
    end

    actorProprietario --> uc13
    actorProprietario --> uc14
    actorProprietario --> uc15
    actorProprietario --> uc16

    actorSistema -.-> uc13
    actorSistema -.-> uc14
    actorSistema -.-> uc16
```
