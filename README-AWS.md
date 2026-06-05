# Deployment na AWS Lambda

Ovaj dokument objasnjava kako deployovati `SendSmsApi` na AWS Lambda koristeci AWS SAM.

## Preduslovi

1. [AWS CLI](https://aws.amazon.com/cli/) instaliran i konfigurisan (`aws configure`)
2. [AWS SAM CLI](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/install-sam-cli.html) instaliran
3. [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) instaliran
4. S3 bucket `zic-lambda-deployments-milica` vec postoji u tvojoj AWS ket

## Korak 1 — Postavi kredencijale u AWS Parameter Store

> ⚠️ **NIKAD ne stavljaj kredencijale u `appsettings.json` na produkciji!**
> Koristi AWS Systems Manager Parameter Store.

```bash
# SDP URL (String)
aws ssm put-parameter \
  --name "/SendSmsApi/SdpSettings/Url" \
  --value "http://212.200.252.224:27010/ParlayXSmsAccess/services/SendSms" \
  --type String

# SDP Username (String)
aws ssm put-parameter \
  --name "/SendSmsApi/SdpSettings/Username" \
  --value "S-UE1K1ZY7481_S-UE1K1ZY7481-1_2@P-NA46AFR7369" \
  --type String

# SDP Password (SecureString — enkriptovan!)
aws ssm put-parameter \
  --name "/SendSmsApi/SdpSettings/Password" \
  --value "TVOJA_LOZINKA" \
  --type SecureString

# Bearer API Token (SecureString — enkriptovan!)
aws ssm put-parameter \
  --name "/SendSmsApi/ApiTokens/0" \
  --value "TVOJ_BEARER_TOKEN" \
  --type SecureString
```

## Korak 2 — Build i Deploy

```bash
# 1. Pozicionuj se u folder projekta
cd C:\Users\milicamiloj\Desktop\materijali\test\Mcp\SendSmsApi

# 2. Build (SAM build)
sam build

# 3. Deploy (prvi put — interaktivno, cuva konfiguraciju)
sam deploy --guided
```

Tokom `sam deploy --guided` unesi:

| Polje | Vrednost |
|-------|----------|
| Stack Name | `SendSmsApiStack` |
| AWS Region | `eu-central-1` (ili tvoj region) |
| S3 Bucket | `zic-lambda-deployments-milica` |
| Confirm changes | `Y` |
| Allow SAM CLI IAM role creation | `Y` |

### Naredni deployevi (bez guided)

```bash
sam build && sam deploy
```

## Korak 3 — Test

Nakon deploya, SAM ce ispisati **URL** API Gatewaya u Outputs sekciji.

```bash
# Primer testiranja
curl -X POST https://<api-id>.execute-api.eu-central-1.amazonaws.com/prod/api/sms/send \
  -H "Authorization: Bearer TVOJ_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"addresses": ["+381641234567"], "senderName": "Test", "message": "Hello"}'
```

## Struktura fajlova za AWS

```
SendSmsApi/
├── template.yaml                  # SAM CloudFormation template
├── aws-lambda-tools-defaults.json # Default konfiguracija za dotnet lambda alat
├── LambdaEntryPoint.cs            # Lambda bootstrap (wrappuje Program.cs)
├── SendSmsApi.csproj              # Dodat Amazon.Lambda.AspNetCoreServer.Hosting
└── README-AWS.md                  # Ovaj fajl
```

## Napomene

- `template.yaml` je trazeni CloudFormation template fajl.
- `Handler: SendSmsApi` odgovara `AssemblyName` u `.csproj`.
- `Runtime: dotnet10` je podrzan na AWS Lambda od januara 2026.
- Secrets (SDP password, API tokeni) su u AWS SSM Parameter Store — **ne u kodu**.
