FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /usr/app
COPY . /usr/app/

EXPOSE 8081

RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs

ENV NPM_CONFIG_PREFIX=/home/node/.npm-global

RUN dotnet tool restore
RUN npm install

ENTRYPOINT [ "npm" ]
CMD [ "start" ]
