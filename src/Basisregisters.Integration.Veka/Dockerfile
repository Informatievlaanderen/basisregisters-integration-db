FROM mcr.microsoft.com/dotnet/runtime-deps:8.0.2-bookworm-slim-amd64

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="integration-db"

COPY / /app
WORKDIR /app

RUN apt-get update && \
    apt-get install curl jq -y && \
    chmod +x ./init.sh

# switch to created user
USER app

ENTRYPOINT ["./init.sh"]
