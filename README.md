# APBD9
Praca domowa z APBD nr 9

## Uruchomienie

Projekt zawiera REST API wykonane w podejściu EF Core Database First.

1. Uruchom SQL Server:

```powershell
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrongPassword123!" -p 14333:1433 --name apbd_lecture9_db_first -d mcr.microsoft.com/mssql/server:2022-latest
```

Jeżeli kontener już istnieje:

```powershell
docker start apbd_lecture9_db_first
```

2. Wykonaj skrypt bazy:

```powershell
docker cp zadanie_1_db_first_university_tasks_setup.sql apbd_lecture9_db_first:/tmp/setup.sql
docker exec apbd_lecture9_db_first /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrongPassword123!" -C -i /tmp/setup.sql
```

3. Uruchom API:

```powershell
dotnet run --project UniversityTasksDbFirstApi
```

Swagger jest dostępny w trybie Development pod adresem z profilu uruchomieniowego, np. `http://localhost:5287/swagger`.

## Endpointy

- `GET /api/courses?activeOnly=true`
- `GET /api/courses/{idCourse}/assignments?publishedOnly=true`
- `GET /api/students/{idStudent}/dashboard`
- `POST /api/submissions`
- `PUT /api/submissions/{idSubmission}/grade`
- `DELETE /api/submissions/{idSubmission}`
