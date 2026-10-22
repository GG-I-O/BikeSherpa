FROM node:20-alpine AS builder

WORKDIR /app

COPY package.json package-lock.json* yarn.lock* pnpm-lock.yaml* ./
RUN npm install

COPY . .

RUN npx expo export --platform web

FROM nginx:alpine

COPY --from=builder /app/dist /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=builder /app/assets/images /usr/share/nginx/html/custom/assets/images
COPY --from=builder /app/assets/colors /usr/share/nginx/html/custom/assets/colors

VOLUME /usr/share/nginx/html/custom

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]