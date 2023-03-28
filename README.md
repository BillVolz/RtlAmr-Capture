### Purpose
A windows service that can capture readings from "smart meters" and log them to an SQL database. These "smart meters" use the 900MHz ISM band and can be read by a cheap rtl-sdr dongle.


### Requirements

- [sdr_tcp] (https://osmocom.org/projects/rtl-sdr/wiki/Rtl-sdr#Windows)  to tune and read the rtl-sdr dongle.
- [rtlamr] (https://github.com/bemasher/rtlamr) to decode the SCM+ messages from the feed.
- GoLang >=1.11 (Go build environment setup guide: http://golang.org/doc/code.html)

### Usage
- Install  [sdr_tcp] on a machine running the sdr-dongle.  This can be installed on its own device if you configure it to allow remote connections.
- Install GO 
- Install the latest rtlamr using GO
- Install MSSQL (Developer / Community Edition) and create a new database.
- Install RtlAmr-Capture as a windows service using scm.exe
- Modify the appsettings.json file, set the to rtlamr and set the connection string for your SQL Server Database.  Start the service and have it capture.

To test,  run rtlamr at the command line using msgtype all, to make sure your able to capture messages.

### ToDo
Change data to use entity migrations and supoprt all compatable databases.