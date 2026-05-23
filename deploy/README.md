# once-server — deployment

CI/CD pipeline: GitHub Actions runs `dotnet test`, builds a Docker image
on every push to `main`, pushes to GHCR, then SSHes into the server and
runs `docker run` to replace the container.

No `docker compose` — single container deploy. The container joins the
shared `once-net` Docker network so it can talk to the ai-backend
services by name (`ai-backend-gateway`, `ai-backend-app`, etc.).

Postgres runs on the host (not in Docker). Set `DB__HOST=host.docker.internal`
(or the host IP) in `/opt/once-server/.env`.

## Required GitHub secrets

| Secret           | What it is                                                                 |
|------------------|----------------------------------------------------------------------------|
| `DEPLOY_HOST`    | Server hostname or IP                                                      |
| `DEPLOY_USER`    | SSH user (e.g. `deploy`)                                                   |
| `DEPLOY_SSH_KEY` | Private key (PEM) for that user                                            |
| `DEPLOY_PORT`    | Optional. SSH port — defaults to `22`                                      |

A protected `production` environment is recommended for approval gating.

## Server bootstrap (do once)

```bash
# Shared network (also used by ai-backend and once-client)
docker network create once-net 2>/dev/null || true

# Project directory + env
sudo mkdir -p /opt/once-server
sudo chown deploy:deploy /opt/once-server
sudo -u deploy nano /opt/once-server/.env
# Set ConnectionStrings__Default to point at host.docker.internal:5432
```

## How it runs on the server

```bash
docker run -d \
  --name once-server-api \
  --restart unless-stopped \
  --network once-net \
  --env-file /opt/once-server/.env \
  -p 44010:8080 \
  ghcr.io/<owner>/once-server:sha-<short>
```

The CI workflow does this for you on every push to `main`.

## Rollback

SSH in and re-run with an older tag:

```bash
docker rm -f once-server-api
docker run -d --name once-server-api --restart unless-stopped \
  --network once-net --env-file /opt/once-server/.env \
  -p 44010:8080 ghcr.io/<owner>/once-server:sha-abc1234
```

## Dockerfile note

The original Dockerfile pointed at `harbor.edcom.uz/internal/dotnet/*`
(unreachable from GitHub-hosted runners) and `Guarantee.*` project names
(repo has since been renamed to `Once.*`). It now uses public
`mcr.microsoft.com/dotnet/*` images. Keep `Dockerfile.harbor` separately
if you still need local builds against the internal registry.
