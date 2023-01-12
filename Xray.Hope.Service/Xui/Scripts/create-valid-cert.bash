# Create a valid certificate using certbot.
certbot certonly --standalone -d $MYDOMAIN --register-unsafely-without-email --non-interactive --agree-tos