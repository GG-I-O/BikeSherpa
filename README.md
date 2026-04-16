# BikeSherpa

Bike Sherpa is a Web based application to manage bike deliveries

## Back
[Readme for back](./sources/core/readme.md)

## Front
[Readme for front](./sources/gui/readme.md)

## Dev certificates
```sh
#Install mkcert
mkcert -install

cd ./infrastructure/logger
mkcert localhost

cd ./sources/gui/Ggio.BikeSherpa.Frontend
mkcert dev.bike.local
```