#!/usr/bin/env bash
# Espera que l'API de YepPet respongui a /health/db des de l'amfitrió (no fa servir docker exec).
# Si el contenidor yeppet-api ha sortit, mostra logs i surt amb error.

set -u

API_PORT="${API_PORT:-5211}"
URL="http://127.0.0.1:${API_PORT}/health/db"
MAX_ROUNDS=200
SLEEP_SEC=2

inspect_status() {
  docker inspect -f '{{.State.Status}}' yeppet-api 2>/dev/null || echo none
}

for i in $(seq 1 "$MAX_ROUNDS"); do
  st="$(inspect_status)"
  if [ "$st" = "exited" ] || [ "$st" = "dead" ]; then
    echo "yeppet-api no està en marxa (estat: $st). Darrers logs:"
    docker logs yeppet-api --tail 150 2>&1
    exit 1
  fi

  if curl -fsS "$URL" >/dev/null 2>&1; then
    echo "API llesta a :${API_PORT}"
    exit 0
  fi

  sleep "$SLEEP_SEC"
done

echo "Timeout esperant ${URL} (després de $((MAX_ROUNDS * SLEEP_SEC))s aprox.)"
st="$(inspect_status)"
if [ "$st" = "exited" ] || [ "$st" = "dead" ]; then
  docker logs yeppet-api --tail 150 2>&1
elif [ "$st" = "running" ]; then
  echo "El contenidor segueix en marxa però l'endpoint no respon. Darrers logs:"
  docker logs yeppet-api --tail 150 2>&1
fi
exit 1
