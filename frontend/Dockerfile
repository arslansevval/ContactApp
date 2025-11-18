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

# Default nginx.conf kullanılabilir, port expose et
EXPOSE 80

# Nginx foreground’da çalışsın
CMD ["nginx", "-g", "daemon off;"]
