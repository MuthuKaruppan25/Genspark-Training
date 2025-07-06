
FROM node:22-alpine AS build


WORKDIR /app


COPY package*.json ./


RUN npm install 


RUN npm install -g @angular/cli


COPY . .


RUN ng build --configuration production


FROM nginx:alpine


RUN rm -rf /usr/share/nginx/html/*


COPY --from=build /app/dist/payment/browser /usr/share/nginx/html


EXPOSE 80


CMD ["nginx", "-g", "daemon off;"]
