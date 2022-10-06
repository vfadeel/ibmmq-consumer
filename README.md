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