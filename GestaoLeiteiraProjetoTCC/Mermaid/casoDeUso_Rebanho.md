---
config:
    layout: elk
---
flowchart LR
    actorProprietario([Proprietario])

    subgraph UC_Rebanho ["Caso de Uso: Gerenciar Rebanho"]
        uc4{{"Cadastrar/Editar Animal"}}
        uc5{{"Cadastrar/Editar Raça"}}
        uc6{{"Iniciar/Finalizar Lactação"}}
        uc7{{"Registrar Produção Diária"}}
        uc8{{"Comparar Produção entre Vacas"}}
        uc9{{"Iniciar/Atualizar Gestação"}}
        uc10{{"Registrar Parto / Cria"}}
        uc11{{"Finalizar Gestação sem Cria"}}
        uc12{{"Consultar Histórico de Produção"}}
        uc13{{"Filtrar/Ordenar Lactações e Produções"}}
    end

    actorProprietario --> uc4
    actorProprietario --> uc5
    actorProprietario --> uc6
    actorProprietario --> uc7
    actorProprietario --> uc8
    actorProprietario --> uc9
    actorProprietario --> uc10
    actorProprietario --> uc11
    actorProprietario --> uc12
    actorProprietario --> uc13

    uc4 --> uc6
    uc6 --> uc7
    uc9 --> uc6
    uc7 --> uc8
    uc9 --> uc10
    uc9 --> uc11
    uc7 --> uc12
    uc6 --> uc13
    uc7 --> uc13
```
