## Set up for Back-End local:

../{name}.json

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
    "Password": "google create app password for gmail"
  },
  "LogFilePath": "Logs"
}
```

Create "Logs" at ../Logs or name that match "LogFilePath"
