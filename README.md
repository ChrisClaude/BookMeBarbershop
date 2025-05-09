# BookMe
This is a web application to allow customers to book appointments with a hairdresser.

## Requirements

| Requirement | Status |
|------------|--------|
| User Authentication | 🟡 In Progress |
| home page (desktop and mobile) | ✅ Completed |
| Appointment Booking | ❌ Not Started |
| Calendar Integration | ❌ Not Started |
| Email Notifications | ❌ Not Started |
| Admin Dashboard | ❌ Not Started |
| Service Management | ❌ Not Started |
Language support (ENG and PL)| ✅ Completed |
Update language flags with svg files| 🟡 Not Started |
Store Language preference in local storage (ENG and PL)| ❌ Not Started |
Create an application wide loader| ✅ Completed |
| Add cookiebot| ✅ Completed |
| Implement alive and health checks endpoints [docs](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-9.0)| ✅ Completed |

Legend:
- ✅ Completed
- 🟡 In Progress
- ❌ Not Started

## Getting Started

Running the api:
```
dotnet run --project BookMeAPI
```

Navigate to `https://localhost:6002/scalar/v1` to view the API documentation.

### Setting up elasticsearch
[Link to docs](https://www.elastic.co/guide/en/elasticsearch/reference/current/run-elasticsearch-locally.html)

Elasticsearch: `http://localhost:9200`
Kibana: `http://localhost:5601`

### Running Jaegar

```
docker run --rm --name jaeger \
  -p 16686:16686 \
  -p 4317:4317 \
  -p 4318:4318 \
  -p 5778:5778 \
  -p 9411:9411 \
  jaegertracing/jaeger:2.5.0
```

## Generate migrations and update database

```
dotnet ef migrations add <migration_name> --context BookMeContext -o ./Data/Migrations --project BookMe.Infrastructure --startup-project BookMeAPI

dotnet ef database update --context BookMeContext --project BookMe.Infrastructure --startup-project BookMeAPI
```

### Remove migrations
```
dotnet ef migrations remove --context BookMeContext --project BookMe.Infrastructure --startup-project BookMeAPI
```
