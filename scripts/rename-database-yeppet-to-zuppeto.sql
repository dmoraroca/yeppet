-- Renames legacy database "yeppet" to "zuppeto" (matches app connection strings).
--
-- Usage:
--   1. Connect to PostgreSQL as a superuser (e.g. user "postgres" or any role with CREATEDB privileges).
--   2. Run against database "postgres" (or any database OTHER than "yeppet"):
--        psql -h localhost -p 5433 -U postgres -d postgres -f scripts/rename-database-yeppet-to-zuppeto.sql
--
-- Prerequisites:
--   - No open connections to "yeppet" except this session (the script terminates other backends).
--   - Target name "zuppeto" must not already exist (drop or pick another name first).

SELECT pg_terminate_backend(pid)
FROM pg_stat_activity
WHERE datname = 'yeppet'
  AND pid <> pg_backend_pid();

ALTER DATABASE yeppet RENAME TO zuppeto;
