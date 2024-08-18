# TelemetryStash, Database

## Testing

Integration tests are done on SQL server with [TestContainers](https://dotnet.testcontainers.org/modules/mssql/) and Docker on [WSL2](https://learn.microsoft.com/en-us/windows/wsl/install).

## Configure Docker on WSL2

### Install Docker container engine via PowerShell

```PowerShell
wsl --install
```

Verify Ubuntu is set to default/listed with an asterisk

```PowerShell
wsl -l -v
```

Login to WSL

```bash
wsl
```

### Add Docker's official GPG key

```bash
sudo apt-get update
sudo apt-get install ca-certificates curl
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc
```

### Add the repository to Apt sources

```bash
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt-get update
```

### Install Docker and net-tools

```bash
sudo apt-get install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
sudo apt-get install net-tools
```

### Add your user to the group who can run docker without elevated priviledges

```bash
sudo usermod -aG docker $USER
```

### Expose docker port

```bash
sudo mkdir -p /etc/systemd/system/docker.service.d
```

Add startup arguments to override.conf
```bash
sudo bash -c "echo '[Service]
ExecStart=
ExecStart=/usr/bin/dockerd -H unix:///var/run/docker.sock -H tcp://0.0.0.0:2375 --tlsverify --tlscacert=/etc/docker/ca.pem --tlscert=/etc/docker/server-cert.pem --tlskey=/etc/docker/server-key.pem' > /etc/systemd/system/docker.service.d/override.conf"
```

Verify override.conf file content

```bash
vim /etc/systemd/system/docker.service.d/override.conf
```

### Set up CA

> :bulb: Note. Make sure to set hostname to "localhost" or any other hostname you use to connect to Docker

> :bulb: Set your own certificate password

```bash
hostname=localhost
certifificatePwd=docker123

mkdir -p /tmp/docker-certs; cd /tmp/docker-certs
openssl genrsa -aes256 -passout pass:$certifificatePwd -out ca-key.pem 2048
openssl req -new -x509 -days 9999 -key ca-key.pem -sha256 -out ca.pem -passin pass:$certifificatePwd -subj /CN=hostname
```

#### Generate server certificate

> :bulb: Note. CN and subject must contain hostname you use to connect to Docker - in this example it's localhost

```bash
echo 'subjectAltName = DNS:'hostname', IP:0.0.0.0, IP:127.0.0.1' > extfile.cnf
openssl genrsa -out server-key.pem 2048
openssl req -subj "/CN=hostname" -new -key server-key.pem -out server.csr
openssl x509 -req -days 9999 -in server.csr -CA ca.pem -CAkey ca-key.pem -CAcreateserial -out server-cert.pem -passin pass:$certifificatePwd -extfile extfile.cnf
```

#### Generate client certificate

```bash
echo 'extendedKeyUsage = clientAuth' > extfile.cnf
openssl genrsa -out key.pem 2048
openssl req -subj '/CN=client' -new -key key.pem -out client.csr
openssl x509 -req -days 9999 -in client.csr -CA ca.pem -passin pass:$certifificatePwd -CAkey ca-key.pem -CAcreateserial -out cert.pem -extfile extfile.cnf
```

#### Copy all keys and certs to /etc/docker

```bash
sudo mkdir -p /etc/docker
sudo cp *pem /etc/docker
```

#### Make client key and certificate accessible from outside

```bash
sudo chmod 644 /etc/docker/key.pem /etc/docker/cert.pem
```

#### Restart docker daemon

```bash
sudo systemctl daemon-reload
sudo systemctl restart docker
```

You should see docker listening on port 2375

```bash
netstat -nl | grep 2375
```

### Set Windows environment variables via PowerShell

```PowerShell
[System.Environment]::SetEnvironmentVariable('DOCKER_HOST','tcp://localhost:2375', 'Machine')
[System.Environment]::SetEnvironmentVariable('DOCKER_TLS_VERIFY','1', 'Machine')
[System.Environment]::SetEnvironmentVariable('DOCKER_CERT_PATH','\\wsl.localhost\Ubuntu\etc\docker', 'Machine')
```

## Restart VisualStudio and run tests

Verify containers are running and removed after tests are done
```bash
docker ps -a
```
