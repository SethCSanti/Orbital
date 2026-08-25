FROM oven/bun:1 AS dependencies
WORKDIR /app

COPY web/package.json web/bun.lock ./
RUN bun install --frozen-lockfile

FROM dependencies AS development
COPY web/ ./
EXPOSE 3000
CMD ["bun", "run", "dev", "--", "--hostname", "0.0.0.0"]

FROM dependencies AS build
ARG NEXT_PUBLIC_API_URL=
ARG NEXT_PUBLIC_SIGNALR_HUB_URL=
ENV NEXT_PUBLIC_API_URL=${NEXT_PUBLIC_API_URL}
ENV NEXT_PUBLIC_SIGNALR_HUB_URL=${NEXT_PUBLIC_SIGNALR_HUB_URL}

COPY web/ ./
RUN bun run build

FROM oven/bun:1 AS production-dependencies
WORKDIR /app
COPY web/package.json web/bun.lock ./
RUN bun install --frozen-lockfile --production

FROM oven/bun:1-slim AS runtime
WORKDIR /app
ENV NODE_ENV=production

COPY --from=production-dependencies /app/node_modules ./node_modules
COPY --from=build /app/.next ./.next
COPY --from=build /app/public ./public
COPY --from=build /app/package.json ./package.json

EXPOSE 3000
CMD ["bun", "run", "start"]
