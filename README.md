IBMMQ - Consumer
============

Esse é um projeto desenvolvido em .NET exemplificando a criação de um consumer para IBM MQ, utilizando os dois SDK's disponíveis pela própria IBM.

## Instalação
1. Clone esse repositório para o seu local.
1. Execute os seguintes comandos:
   1. dotnet restore
   1. dotnet build

---

## Samples
A pasta samples contém exemplos disponibilizados e desenvolvidos pela própria IBM. Resolvi mantê-los no projeto pois atualmente a única forma de acessá-los é instalando o MQ localmente, pois esses projetos se encontram no diretório local de instalação.

## IBM.XMS x IBM.WMQ
**IBM.XMS**: É a implementação de uma biblioteca bastante conhecida e amplamente utilizada em Java chamado JMS. Possui facilidades que a biblioteca nativa não possui.
**IBM.WMQ**: Biblioteca nativa de desenvolvimento para IBM MQ, expõe os principais serviços de forma bem básica e simples.

## Dicas
Durante a criação do container docker, não utilizar senha como parâmetro para evitar a obrigatoriedade de autenticação durante o desenvolvimento.

## Links
Links úteis que podem ser úteis durante o desenvolvimento para IBM MQ.
https://www.ibm.com/docs/en/ibm-mq/9.0?topic=scenarios-getting-started-mq
https://developer.ibm.com/learningpaths/ibm-mq-badge/
https://developer.ibm.com/learningpaths/ibm-mq-badge/create-configure-queue-manager/#step-2-get-the-mq-in-docker-image
https://developer.ibm.com/tutorials/mq-connect-app-queue-manager-containers/
https://github.com/ibm-messaging/mq-container/issues/387
