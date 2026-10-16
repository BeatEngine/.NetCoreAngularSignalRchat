$sqlpw = "378Sh4398h!38h&f3498";
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=${sqlpw}" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
