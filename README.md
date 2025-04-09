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

Setting up elasticsearch:
[Link to docs](https://www.elastic.co/guide/en/elasticsearch/reference/current/run-elasticsearch-locally.html)

Elasticsearch: `http://localhost:9200`
Kibana: `http://localhost:5601`
