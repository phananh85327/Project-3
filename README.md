Set up for Back-End local:

../EC/{name}.json
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

Create "Logs" at ../EC/Logs or name that match "LogFilePath"
