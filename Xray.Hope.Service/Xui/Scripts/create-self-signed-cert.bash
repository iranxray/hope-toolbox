#Create a self-signed certificate using openssl.
openssl req -x509 -newkey rsa:4096 -keyout privkey.pem -out fullchain.pem -sha256 -days 365 -nodes
