# EducaReuse — API

API da plataforma EducaReuse (ASP.NET Core 8, Minimal APIs, EF Core + PostgreSQL, JWT).

## Primeira execução

```bash
dotnet tool install --global dotnet-ef          # uma vez só
dotnet restore
dotnet ef migrations add Inicial --project src/EducaReuse.Api --output-dir Infrastructure/Data/Migrations
dotnet run --project src/EducaReuse.Api
```

- Em desenvolvimento, a API sobe em `http://localhost:5080` e usa o banco de `appsettings.Development.json`.
- As migrations são aplicadas automaticamente na inicialização (`Database:AplicarMigrationsNaInicializacao`).

## Endpoints

| Método | Rota | Perfil | Requisito |
|---|---|---|---|
| POST | `/api/auth/registrar` | público | RF01 |
| POST | `/api/auth/login` | público | RF01 |
| GET | `/api/auth/me` | autenticado | RF01 |
| POST | `/api/materiais` | Doador | RF02 |
| GET | `/api/materiais?categoria=&busca=` | público | RF03 |
| GET | `/api/materiais/{id}` | público | RF03 |
| GET | `/api/materiais/meus` | Doador | RF06 |
| POST | `/api/materiais/{id}/solicitacoes` | Beneficiário | RF04 |
| GET | `/api/solicitacoes/minhas` | Beneficiário | RF06 |
| GET | `/api/solicitacoes/recebidas` | Doador | RF06 |
| POST | `/api/solicitacoes/{id}/aceitar` | Doador | RF05 |
| POST | `/api/solicitacoes/{id}/recusar` | Doador | RF05 |
| POST | `/api/solicitacoes/{id}/entregar` | Doador | RF06 |
| GET | `/health` | público | — |

## Deploy no Render (Web Service, Docker)

- Root directory: `server` · Runtime: Docker (usa o `Dockerfile` desta pasta).
- Variáveis de ambiente:
  - `ConnectionStrings__Default` — string do **Session pooler** do Supabase, por exemplo
    `Host=aws-0-sa-east-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.<ref>;Password=<senha>;SSL Mode=Require`
  - `Jwt__Key` — texto aleatório com pelo menos 32 caracteres.
  - `Cors__Origens__0` — URL do frontend publicado.
- Health check path: `/health`.
