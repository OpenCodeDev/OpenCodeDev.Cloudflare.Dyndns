# Cloudflare DynDNS
Just a quick script to run DynDNS update on cloudflare.

## Features
- Run on Windows and Linux
- Able to Run on Multiple domains
- Requires Record's name instead of record id! which is so much simpler.
- Only update when ip has changed.

## Planned
- Add Fallback api to fetch external public ip.

## Maybe
- Add other providers such as DO, Google, Azure...


## How to use (Debian Linux)


Create a folder and cd in:

```
mkdir /cfdns
cd /cfdns
```

Get the link to latest version from https://github.com/OpenCodeDev/OpenCodeDev.Cloudflare.Dyndns/releases/latest
```
wget https://github.com/OpenCodeDev/OpenCodeDev.Cloudflare.Dyndns/releases/download/test/cfdyndns

```

Create file nano ./config.json

You can place your A records you wish to update when ip address changes... list as many under cloudflare.. we may add other providers later... if requested for now we just use cloudflare.
```
{
  "cloudflare": [
    {
      "key": "YOUR_CLOUFLARE_API",
      "record": "sub.domain.com",
      "zoneid":  "ZONE_ID"
    },
    {
      "key": "YOUR_CLOUFLARE_API",
      "record": "sub2.domain.com",
      "zoneid":  "ZONE_ID"
    }
  ]
}

```

Create System Service in /etc/systemd/system/cfdns.service
```
[Unit]
Description=Update cloudflare record with dynamic dns.
After=network.target

[Service]
Type=oneshot
WorkingDirectory=/cfdns/
ExecStart=/cfdns/cfdyndns

```

Create 10 Minute Timer Service in /etc/systemd/system/cfdns.timer (same name .timer)

```
[Unit]
Description=
Requires=cfdns.service

[Timer]
OnCalendar=*:0/10

[Install]
WantedBy=timers.target

```

Enable your timer so it gets activated at boot time.
```
systemctl enable cfdns.timer
```

