FROM node:20-alpine AS builder

WORKDIR /app

COPY package.json package-lock.json* yarn.lock* pnpm-lock.yaml* ./
RUN npm install

COPY . .

RUN npx expo export --platform web

FROM nginx:alpine

COPY --from=builder /app/dist /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=builder /app/assets/images /opt/default-custom/assets/images
COPY --from=builder /app/assets/colors /opt/default-custom/assets/colors

# Seeding script: copies each default file only if it's missing in the volume
COPY <<'EOF' /docker-entrypoint.d/40-seed-custom.sh
#!/bin/sh
set -e
SRC=/opt/default-custom
DST=/usr/share/nginx/html/custom
cd "$SRC"
find . -type f | while IFS= read -r f; do
  if [ ! -e "$DST/$f" ]; then
    mkdir -p "$(dirname "$DST/$f")"
    cp "$f" "$DST/$f"
  fi
done
EOF

RUN chmod +x /docker-entrypoint.d/40-seed-custom.sh && \
    mkdir -p /usr/share/nginx/html/custom
    
VOLUME /usr/share/nginx/html/custom

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]