### Purpose
A windows service that can capture readings from "smart meters" and log them to an SQL database. These "smart meters" use the 900MHz ISM band and can be read by a cheap rtl-sdr dongle.

This project uses a combination of the following tools.

[sdr_tcp] (https://manpages.ubuntu.com/manpages/trusty/man1/rtl_tcp.1.html) to tune and read the rtl-sdr dongle.
[rtlamr] (https://github.com/bemasher/rtlamr) to decode the SCM+ messages from the feed.


### Usage
Install  [sdr_tcp] (https://osmocom.org/projects/rtl-sdr/wiki/Rtl-sdr#Windows) on a machine running the sdr-dongle.  This can be installed on its own device if configure to allow remote connections.

Install go and download the latest rtlamr
Install MSSQL (Developer edition) and create a new database.
Install RtlAmr-Capture window service using scm.exe
Modify the appsettings.json file, set the to rtlamr and set the connection string for your SQL Server Database.
Start the service and have it capture.

To test,  run rtlamr at the command line using msgtype all, to make sure your able to capture messages.

### ToDo
Change data to use entity migrations and supoprt all compatable databases.