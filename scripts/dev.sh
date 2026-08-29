#!/usr/bin/env bash
# Starts db+api via docker compose (detached) and the frontend dev server
# (foreground). Ctrl+C stops the frontend AND tears the docker services back
# down again, so nothing keeps running in the background afterwards.
set -e

cd "$(dirname "$0")/.."

cleaned_up=0
cleanup() {
  [ "$cleaned_up" -eq 1 ] && return
  cleaned_up=1
  echo ""
  echo "Stopping docker compose services (db, api)..."
  docker compose down
}
trap cleanup EXIT INT TERM

docker compose up -d db api
npx dotenv -e .env -- env BACKEND_URL=http://localhost:8080 npm run dev --prefix GameDevsConnect.Frontend
