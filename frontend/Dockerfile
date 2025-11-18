# 1️⃣ Build stage
FROM node:20-alpine AS build

WORKDIR /app

# Package.json ve package-lock.json kopyala
COPY package*.json ./

# Bağımlılıkları yükle
RUN npm install

# Tüm kaynakları kopyala
COPY . .

# Production build
RUN npm run build

# 2️⃣ Serve stage (nginx ile)
FROM nginx:alpine

# Build output’u nginx html dizinine kopyala
COPY --from=build /app/dist /usr/share/nginx/html

# Kendi nginx config'imizi default config'in üzerine yazıyoruz
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
