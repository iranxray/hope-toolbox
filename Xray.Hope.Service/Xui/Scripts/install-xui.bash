sudo -i;

apt update -y;

# Install required apps.

yes | apt-get install certbot;
yes | apt-get install openssl;
yes | apt-get install ufw;


# Download and install x-ui.
wget https://raw.githubusercontent.com/vaxilu/x-ui/master/install.sh

echo "y
$MYUSER
$MYPASS
$MYPORT
" | bash install.sh;

x-ui start;

# Config the firewall.

ufw allow ssh;
ufw allow http;
ufw allow https;
ufw allow $MYPORT/tcp;

echo "y" | ufw enable;

ufw status;