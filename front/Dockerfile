# Build stage
FROM node:25.1.0-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
# Baked into the bundle at build time; the browser calls this URL directly
ARG VITE_API_URL=http://localhost:5093/
ENV VITE_API_URL=$VITE_API_URL
RUN npm run build-only

# Runtime stage
FROM nginx:1.27-alpine AS runtime
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
