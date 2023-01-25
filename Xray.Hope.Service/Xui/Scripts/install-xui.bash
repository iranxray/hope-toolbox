sudo -i;

# Install required apps.

yes | apt-get install certbot;
yes | apt-get install openssl;
yes | apt-get install ufw;


# Download and install x-ui.
wget https://raw.githubusercontent.com/vaxilu/x-ui/master/install.sh

echo 'y
$MYUSER
$MYPASS
$MYPORT
' | bash install.sh;

# Config the firewall.

ufw allow ssh;
ufw allow http;
ufw allow https;
ufw allow $MYPORT/tcp;

echo "y" | ufw enable;

ufw status;

x-ui start;

cd "/usr/local/x-ui/bin"

wget https://github.com/v2fly/domain-list-community/releases/latest/download/dlc.dat
wget https://github.com/chiroots/iran-hosted-domains/releases/download/202301210059/iran.dat


x-ui restart;