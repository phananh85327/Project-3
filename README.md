## Set up config .env file and logs folder for local:

Location ../Configs/gitignore.json

```json
{
  "ConnectionStrings": {
    "Default": "..."
  },
  "Client": {
    "URL": "http://localhost:3000"
  },
  "JWT": {
    "SecretKey": "High-Secured-Password-That-I-Will-Not-Hide-When-Push-To-Repository"
  },
  "Email": {
    "Name": "...",
    "Address": "...",
    "Password": "Google create app password for Gmail"
  },
  "LogFilePath": "Logs"
}
```

Create a "Logs" folder at ../Logs or a name that matches "LogFilePath"

## Set up DB:

Use Library.bak and Library.sql included in the repo
