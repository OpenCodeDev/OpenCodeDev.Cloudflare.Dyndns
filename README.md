# OpenCodeDev.Cloudflare.Dyndns
Just a quick script to run DynDNS update on cloudflare.

## Features
- Run on Windows and Linux
- Able to Run on Multiple domains
- Use Record Name instead of ID (script fetch A records by given name eg: subdomain.opencodedev.com).
- Only update when ip has changed.

## Planned
- Add Fallback api to fetch external public ip.
- Add Auto-Self Update (Windows and Linux)

## Note
This is an Alpha Test, it works but not pristine! There will be some extra work to do... hopefully we'll have some time to provide updates soon... as for now it does what it is suppose to do.

## How to use (Debian Linux)

Get the link to latest version from https://github.com/OpenCodeDev/OpenCodeDev.Cloudflare.Dyndns/releases/latest
```
wget https://github.com/OpenCodeDev/OpenCodeDev.Cloudflare.Dyndns/releases/download/0.2.0-alpha/CloudflareDynDNS.tar
```

Extract Archive
```
tar -xvf CloudflareDynDNS.tar --directory /
tar -xf CloudflareDynDNS.tar
```
