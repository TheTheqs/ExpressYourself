ExpressYourself API

Overview
The ExpressYourself API allows users to query the geographical origins of IPv4 addresses, providing country information and enabling data reports for stored IPs.

Features
-Retrieve country data for specific IPv4 addresses.
-Generate reports on stored IP data, including IP counts and last update timestamps.
-Support for filtered reports based on country codes.
-Automatic hourly updates for data consistency.
-Built using .NET Core with PostgreSQL and Entity Framework (Database-First approach).

Endpoints
1. Base Endpoint
/
Description: Hello User.

2. IP Lookup
/{IP}
Description: Replace {IP} with an IPv4 address.
Response: A JSON object containing:
CountryName: Full name of the country.
TwoLetterCode: ISO two-letter country code.
ThreeLetterCode: ISO three-letter country code.

3. Full Report
/report
Description: Provides a full report of all countries stored in the database.
Response: A list of countries, the count of associated IPs, and the timestamp of the most recent update.

4. Filtered Report
/reporta?countries=[countries]
Description: Replace [countries] with a comma-separated list of ISO two-letter country codes.
Response: A filtered list of data for the specified countries. If any code is invalid, an error message is included in the response.

Check requests screeshots for examples.

How It Works
Data Lookup Sequence: The API retrieves data using the following priority:
-Cache.
-Database.
-External API (IP2C).
Performance Note: Response times may vary depending on whether the data is retrieved from the cache, database, or external API.
Automatic Updates: Data is automatically updated every hour to ensure consistency.

Developer Notes
Enabling Testing Tasks
In the Program.cs file, there are commented-out service calls for automatic testing tasks.
//builder.Services.AddHostedService<EH>();
//builder.Services.AddHostedService<HT>();
To enable these tasks: Remove the // comment markers.
Adjust the task frequency by modifying the settings in the respective task files (EH and HT).
You can also look at the output.txt to see the log from a Automatic test made by me.

Database Configuration
The project uses PostgreSQL with Entity Framework (Database-First approach).

Ensure the following:
Do not pre-populate the database to avoid ID conflicts with auto-increment attributes in PostgreSQL.
If ID conflicts occur, update the PostgreSQL sequence number to match the highest existing ID + 1:
sql

ALTER SEQUENCE <sequence_name> RESTART WITH <higher_value + 1>;
Replace <sequence_name> with the name of the sequence and <higher_value> with the higher ID.

Update the database connection strings in both: appsettings.json ; Entity Framework configurations.

For further assistance or inquiries, please reach out to the development team.